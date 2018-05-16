using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace captivate_express_webapp.Utils.GetResponse
{
  public class ApiResult
  {
    public HttpStatusCode Code { get; set; }

    public string Message { get; set; }

    public string Output { get; set; }

    public string Data { get; set; }
  }
}
