using System;
using System.Collections.Generic;
using System.Text;
using FanfictionMultisearch.Search.Requests;
using FanfictionMultisearch.Search;
using Xunit;
using System.Linq;

namespace FanfictionMultisearchTests
{
    public class TestFanfictionHtmlGrab
    {
        [Fact(DisplayName = "Get Fanfiction.net HTML Test")]
        public void TestGrabFromFanfictionNet()
        {
            var r = new FanfictionRequest
            {
                Query = "abcd"
            };

            r.Result.LoadHtml(SearchManager.GetHtml(r.GetRequestString(new bool[18] {true, true, true, true, true,
                    true, true, true, true, true, true, true, true, true,
                    true, true, true, true})));

            r.FixBasicErrors();

            Assert.True(r.Result.ParseErrors.ToList().Count == 0);
        }

        [Fact(DisplayName = "Get ArchiveOfOurOwn HTML Test")]
        public void TestGrabFromAO3()
        {
            var r = new AO3Request
            {
                Query = "abcd"
            };

            r.Result.LoadHtml(SearchManager.GetHtml(r.GetRequestString(new bool[18] {true, true, true, true, true,
                    true, true, true, true, true, true, true, true, true,
                    true, true, true, true})));

            r.FixBasicErrors();

            Assert.True(r.Result.ParseErrors.ToList().Count == 0);
        }

        [Fact(DisplayName = "Get Wattpad HTML Test")]
        public void TestGrabFromWattpad()
        {
            var r = new WattpadRequest
            {
                Query = "abcd"
            };

            r.Result.LoadHtml(SearchManager.GetHtml(r.GetRequestString(new bool[18] {true, true, true, true, true,
                    true, true, true, true, true, true, true, true, true,
                    true, true, true, true})));

            r.FixBasicErrors();

            Assert.True(r.Result.ParseErrors.ToList().Count == 0);
        }
    }
}
