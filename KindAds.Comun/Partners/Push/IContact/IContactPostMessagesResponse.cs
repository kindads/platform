using KindAds.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace KindAds.Common.Partners.IContact
{
  public class IContactPostMessagesResponse : IResponse
  {
    public string Id { set; get; }

    public List<IContactPostMessageResponse> messages { set; get; }

    public HttpStatusCode StatusCode { set; get; }

    public IContactPostMessagesResponse()
    {
      messages = new List<IContactPostMessageResponse>();
    }
  }
}
