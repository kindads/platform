using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KindAds.Common.Partners.IContact
{
  public class IContactPostSendResponse
  {
    public int messageId { set; get; }
    public string scheduledTime { set; get; }

    public string includeListIds { set; get; }

    public string includeSegmentIds { set; get; }

    public string excludeListIds { set; get; }

    public string sendId { set; get; }

    public string recipientCount { set; get; }

    public string status { set; get; }

    public string releasedTime { set; get; }

    public SpamCheck spamCheck { set; get; }
  }
}
