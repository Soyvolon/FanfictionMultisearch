using System;
using System.Collections.Generic;

namespace FanfictionMultisearch.Search
{
    public class FanFic
    {
        public Tuple<string, string> Title { get; set; }
        public Tuple<string, string> Author { get; set; }
        public List<Tuple<string, string>> Fandoms { get; set; }
        public List<Tuple<string, string>> Tags { get; set; }
        public long Likes { get; set; }
        public long Views { get; set; }
        public string Description { get; set; }
        public bool PaidStory { get; set; }
        public bool Completed { get; set; }

        public FanFic()
        {
            Tags = new List<Tuple<string, string>>();
            Fandoms = new List<Tuple<string, string>>();
            Likes = 0;
            PaidStory = false;
            Completed = false;
        }

        /// <summary>
        /// Adds a tag to the FanFic's Tag list
        /// </summary>
        /// <param name="tag">Tag to add</param>
        public void AddTag(Tuple<string,string> tag)
        {
            Tags.Add(tag);
        }

        /// <summary>
        /// Removes a tag from the FanFic's tag list
        /// </summary>
        /// <param name="tag">Tag to remove</param>
        /// <returns>True if tag was removed</returns>
        public bool RemoveTag(Tuple<string, string> tag)
        {
            return Tags.Remove(tag);
        }
    }
}