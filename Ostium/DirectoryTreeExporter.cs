using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Ostium
{
    public class DirectoryTreeExporter
    {
        public void ExportDirectoryTree(string directoryPath, string outputFilePath)
        {
            if (Directory.Exists(directoryPath))
            {
                StringBuilder tree = new StringBuilder();
                tree.AppendLine("Directory tree : " + directoryPath);
                tree.AppendLine(new string('=', 50));
                BuildTree(directoryPath, tree, "");

                File.WriteAllText(outputFilePath, tree.ToString(), Encoding.UTF8);
            }
        }

        public void ExportDirectoryTreeAsJson(string directoryPath, string outputJsonPath)
        {
            if (Directory.Exists(directoryPath))
            {
                var directoryStructure = BuildJsonTree(directoryPath);
                string json = JsonConvert.SerializeObject(directoryStructure, Formatting.Indented);
                File.WriteAllText(outputJsonPath, json, Encoding.UTF8);
            }
        }

        public void ExportDirectoryTreeAsXml(string directoryPath, string outputXmlPath)
        {
            if (Directory.Exists(directoryPath))
            {
                XElement xmlTree = BuildXmlTree(directoryPath);
                xmlTree.Save(outputXmlPath);
            }
        }

        private void BuildTree(string directoryPath, StringBuilder tree, string indent)
        {
            string[] directories = Directory.GetDirectories(directoryPath);
            string[] files = Directory.GetFiles(directoryPath);

            for (int i = 0; i < directories.Length; i++)
            {
                bool isLast = (i == directories.Length - 1) && (files.Length == 0);
                tree.AppendLine(indent + (isLast ? "└── " : "├── ") + Path.GetFileName(directories[i]));
                BuildTree(directories[i], tree, indent + (isLast ? "    " : "│   "));
            }

            for (int i = 0; i < files.Length; i++)
            {
                bool isLast = (i == files.Length - 1);
                tree.AppendLine(indent + (isLast ? "└── " : "├── ") + Path.GetFileName(files[i]));
            }
        }

        private object BuildJsonTree(string directoryPath)
        {
            var directoryInfo = new DirectoryInfo(directoryPath);
            return new
            {
                directoryInfo.Name,
                Type = "Directory",
                Children = new List<object>(
                    Directory.GetDirectories(directoryPath)
                    .Select(subDir => BuildJsonTree(subDir))
                    .Concat(
                        Directory.GetFiles(directoryPath)
                        .Select(file => new
                        {
                            Name = Path.GetFileName(file),
                            Type = "File"
                        })
                    )
                )
            };
        }

        private XElement BuildXmlTree(string directoryPath)
        {
            var directoryInfo = new DirectoryInfo(directoryPath);
            return new XElement("Directory",
                new XAttribute("Name", directoryInfo.Name),
                Directory.GetDirectories(directoryPath).Select(subDir => BuildXmlTree(subDir)),
                Directory.GetFiles(directoryPath).Select(file =>
                    new XElement("File", new XAttribute("Name", Path.GetFileName(file))))
            );
        }
    }
}
