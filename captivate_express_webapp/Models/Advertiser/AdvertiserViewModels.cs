using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using captivate_express_webapp.Models;
using captivate_express_webapp.Models.Message;

namespace captivate_express_webapp.Models.Advertiser
{
  public class HomeProductsViewModel
  {
    public IEnumerable<ProductPurchasedViewModel> ListProducts { set; get; }
    [Display(Name = "Channel")]
    public System.Guid ProductTypeSelect { get; set; }
    public List<PRODUCT_TYPE> listProductType { get; set; }

    [Display(Name = "Category")]
    public short CategoryTypeSelect { get; set; }
    public List<CATEGORY> ListCategory { get; set; }

    [Display(Name = "Tag")]
    public short TagTypeSelect { get; set; }
    public List<TAG> ListTag { set; get; }

    [Display(Name = "Provider")]
    public System.Guid ParterTypeSelect { get; set; }
    public List<PARTNER> ListPartner { set; get; }

    [Display(Name = "Price")]
    public double PriceSelecc { set; get; }
  }

  public class ProductPurchasedViewModel
  {
    public PRODUCT_TYPE ProductType { set; get; }
    public PRODUCT Product { set; get; }
    public SITE Site { set; get; }
    public double Price { set; get; }
    public ICollection<CATEGORY> ListCategory {set; get;}
  }

  public class CreateCampaingModel
  {
    public MessgeViewModels Message { get; set; } = new MessgeViewModels();
    public bool SendEmailAfterSubs { set; get; }
    public bool Url { set; get; }
    public PRODUCT_TYPE ProductType { set; get; }
    public PRODUCT Product { set; get; }
    [Display(Name = "Cost By Push")]
    public double CostByPush { set; get; }
    [Display(Name = "Time Duration")]
    public int TimeDuration { set; get; }
    public CAMPAIGN Campaign { set; get; }
    [Display(Name = "Title")]
    [Required(ErrorMessage ="Title is required")]
    [MaxLength(48, ErrorMessage = "Maximum length is {1} characters")]
    public string TitleText { get; set; }
    [Display(Name = "Message")]
    //[Required(ErrorMessage = "Message is required")]
    [MaxLength(100, ErrorMessage = "Maximum length is {1} characters")]
    public string MessageText { set; get; }
    [AllowHtml]
    [Display(Name = "Message")]
    //[Required(ErrorMessage = "Message is required")]
    [MaxLength(15000, ErrorMessage = "Maximum length is {1} characters")]
    public string MessageTextHtml { set; get; }
    public string UrlText { set; get; }

    [Display(Name = "UTM Source (optional)")]
    public string UTM_Source { get; set; }
    [Display(Name = "UTM Medium (optional)")]
    public string UTM_Medium { get; set; }
    [Display(Name = "UTM Campaign (optional)")]
    public string UTM_Campaign { get; set; }

    [Display(Name = "Title campaign")]
    public string TitleCampaignMC { set; get; }
    [Display(Name = "Subject")]
    public string SubjectMC { set; get; }
    [Display(Name = "From email")]
    public string FromEmailMC { set; get; }
    [Display(Name = "From name")]
    public string FromNameMC { set; get; }

    public Guid IdCampaign { get; set; }

    public string Image { get; set; }

    [Display(Name = "Send to revision")]
    public bool IsSendRevision { get; set; }

    public List<CAMPAIGN_CHAT> ListCampaingMessageChat { get; set; } = new List<CAMPAIGN_CHAT>();

    [DataType(DataType.Date)]
    [Display(Name = "Start date")]
    [Required(ErrorMessage = "Start date is required")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime StartDate { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "End date")]
    [Required(ErrorMessage = "End date is required")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime EndDate { get; set; }

    [Display(Name = "Segment")]
    public string Segment { set; get; }

    [Display(Name = "Subject")]
    public string Subject { set; get; }
  }

}
