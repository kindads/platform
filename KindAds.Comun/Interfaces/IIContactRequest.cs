using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Common.Interfaces
{
  public interface IIContactRequest : IRequest
  {
    string ApiVersion { set; get; }
    string ApiAppId { set; get; }

    string ApiUserName { set; get; }

    string ApiUserPassword { set; get; }

    string AccountId { set; get; }

    string ClientFolderId { set; get; }

    string BaseUrlProduction { set; get; }
    string BaseUrlSandBox { set; get; }
  }

  public interface IEweberRequest : IRequest
  {
    string ApiVersion { set; get; }
    string ApiAppId { set; get; }

    string ApiUserName { set; get; }

    string ApiUserPassword { set; get; }

    string AccountId { set; get; }

    string ClientFolderId { set; get; }

    string BaseUrlProduction { set; get; }
    string BaseUrlSandBox { set; get; }
  }
}
