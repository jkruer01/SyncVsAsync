using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HtmlAgilityPack;

namespace SyncVsAsync.Services
{
    public class SyncService
    {
        public List<string> Search()
        {
            var bingUrls = getBingUrls();
            return bingUrls.SelectMany(CrawlBingSearchPage).ToList();
        }

        private List<string> CrawlBingSearchPage(string url)
        {
            var result = new List<string>
            {
                url
            };

            var doc = LoadDocument(url);
            var searchResultsGrid = GetSearchResultsGrid(doc);
            if (searchResultsGrid == null) return result;

            var searchResultsLinks = GetSearchResultsLinks(searchResultsGrid);

            foreach (var linkUrl in searchResultsLinks)
            {
                result.Add(linkUrl);
                LoadDocument(linkUrl);
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

        private HtmlDocument LoadDocument(string url)
        {
            try
            {
                using (var client = new WebClient())
                {
                    var content = client.DownloadString(url);
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
        }
    }
}
