using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Captivate.Negocio.Partners.IContact
{
  public class IContactPostCampaignResponse 
  {
    public string campaignId { set; get; }
    public string accountId { set; get; }
    public string clientFolderId { set; get; }
    public string name { set; get; }
    public string description { set; get; }
    public string fromEmail { set; get; }
    public string fromName { set; get; }
    public int forwardToFriend { set; get; }
    public int subscriptionManagement { set; get; }
    public int clickTrackMode { set; get; }
    public int useAccountAddress { set; get; }
    public string street { set; get; }
    public string city { set; get; }
    public string state { set; get; }
    public string zip { set; get; }
    public string country { set; get; }
    public string archiveByDefault { set; get; }
  }
}
