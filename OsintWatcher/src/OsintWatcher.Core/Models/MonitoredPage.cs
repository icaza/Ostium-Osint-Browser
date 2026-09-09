namespace OsintWatcher.Core.Models;

/// <summary>
/// A target page the investigator has registered for surveillance.
/// </summary>
public sealed class MonitoredPage
{
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>Absolute http/https URL under surveillance.</summary>
    public required string Url { get; set; }

    /// <summary>Free-text label chosen by the investigator (e.g. case reference).</summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>Optional case / investigation reference for evidentiary grouping.</summary>
    public string CaseReference { get; set; } = string.Empty;

    /// <summary>Investigator notes, never sent anywhere.</summary>
    public string Notes { get; set; } = string.Empty;

    public DateTimeOffset AddedAtUtc { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>Manual = investigator clicks "Run analysis"; Randomized = engine checks
    /// automatically at a jittered interval (avoids a fixed, detectable request pattern).</summary>
    public MonitoringMode Mode { get; set; } = MonitoringMode.Manual;

    /// <summary>Base interval in minutes for Randomized mode.</summary>
    public int IntervalMinutes { get; set; } = 60;

    /// <summary>+/- percentage jitter applied around IntervalMinutes (0-90).</summary>
    public int JitterPercent { get; set; } = 30;

    public bool Enabled { get; set; } = true;

    /// <summary>When true, disables the SSRF guard for this specific page so it may target a
    /// loopback/private/internal address. Off by default; must be explicitly opted into.</summary>
    public bool AllowPrivateTargetsFlag { get; set; } = false;

    public DateTimeOffset? LastCheckedUtc { get; set; }
    public DateTimeOffset? NextDueUtc { get; set; }
}

public enum MonitoringMode
{
    Manual,
    RandomizedInterval
}
