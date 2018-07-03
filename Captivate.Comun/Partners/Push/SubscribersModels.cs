using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Captivate.Common.Partners.Push
{
  public class SubscribersModels
  {
    public class VerifyKeyResponse
    {
      public int total_subscribers { get; set; }
      public string url { get; set; }

      public string[] errors { get; set; }
    }

    public class MessageRequest
    {
      public string title { get; set; }
      public string body { get; set; }
      public string landing_page_url { get; set; }
      public UTM utm { get; set; }
      public string image_url { get; set; }
      public METADATA metadata { get; set; }
    }
    public class UTM
    {
      public string source { get; set; }
      public string medium { get; set; }
      public string campaign { get; set; }
    }

    public class METADATA
    {
      public string additionalProp1 { get; set; }
      public string additionalProp2 { get; set; }
      public string additionalProp3 { get; set; }
    }

    public class MessageResponse
    {
      public string uuid { get; set; }
    }
  }
}
