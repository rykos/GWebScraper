using System.IO;
using System.Linq;
using webScraper;
using System.Text.Json;
using System.Collections.Generic;

namespace HandleFiles
{
    public static class FileManagment
    {
        public static void RebuildDependables()
        {
            if (!Directory.Exists("sites"))
            {
                Directory.CreateDirectory("sites");
            }
        }

        public static string[] LoadRequirements()
        {
            if (!File.Exists("Requirements"))
            {
                throw new System.Exception("Requirements file does not exist in root directory");
            }
            string[] reqs = File.ReadAllLines("Requirements").Where(line =>
            {
                return (line[0] == '#') ? false : true;
            }).ToArray();
            if (reqs.Count() < 1)
            {
                throw new System.Exception("There are no information specified to gather");
            }
            return reqs;
        }

        public static string[] LoadLinesFromFile(string path)
        {
            string[] lines = File.ReadAllLines(path);
            return lines;
        }

        public static void LoadSettings(out Settings settings)
        {
            settings = JsonSerializer.Deserialize<Settings>(File.ReadAllText("Settings.json"));
        }

        public static void SaveScrapedDataToFile(List<Dictionary<string, string>> nodesDataList, Settings settings)
        {
            string json = JsonSerializer.Serialize(nodesDataList, new JsonSerializerOptions() { WriteIndented = !settings.minimizeJson });
            File.WriteAllText("JsonData.json", json);
        }
    }
}