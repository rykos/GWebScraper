using System;
using System.IO;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace webScraper
{
    class Program
    {
        static readonly string[] reqs = LoadReq();

        static void Main(string[] args)
        {
            string[] filePaths = Directory.GetFiles("sites");
            List<Dictionary<string, string>> nodesDataList = new List<Dictionary<string, string>>();
            foreach (string filePath in filePaths)
            {
                try
                {
                    Dictionary<string, string> skimmedWebsite = SkimWebsite(filePath);
                    if(skimmedWebsite.Keys.Count < reqs.Count()){
                        throw new System.Exception();
                    }
                    nodesDataList.Add(skimmedWebsite);
                }
                catch
                {
                    Console.WriteLine("Failed to skimm at: " + filePath);
                }
            }
            string json = JsonSerializer.Serialize(nodesDataList, new JsonSerializerOptions() { WriteIndented = true });
            File.WriteAllText("JsonData.json", json);
        }

        private static Dictionary<string, string> SkimWebsite(string path)
        {
            Dictionary<string, string> nodesData = new Dictionary<string, string>();
            HtmlDocument doc = new HtmlDocument();
            doc.Load(path);
            var masterNodes = doc.DocumentNode.SelectNodes("//div[@class='hAyfc']");
            foreach (var n in masterNodes)
            {
                var node = n.SelectSingleNode(".//div[@class='BgcNfc']");
                if (reqs.Contains(node.InnerText))//Name node
                {
                    string nodeName = node.InnerText;
                    var informationNode = n.SelectSingleNode(".//div[@class='IQ1z0d']//span");
                    Regex regex = new Regex("[ ]{2,}|\\n|\\u002B", RegexOptions.None);
                    string informationNodeText = regex.Replace(informationNode.InnerText, " ");
                    nodesData.Add(node.InnerText, informationNodeText);//Add result to data
                }
            }
            Regex localRegex = new Regex("[ ]{2,}|\\n|\\u002B", RegexOptions.None);
            nodesData.Add("Rating", doc.DocumentNode.SelectSingleNode("//div[@class='BHMmbe']").InnerText);
            nodesData.Add("Ratings", doc.DocumentNode.SelectSingleNode("//span[@class='EymY4b']").InnerText);
            string modifiedString = localRegex.Replace(doc.DocumentNode.SelectSingleNode("//h1[@class='AHFaub']").InnerText, " ");
            nodesData.Add("Name", modifiedString);
            return nodesData;
        }

        private static string[] LoadReq()
        {
            if (!File.Exists("Requirements"))
            {
                throw new System.Exception("Requirements file does not exist in root directory");
            }
            string[] reqs = File.ReadAllLines("Requirements");
            return reqs;
        }
    }
}
