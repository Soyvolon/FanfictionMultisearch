using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using FanfictionMultisearch.Search;
using System.Globalization;

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
        [BindProperty]
        public string UpdateBeforeStr { get; set; }
        [BindProperty]
        public string PublishBeforeStr { get; set; }
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

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            
        }

        public void OnPost()
        {
            SearchManager.NewSearch(BasicSearch, TitleStr, AuthorStr, CharacterStr, RelationshipStr, FandomsStr, OtherTagsStr,
                new Tuple<int, int>(WordCountMin, WordCountMax), new Tuple<int, int>(ViewsMin, ViewsMax),
                new Tuple<int, int>(CommentsMin, CommentsMax), new Tuple<int, int>(WordCountMin, WordCountMax));
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
