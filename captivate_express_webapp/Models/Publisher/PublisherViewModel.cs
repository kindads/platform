using Captivate.Comun.Models.Entities;
using Captivate.Comun.Partners.Mail.SendinBlue;
using Captivate.Comun.Utils;
using Captivate.Negocio.Partners.IContact;
using captivate_express_webapp.Models.Core;
using captivate_express_webapp.Models.Partner;
using captivate_express_webapp.Utils.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace captivate_express_webapp.Models.Publisher
{
  public class CreateSiteModel
  {
    [Display(Name = "Website Name")]
    [Required(ErrorMessage = "Site Name is required")]
    public string Name { set; get; }

    [Display(Name = "Website Url")]
    [Required(ErrorMessage = "Site Url is required")]
    public string URL { set; get; }

    [Display(Name = "Categories")]
    [Required(ErrorMessage = "Category is required")]
    public short CategoryTypeSelect { get; set; }
    public List<CATEGORY> ListCategory { get; set; }
  }

  public class CreateProductModel
  {
    [Display(Name = "Channel")]
    [Required(ErrorMessage = "Channel is required")]
    public Guid ProductTypeSelect { get; set; }
    public List<PRODUCT_TYPE> listProductType { get; set; }

    [Display(Name = "Provider")]
    [Required(ErrorMessage = "Provider is required")]
    public Guid ParterTypeSelect { get; set; }
    public List<PARTNER> ListPartner { set; get; }

    [Display(Name = "Price in KIND")]
    [Required(ErrorMessage = "Price is required")]
    public double PriceSelecc { set; get; }

    public Guid SiteTypeSelecc { set; get; }

    [Display(Name = "Name")]
    [Required(ErrorMessage = "Name is required")]
    public string Name { set; get; }

    [Display(Name = "API Token")]
    public string Token { set; get; }

    public string ListMCSelecc { set; get; }
    public int TemplateMCSelecc { set; get; }

    public int TypeChannelSelecc { set; get; }

    public Guid ProviderSelecc { set; get; }

    public string ListCMSelecc { set; get; }
    public string ClientCMSelecc { set; get; }
    public string SegmentCMSelecc { set; get; }

    public ProviderAWeberApiResult providerAWeberApiResult;
    public string ListAWSelecc { set; get; }

    public string ListSGSelecc { set; get; }
    public string SenderSGSelecc { set; get; }
    public string UnsubscribeGroupSGSelecc { set; get; }

    [Display(Name = "Url")]
    public string UrlActiveCampaign { set; get; }
    public string ListACSelecc { set; get; }
    public string URLACSelecc { set; get; }

    public string ListGRSelecc { set; get; }
    public string FromFieldGRSelecc { set; get; }

    public Captivate.Negocio.Partners.IContact.IContactRequest IContact { set; get; }

    public SendinBlueCampaignRequest SendinBlue { set; get; }

    public string ListICSelecc { set; get; }

    public string ListSBSelecc { set; get; }

    public string ListAppOSSelecc { set; get; }
    public string AuthAppOSSelecc { set; get; }

    [Display(Name = "Secret Key")]
    public string SecretKeyMJ { set; get; }

    public string ListMJSelecc { set; get; }
    public string SegmentMJSelecc { set; get; }
    public string SecretKeySelecc { set; get; }

    public CreateProductModel()
    {
      IContact = new Captivate.Negocio.Partners.IContact.IContactRequest(ProviderEnvironment.Production);
      SendinBlue = new SendinBlueCampaignRequest();
    }

  }

  public class ShowSitesViewModel
  {
    public List<SiteEntity> ListSitesPending { get; set; }
    public List<SiteEntity> ListSitesVerify { get; set; }

    public int PageSize { get; set; }
    public int TotalRecord { get; set; }
    public int NoOfPages { get; set; }
  }

  public class ShowProductViewModel
  {
    public List<PRODUCT> ListProducts { get; set; }

    public int PageSize { get; set; }
    public int TotalRecord { get; set; }
    public int NoOfPages { get; set; }
  }

  public class TransactionsViewModel
  {
    public List<Models.Wallet.walletTransactions> ListTransactions { get; set; }
  }
}
