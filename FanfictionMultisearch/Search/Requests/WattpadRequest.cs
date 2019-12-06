using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FanfictionMultisearch.Search.Requests
{
    public class WattpadRequest : RequestBase
    {
        private readonly string link_base = "http://wattpad.com";

        private readonly string request_base = "https://www.wattpad.com/search/";
        private readonly string tags_base = "%20%23";

        public WattpadRequest() : base()
        {

        }

        public WattpadRequest(Search search) : base(search)
        {

        }
        private string CompileTagString()
        {
            string tags = string.Empty;
            Characters.ForEach(x => tags += tags_base + x);
            Fandoms.ForEach(x => tags += tags_base + x); // all three take each item and add it to the tags string with tags_base before it
            OtherTags.ForEach(x => tags += tags_base + x);
            tags.Trim();
            return tags;
        }

        public override string GetRequestString()
        {
            return request_base + Query + CompileTagString();
        }

        public override List<FanFic> DecodeHTML()
        {
            List<FanFic> fics = new List<FanFic>();



            return fics;
        }
    }
}
