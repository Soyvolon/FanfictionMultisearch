using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using FanfictionMultisearch.Search;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using X.PagedList;
using System.IO;
using FanfictionMultisearch.NoticeData;
using Newtonsoft.Json;

namespace FanfictionMultisearch.Pages
{
    public class IndexModel : PageModel
    {
        public SearchManager SearchManager = new SearchManager();
        private readonly ILogger<IndexModel> _logger;

        #region String Search Properties
        [BindProperty]
        public string BasicSearch { get; set; }

        [BindProperty]
        public string TitleStr { get; set; }
        [BindProperty]
        public string AuthorStr { get; set; }
        [BindProperty]
        public string CharacterStr { get; set; }
        [BindProperty]
        public string RelationshipStr { get; set; }
        [BindProperty]
        public string FandomsStr { get; set; }
        [BindProperty]
        public string OtherTagsStr { get; set; }
        #endregion

        #region Numerical Serach Properties
        [BindProperty]
        public int LikesMin { get; set; }
        [BindProperty]
        public int LikesMax { get; set; }
        [BindProperty]
        public int ViewsMin { get; set; }
        [BindProperty]
        public int ViewsMax { get; set; }
        [BindProperty]
        public int CommentsMin { get; set; }
        [BindProperty]
        public int CommentsMax { get; set; }
        [BindProperty]
        public int WordCountMin { get; set; }
        [BindProperty]
        public int WordCountMax { get; set; }
        #endregion

        #region DateTime Variables
        [BindProperty]
        public string UpdateBeforeStr { get; set; }
        [BindProperty]
        public string PublishBeforeStr { get; set; }
        #endregion

        #region Enum Variables
        [BindProperty]
        public SearchDirection SearchDir { get; set; }
        [BindProperty]
        public SearchBy SearchFicsBy { get; set; }
        [BindProperty]
        public Raiting FicRating { get; set; }
        [BindProperty]
        public FicStatus Status { get; set; }
        [BindProperty]
        public CrossoverStatus Crossover { get; set; }
        #endregion

        public HomepageNotice Notice { get; private set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            if (!Directory.Exists("NoticeData"))
                Directory.CreateDirectory("NoticeData");

            if(!System.IO.File.Exists("NoticeData/homepage_notice.json"))
            {
                System.IO.File.WriteAllText("NoticeData/homepage_notice.json",
                    JsonConvert.SerializeObject(new HomepageNotice
                    {
                        Content = ""
                    }));
            }

            var json = "";

            using (FileStream fs = new FileStream("NoticeData/homepage_notice.json", FileMode.OpenOrCreate))
            using (StreamReader sr = new StreamReader(fs))
                json = sr.ReadToEnd();

            Notice = JsonConvert.DeserializeObject<HomepageNotice>(json);
        }

        public void OnPost()
        {
            var updateTime = ParseToDateTime(UpdateBeforeStr);
            var publishTime = ParseToDateTime(PublishBeforeStr);

            SearchManager.NewSearch(BasicSearch, TitleStr, AuthorStr, CharacterStr, RelationshipStr, FandomsStr, OtherTagsStr,
                new Tuple<int, int>(WordCountMin, WordCountMax), new Tuple<int, int>(ViewsMin, ViewsMax),
                new Tuple<int, int>(CommentsMin, CommentsMax), new Tuple<int, int>(WordCountMin, WordCountMax),
                updateTime, publishTime, SearchDir, SearchFicsBy, FicRating, Status, Crossover);
        }

        private Tuple<DateTime, DateTime> ParseToDateTime(string input)
        {
            if (input is null || input == "") // No input, return defaults
                return new Tuple<DateTime, DateTime>(default, default);

            DateTime dateStart = default;
            DateTime dateEnd = default;
            int start = 0;
            int end = 0;
            // Follows rules for AO3 Date Search
            // Split the item by blank spaces.
            var items = input.Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries);
            // Create a counter to keep track of position
            int c = 0;

            // Time to find the actuall timeframe, whats the start and end numbers.
            if (items.Length > 0)
            {
                // See if the first item is a number.
                if (int.TryParse(items[c], out int res1))
                {
                    // If it is, assign it to the start positon.
                    start = res1;
                }
                else
                {
                    // See if the first item is a symbol.
                    if (items[c].StartsWith("<") || items[c].StartsWith(">"))
                    { // If it is, split the grouping into symbol and number.
                        var startStr = items[c][0..1];
                        var dataStr = items[c][1..];
                        // And find out if the number is actually a number.
                        if (int.TryParse(dataStr, out int res2))
                        { // If it is, test for the direction of the symbol
                            if (startStr.Equals("<"))
                            { // If its less than, start becomes the result and end is 0.
                                start = res2;
                                end = 0;
                            }
                            else
                            { // If its greater than, end becomes the result and start is 0.
                                start = 0;
                                end = res2;
                            }
                        }
                        else
                        { // If it is not a number, fail the check and reutrn the default value.
                            return default;
                        }
                    } // Otherwise, see if it is a range and contains the range operator
                    else if (items[c].Contains("-"))
                    { // if it is, split the item by the range operator
                        var split = items[c].Split("-", StringSplitOptions.RemoveEmptyEntries);
                        var startStr = split[0];
                        var endStr = split[1];
                        // Check both the start number and end number to ensure they are numbers.
                        if (int.TryParse(startStr, out int resStart))
                        {
                            if (int.TryParse(endStr, out int resEnd))
                            { // Assign the start to start, and end to end.
                                start = resEnd;
                                end = resStart;
                            }
                        } // If either fails, ignore it, they both wind up being zero and default will be returned later.
                    }
                    else
                    { // lastly, check to see if there were spaces between the range operator.
                        if(input.Contains("-"))
                        { // if there was, split by the range operator and find the two numerical values.
                            string startStr;
                            string endStr;
                            try
                            {
                                if (items[1].Contains("-"))
                                { // We know where the dash is, so find the start and end values.
                                    startStr = items[0];
                                    // If the dash item is also the second number, because it starts with a dash but does not equal it,
                                    if (items[1].StartsWith("-") && !items[1].Equals("-"))
                                    { // Then substring the dash from the item.
                                        endStr = items[1][1..];
                                        c = 1; // Adjust c for accuracy
                                    }
                                    else
                                    { // Otherwise, use the next item.
                                        endStr = items[2];
                                        c = 2; // Adjust c for accuracy
                                    }
                                }
                                else
                                { // The dash is in the wrong spot, so ignore this section. Return default.
                                    return default;
                                }
                            }
                            catch (IndexOutOfRangeException)
                            { // Looks like there was some missing data, return default
                                return default;
                            }

                            // Check both the start number and end number to ensure they are numbers.
                            if (int.TryParse(startStr, out int resStart))
                            {
                                if (int.TryParse(endStr, out int resEnd))
                                { // Assign the start to start, and end to end.
                                    start = resEnd;
                                    end = resStart;
                                }
                            } // If either fails, ignore it, they both wind up being zero and default will be returned later.
                        }
                        // if there was not, return default.
                        return default;
                    }
                }
            }

            // If nothing was changed from start, a value was not found. Return the default value.
            if (start == 0 && end == 0) return default;

            c++; // Moving to the next part

            TimeSpan startSpan;
            TimeSpan endSpan;

            // Time to find the span, is it months, days, weeks, etc?
            if(items.Length > c)
            {
                var edited = items[c].Trim().ToLower();

                if (edited.Contains("week"))
                {
                    startSpan = TimeSpan.FromDays(start * 7);
                    endSpan = TimeSpan.FromDays(end * 7);
                }
                else if (edited.Contains("month"))
                {
                    startSpan = TimeSpan.FromDays(start * 30);
                    endSpan = TimeSpan.FromDays(end * 30);
                }
                else if (edited.Contains("year"))
                {
                    startSpan = TimeSpan.FromDays(start * 365);
                    endSpan = TimeSpan.FromDays(end * 365);
                }
                else
                { // Nothing matches, assume days
                    startSpan = TimeSpan.FromDays(start);
                    endSpan = TimeSpan.FromDays(end);
                }
            }
            else
            { // there is no time modifier, so assume days.
                startSpan = TimeSpan.FromDays(start);
                endSpan = TimeSpan.FromDays(end);
            }

            if (startSpan == TimeSpan.Zero)
            {
                dateStart = default;
            }
            else
            {
                dateStart = DateTime.Now - startSpan;
            }

            if (endSpan == TimeSpan.Zero)
            {
                dateEnd = default;
            }
            else
            {
                dateEnd = DateTime.Now - endSpan;
            }

            return new Tuple<DateTime, DateTime>(dateStart, dateEnd);
        }

        /*
        private static Tuple<int, int> ParseToDualInts(string s)
        {
            var parts = s.Trim().Split('-', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length <= 1)
                if (int.TryParse(parts[0], out int res))
                    return new Tuple<int, int>(0, res);

            if (int.TryParse(parts[0], out int resLower))
                if (int.TryParse(parts[1], out int resHigher))
                    return new Tuple<int, int>(resLower, resHigher);
                else
                    return new Tuple<int, int>(0, resLower);

            return new Tuple<int, int>(0, 0);
        }
        */
    }
}
