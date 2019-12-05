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

        public RequestBase()
        {

        }

        public virtual string GetRequestString()
        {
            return null;
        }

        public virtual List<FanFic> DecodeHTML()
        {
            return null;
        }
    }
}
