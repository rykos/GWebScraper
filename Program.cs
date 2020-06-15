using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using GlobalVariables;
using System.Net;
using HandleFiles;

namespace webScraper
{
    class Program
    {
        static readonly string[] Requirements = FileManagment.LoadRequirements();//Fields to scrap
        static Settings settings;
        static Raport raport = new Raport();

        static void Main(string[] args)
        {
            FileManagment.LoadSettings(out settings);
            FileManagment.RebuildDependables();
            ParseArgs(args);
            SelectScrapMode();
        }

        private static void SelectScrapMode()
        {
            Console.WriteLine("1/Scrap content inside ./sites\n2/Scrap content online using links file\n");
            int choice;
            do
            {
                int.TryParse(Console.ReadLine(), out choice);
            } while (choice != 1 && choice != 2);
            if (choice == 1)
            {
                LocalScrap();
            }
            else
            {
                OnlineScrap();
            }
        }

        private static void LocalScrap()
        {
            if (!Directory.Exists("sites"))
            {
                Directory.CreateDirectory("sites");
            }
            string[] filePaths = Directory.GetFiles(settings.GetInputPath());
            if (filePaths.Count() < 1)
            {
                throw new System.Exception(string.Format("There are no sites to scrap in {0} folder at root directory", settings.inputPath));
            }
            List<Dictionary<string, string>> nodesDataList = new List<Dictionary<string, string>>();//Scraped data
            raport.AllFiles = filePaths.Count();
            foreach (string filePath in filePaths)
            {
                ProcessFile(LoadSiteFromPath(filePath), ref nodesDataList);
            }
            FileManagment.SaveScrapedDataToFile(nodesDataList, settings);
        }

        private static void OnlineScrap()
        {
            List<Dictionary<string, string>> nodesDataList = new List<Dictionary<string, string>>();//Scraped data
            string[] links = FileManagment.LoadLinesFromFile(settings.GetInputFilePath());
            raport.AllFiles = links.Count();
            foreach (string link in links)
            {
                ProcessFile(LoadSiteFromUrl(link), ref nodesDataList);
            }
            FileManagment.SaveScrapedDataToFile(nodesDataList, settings);
        }

        private static bool ValidNodeData(Dictionary<string, string> nodeData)
        {
            if (nodeData.Keys.Count < Requirements.Count())
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private static bool ProcessFile(HtmlDocument doc, ref List<Dictionary<string, string>> nodesDataList)
        {
            try
            {
                raport.WriteRaport();
                Dictionary<string, string> skimmedWebsite = ScrapWebsite(doc);
                if (skimmedWebsite.Keys.Count < Requirements.Count())
                {
                    throw new System.Exception();
                }
                if (ValidNodeData(skimmedWebsite))
                {
                    nodesDataList.Add(skimmedWebsite);
                    raport.FinishedFile(true);
                    return true;
                }
                else
                {
                    throw new System.Exception();
                }
            }
            catch
            {
                Console.WriteLine("Failed to scrap site");
                raport.FinishedFile(false);
                return false;
            }
        }

        private static HtmlDocument LoadSiteFromUrl(string url)
        {
            HtmlDocument doc = new HtmlDocument();
            WebClient webClient = new WebClient();
            doc.LoadHtml(webClient.DownloadString(url));
            return doc;
        }

        private static HtmlDocument LoadSiteFromPath(string path)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.Load(path);
            return doc;
        }

        private static Dictionary<string, string> ScrapWebsite(HtmlDocument doc)
        {
            Dictionary<string, string> scrappedData = new Dictionary<string, string>();
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
            Regex regex = new Regex("[ ]{2,}|\\n|\\u002B|\\u00A0", RegexOptions.None);
            return regex.Replace(input, " "); ;
        }

        private static void ParseArgs(string[] args)
        {
            if (args.Count() > 0 && args[0] == "-h")
            {
                Console.WriteLine("-i input_path\n-o output_path\n-m minimize json output\n-dm disable minimize json output");
                Environment.Exit(0);
            }
            for (int i = 0; i < args.Count(); i++)
            {
                try
                {
                    switch (args[i])
                    {
                        case "-i"://Links file
                            settings.inputPath = args[i + 1];
                            break;
                        case "-o"://Output path
                            settings.outputPath = args[i + 1];
                            break;
                        case "-m"://Minimize
                            settings.minimizeJson = true;
                            break;
                        case "-dm":
                            settings.minimizeJson = false;
                            break;
                    }
                }
                catch
                {
                    throw new System.Exception("Invalid args");
                }
            }
        }
    }
}