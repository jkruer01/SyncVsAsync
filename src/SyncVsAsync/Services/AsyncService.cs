using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace SyncVsAsync.Services
{
    public class AsyncService : IAsyncService
    {
        public async Task<List<string>> SearchAsync()
        {
            var bingUrls = getBingUrls();

            var result = new List<string>();
            foreach (var bingUrl in bingUrls)
            {
                result.AddRange(await CrawlBingSearchPageAsync(bingUrl));
            }
            return result;
        }

        private async Task<List<string>> CrawlBingSearchPageAsync(string url)
        {
            var result = new List<string>
            {
                url
            };

            var doc = await LoadDocumentAsync(url);
            var searchResultsGrid = GetSearchResultsGrid(doc);
            if (searchResultsGrid == null) return result;

            var searchResultsLinks = GetSearchResultsLinks(searchResultsGrid);
            foreach (var linkUrl in searchResultsLinks)
            {
                result.Add(linkUrl);
                await LoadDocumentAsync(linkUrl);
            }

            return result;
        }

        private static List<string> GetSearchResultsLinks(HtmlNode searchResultsGrid)
        {
            return searchResultsGrid.SelectNodes("//li/h2/a[@href]")
                .Select(link => link.GetAttributeValue("href", null))
                .Where(linkUrl => linkUrl != null
                                  && linkUrl.ToLower().StartsWith("http"))
                .Take(5)
                .ToList();
        }

        private static HtmlNode GetSearchResultsGrid(HtmlDocument doc)
        {
            return doc.DocumentNode
                .Descendants("ol")
                .FirstOrDefault(d => d.Attributes.Contains("id")
                                     && d.Attributes["id"].Value.Contains("b_results"));
        }

        private async Task<HtmlDocument> LoadDocumentAsync(string url)
        {
            try
            {
                using (var client = new WebClient())
                {
                    var content = await client.DownloadStringTaskAsync(url);
                    var doc = new HtmlDocument();
                    doc.LoadHtml(content);
                    return doc;
                }
            }
            catch (Exception)
            {
                //I don't care if it fails for this demo.
                return new HtmlDocument();
            }
        }

        private IEnumerable<string> getBingUrls()
        {
            return new List<string>
            {
                "http://www.bing.com/search?q=C%23&first=0"
            };

            //var searchString = "http://www.bing.com/search?q=C%23&first=";
            //var result = new List<string>();
            //for (var x = 0; x < 13; x = x + 13)
            //{
            //    result.Add($"{searchString}{x}");
            //}
            //return result;
        }
    }
}
