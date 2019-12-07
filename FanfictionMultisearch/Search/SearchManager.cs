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
        Ascending,
        Descending
    }

    public enum SearchBy
    {
        Likes,
        Views,
        UpdatedDate,
        PublishedDate,
        Comments
    }

    public enum Raiting
    {
        General,
        Teen,
        Mature,
        Explicit,
        None // implys no raiting filter
    }

    public enum FicStatus
    {
        None = -1, // implys no preference
        InProgress,
        Complete
    }

    public enum CrossoverStatus
    {
        None = -1,
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
            string updateBefore = "0", string publishBefore = "0", // where 0 is no limit
            SearchDirection direction = SearchDirection.Descending, SearchBy searchBy = SearchBy.Likes, Raiting raiting = Raiting.None,
            FicStatus ficStatus = FicStatus.None, CrossoverStatus crossover = CrossoverStatus.None)
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
                UpdateBefore = updateBefore, // TODO 
                PublishBefore = publishBefore, // Rework this and the one above to be DateTimes
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
            foreach (RequestBase request in ActiveSearch.Requests)
            {
                string rstring = request.GetRequestString();
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
