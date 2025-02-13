using HtmlAgilityPack;
using Ostium;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

public class FaviconDownloader
{
    readonly HttpClient _httpClient;

    public FaviconDownloader()
    {
        if (string.IsNullOrWhiteSpace(Class_Var.URL_USER_AGENT_SRC_PAGE))
            Class_Var.URL_USER_AGENT_SRC_PAGE = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36 Edg/131.0.0.0";

        _httpClient = new HttpClient(new HttpClientHandler
        {
            AllowAutoRedirect = true,
            UseCookies = false
        });
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(Class_Var.URL_USER_AGENT_SRC_PAGE);
    }

    public async Task<byte[]> GetFaviconAsync(string url, bool tryAllMethods = true)
    {
        var uri = new Uri(url);

        try
        {
            return await GetStandardFaviconAsync(uri);
        }
        catch
        {
            if (!tryAllMethods) throw;
        }

        try
        {
            return await GetFaviconFromHtmlAsync(uri);
        }
        catch
        {
            return await GetAppleTouchIconAsync(uri);
        }
    }

    async Task<byte[]> GetStandardFaviconAsync(Uri uri)
    {
        var faviconUri = new Uri(uri, "/favicon.ico");
        return await DownloadIconAsync(faviconUri);
    }

    async Task<byte[]> GetFaviconFromHtmlAsync(Uri uri)
    {
        var html = await _httpClient.GetStringAsync(uri);
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        var links = htmlDoc.DocumentNode.SelectNodes("//link[@rel='icon' or @rel='shortcut icon']");

        var faviconLink = links?
            .Select(link => link.GetAttributeValue("href", null))
            .FirstOrDefault(href => !string.IsNullOrEmpty(href));

        if (faviconLink == null) throw new FileNotFoundException();

        var faviconUri = new Uri(uri, faviconLink);
        return await DownloadIconAsync(faviconUri);
    }

    async Task<byte[]> GetAppleTouchIconAsync(Uri uri)
    {
        var appleIconUri = new Uri(uri, "/apple-touch-icon.png");
        return await DownloadIconAsync(appleIconUri);
    }

    async Task<byte[]> DownloadIconAsync(Uri iconUri)
    {
        var response = await _httpClient.GetAsync(iconUri);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsByteArrayAsync();
    }
}