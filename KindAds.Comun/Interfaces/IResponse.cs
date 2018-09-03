using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Common.Interfaces
{
  public interface IResponse
  {
    string Id { set; get; }

    HttpStatusCode StatusCode { set; get; }
  }
}
