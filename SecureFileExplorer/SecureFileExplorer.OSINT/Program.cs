using SecureFileExplorer.OSINT.Server;
using SecureFileExplorer.OSINT.Services;
using System.Text;

class Program
{
    static readonly string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", $"app_{DateTime.Now:yyyyMMdd}.log");

    static void Main()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));

            Console.WriteLine("==================================================");
            Console.WriteLine("██╗    ██╗███████╗██╗      ██████╗ ██████╗ ███╗   ███╗███████╗");
            Console.WriteLine("██║    ██║██╔════╝██║     ██╔════╝██╔═══██╗████╗ ████║██╔════╝");
            Console.WriteLine("██║ █╗ ██║█████╗  ██║     ██║     ██║   ██║██╔████╔██║█████╗  ");
            Console.WriteLine("██║███╗██║██╔══╝  ██║     ██║     ██║   ██║██║╚██╔╝██║██╔══╝  ");
            Console.WriteLine("╚███╔███╔╝███████╗███████╗╚██████╗╚██████╔╝██║ ╚═╝ ██║███████╗");
            Console.WriteLine(" ╚══╝╚══╝ ╚══════╝╚══════╝ ╚═════╝ ╚═════╝ ╚═╝     ╚═╝╚══════╝");
            Console.WriteLine("==================================================");
            Console.WriteLine("  [*] SecureFileExplorer - BOOT SEQUENCE INITIATED");
            Console.WriteLine("  [*] " + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
            Console.WriteLine("==================================================");
            Console.WriteLine();

            LogInfo("Application démarrée");

            Console.WriteLine("  [>] LOADING CONFIGURATION PARAMETERS...");
            Thread.Sleep(500);

            AppConfig config;
            try
            {
                config = AppConfig.Load();
                Console.WriteLine($"  [+] ROOT DIRECTORY: {config.RootDirectory}");
                Console.WriteLine($"  [+] PORT ASSIGNED: {config.Port}");
                Console.WriteLine("  [✓] CONFIGURATION LOADED SUCCESSFULLY");
                Console.WriteLine();
                LogInfo($"Configuration chargée - Root: {config.RootDirectory}, Port: {config.Port}");
            }
            catch (Exception ex)
            {
                LogError("Erreur lors du chargement de la configuration", ex);
                Console.WriteLine("  [✗] ERREUR: Impossible de charger la configuration");
                Console.WriteLine($"  [✗] {ex.Message}");
                WaitForExit();
                return;
            }

            Console.WriteLine("  [>] INITIATING FILE INDEXING PROTOCOL...");
            Thread.Sleep(300);

            FileIndexer indexer;
            int fileCount = 0;
            try
            {
                indexer = new FileIndexer(config);
                indexer.Build();
                fileCount = indexer.All().Count();
                Console.WriteLine($"  [+] FILES INDEXED: {fileCount}");
                Console.WriteLine($"  [✓] INDEXING COMPLETE - {fileCount} ENTRIES PROCESSED");
                Console.WriteLine();
                LogInfo($"Indexation terminée - {fileCount} fichiers");
            }
            catch (UnauthorizedAccessException ex)
            {
                LogError("Accès refusé lors de l'indexation", ex);
                Console.WriteLine("  [✗] ERREUR: Accès refusé au répertoire");
                Console.WriteLine($"  [✗] {ex.Message}");
                WaitForExit();
                return;
            }
            catch (DirectoryNotFoundException ex)
            {
                LogError("Répertoire introuvable", ex);
                Console.WriteLine("  [✗] ERREUR: Le répertoire spécifié n'existe pas");
                Console.WriteLine($"  [✗] {ex.Message}");
                WaitForExit();
                return;
            }
            catch (Exception ex)
            {
                LogError("Erreur lors de l'indexation des fichiers", ex);
                Console.WriteLine("  [✗] ERREUR: Échec de l'indexation");
                Console.WriteLine($"  [✗] {ex.Message}");
                WaitForExit();
                return;
            }

            Console.WriteLine("  [>] ACTIVATING HTTP SERVER MODULE...");
            Thread.Sleep(400);

            LocalHttpServer server;
            try
            {
                server = new LocalHttpServer(config, indexer);
                server.Start();
                Console.WriteLine($"  [+] SERVER STATUS: ONLINE");
                Console.WriteLine($"  [+] ACCESS URL: http://localhost:{config.Port}");
                Console.WriteLine($"  [+] LISTENING ON PORT: {config.Port}");
                Console.WriteLine("  [✓] SERVER OPERATIONAL");
                Console.WriteLine();
                LogInfo($"Serveur démarré sur le port {config.Port}");
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                LogError($"Impossible de démarrer le serveur sur le port {config.Port}", ex);
                Console.WriteLine("  [✗] ERREUR: Le port est déjà utilisé ou inaccessible");
                Console.WriteLine($"  [✗] {ex.Message}");
                WaitForExit();
                return;
            }
            catch (Exception ex)
            {
                LogError("Erreur lors du démarrage du serveur", ex);
                Console.WriteLine("  [✗] ERREUR: Impossible de démarrer le serveur");
                Console.WriteLine($"  [✗] {ex.Message}");
                WaitForExit();
                return;
            }

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
            LogInfo("Arrêt du système initié");
            Thread.Sleep(300);
            Console.WriteLine("  [>] TERMINATING HTTP SERVER...");
            Thread.Sleep(300);
            Console.WriteLine("  [>] CLEARING SYSTEM CACHE...");
            Thread.Sleep(300);
            Console.WriteLine("  [✓] SecureFileExplorer.OSINT - SYSTEM OFFLINE");
            Console.WriteLine("==================================================");
            LogInfo("Application arrêtée normalement");
        }
        catch (Exception ex)
        {
            LogError("Erreur critique non gérée", ex);
            Console.WriteLine();
            Console.WriteLine("==================================================");
            Console.WriteLine("  [✗] ERREUR CRITIQUE");
            Console.WriteLine("==================================================");
            Console.WriteLine($"  {ex.Message}");
            Console.WriteLine("==================================================");
            WaitForExit();
        }
    }

    static void LogInfo(string message)
    {
        Log("INFO", message);
    }

    static void LogError(string message, Exception ex)
    {
        Log("ERROR", $"{message}: {ex.Message}\n{ex.StackTrace}");
    }

    static void Log(string level, string message)
    {
        try
        {
            var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}\n";
            File.AppendAllText(logFilePath, logEntry, Encoding.UTF8);
        }
        catch
        { }
    }

    static void WaitForExit()
    {
        Console.WriteLine();
        Console.WriteLine("  [i] Appuyez sur ENTER pour quitter...");
        Console.ReadLine();
    }
}