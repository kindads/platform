using KindAds.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KindAds.Models
{
  public class RequestSettings<T> : IRequestSettings<T> where T: IRequest
  {
    public string method { set; get; }
    public string accept { set; get; }
    public string contentType { set; get; }
    public string authorization { set; get; }
    public string data { set; get; }
    public Dictionary<string, string> headers { set; get; }
    public T mailingProvider { set; get; }

    public RequestSettings()
    {
      method = string.Empty;
      accept = "application/json";
      contentType = "application/json";
      authorization = string.Empty;
      headers = new Dictionary<string, string>();
    }

  }
}
