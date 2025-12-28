using SecureFileExplorer.OSINT.Services;
using System.Net;

namespace SecureFileExplorer.OSINT.Server;

public class LocalHttpServer
{
    private readonly HttpListener _listener = new();
    private readonly AppConfig _config;
    private readonly FileIndexer _indexer;

    public LocalHttpServer(AppConfig config, FileIndexer indexer)
    {
        _config = config;
        _indexer = indexer;
        _listener.Prefixes.Add($"http://localhost:{config.Port}/");
    }

    public void Start()
    {
        _listener.Start();
        Task.Run(Listen);
    }

    private async Task Listen()
    {
        while (true)
        {
            var ctx = await _listener.GetContextAsync();
            _ = Task.Run(() => ApiRouter.Route(ctx, _config, _indexer));
        }
    }
}
