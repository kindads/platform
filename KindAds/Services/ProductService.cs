using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Data.Entity;
using KindAds.Models;
using KindAds.Models.Publisher;
using System.Linq.Dynamic;
using KindAds.Utils.Enums;
using System.IO;
using KindAds.Common.Models.Entities;
using KindAds.Negocio;
using KindAds.DataAccess;

namespace KindAds.Services
{
  public class ProductService
  {
    private KindadsEntities _context;


    public ProductService()
    {
      _context = new KindadsEntities();

    }

    

    public bool SaveProduct(Models.Publisher.CreateProductModel _createModel, string UserIdentity, Models.Core.FileUpload fileUpload)
    {    
     //Creamos los repositorios      
      SiteRepository siteRepository = new SiteRepository();
      AspNetUserRepository aspNetUserRepository = new AspNetUserRepository ();
      ProductTypeRepository productTypeRepository = new ProductTypeRepository ();
      PartnerRepository partnerRepository = new PartnerRepository ();
      ProductRepository productRepository = new ProductRepository ();

      //Obtenemos los conjuntos
      List<SiteEntity> sites = siteRepository.GetAll().ToList();
      List<AspNetUserEntity> users = aspNetUserRepository.GetAll().ToList();
      List<ProductTypeEntity> productstype = productTypeRepository.GetAll().ToList();
      List<PartnerEntity> partners = partnerRepository.GetAll().ToList();

      //Obtenemos las entidades deseadas.
      SiteEntity site = (from s in sites
                         where s.IdSite == _createModel.SiteTypeSelecc
                         select s).FirstOrDefault();
      AspNetUserEntity user= (from u in users
                              where u.Id == UserIdentity
                              select u).FirstOrDefault();

      ProductTypeEntity productType= (from pr in productstype
                                      where pr.IdProductType == _createModel.ProductTypeSelect
                                      select pr).FirstOrDefault();
      PartnerEntity partner= (from p in partners
                              where p.IdPartner == _createModel.ParterTypeSelect
                              select p).FirstOrDefault();

      ProductEntity product = new ProductEntity
      {
        //AspNetUser = user,
        AspNetUsers_Id = UserIdentity,
        IdProduct = Guid.NewGuid(),
        Price = _createModel.PriceSelecc,
        ShortDescription = _createModel.Name,
        FullDescription = string.Empty,
        StartTime = Helpers.DateTimeHelper.GetCurrentDateString(),
        EndTime = Helpers.DateTimeHelper.GetCurrentDateString(0, 30),
        RegistrationDate = DateTime.Now,
        SITE_IdSite = site.IdSite,
        PRODUCT_TYPE_IdProductType = productType.IdProductType,
        PARTNER_IdPartner = partner.IdPartner,
        Image = string.Empty,
        IsActive=true,
        IsPremium = _createModel.IsPremium
      };

      if (fileUpload != null)
      {
        product.Image = Helpers.AzureStorageHelper.CreateBlobImageFile(fileUpload.FileData, fileUpload.Filextension);
      }
      else
      {
        product.Image = Helpers.AzureStorageHelper.CreateBlobImageFile(getImageFromUrl(""), ".png");
      }

      
      productRepository.Add(product);

      if (_createModel.ParterTypeSelect.Equals(new Guid(Utils.Constants.PROVIDER_SUBSCRIBERS)) ||
        _createModel.ParterTypeSelect.Equals(new Guid(Utils.Constants.PROVIDER_PUSH_CREW)) ||
        _createModel.ParterTypeSelect.Equals(new Guid(Utils.Constants.PROVIDER_PUSH_ENGAGE)))
      {
        AddProductSettingEntity(product, "pushApiToken", _createModel.Token);
      }
      else if (_createModel.ParterTypeSelect.Equals(new Guid(Utils.Constants.PROVIDER_MAIL_CHIMP)))
      {
        AddProductSettingEntity(product, "mailChimpApiToken", _createModel.Token);
        AddProductSettingEntity(product, "mailChimpList", _createModel.ListMCSelecc);
        AddProductSettingEntity(product, "mailChimpTemplate", Convert.ToString(_createModel.TemplateMCSelecc));
      }
      else if (_createModel.ParterTypeSelect.Equals(new Guid(Utils.Constants.PROVIDER_CAMPAIGN_MONITOR)))
      {
        AddProductSettingEntity(product, "campaignMonitorApiToken", _createModel.Token);
        AddProductSettingEntity(product, "campaignMonitorList", _createModel.ListCMSelecc);
        AddProductSettingEntity(product, "campaignMonitorClient", Convert.ToString(_createModel.ClientCMSelecc));
        AddProductSettingEntity(product, "campaignMonitorSegment", Convert.ToString(_createModel.SegmentCMSelecc));
      }
      else if (_createModel.ParterTypeSelect.Equals(new Guid(Utils.Constants.PROVIDER_AWEBER)))
      {
        AddProductSettingEntity(product, "aweberApiToken", _createModel.Token);
        AddProductSettingEntity(product, "aweberList", _createModel.ListAWSelecc);
        AddProductSettingEntity(product, "aweberApplicationKey", _createModel.providerAWeberApiResult.ApplicationKey);
        AddProductSettingEntity(product, "aweberApplicationSecret", _createModel.providerAWeberApiResult.ApplicationSecret);
        AddProductSettingEntity(product, "aweberOAuthToken", _createModel.providerAWeberApiResult.OAuthToken);
        AddProductSettingEntity(product, "aweberOAuthTokenSecret", _createModel.providerAWeberApiResult.OAuthTokenSecret);
        AddProductSettingEntity(product, "aweberOauthVerifier", _createModel.providerAWeberApiResult.OauthVerifier);
        AddProductSettingEntity(product, "aweberCallbackURL", _createModel.providerAWeberApiResult.CallbackURL);
      }
      else if (_createModel.ParterTypeSelect.Equals(new Guid(Utils.Constants.PROVIDER_SEND_GRID)))
      {
        AddProductSettingEntity(product, "sendGridApiToken", _createModel.Token);
        AddProductSettingEntity(product, "sendGridList", _createModel.ListSGSelecc);
        AddProductSettingEntity(product, "sendGridSender", _createModel.SenderSGSelecc);
        AddProductSettingEntity(product, "sendGridUnsubscribeGroup", _createModel.UnsubscribeGroupSGSelecc);
      }
      else if (_createModel.ParterTypeSelect.Equals(new Guid(Utils.Constants.PROVIDER_ACTIVE_CAMPAIGN)))
      {
        AddProductSettingEntity(product, "activeCampaignApiToken", _createModel.Token);
        AddProductSettingEntity(product, "activeCampaignList", _createModel.ListACSelecc);
        AddProductSettingEntity(product, "activeCampaignUrl", _createModel.URLACSelecc);
      }
      else if (_createModel.ParterTypeSelect.Equals(new Guid(Utils.Constants.PROVIDER_GETRESPONSE)))
      {
        AddProductSettingEntity(product, "getResponseApiToken", _createModel.Token);
        AddProductSettingEntity(product, "getResponseList", _createModel.ListGRSelecc);
        AddProductSettingEntity(product, "getResponseFromField", _createModel.FromFieldGRSelecc);
      }
      else if (_createModel.ParterTypeSelect.Equals( new Guid(Utils.Constants.PROVIDER_ICONTACT) ))
      {

        // Crear campaign y guardar el Id
        string IdCampaign = string.Empty;

        AddProductSettingEntity(product, "icontactIdCampaign", _createModel.IContact.IdCampaign);
        AddProductSettingEntity(product, "icontactApiAppId", _createModel.IContact.ApiAppId);
        AddProductSettingEntity(product, "icontactUserName", _createModel.IContact.ApiUserName);
        AddProductSettingEntity(product, "icontactUserPassword", _createModel.IContact.ApiUserPassword);
        AddProductSettingEntity(product, "icontactAccountId", _createModel.IContact.AccountId);
        AddProductSettingEntity(product, "icontactClientFolderId", _createModel.IContact.ClientFolderId);
        AddProductSettingEntity(product, "icontactIdList", _createModel.IContact.ListId);

      }
      else if (_createModel.ParterTypeSelect.Equals(new Guid(Utils.Constants.PROVIDER_SENDINBLUE)))
      {
        //Store product settings
        AddProductSettingEntity(product, "sendinBlueApiKey", _createModel.Token);
        AddProductSettingEntity(product, "sendinBlueCategory", _createModel.SendinBlue.Category);
        AddProductSettingEntity(product, "sendinBlueFromEmail", _createModel.SendinBlue.FromEmail);
        AddProductSettingEntity(product, "sendinBlueListId", _createModel.SendinBlue.ListIds[0].ToString());
      }
      else if (_createModel.ParterTypeSelect.Equals(new Guid(Utils.Constants.PROVIDER_ONE_SIGNAL)))
      {
        AddProductSettingEntity(product, "oneSignalApiKey", _createModel.Token);
        AddProductSettingEntity(product, "oneSignalAppId", _createModel.ListAppOSSelecc);
        AddProductSettingEntity(product, "oneSignalAppKey", _createModel.AuthAppOSSelecc);
      }
      else if (_createModel.ParterTypeSelect.Equals(new Guid(Utils.Constants.PROVIDER_MAILJET)))
      {
        AddProductSettingEntity(product, "mailJetApiKey", _createModel.Token);
        AddProductSettingEntity(product, "mailJetList", _createModel.ListMJSelecc);
        AddProductSettingEntity(product, "mailJetSegment", _createModel.SegmentMJSelecc);
        AddProductSettingEntity(product, "mailJetSecretKey", _createModel.SecretKeyMJ);
      }

      return true;

    }

    private void AddProductSetting(PRODUCT product, string settingName, string settingValue)
    {
      PRODUCT_SETTINGS _productSettings = new PRODUCT_SETTINGS()
      {
        IdProductSetting = Guid.NewGuid(),
        PRODUCT_IdProduct = product.IdProduct,
        SettingName = settingName,
        SettingValue = settingValue
      };

      _productSettings.PRODUCT = product;
      _context.PRODUCT_SETTINGS.Add(_productSettings);
    }

    private void AddProductSettingEntity(ProductEntity product, string settingName, string settingValue)
    {
      ProductSettingsRepository productSettingsRepository = new ProductSettingsRepository ();
      ProductSettingsEntity productSettings = new ProductSettingsEntity()
      {
        IdProductSetting = Guid.NewGuid(),
        PRODUCT_IdProduct = product.IdProduct,
        SettingName = settingName,
        SettingValue = settingValue
      };

      //productSettings.PRODUCT = product;
      productSettingsRepository.Add(productSettings);
      //productSettingsRepository.Save();
    }

    public async Task<List<PRODUCT>> GetProductsByIdUser(string idUser)
    {
      return await (from r in _context.PRODUCTS where r.AspNetUsers_Id.Equals(idUser) select r).ToListAsync();
    }


    public ShowProductViewModel GetProducts(string idUser, int page = 1, string sort = "ShortDescription", string sortdir = "ASC")
    {
      int pageSize = 4;
      ShowProductViewModel m = new ShowProductViewModel();
      m.PageSize = pageSize;
      m.TotalRecord = (from r in _context.PRODUCTS where r.AspNetUsers_Id.Equals(idUser) select r).Count();
      m.NoOfPages = (m.TotalRecord / m.PageSize) + ((m.TotalRecord % m.PageSize) > 0 ? 1 : 0);
      m.ListProducts = _context.PRODUCTS.OrderBy(sort + " " + sortdir).Where(p => p.AspNetUsers_Id.Equals(idUser)).Skip((page - 1) * m.PageSize).Take(m.PageSize).ToList();
      return m;
    }

    public byte[] getImageFromUrl(string url)
    {
      url = "https://slack-redir.net/link?url=https%3A%2F%2Fdocskindads.blob.core.windows.net%2Fpubimageskindads%2F5e4a755c-678d-45ce-b245-667aecaf3df1.png";
      System.Net.HttpWebRequest request = null;
      System.Net.HttpWebResponse response = null;
      byte[] b = null;

      request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
      response = (System.Net.HttpWebResponse)request.GetResponse();

      if (request.HaveResponse)
      {
        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
          Stream receiveStream = response.GetResponseStream();
          using (BinaryReader br = new BinaryReader(receiveStream))
          {
            b = br.ReadBytes(500000);
            br.Close();
          }
        }
      }

      return b;
    }
  }
}
