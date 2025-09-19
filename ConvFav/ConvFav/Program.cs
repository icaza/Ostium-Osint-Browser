using CsvHelper;
using CsvHelper.Configuration;
using HtmlAgilityPack;
using System.Data.SQLite;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

internal class Program
{
    static string loadHTML = "";
    static readonly string databasePath = "output.db";
    static readonly string csvFilePath = "output.csv";
    static readonly Regex SpecialCharsRegex = new("[^a-zA-Z0-9_]", RegexOptions.Compiled);

    static void Main()
    {
        Console.Title = GetAppTitle();

        SelectChoice();

        Console.Clear();

        try
        {
            var data = ExtractDataFromHtml(loadHTML);
            WriteToCsv(csvFilePath, data);
            WriteToDB();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Operation completed successfully. Data saved to: " + databasePath);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: " + ex.Message);
        }
        finally
        {
            Console.ResetColor();
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    static string GetAppTitle()
    {
        Assembly thisAssem = typeof(Program).Assembly;
        AssemblyName thisAssemName = thisAssem.GetName();
        Version ver = thisAssemName.Version;
        return $"{thisAssemName.Name} {ver}";
    }

    static void SelectChoice()
    {
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine(new string('#', 63));
        Console.WriteLine($"        {GetAppTitle()} by Icaza Media");
        Console.WriteLine(new string('#', 63));
        Console.WriteLine();

        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n   [Insert HTML file name].html\n");
            Console.Write("   |>>  ");
            Console.ForegroundColor = ConsoleColor.Green;

            string input = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid input. Please enter a valid filename.");
                continue;
            }

            if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                Environment.Exit(0);
            }

            if (!File.Exists(input))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Clear();
                Console.WriteLine($"\n   Error: The file \"{input}\" does not exist!");
                continue;
            }

            loadHTML = input;
            break;
        }
    }

    static List<(string Category, string Title, string Link)> ExtractDataFromHtml(string filePath)
    {
        var doc = new HtmlDocument();
        doc.Load(filePath, Encoding.UTF8);

        var data = new List<(string Category, string Title, string Link)>();
        var dtNodes = doc.DocumentNode.SelectNodes("//dt");

        if (dtNodes == null) return data;

        string currentCategory = "No category";

        foreach (var dtNode in dtNodes)
        {
            var h3Node = dtNode.SelectSingleNode("./h3");
            if (h3Node != null)
            {
                currentCategory = h3Node.InnerText.Trim();
            }

            var aNode = dtNode.SelectSingleNode("./a");
            if (aNode != null)
            {
                string title = aNode.InnerText.Trim();
                string href = aNode.GetAttributeValue("href", "N/A");
                data.Add((currentCategory, title, href));
            }
        }

        return data;
    }

    static void WriteToCsv(string filePath, List<(string Category, string Title, string Link)> data)
    {
        try
        {
            using var writer = new StreamWriter(filePath, false, Encoding.UTF8);
            writer.WriteLine("Category,Title,Link");
            foreach (var (Category, Title, Link) in data)
            {
                writer.WriteLine($"\"{Category}\",\"{Title}\",\"{Link}\"");
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to write to CSV file: " + ex.Message);
        }
    }

    static void WriteToDB()
    {
        try
        {
            using var connection = new SQLiteConnection($"Data Source={databasePath};Version=3;");
            connection.Open();
            using var reader = new StreamReader(csvFilePath);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = "," });

            csv.Read();
            csv.ReadHeader();

            while (csv.Read())
            {
                string input = csv.GetField(0);
                string category = SanitizeCategory(input);
                string urlName = csv.GetField(1);
                string urlAddress = csv.GetField(2);
                string dateNow = DateTime.Now.ToString("dd/MM/yyyy");

                string createTableQuery = $@"
                        CREATE TABLE IF NOT EXISTS [{category}] (
                            url_date TEXT,
                            url_name TEXT,
                            url_adress TEXT
                        );";

                using (var cmd = new SQLiteCommand(createTableQuery, connection))
                {
                    cmd.ExecuteNonQuery();
                }

                string insertQuery = $@"INSERT INTO [{category}] (url_date, url_name, url_adress) VALUES (@date, @name, @address)";

                using (var cmd = new SQLiteCommand(insertQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@date", dateNow);
                    cmd.Parameters.AddWithValue("@name", urlName);
                    cmd.Parameters.AddWithValue("@address", urlAddress);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to write to DataBase: " + ex.Message);
        }
    }

    static string SanitizeCategory(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        input = input.Replace(" ", "_");
        input = SpecialCharsRegex.Replace(input, "");

        string[] numToChar = ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J"];
        StringBuilder result = new();

        int total = input.Length;

        Console.Write("\r[                              ] 0%");
        Console.SetCursorPosition(0, Console.CursorTop);

        for (int i = 0; i < total; i++)
        {
            char c = input[i];

            if (char.IsDigit(c))
                result.Append(numToChar[c - '0']);
            else
                result.Append(c);

            DisplayProgress(i + 1, total, input);
        }

        Console.WriteLine();
        return result.ToString();
    }

    static void DisplayProgress(int current, int total, string input)
    {
        int percentage = (current * 100) / total;
        int progressBarWidth = 30;
        int progress = (current * progressBarWidth) / total;

        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write(input + " [");
        Console.Write(new string('=', progress));
        Console.Write(new string(' ', progressBarWidth - progress));
        Console.Write($"] {percentage}%");

        Thread.Sleep(30);
    }
}
