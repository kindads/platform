using KindAds.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace KindAds.Common.Partners.IContact
{
    public class IContactGetListsResponse : IResponse
    {
        public string Id { set; get; }

        public HttpStatusCode StatusCode { set; get; }

        public List<IContactGetListResponse> lists { set; get; }

        public IContactGetListsResponse()
        {
            lists = new List<IContactGetListResponse>();
        }
    }
}