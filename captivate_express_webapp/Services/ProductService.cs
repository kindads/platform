using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Data.Entity;
using captivate_express_webapp.Models;
using captivate_express_webapp.Models.Publisher;
using System.Linq.Dynamic;
using captivate_express_webapp.Utils.Enums;
using System.IO;
using Captivate.Comun.Models.Entities;
using Captivate.Negocio;
using Captivate.DataAccess;

namespace captivate_express_webapp.Services
{
  public class ProductService
  {
    private KindadsEntities _context;
    public KindadsContext context { set; get; }

    public ProductService()
    {
      _context = new KindadsEntities();
      context = new KindadsContext();
    }

    

    public bool SaveProduct(Models.Publisher.CreateProductModel _createModel, string UserIdentity, Models.Core.FileUpload fileUpload)
    {    
     //Creamos los repositorios      
      SiteRepository siteRepository = new SiteRepository { Context= context };
      AspNetUserRepository aspNetUserRepository = new AspNetUserRepository { Context = context };
      ProductTypeRepository productTypeRepository = new ProductTypeRepository { Context = context };
      PartnerRepository partnerRepository = new PartnerRepository { Context = context };
      ProductRepository productRepository = new ProductRepository { Context = context };

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
        AspNetUser = user,
        AspNetUsers_Id = UserIdentity,
        IdProduct = Guid.NewGuid(),
        Price = _createModel.PriceSelecc,
        ShortDescription = _createModel.Name,
        FullDescription = string.Empty,
        StartTime = Helpers.DateTimeHelper.GetCurrentDateString(),
        EndTime = Helpers.DateTimeHelper.GetCurrentDateString(0, 30),
        RegistrationDate = DateTime.Now,
        SITE = site,
        PRODUCT_TYPE = productType,
        PARTNER = partner,
        Image = string.Empty,
        IsActive=true
      };

      if (fileUpload != null)
      {
        product.Image = Helpers.AzureStorageHelper.CreateBlobFile(fileUpload.FileData, fileUpload.Filextension);
      }
      else
      {
        product.Image = Helpers.AzureStorageHelper.CreateBlobFile(getImageFromUrl(""), ".png");
      }

      
      productRepository.Add(product);
      //productRepository.Save();

      //SITE _site = (from r in _context.SITES where r.IdSite.Equals(_createModel.SiteTypeSelecc) select r).FirstOrDefault();
      //AspNetUser _aspnetuser = (AspNetUser)((from d in _context.AspNetUsers where d.Id == UserIdentity select d).FirstOrDefault());
      //PRODUCT_TYPE _productType = (PRODUCT_TYPE)((from d in _context.PRODUCT_TYPE where d.IdProductType == _createModel.ProductTypeSelect select d).FirstOrDefault());
      //PARTNER _partner = (PARTNER)((from d in _context.PARTNERS where d.IdPartner == _createModel.ParterTypeSelect select d).FirstOrDefault());

      //PRODUCT _product = new PRODUCT();
      //_product.AspNetUser = _aspnetuser;
      //_product.AspNetUsers_Id = UserIdentity;
      //_product.IdProduct = Guid.NewGuid();
      //_product.Price = _createModel.PriceSelecc;
      //_product.ShortDescription = _createModel.Name;
      //_product.FullDescription = "";
      //_product.StartTime = Helpers.DateTimeHelper.GetCurrentDateString();
      //_product.EndTime = Helpers.DateTimeHelper.GetCurrentDateString(0, 30);
      //_product.RegistrationDate = DateTime.Now;

      //_product.SITE = _site;
      //_product.PRODUCT_TYPE = _productType;
      //_product.PARTNER = _partner;
      //_product.Image = "";

      //if (fileUpload != null)
      //{
      //  _product.Image = Helpers.AzureStorageHelper.CreateBlobFile(fileUpload.FileData, fileUpload.Filextension);
      //}
      //else
      //{
      //  _product.Image = Helpers.AzureStorageHelper.CreateBlobFile(getImageFromUrl(""), ".png");
      //}

      //_context.PRODUCTS.Add(_product);


      if (_createModel.ParterTypeSelect.Equals(new Guid(Utils.Constants.PROVIDER_SUBSCRIBERS)) || _createModel.ParterTypeSelect.Equals(new Guid(Utils.Constants.PROVIDER_PUSH_CREW)))
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
    
      return context.SaveChanges() > 0;

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
      ProductSettingsRepository productSettingsRepository = new ProductSettingsRepository { Context = context };
      ProductSettingsEntity productSettings = new ProductSettingsEntity()
      {
        IdProductSetting = Guid.NewGuid(),
        PRODUCT_IdProduct = product.IdProduct,
        SettingName = settingName,
        SettingValue = settingValue
      };

      productSettings.PRODUCT = product;
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
