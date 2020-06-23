using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace FanfictionMultisearch.Search.Requests
{
    /// <summary>
    /// Holds data to send a GET request to Archiveofourown.org and information to decompile the HTML script into a FanFic object
    /// </summary>
    public class AO3Request : RequestBase
    {
        private readonly string link_base = "http://archiveofourown.org";

        //TODO: Expand search results to include more information than just a basic search and page number
        private readonly string request_body = "https://archiveofourown.org/works/search?utf8=%E2%9C%93";
        private readonly string request_part_basic = "work_search%5Bquery%5D=";
        private readonly string request_part_title = "work_search%5Btitle%5D=";

        private readonly string request_part_authors = "work_search%5Bcreators%5D=";
        private readonly string request_part_characters = "work_search%5Bcharacter_names%5D=";
        private readonly string request_part_fandoms = "work_search%5Bfandom_names%5D=";
        private readonly string request_part_other_tags = "work_search%5Bfreeform_names%5D=";

        private readonly string request_part_likes = "work_search%5Bkudos_count%5D=";
        private readonly string request_part_views = "work_search%5Bhits%5D=";
        private readonly string request_part_comments = "work_search%5Bcomments_count%5D=";
        private readonly string request_part_word_count = "work_search%5Bword_count%5D";

        private readonly string request_part_update_before = "work_search%5Brevised_at%5D=";
        //private readonly string request_part_publish_before = ""; Archive does not have this option.

        private readonly string request_part_sort_by = "work_search%5Bsort_column%5D=";
        private readonly string request_part_sort_dir = "work_search%5Bsort_direction%5D=";
        private readonly string request_part_raiting = "work_search%5Brating_ids%5D=";
        private readonly string request_part_status = "work_search%5Bcomplete%5D=";
        private readonly string request_part_crossover = "work_search%5Bcrossover%5D=";

        private readonly string page_request_body = "page=";

        public AO3Request() : base()
        {

        }

        public AO3Request(Search search) : base(search)
        {
            // Archive specefic features


            switch (search.SearchFicsBy)
            {
                case SearchBy.BestMatch:
                    SortBy = "_score";
                    break;
                case SearchBy.Comments:
                    SortBy = "comments_count";
                    break;
                case SearchBy.Likes:
                    SortBy = "kudos_count";
                    break;
                case SearchBy.Views:
                    SortBy = "hits";
                    break;
                case SearchBy.UpdatedDate:
                    SortBy = "revised_at";
                    break;
                case SearchBy.PublishedDate:
                    SortBy = "created_at";
                    break;
            }

            switch(search.Direction)
            {
                case SearchDirection.Descending:
                    SortDir = "desc";
                    break;
                case SearchDirection.Ascending:
                    SortDir = "asc";
                    break;
            }
        }

        public override string GetRequestString(bool[] usedParts)
        {
            // Alway use the basic request, even if it is blank.
            string request = $"{request_body}&{request_part_basic}{Query}";

            if (usedParts[1])
                request += $"&{request_part_title}{Title}";

            if(usedParts[2])
            {
                if (Authors.Count > 0)
                {
                    request += $"&{request_part_authors}{Authors[0]}";

                    for (int i = 1; i < Authors.Count; i++)
                        request += $"%2C{Authors[i]}";
                }
            }
            if (usedParts[3])
            {
                if (Characters.Count > 0)
                {
                    request += $"&{request_part_characters}{Characters[0]}";

                    for (int i = 1; i < Characters.Count; i++)
                        request += $"%2C{Characters[i]}";
                }
            }
            if (usedParts[4])
            {
                if (Fandoms.Count > 0)
                {
                    request += $"&{request_part_fandoms}{Fandoms[0]}";

                    for (int i = 1; i < Fandoms.Count; i++)
                        request += $"%2C{Fandoms[i]}";
                }
            }
            if (usedParts[5])
            {
                if (OtherTags.Count > 0)
                {
                    request += $"&{request_part_other_tags}{OtherTags[0]}";

                    for (int i = 1; i < OtherTags.Count; i++)
                        request += $"%2C{OtherTags[i]}";
                }
            }

            if (usedParts[6])
                request += $"&{request_part_likes}{Likes}";
            if (usedParts[7])
                request += $"&{request_part_views}{Views}";
            if (usedParts[8])
                request += $"&{request_part_comments}{Comments}";
            if (usedParts[9])
                request += $"&{request_part_word_count}{WordCount}";

            if (usedParts[10])
                request += $"&{request_part_update_before}{UpdateBefore}";

            if (usedParts[12])
                request += $"&{request_part_sort_dir}{SortDir}";
            if (usedParts[13])
                request += $"&{request_part_sort_by}{SortBy}";
            if (usedParts[14])
                request += $"&{request_part_raiting}{Raiting}";
            if (usedParts[15])
                request += $"&{request_part_status}{Status}";
            if (usedParts[16])
                request += $"&{request_part_crossover}{Crossover}";

            return request;
        }
        /// <summary>
        /// Breaks apart the result of the serach and reurns a list of FanFic that were found in the result
        /// </summary>
        /// <returns></returns>
        public override List<FanFic> DecodeHTML()
        {
            var nodes = Result.DocumentNode.SelectNodes("//li[contains(@class, 'work')]");

            List<FanFic> fics = new List<FanFic>();

            if (nodes != null)
            {
                foreach (HtmlNode node in nodes)
                { // TODO: Determine error problems when reading information
                    try
                    {
                        FanFic fic = new FanFic();
                        // Gather story information
                        var header_nodes = node.SelectNodes(".//h4[contains(@class, 'heading')]//a");
                        fic.Title = new Tuple<string, string>(header_nodes[0].InnerText, link_base + header_nodes[0].Attributes["href"].Value);
                        fic.Author = new Tuple<string, string>(header_nodes[1].InnerText, link_base + header_nodes[1].Attributes["href"].Value);

                        try // non-essential
                        {
                            // Get fandom information
                            var fandom_node = node.SelectSingleNode(".//h5[contains(@class, 'fandoms')]");
                            foreach (var fandom in fandom_node.SelectNodes(".//a"))
                            {
                                fic.Fandoms.Add(new Tuple<string, string>(fandom.InnerText, link_base + fandom.Attributes["href"].Value));
                            }
                        }
                        catch { }

                        try // non-essential
                        {
                            // Get tag information
                            var tag_nodes = node.SelectNodes(".//ul[contains(@class, 'tags')]//li");
                            foreach (var tag_node in tag_nodes)
                            {
                                var tag = tag_node.SelectSingleNode(".//a");
                                fic.AddTag(new Tuple<string, string>(tag.InnerText, link_base + tag.Attributes["href"].Value));
                            }
                        }
                        catch { }

                        try // non-essential
                        {
                            // Get fic description
                            var fic_desc = node.SelectSingleNode(".//blockquote[contains(@class, 'summary')]");
                            fic.Description = fic_desc.InnerText;
                        }
                        catch { }

                        try
                        {
                            var fic_likes = node.SelectSingleNode(".//dd[contains(@class, 'kudos')]");
                            fic.Likes = Convert.ToInt64(fic_likes.InnerText);
                        }
                        catch { }

                        fics.Add(fic);
                    }
                    catch
                    {
                        continue; //TODO: Fix the error catching later
                    }
                }
            }

            return fics;
        }
    }
}
