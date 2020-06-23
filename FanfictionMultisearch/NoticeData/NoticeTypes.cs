using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FanfictionMultisearch.NoticeData
{
    public struct HomepageNotice
    {
        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }
    }
}
