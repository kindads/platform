using Captivate.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Captivate.Common.Partners.IContact
{
  public class IContactPostCampaignsResponse: IResponse
  {
    public string Id { set; get; }

    public HttpStatusCode StatusCode { set; get; }

    public List<IContactPostCampaignResponse> campaigns { set; get; }
  }
}
