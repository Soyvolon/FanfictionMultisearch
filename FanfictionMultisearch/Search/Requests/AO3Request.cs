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
        private readonly string request_body = "https://archiveofourown.org/works/search?utf8=%E2%9C%93&work_search%5Bquery%5D=";        
        private readonly string page_request_body = "page=";

        public AO3Request() : base()
        {

        }

        public AO3Request(Search search) : base(search)
        {
            // Archive specefic features
        }

        public override string GetRequestString()
        {
            return $"{request_body}{Query}";
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
