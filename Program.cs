using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using GlobalVariables;

namespace webScraper
{
    class Program
    {
        static readonly string[] Requirements = LoadRequirements();

        static void Main(string[] args)
        {
            RebuildDependables();
            string[] filePaths = Directory.GetFiles("sites");
            if (filePaths.Count() < 1)
            {
                throw new System.Exception("There are no sites to scrap in 'sites' folder at root directory");
            }
            List<Dictionary<string, string>> nodesDataList = new List<Dictionary<string, string>>();
            foreach (string filePath in filePaths)
            {
                try
                {
                    Dictionary<string, string> skimmedWebsite = ScrapWebsiteAt(filePath);
                    if (skimmedWebsite.Keys.Count < Requirements.Count())
                    {
                        throw new System.Exception();
                    }
                    nodesDataList.Add(skimmedWebsite);
                }
                catch
                {
                    Console.WriteLine("Failed to scrap site: " + filePath);
                }
            }
            string json = JsonSerializer.Serialize(nodesDataList, new JsonSerializerOptions() { WriteIndented = true });
            File.WriteAllText("JsonData.json", json);
        }

        private static Dictionary<string, string> ScrapWebsiteAt(string path)
        {
            Dictionary<string, string> scrappedData = new Dictionary<string, string>();
            HtmlDocument doc = new HtmlDocument();
            doc.Load(path);
            HtmlNodeCollection appDataNodes = doc.DocumentNode.SelectNodes(Variables.AppDataNode);
            foreach (HtmlNode node in appDataNodes)
            {
                HtmlNode currentNode = node.SelectSingleNode(Variables.AppDataCell);
                string currentNodeName = currentNode.InnerText;
                if (Requirements.Contains(currentNodeName))//Data we are looking for
                {
                    HtmlNode informationNode = node.SelectSingleNode(Variables.AppDataCellCenter);
                    string informationNodeText = FormatScrapedString(informationNode.InnerText);
                    scrappedData.Add(currentNodeName, informationNodeText); //Add result to data
                }
            }
            scrappedData.Add("Rating", doc.DocumentNode.SelectSingleNode(Variables.AppRating).InnerText);
            scrappedData.Add("Ratings", doc.DocumentNode.SelectSingleNode(Variables.AppRatingsAmount).InnerText);
            scrappedData.Add("Name", FormatScrapedString(doc.DocumentNode.SelectSingleNode(Variables.AppName).InnerText));
            return scrappedData;
        }

        private static string FormatScrapedString(string input)
        {
            Regex regex = new Regex("[ ]{2,}|\\n|\\u002B", RegexOptions.None);
            return regex.Replace(input, " "); ;
        }

        private static string[] LoadRequirements()
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
        private static void RebuildDependables()
        {
            if (!Directory.Exists("sites"))
            {
                Directory.CreateDirectory("sites");
            }
        }
    }
}