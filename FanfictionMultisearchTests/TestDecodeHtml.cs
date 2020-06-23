using System;
using System.Collections.Generic;
using System.Text;
using FanfictionMultisearch.Search.Requests;
using FanfictionMultisearch.Search;
using Xunit;
using System.Linq;

namespace FanfictionMultisearchTests
{
    public class TestDecodeHtml
    {
        [Fact(DisplayName = "Decode HTML for AO3")]
        public void TestAO3HTMLDecode()
        {
            var r = new AO3Request
            {
                Query = "Story"
            };

            r.Result.LoadHtml(SearchManager.GetHtml(r.GetRequestString(new bool[18] {true, true, true, true, true,
                    true, true, true, true, true, true, true, true, true,
                    true, true, true, true})));

            r.FixBasicErrors();

            Assert.NotNull(r.DecodeHTML());
        }

        [Fact(DisplayName = "Decode HTML for Fanfiction")]
        public void TestFanfictionNetTMLDecode()
        {
            var r = new FanfictionRequest
            {
                Query = "Story"
            };

            r.Result.LoadHtml(SearchManager.GetHtml(r.GetRequestString(new bool[18] {true, true, true, true, true,
                    true, true, true, true, true, true, true, true, true,
                    true, true, true, true})));

            r.FixBasicErrors();

            Assert.NotNull(r.DecodeHTML());
        }

        [Fact(DisplayName = "Decode HTML for Wattpad")]
        public void TestWattpadHTMLDecode()
        {
            var r = new WattpadRequest
            {
                Query = "This is a story"
            };

            r.Result.LoadHtml(SearchManager.GetHtml(r.GetRequestString(new bool[18] {true, true, true, true, true,
                    true, true, true, true, true, true, true, true, true,
                    true, true, true, true})));

            r.FixBasicErrors();

            Assert.NotNull(r.DecodeHTML());
        }
    }
}
