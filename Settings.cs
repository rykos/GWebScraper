namespace webScraper
{
    public struct Settings
    {
        public bool minimizeJson { get; set; }
        public string fetchSitesLinksFile { get; set; }
        public string inputPath;
        public string outputPath;

        //Return input path if specified, else return default directory
        public string GetInputPath(){
            return (inputPath == null) ? "sites" : inputPath;
        }
    }
}