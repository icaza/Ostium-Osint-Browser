using OsintWatcher.Core.Models;
using OsintWatcher.Core.Storage;

namespace OsintWatcher.Core.Services;

public sealed class CheckOutcome
{
    public required MonitoredPage Page { get; init; }
    public required ScanResult Result { get; init; }
}

/// <summary>
/// Orchestrates a single check of a page: fetch, fingerprint, compare against the last
/// stored fingerprint, diff if needed, persist, and (for randomized-mode pages) schedule the
/// next due time with jitter so requests don't fall into a fixed, easily fingerprinted
/// pattern from the target's point of view.
/// </summary>
public sealed class MonitoringEngine
{
    private readonly SecureHttpFetcher _fetcher;
    private readonly EncryptedStore _store;
    private readonly Random _rng = new();

    public MonitoringEngine(SecureHttpFetcher fetcher, EncryptedStore store)
    {
        _fetcher = fetcher;
        _store = store;
    }

    public async Task<CheckOutcome> RunCheckAsync(MonitoredPage page, CancellationToken ct = default)
    {
        var previous = _store.GetLatest(page.Id);
        var fetch = await _fetcher.FetchAsync(page.Url, page.AllowPrivateTargetsFlag, ct);

        ScanResult result;
        if (!fetch.Success)
        {
            result = new ScanResult
            {
                PageId = page.Id,
                Url = page.Url,
                Verdict = ScanVerdict.Unreachable,
                ErrorMessage = fetch.ErrorMessage,
                HttpStatusCode = fetch.HttpStatusCode,
                RelevantHeaders = fetch.RelevantHeaders
            };
        }
        else
        {
            var fingerprint = FingerprintService.Compute(fetch);
            var visibleText = FingerprintService.ExtractVisibleText(fetch.Html);

            ScanVerdict verdict;
            DiffResult? diff = null;

            if (previous?.Fingerprint is null)
            {
                verdict = ScanVerdict.Baseline;
            }
            else if (previous.Fingerprint.VisibleTextSha256 == fingerprint.VisibleTextSha256 &&
                     previous.Fingerprint.StructuralSha256 == fingerprint.StructuralSha256)
            {
                verdict = ScanVerdict.Unchanged;
            }
            else
            {
                verdict = ScanVerdict.Modified;
                var previousText = previous.SnapshotTextGz is not null
                    ? EncryptedStore.DecompressSnapshot(previous.SnapshotTextGz)
                    : string.Empty;
                diff = DiffService.Compare(previousText, visibleText);
            }

            result = new ScanResult
            {
                PageId = page.Id,
                Url = page.Url,
                Verdict = verdict,
                Fingerprint = fingerprint,
                HttpStatusCode = fetch.HttpStatusCode,
                RelevantHeaders = fetch.RelevantHeaders,
                Diff = diff,
                SnapshotTextGz = EncryptedStore.CompressSnapshot(visibleText)
            };
        }

        _store.AppendScanResult(page.Id, result);

        page.LastCheckedUtc = result.TimestampUtc;
        if (page.Mode == MonitoringMode.RandomizedInterval)
            page.NextDueUtc = ComputeNextDueTime(page);

        return new CheckOutcome { Page = page, Result = result };
    }

    /// <summary>Base interval +/- JitterPercent, so automated checks land at an irregular,
    /// non-predictable cadence rather than every exactly-N-minutes.</summary>
    public DateTimeOffset ComputeNextDueTime(MonitoredPage page)
    {
        var baseMinutes = Math.Max(1, page.IntervalMinutes);
        var jitterFraction = Math.Clamp(page.JitterPercent, 0, 90) / 100.0;
        var minMinutes = baseMinutes * (1 - jitterFraction);
        var maxMinutes = baseMinutes * (1 + jitterFraction);
        var minutes = minMinutes + _rng.NextDouble() * (maxMinutes - minMinutes);
        return DateTimeOffset.UtcNow.AddMinutes(minutes);
    }
}
