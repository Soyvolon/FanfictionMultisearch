using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FanfictionMultisearch.Search.Requests;
using HtmlAgilityPack;

namespace FanfictionMultisearch.Search
{
    public class SearchManager
    {
        public int ItemsPerPage { get; set; }
        public int PageNumber { get; set; }
        public List<FanFic> FanFics { get; set; }
        public Search ActiveSearch { get; private set; }
        public List<FanFic> GetCurrentResults()
        {
            FanFics.Sort((x, y) => x.Likes.CompareTo(y.Likes));
            FanFics.Reverse(); // Sorts fics by highest likes -- should be an option to change eventualy
            return FanFics;
        }

        public SearchManager(int itemsPerPage = 20)
        {
            FanFics = new List<FanFic>();
            PageNumber = 0; // start on first page
            ItemsPerPage = itemsPerPage;
        }

        public async Task NewSearch(string basicSearch)
        {
            ActiveSearch = new Search // Build search from passed form results
            {
                BasicSearch = basicSearch
            };

            ActiveSearch.BuildRequests(); // Configure the seprate requests
            await MakeWebRequest().ConfigureAwait(true); // Make a new web request
        }

        private async Task MakeWebRequest()
        {

            HtmlWeb web = new HtmlWeb();

            foreach (RequestBase request in ActiveSearch.Requests)
            {
                string rstring = request.GetRequestString();
                if (request.GetType() == typeof(FanfictionRequest))
                {
                    string html = string.Empty;

                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(rstring);
                    req.AutomaticDecompression = DecompressionMethods.GZip;

                    using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
                    using (Stream stream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        html = reader.ReadToEnd();
                    }

                    request.Result = new HtmlDocument();
                    request.Result.LoadHtml(html);

                    FanFics.AddRange(request.DecodeHTML());
                }
                else if (rstring != null && rstring != "")
                {
                    HtmlDocument htmlDoc = await web.LoadFromWebAsync(rstring).ConfigureAwait(true);
                    request.Result = htmlDoc;
                    FanFics.AddRange(request.DecodeHTML());
                }
            }
        }
    }
}
