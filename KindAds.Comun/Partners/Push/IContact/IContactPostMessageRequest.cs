using KindAds.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;

namespace KindAds.Common.Partners.IContact
{
  public class IContactPostMessageRequest : IRequest
  {
    public string messageType { set; get; }

    [ScriptIgnore]
    private string _htmlBody;

    [ScriptIgnore]
    public string BaseUrl { set; get; }

    public int campaignId { set; get; }
   

    public string subject { set; get; }

   
    public string textBody { set; get; }
  
    public IContactPostMessageRequest()
    {
      messageType = "normal";
      subject = "";  
      textBody = string.Empty;
    }
  }
}
