using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace FanfictionMultisearch.Search.Requests
{
    public class RequestBase
    {
        public HtmlDocument Result { get; set; }

        public string Query { get; set; }

        public List<string> Characters { get; set; }
        public List<string> Fandoms { get; set; }
        public List<string> OtherTags { get; set; }

        public RequestBase()
        {
            Result = new HtmlDocument();
            Characters = new List<string>();
            Fandoms = new List<string>();
            OtherTags = new List<string>();
        }

        public RequestBase(Search search) : this() // load attribute lists
        {
            Query = search.Basic;
        }

        public virtual string GetRequestString()
        {
            return null;
        }

        public virtual List<FanFic> DecodeHTML()
        {
            return null;
        }

        public void FixBasicErrors()
        {
            var errors = Result.ParseErrors.ToList();
            foreach(var error in errors)
            {
                switch(error.Code)
                {
                    case HtmlParseErrorCode.TagNotOpened:
                        Result.LoadHtml("<html>\n" + Result.Text);
                        break;
                }
            }
        }
    }
}
