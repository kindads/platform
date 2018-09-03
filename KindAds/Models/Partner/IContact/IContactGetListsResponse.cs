using captivate_express_webapp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace captivate_express_webapp.Models.Partner.IContact
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
