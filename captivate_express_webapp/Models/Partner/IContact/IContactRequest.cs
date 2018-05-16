using captivate_express_webapp.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace captivate_express_webapp.Models.Partner
{
  public class IContactRequest : IIContactRequest
  {
    public string ListId { set; get; }
    public string IdMessage { set; get; }
    public string IdCampaign { set; get; }
    public string BaseUrlProduction { set; get; }
    public string BaseUrlSandBox { set; get; }
    public string ApiVersion { set; get; }

    [Display(Name="API App Id")]
    public string ApiAppId { set; get; }
    [Display(Name = "API Username")]
    public string ApiUserName { set; get; }
    [Display(Name = "API User password")]
    public string ApiUserPassword { set; get; }

    public string BaseUrl { set; get; }
    [Display(Name = "Account Id")]
    public string AccountId { set; get; }
    [Display(Name = "Client Folder Id")]
    public string ClientFolderId { set; get; }

    public IContactRequest(ProviderEnvironment environment)
    {
      IdCampaign = string.Empty;
      BaseUrlSandBox = "https://app.sandbox.icontact.com/icp";
      BaseUrlProduction = "https://app.icontact.com/icp";

      ApiVersion = "2.2";
      ApiAppId = string.Empty;
      ApiUserName = string.Empty;
      ApiUserName = string.Empty;

      BaseUrl = environment == ProviderEnvironment.Production ? BaseUrlProduction : BaseUrlSandBox;
      AccountId = string.Empty;
      ClientFolderId = string.Empty;
    }
  }

  public enum ProviderEnvironment { SandBox=1, Production=2 }
}
