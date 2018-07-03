using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Captivate.Common.Partners.IContact
{
  public class IContactPostMessageResponse
  {
    public string messageId { set; get; }
    public int campaignId { set; get; }
    public string subject { set; get; }
    public string messageType { set; get; }
    public string htmlBody { set; get; }
    public string textBody { set; get; }
    public string createDate { set; get; }
    public string clientFolderId { set; get; }
    public string clientId { set; get; }
    public string messageName { set; get; }

    public SpamCheck spamCheck { set; get; }

    public IContactPostMessageResponse()
    {
      spamCheck = new SpamCheck();
    }
  }
}
