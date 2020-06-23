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
    public enum SearchDirection
    {
        Descending = 0,
        Ascending
    }

    public enum SearchBy
    {
        BestMatch = 0,
        Likes,
        Views,
        UpdatedDate,
        PublishedDate,
        Comments
    }

    public enum Raiting
    {
        Any = 0,
        General,
        Teen,
        Mature,
        Explicit,
        NotExplicit
    }

    public enum FicStatus
    {
        Any = 0, // implys no preference
        InProgress,
        Complete
    }

    public enum CrossoverStatus
    {
        Any = 0,
        NoCrossover,
        Crossover
    }

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

        public void NewSearch(string basic = "", string title = "", string authors = "", string characters = "", string relationships = "",
            string fandoms = "", string otherTags = "",
            Tuple<int, int> likes = null, Tuple<int, int> views = null, Tuple<int, int> comments = null, Tuple<int, int> wordCount = null,
            Tuple<DateTime, DateTime> updateBefore = default, Tuple<DateTime, DateTime> publishBefore = default,
            SearchDirection direction = SearchDirection.Descending, SearchBy searchBy = SearchBy.Likes, Raiting raiting = Raiting.Any,
            FicStatus ficStatus = FicStatus.Any, CrossoverStatus crossover = CrossoverStatus.Any)
        {
            ActiveSearch = new Search // Build search from passed form results
            {
                Basic = basic,
                Title = title,
                Authors = authors,
                Characters = characters,
                Relationships = relationships,
                Fandoms = fandoms,
                OtherTags = otherTags,
                Likes = likes,
                Views = views,
                Comments = comments,
                WordCount = wordCount,
                UpdateBefore = updateBefore,
                PublishBefore = publishBefore,
                Direction = direction,
                SearchFicsBy = searchBy,
                FicRaiting = raiting,
                Status = ficStatus,
                Crossover = crossover
            };

            ActiveSearch.BuildRequests(); // Configure the seprate requests
            MakeWebRequest(); // Make a new web request
        }

        private void MakeWebRequest()
        {
            var used = ActiveSearch.BuildUsedRequestArray();
            foreach (RequestBase request in ActiveSearch.Requests)
            {
                string rstring = request.GetRequestString(used);
                if (rstring != null && rstring != "")
                {
                    var html = GetHtml(rstring);
                    
                    request.Result.LoadHtml(html);
                    request.FixBasicErrors();

                    FanFics.AddRange(request.DecodeHTML());
                }
            }
        }

        public static string GetHtml(string url)
        {
            string html = string.Empty;

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }

            return html;
        }
    }
}