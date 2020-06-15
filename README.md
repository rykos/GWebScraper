# GWebScraper
Provides information on apps released on google play by scraping corresponding website.
It supports local files as well as fetching website directly from url.

## Usage
Either provide websites as .html files to sites directory or provide file with links to desired websites.
Settings.json specify where file containing links is located and json minimize state
```json
{
    "fetchSitesLinksFile": "links.scrap",
    "minimizeJson": true
}
```
Now to start simply run the app
```bash
./webScraper
./webScraper -h # for help
./webScraper -i /links.txt -o /output.json -m # Takes external links file and writes minimized output to external location
```
Output is saved in root directory as JsonData.json by default
