using Captivate.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace Captivate.Common.Partners.IContact
{
 
  public class IContactPostCampaignRequest : IRequest
  {

    public string fromEmail { set; get; }
    public string name { set; get; }

    public string fromName { set; get; }

    public int forwardToFriend { set; get; }

    public int clickTrackMode { set; get; }

    public int useAccountAddress { set; get; }

    public int archiveByDefault { set; get; }

    public int subscriptionManagement { set; get; }

    [ScriptIgnore]
    public string BaseUrl { set; get; }

    public IContactPostCampaignRequest()
    {
      name = string.Empty;
      fromEmail = string.Empty;
      fromName = string.Empty;
      forwardToFriend = 0;
      clickTrackMode = 0;
      useAccountAddress = 1;
      archiveByDefault = 0;
      subscriptionManagement = 0;
      BaseUrl = string.Empty;
    }
  }
}
