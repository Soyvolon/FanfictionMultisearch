using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FanfictionMultisearch.Search.Requests;

namespace FanfictionMultisearch.Search
{
    public class Search
    {
        public string BasicSearch { get; set; }
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
            FanfictionRequest fr = new FanfictionRequest()
            {
                Query = BasicSearch
            };
            UpdateRequestString(fr);
        }

        private void BuildAO3Request()
        {
            AO3Request ar = new AO3Request()
            {
                Query = BasicSearch
            };
            UpdateRequestString(ar);
        }

        private void BuildWattpadRequest()
        {
            WattpadRequest wr = new WattpadRequest()
            {
                Query = BasicSearch
            };
            UpdateRequestString(wr);
        }
    }
}
