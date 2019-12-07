﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using FanfictionMultisearch.Search;

namespace FanfictionMultisearch.Pages
{
    public class IndexModel : PageModel
    {
        public SearchManager SearchManager = new SearchManager();
        private readonly ILogger<IndexModel> _logger;

        [BindProperty]
        public string BasicSearch { get; set; }
        [BindProperty]
        public string CharacterStr { get; set; }
        [BindProperty]
        public string RelationshipStr { get; set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            
        }

        public void OnPost()
        {
            SearchManager.NewSearch(BasicSearch);
        }
    }
}
