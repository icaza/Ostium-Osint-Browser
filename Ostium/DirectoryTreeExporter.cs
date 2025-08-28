using Newtonsoft.Json;
using System;
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

        void BuildTree(string directoryPath, StringBuilder tree, string indent)
        {
            _ = Array.Empty<string>();
            string[] files = Array.Empty<string>();
            string[] directories;

            try
            {
                directories = Directory.GetDirectories(directoryPath);
            }
            catch (UnauthorizedAccessException)
            {
                tree.AppendLine(indent + "├── [Access denied to directories]");
                return;
            }
            catch (DirectoryNotFoundException)
            {
                tree.AppendLine(indent + "├── [Directory not found]");
                return;
            }
            catch (Exception ex)
            {
                tree.AppendLine(indent + $"├── [Error: {ex.Message}]");
                return;
            }

            try
            {
                files = Directory.GetFiles(directoryPath);
            }
            catch (UnauthorizedAccessException)
            {
                tree.AppendLine(indent + "├── [Access denied to files]");
            }
            catch (Exception ex)
            {
                tree.AppendLine(indent + $"├── [File errors: {ex.Message}]");
            }

            for (int i = 0; i < directories.Length; i++)
            {
                try
                {
                    bool isLastDir = (i == directories.Length - 1) && (files.Length == 0);
                    tree.AppendLine(indent + (isLastDir ? "└── " : "├── ") + Path.GetFileName(directories[i]));

                    string newIndent = indent + (isLastDir ? "    " : "│   ");
                    BuildTree(directories[i], tree, newIndent);
                }
                catch (UnauthorizedAccessException)
                {
                    bool isLastDir = (i == directories.Length - 1) && (files.Length == 0);
                    tree.AppendLine(indent + (isLastDir ? "└── " : "├── ") + Path.GetFileName(directories[i]) + " [Access denied]");
                }
                catch (DirectoryNotFoundException)
                {
                    bool isLastDir = (i == directories.Length - 1) && (files.Length == 0);
                    tree.AppendLine(indent + (isLastDir ? "└── " : "├── ") + Path.GetFileName(directories[i]) + " [Not found]");
                }
                catch (Exception ex)
                {
                    bool isLastDir = (i == directories.Length - 1) && (files.Length == 0);
                    tree.AppendLine(indent + (isLastDir ? "└── " : "├── ") + Path.GetFileName(directories[i]) + $" [Error: {ex.Message}]");
                }
            }

            for (int i = 0; i < files.Length; i++)
            {
                try
                {
                    bool isLastFile = (i == files.Length - 1);
                    tree.AppendLine(indent + (isLastFile ? "└── " : "├── ") + Path.GetFileName(files[i]));
                }
                catch (UnauthorizedAccessException)
                {
                    bool isLastFile = (i == files.Length - 1);
                    tree.AppendLine(indent + (isLastFile ? "└── " : "├── ") + Path.GetFileName(files[i]) + " [Access denied]");
                }
                catch (Exception ex)
                {
                    bool isLastFile = (i == files.Length - 1);
                    tree.AppendLine(indent + (isLastFile ? "└── " : "├── ") + Path.GetFileName(files[i]) + $" [Error: {ex.Message}]");
                }
            }
        }

        object BuildJsonTree(string directoryPath)
        {
            try
            {
                var directoryInfo = new DirectoryInfo(directoryPath);

                var children = new List<object>();

                try
                {
                    var subDirectories = Directory.GetDirectories(directoryPath);
                    foreach (var subDir in subDirectories)
                    {
                        try
                        {
                            var subTree = BuildJsonTree(subDir);
                            children.Add(subTree);
                        }
                        catch (UnauthorizedAccessException)
                        {
                            children.Add(new
                            {
                                Name = Path.GetFileName(subDir),
                                Type = "Directory",
                                Error = "Access denied"
                            });
                        }
                        catch (Exception ex)
                        {
                            children.Add(new
                            {
                                Name = Path.GetFileName(subDir),
                                Type = "Directory",
                                Error = $"Error: {ex.Message}"
                            });
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {}
                catch (Exception)
                {}

                try
                {
                    var files = Directory.GetFiles(directoryPath);
                    foreach (var file in files)
                    {
                        try
                        {
                            children.Add(new
                            {
                                Name = Path.GetFileName(file),
                                Type = "File"
                            });
                        }
                        catch (UnauthorizedAccessException)
                        {
                            children.Add(new
                            {
                                Name = Path.GetFileName(file),
                                Type = "File",
                                Error = "Access denied"
                            });
                        }
                        catch (Exception ex)
                        {
                            children.Add(new
                            {
                                Name = Path.GetFileName(file),
                                Type = "File",
                                Error = $"Error: {ex.Message}"
                            });
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {}
                catch (Exception)
                {}

                return new
                {
                    directoryInfo.Name,
                    Type = "Directory",
                    Children = children
                };
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException ||
                                      ex is DirectoryNotFoundException ||
                                      ex is PathTooLongException)
            {
                return new
                {
                    Name = Path.GetFileName(directoryPath),
                    Type = "Directory",
                    Error = ex.Message,
                    Children = new List<object>()
                };
            }
        }

        XElement BuildXmlTree(string directoryPath)
        {
            try
            {
                var directoryInfo = new DirectoryInfo(directoryPath);
                var directoryElement = new XElement("Directory",
                    new XAttribute("Name", directoryInfo.Name)
                );

                try
                {
                    var subDirectories = Directory.GetDirectories(directoryPath);
                    foreach (var subDir in subDirectories)
                    {
                        try
                        {
                            var subTree = BuildXmlTree(subDir);
                            directoryElement.Add(subTree);
                        }
                        catch (UnauthorizedAccessException)
                        {
                            directoryElement.Add(new XElement("Directory",
                                new XAttribute("Name", Path.GetFileName(subDir)),
                                new XAttribute("Error", "Access denied")
                            ));
                        }
                        catch (Exception ex)
                        {
                            directoryElement.Add(new XElement("Directory",
                                new XAttribute("Name", Path.GetFileName(subDir)),
                                new XAttribute("Error", $"Error: {ex.Message}")
                            ));
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    directoryElement.Add(new XElement("Error", "Access to subdirectories denied"));
                }
                catch (Exception ex)
                {
                    directoryElement.Add(new XElement("Error", $"Error reading directories: {ex.Message}"));
                }

                try
                {
                    var files = Directory.GetFiles(directoryPath);
                    foreach (var file in files)
                    {
                        try
                        {
                            directoryElement.Add(new XElement("File",
                                new XAttribute("Name", Path.GetFileName(file))
                            ));
                        }
                        catch (UnauthorizedAccessException)
                        {
                            directoryElement.Add(new XElement("File",
                                new XAttribute("Name", Path.GetFileName(file)),
                                new XAttribute("Error", "Access denied")
                            ));
                        }
                        catch (Exception ex)
                        {
                            directoryElement.Add(new XElement("File",
                                new XAttribute("Name", Path.GetFileName(file)),
                                new XAttribute("Error", $"Error: {ex.Message}")
                            ));
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    directoryElement.Add(new XElement("Error", "File access denied"));
                }
                catch (Exception ex)
                {
                    directoryElement.Add(new XElement("Error", $"File reading error: {ex.Message}"));
                }

                return directoryElement;
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException ||
                                      ex is DirectoryNotFoundException ||
                                      ex is PathTooLongException)
            {
                return new XElement("Directory",
                    new XAttribute("Name", Path.GetFileName(directoryPath)),
                    new XAttribute("Error", ex.Message)
                );
            }
        }
    }
}
