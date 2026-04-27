using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;

namespace SaintsConstSourceGenerator
{
    public static class Utils
    {
        public static void DebugToFile(string toWrite, [CallerLineNumber] int lineNumber = 0)
        {
            if (!Debug)
            {
                return;
            }
            // #if DEBUG
            if (string.IsNullOrEmpty(_tempFolderPath))
            {
                _tempFolderPath = Path.GetTempPath();
            }

            string tempFilePath = Path.Combine(_tempFolderPath, "SaintsDebug.txt");
            //tempFilePath = "/tmp/SaintsDebug.txt";
            using (StreamWriter writer = new StreamWriter(tempFilePath, true, Encoding.UTF8))
            {
                writer.WriteLine($"[{lineNumber}] {toWrite}");
            }
            // #endif
        }

        public static bool Debug = false;
        private static string _tempFolderPath;

        public static T ParseFile<T>(string filePath)
        {
            IDeserializer deserializer = new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .Build();
            string content = SanitizeUnityYaml(File.ReadAllText(filePath));
            return deserializer.Deserialize<T>(content);
        }

        private static string SanitizeUnityYaml(string content)
        {
            StringBuilder sanitized = new StringBuilder(content.Length);
            using (StringReader reader = new StringReader(content))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("%YAML ") || line.StartsWith("%TAG "))
                    {
                        continue;
                    }

                    if (line.StartsWith("--- "))
                    {
                        sanitized.AppendLine(Regex.Replace(line, @"^---\s+!\S+\s+&\S+\s*$", "---"));
                        continue;
                    }

                    sanitized.AppendLine(line);
                }
            }

            return sanitized.ToString();
        }

        private static readonly char[] Numbers = {'0', '1', '2',  '3', '4', '5', '6', '7', '8', '9'};
        private static readonly Regex InvalidRegex = new Regex("[^a-zA-Z0-9_]+");

        public static string ProperVarName(string tagValue)
        {
            char firstLetter = tagValue[0];
            string prepend = "";
            if (Array.IndexOf(Numbers, firstLetter) >= 0)
            {
                prepend = "_";
            }
            string result = InvalidRegex.Replace(tagValue, "_");

            return $"{prepend}{result}";
        }
    }
}
