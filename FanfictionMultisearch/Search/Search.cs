using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FanfictionMultisearch.Search.Requests;

namespace FanfictionMultisearch.Search
{
    public class Search
    {
        public string Basic {get; set;}
        public string Title {get; set;}
        public string Authors {get; set;}
        public string Characters {get; set;}
        public string Relationships {get; set;}
        public string Fandoms {get; set;}
        public string OtherTags {get; set;}
        public Tuple<int, int> Likes {get; set;}
        public Tuple<int, int> Views {get; set;}
        public Tuple<int, int> Comments {get; set;}
        public Tuple<int, int> WordCount {get; set;}
        public string UpdateBefore {get; set;}
        public string PublishBefore {get; set;}
        public SearchDirection Direction {get; set;}
        public SearchBy SearchFicsBy {get; set;}
        public Raiting FicRaiting {get; set;}
        public FicStatus Status {get; set;}
        public CrossoverStatus Crossover {get; set;}
        public List<RequestBase> Requests { get; private set; }

        public Search()
        {
            Requests = new List<RequestBase>();
        }

            /// <summary>
            /// Removes old requests of the same request type, adds new reuqest to the list
            /// </summary>
            /// <param name="request">New Request</param>
            public void UpdateRequestString(RequestBase request)
        {
            int i = Requests.FindIndex(x => x.GetType() == request.GetType());
            if (i != -1) Requests.RemoveAt(i); // if there is an item, remove it.
            Requests.Add(request);
        }
        /// <summary>
        /// Tells the Search to compile the seprate search requests for different platforms
        /// </summary>
        public void BuildRequests()
        {
            BuildFanficRequest();
            BuildAO3Request();
            BuildWattpadRequest();
        }

        private void BuildFanficRequest()
        {
            FanfictionRequest fr = new FanfictionRequest(this);
            UpdateRequestString(fr);
        }

        private void BuildAO3Request()
        {
            AO3Request ar = new AO3Request(this);
            UpdateRequestString(ar);
        }

        private void BuildWattpadRequest()
        {
            WattpadRequest wr = new WattpadRequest(this);
            UpdateRequestString(wr);
        }
    }
}
