namespace webScraper
{
    public struct Settings
    {
        public bool minimizeJson { get; set; }
        public string fetchSitesLinksFile { get; set; }
        public string inputPath;
        public string outputPath;

        //Return input path if specified, else return default
        public string GetInputPath()
        {
            return (inputPath == "" || inputPath == null) ? "sites" : inputPath;
        }

        public string GetInputFilePath()
        {
            return (inputPath == "" || inputPath == null) ? "links.scrap" : inputPath;
        }
    }
}