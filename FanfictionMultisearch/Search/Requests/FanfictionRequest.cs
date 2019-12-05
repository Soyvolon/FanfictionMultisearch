﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FanfictionMultisearch.Search.Requests
{
    public class FanfictionRequest : RequestBase
    {
        private readonly string link_base = "http://fanfiction.net";

        //TODO: Expand search results to include more information than just a basic search and page number
        private readonly string request_body = "http://www.fanfiction.net/search/?ready=1";
        private readonly string type_body = "&type=";
        private readonly string keywords_body = "&keywords=";

        public string type { get; set; }
        

        public FanfictionRequest()
        {
            type = "story"; // defaults to a story serach
        }

        private string GetQuery()
        {
            string s = "";
            var words = Query.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            foreach (string word in words)
            {
                if (word != words.Last())
                {
                    s += word + "+";
                }
                else
                {
                    s += word;
                }
            }
            return s;
        }

        public override string GetRequestString()
        {
            return request_body + keywords_body + GetQuery() + type_body + type;
            //return "http://fanfiction.net";
        }

        public override List<FanFic> DecodeHTML()
        {
            List<FanFic> fics = new List<FanFic>();

            var nodes = Result.DocumentNode.SelectNodes("//div[contains(@class, 'z-list')]");

            foreach(var node in nodes)
            {
                FanFic fic = new FanFic();

                try
                {
                    // Get title info
                    var title_node = node.SelectSingleNode(".//a[contains(@class, 'stitle')]");
                    fic.Name = new Tuple<string, string>(title_node.InnerText, link_base + title_node.Attributes["href"].Value);

                    // Get Author info
                    var author_node = node.SelectNodes(".//a")[1]; // Get author info
                    fic.Author = new Tuple<string, string>(author_node.InnerText, author_node.Attributes["href"].Value);

                    // Get fic info
                    var data_node = node.SelectSingleNode(".//div[contains(@class, 'z-padtop2')]");
                    var fic_data = data_node.InnerText.Split("-", StringSplitOptions.RemoveEmptyEntries).ToList();
                    fic_data.ForEach(x => x = x.Trim()); // removing trailing and leading whitespace

                    fic.Fandoms.Add(new Tuple<string, string>(fic_data[0], ""));

                    // Find tag information
                    if(fic_data.Last().Contains("Complete") && fic_data.FindIndex(x => x.Contains("Published")) != fic_data.Count - 2)
                    {
                        // Run if the last item is complete and second to last is not Published date -- means there are tags
                        fic.Tags.Add(new Tuple<string, string>(fic_data[fic_data.Count - 2], ""));
                    }
                    else if (!fic_data.Last().Contains("Published") && !fic_data.Last().Contains("Complete"))
                    {
                        // Run if the last item is not published and the fic is not complete
                        fic.Tags.Add(new Tuple<string, string>(fic_data.Last(), ""));
                    }

                    // Get favorites information
                    var fav_str = fic_data.Find(x => x.Contains("Favs"));
                    var fav_num = fav_str.Split(" ", StringSplitOptions.RemoveEmptyEntries).Last();
                    fav_num = fav_num.Replace(",", "");
                    fic.Likes = Convert.ToInt64(fav_num);

                    // Get description info
                    var desc_data = node.SelectSingleNode(".//div[contains(@class, 'z-padtop')]");
                    fic.Description = desc_data.InnerText.Replace(data_node.InnerText, "");

                    fics.Add(fic);
                }
                catch
                {
                    continue;
                }
            }

            return fics;
        }

    }
}
