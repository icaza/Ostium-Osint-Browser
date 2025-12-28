using SecureFileExplorer.OSINT.Services;
using SecureFileExplorer.OSINT.Server;

Console.WriteLine("==================================================");
Console.WriteLine("██╗    ██╗███████╗██╗      ██████╗ ██████╗ ███╗   ███╗███████╗");
Console.WriteLine("██║    ██║██╔════╝██║     ██╔════╝██╔═══██╗████╗ ████║██╔════╝");
Console.WriteLine("██║ █╗ ██║█████╗  ██║     ██║     ██║   ██║██╔████╔██║█████╗  ");
Console.WriteLine("██║███╗██║██╔══╝  ██║     ██║     ██║   ██║██║╚██╔╝██║██╔══╝  ");
Console.WriteLine("╚███╔███╔╝███████╗███████╗╚██████╗╚██████╔╝██║ ╚═╝ ██║███████╗");
Console.WriteLine(" ╚══╝╚══╝ ╚══════╝╚══════╝ ╚═════╝ ╚═════╝ ╚═╝     ╚═╝╚══════╝");
Console.WriteLine("==================================================");
Console.WriteLine("  [*] SecureFileExplorer.OSINT - SYSTEM BOOT SEQUENCE INITIATED");
Console.WriteLine("  [*] " + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
Console.WriteLine("==================================================");
Console.WriteLine();

Console.WriteLine("  [>] LOADING CONFIGURATION PARAMETERS...");
Thread.Sleep(500);
var config = AppConfig.Load();
Console.WriteLine($"  [+] ROOT DIRECTORY: {config.RootDirectory}");
Console.WriteLine($"  [+] PORT ASSIGNED: {config.Port}");
Console.WriteLine("  [✓] CONFIGURATION LOADED SUCCESSFULLY");
Console.WriteLine();

Console.WriteLine("  [>] INITIATING FILE INDEXING PROTOCOL...");
Thread.Sleep(300);
var indexer = new FileIndexer(config);
indexer.Build();
var fileCount = indexer.All().Count();
Console.WriteLine($"  [+] FILES INDEXED: {fileCount}");
Console.WriteLine($"  [✓] INDEXING COMPLETE - {fileCount} ENTRIES PROCESSED");
Console.WriteLine();

Console.WriteLine("  [>] ACTIVATING HTTP SERVER MODULE...");
Thread.Sleep(400);
var server = new LocalHttpServer(config, indexer);
server.Start();
Console.WriteLine($"  [+] SERVER STATUS: ONLINE");
Console.WriteLine($"  [+] ACCESS URL: http://localhost:{config.Port}");
Console.WriteLine($"  [+] LISTENING ON PORT: {config.Port}");
Console.WriteLine("  [✓] SERVER OPERATIONAL");
Console.WriteLine();

Console.WriteLine("==================================================");
Console.WriteLine("  SYSTEM READY - AWAITING USER COMMAND");
Console.WriteLine("==================================================");
Console.WriteLine("  [i] PRESS 'ENTER' TO INITIATE SHUTDOWN SEQUENCE");
Console.WriteLine("  [i] PRESS 'CTRL+C' FOR EMERGENCY TERMINATION");
Console.WriteLine("==================================================");
Console.Write("  >>> ");

Console.ReadLine();

Console.WriteLine();
Console.WriteLine("==================================================");
Console.WriteLine("  [>] SHUTDOWN SEQUENCE INITIATED...");
Thread.Sleep(300);
Console.WriteLine("  [>] TERMINATING HTTP SERVER...");
Thread.Sleep(300);
Console.WriteLine("  [>] CLEARING SYSTEM CACHE...");
Thread.Sleep(300);
Console.WriteLine("  [✓] SecureFileExplorer.OSINT - SYSTEM OFFLINE");
Console.WriteLine("==================================================");
