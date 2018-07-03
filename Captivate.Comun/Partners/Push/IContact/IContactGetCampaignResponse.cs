using Captivate.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Captivate.Common.Partners.IContact
{
  public class IContactGetCampaignResponse : IResponse
  {
    public string Id { set; get; }

    public HttpStatusCode StatusCode { set; get; }

  }
}
