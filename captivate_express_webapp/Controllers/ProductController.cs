using captivate_express_webapp.Models;
using captivate_express_webapp.Utils.Enums;
using captivate_express_webapp.Utils.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MailChimp;
using createsend_dotnet;
using Aweber;
using captivate_express_webapp.Models.Core;
using System.Net;
using captivate_express_webapp.Services;
using captivate_express_webapp.Models.Publisher;
using Captivate.Comun.Models;
using Captivate.Comun.Interfaces;
using Captivate.Negocio;
using Captivate.Comun.Models.ViewModel;
using Captivate.Negocio.Partners.IContact;
using Captivate.Negocio.Partners.Mail;
using Captivate.Comun.Partners.Mail.SendinBlue;
using Captivate.Negocio.Partners.Push;

namespace captivate_express_webapp.Controllers
{
  [AuthorizeRoles(Roles.Publisher)]
  public class ProductController : AsyncController
  {
    private Services.PublisherService _publisherService;
    private Services.CatalogService _catalogService;
    private Services.ProductService _productService;
    private Services.SiteService _siteService;
    private Services.SendGridService _sendGridService;
    private Services.ActiveCampaignService _activeCampaignService;
    private Services.GetResponseService _getResponseService;


    public ProductController()
    {
      _publisherService = new Services.PublisherService();
      _catalogService = new Services.CatalogService();
      _productService = new Services.ProductService();
      _siteService = new Services.SiteService();
      _sendGridService = new Services.SendGridService();
      _activeCampaignService = new Services.ActiveCampaignService();
      _getResponseService = new Services.GetResponseService();
    }

    public ActionResult Index()
    {
      return View();
    }

    public async Task<ActionResult> CreateProduct()
    {
      ViewBag.Message = "";
      CleanSession();
      await FillProductTypeAsync();
      await FillCategory();
      await FillSites();
      FillPrice();

      CreateProductModel model = new CreateProductModel();
      return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> CreateProduct(Models.Publisher.CreateProductModel _createProduct, HttpPostedFileBase fileup)
    {
      if (ModelState.IsValid)
      {
        var productType = new Guid(Session["ProductTypeSelecc"].ToString());
        var partner = new Guid(Session["PartnerSelecc"].ToString());
        var apiToken = Session["ApiToken"].ToString();
        var price = Session["ProceSelecc"] == null ? _createProduct.PriceSelecc : Convert.ToDouble(Session["PriceSelecc"].ToString());
        var site = new Guid(Session["SiteSelecc"].ToString());
        var userId = Microsoft.AspNet.Identity.IdentityExtensions.GetUserId(User.Identity);


        if (partner.Equals(new Guid(Utils.Constants.PROVIDER_MAIL_CHIMP)))
        {
          var listMailChimp = (string)TempData["ListMCSelecc"];
          var templateMailChimp = Convert.ToInt32(TempData["TemplateMCSelecc"]);

          _createProduct.ListMCSelecc = listMailChimp;
          _createProduct.TemplateMCSelecc = templateMailChimp;
        }
        else if (partner.Equals(new Guid(Utils.Constants.PROVIDER_CAMPAIGN_MONITOR)))
        {
          var listCampaignMonitor = (string)TempData["ListCMSelecc"];
          var client = (string)TempData["ClientCMSelecc"];
          var segment = (string)TempData["SegmentCMSelecc"];

          _createProduct.ListCMSelecc = listCampaignMonitor;
          _createProduct.ClientCMSelecc = client;
          _createProduct.SegmentCMSelecc = segment;
        }
        else if (partner.Equals(new Guid(Utils.Constants.PROVIDER_AWEBER)))
        {
          var listAWeber = (string)TempData["ListAWSelecc"];
          _createProduct.providerAWeberApiResult = (ProviderAWeberApiResult)Session["ProviderAWeberApiResult"];
          _createProduct.ListAWSelecc = listAWeber;
        }
        else if (partner.Equals(new Guid(Utils.Constants.PROVIDER_GETRESPONSE)))
        {
          _createProduct.ListGRSelecc = (string)TempData["ListsGRSelecc"];
          _createProduct.FromFieldGRSelecc = (string)TempData["FromFieldSelecc"];
        }
        else if (partner.Equals(new Guid(Utils.Constants.PROVIDER_SEND_GRID)))
        {
          _createProduct.ListSGSelecc = (string)TempData["ListsSGSelecc"];
          _createProduct.SenderSGSelecc = (string)TempData["SenderSGSelecc"];
          _createProduct.UnsubscribeGroupSGSelecc = (string)TempData["UnsubscribeGroupSGSelecc"];
        }
        else if (partner.Equals(new Guid(Utils.Constants.PROVIDER_ACTIVE_CAMPAIGN)))
        {
          _createProduct.ListACSelecc = (string)TempData["ListsACSelecc"];
          _createProduct.URLACSelecc = (string)Session["WildCard"];
        }
        else if (partner.Equals(new Guid(Utils.Constants.PROVIDER_ICONTACT)))
        {
          _createProduct.ListICSelecc = (string)TempData["ListsICSelecc"];
          _createProduct.IContact.ListId = _createProduct.ListICSelecc;

          //Generar campaña
          IContactService<ICampaign, IContactPostCampaignsResponse> iContactProvider = new IContactService<ICampaign, IContactPostCampaignsResponse>();
          IContactRequest requestFrm = (IContactRequest)Session["IContactRequest"];
          IContactPostCampaignRequest requestBody = new IContactPostCampaignRequest();
          IContactPostCampaignsResponse responseCampaign = new IContactPostCampaignsResponse();

          // Fill object
          requestBody.fromEmail = requestFrm.ApiUserName;
          requestBody.name = _createProduct.Name;
          requestBody.fromName = requestFrm.ApiUserName;
          responseCampaign = (IContactPostCampaignsResponse)iContactProvider.CreateCampaign(requestBody, requestFrm);
          _createProduct.IContact.IdCampaign = responseCampaign.campaigns[0].campaignId;

        }
        else if (partner.Equals(new Guid(Utils.Constants.PROVIDER_SENDINBLUE)))
        {
          _createProduct.ListSBSelecc = (string)TempData["ListsSBSelecc"];
          _createProduct.SendinBlue.ListIds = new List<int> { Convert.ToInt32(_createProduct.ListSBSelecc) };
        }
        else if (partner.Equals(new Guid(Utils.Constants.PROVIDER_ONE_SIGNAL)))
        {
          _createProduct.ListAppOSSelecc = (string)TempData["ListsAppOSSelecc"];
          _createProduct.AuthAppOSSelecc = (string)TempData["AuthAppOSSelecc"];
        }
        else if (partner.Equals(new Guid(Utils.Constants.PROVIDER_MAILJET)))
        {
          _createProduct.ListMJSelecc = (string)TempData["ListsMJSelecc"];
          _createProduct.SegmentMJSelecc = (string)TempData["SegmentMJSelecc"];
        }

        _createProduct.PriceSelecc = price;
        _createProduct.Token = apiToken;
        _createProduct.ProductTypeSelect = productType;
        _createProduct.ParterTypeSelect = partner;
        _createProduct.SiteTypeSelecc = site;

        if (_productService.SaveProduct(_createProduct, userId, GetFileUpload(fileup)))
        {
          ViewBag.Message = "Congratulations!!!";
        }
        else
        {
          ViewBag.Message = "Error";
        }
      }

      await FillProductTypeAsync();
      await FillCategory();
      FillPrice();
      await FillSites();
      return View(_createProduct);
    }

    private Models.Core.FileUpload GetFileUpload(HttpPostedFileBase fileup)
    {
      Models.Core.FileUpload fileUpload = null;
      byte[] fileData = null;

      if (fileup != null && fileup.ContentLength > 0)
      {
        string sfilextension = Helpers.FileHelper.GetFileExtension(fileup.FileName);
        using (var binaryReader = new BinaryReader(fileup.InputStream))
        {
          fileData = binaryReader.ReadBytes(fileup.ContentLength);
          fileUpload = new Models.Core.FileUpload() { FileData = fileData, Filextension = sfilextension };
        }
      }
      return fileUpload;
    }

    public ActionResult ShowProducts()
    {
      if (Session["SuccessfulDelete"] == null)
      {
        Session["SuccessfulDelete"] = string.Empty;
        ViewBag.SuccessfulDelete = string.Empty;
      }
      else
      {
        ViewBag.SuccessfulDelete = Session["SuccessfulDelete"];
        Session["SuccessfulDelete"] = string.Empty;
      }
      ProductViewModel model = new ProductViewModel();
      //Models.Publisher.ShowProductViewModel p = new Models.Publisher.ShowProductViewModel();
      return View(model);
    }

    public ActionResult GetProducts(int page = 1, string sort = "ShortDescription", string sortdir = "ASC")
    {
      string idUser = Microsoft.AspNet.Identity.IdentityExtensions.GetUserId(User.Identity);
      ProductManager manager = new ProductManager();
      ProductViewModel model = new ProductViewModel();
      model = manager.GetBlock(idUser, page);
      return PartialView("_TableProducts", model);
    }

    public ActionResult Delete(Guid Id)
    {
      bool resultado = false;
      string idUser = Microsoft.AspNet.Identity.IdentityExtensions.GetUserId(User.Identity);
      ProductManager manager = new ProductManager();
      resultado = manager.LogicalDelete(Id);
      Session["SuccessfulDelete"] = resultado == true ? "SI" : "NO";
      var model = manager.GetBlock(idUser, 1);
      return RedirectToAction("ShowProducts");
    }


    public JsonResult GetPartners(string id)
    {
      Session["ProductTypeSelecc"] = id;
      var listPartners = _publisherService.GetPartnersByIdProductType(id);
      return Json(new SelectList(listPartners, "IdPartner", "Name"));
    }


    public JsonResult ObtenerListsFromFolder(int ids)
    {
      Session["FolderTypeSelect"] = ids;
      string ApiKey = Session["ApiToken"].ToString();
      SendinBlueManager sendinBlueManager = new SendinBlueManager(ApiKey);
      List<Folder> folders = sendinBlueManager.GetAllFolders();
      List<Lista> listas = (from folder in folders
                            where folder.id == ids
                            select folder.Listas).FirstOrDefault();

      SelectList selectList = new SelectList(listas, "id", "name");
      return Json(selectList);

    }

    private async Task<List<PRODUCT_TYPE>> FillProductTypeAsync()
    {
      {
        var listProductType = await _catalogService.GetAllProductTypeAsync();
        ViewData["products"] = new SelectList(listProductType, "IdProductType", "Name");
        return listProductType;
      }
    }

    private async Task<List<CATEGORY>> FillCategory()
    {
      var listCategory = await _catalogService.GetAllCategoryAsync();
      ViewData["categories"] = new SelectList(listCategory, "IdCategory", "Description");
      return listCategory;
    }

    private List<String> FillPrice()
    {
      List<string> listPrice = new List<string>();
      for (int i = 1; i <= 5; i++)
      {
        listPrice.Add(Convert.ToString(i));
      }
      ViewData["prices"] = new SelectList(listPrice);
      return listPrice;
    }

    private async Task<List<SITE>> FillSites()
    {
      var listProducts = await _publisherService.GetSitesListByIdUser(Microsoft.AspNet.Identity.IdentityExtensions.GetUserId(User.Identity));
      ViewData["sites"] = new SelectList(listProducts, "IdSite", "Name");
      return listProducts;
    }

    public JsonResult TagSelecc(string id)
    {
      Session["TagSelecc"] = id;
      return Json("success");
    }

    public JsonResult PartnerSelecc(string id)
    {
      Session["PartnerSelecc"] = id;
      if (Utils.Constants.PROVIDER_AWEBER.Equals(id))
      {
        return Json(new { success = true, isProviderAWeber = true, linkAWeber = Utils.Configuration.AppSettings.AWeberAuthorizeAppUrl, isProviderActiveCampaign = false, isProviderIContact = false });
      }
      else if (Utils.Constants.PROVIDER_ACTIVE_CAMPAIGN.Equals(id))
      {
        return Json(new { success = true, isProviderActiveCampaign = true, isProviderAWeber = false, isProviderIContact = false });
      }
      else if (Utils.Constants.PROVIDER_ICONTACT.Equals(id))
      {
        return Json(new { success = true, isProviderActiveCampaign = false, isProviderAWeber = false, isProviderIContact = true });
      }
      else if (Utils.Constants.PROVIDER_MAILJET.Equals(id))
      {
        return Json(new { success = true, isProviderMailJet = true });
      }
      return Json(new { success = true, isProviderAWeber = false, isProviderActiveCampaign = false, isProviderIContact = false });
    }

    public JsonResult PriceSelecc(string id)
    {
      Session["PriceSelecc"] = id;
      return Json("success");
    }

    public JsonResult SiteSelecc(string id)
    {
      Session["SiteSelecc"] = id;
      return Json("success");
    }

    public JsonResult TemplateMCSelecc(string id)
    {
      TempData["TemplateMCSelecc"] = id;
      return Json("success");
    }

    public JsonResult ListMCSelecc(string id)
    {
      TempData["ListMCSelecc"] = id;
      return Json("success");
    }

    public JsonResult ListCMSelecc(string id)
    {
      TempData["ListCMSelecc"] = id;
      return Json("success");
    }

    public JsonResult ListAWSelecc(string id)
    {
      TempData["ListAWSelecc"] = id;
      return Json("success");
    }

    public JsonResult SegmentCMSelecc(string id)
    {
      TempData["SegmentCMSelecc"] = id;
      return Json("success");
    }

    public JsonResult ListsSGSelecc(string id)
    {
      TempData["ListsSGSelecc"] = id;
      return Json("success");
    }

    public JsonResult SenderSGSelecc(string id)
    {
      TempData["SenderSGSelecc"] = id;
      return Json("success");
    }

    public JsonResult UnsubscribeGroupSGSelecc(string id)
    {
      TempData["UnsubscribeGroupSGSelecc"] = id;
      return Json("success");
    }

    public JsonResult ListsACSelecc(string id)
    {
      TempData["ListsACSelecc"] = id;
      return Json("success");
    }

    public JsonResult ListsGRSelecc(string id)
    {
      TempData["ListsGRSelecc"] = id;
      return Json("success");
    }

    public JsonResult ListsICSelecc(string id)
    {
      TempData["ListsICSelecc"] = id;
      return Json("success");
    }

    public JsonResult FromFieldSelecc(string id)
    {
      TempData["FromFieldSelecc"] = id;
      return Json("success");
    }

    public JsonResult ListsSBSelecc(string id)
    {
      TempData["ListsSBSelecc"] = id;
      return Json("success");
    }

    public JsonResult ListsAppsOSSelecc(string id)
    {
      OneSignalManager oneSignalManager = new OneSignalManager();
      TempData["ListsAppOSSelecc"] = id;
      TempData["AuthAppOSSelecc"] = oneSignalManager.GetApp(Session["ApiToken"].ToString(), id).basic_auth_key;
      return Json("success");
    }

    public JsonResult ListsMJSelecc(string id)
    {
      TempData["ListsMJSelecc"] = id;
      return Json("success");
    }

    public JsonResult SegmentMJSelecc(string id)
    {
      TempData["SegmentMJSelecc"] = id;
      return Json("success");
    }

    public JsonResult ValidateApiAppId(string ApiAppId, string UserName, string UserPassword, string AccountId, string ClientFolderId, string ApiToken, string idSite, string idProvider, string wildCard)
    {

      Session["ApiToken"] = ApiToken;
      Session["AccountId"] = wildCard;
      Session["WildCard"] = wildCard;
      string mailingProvider = Session["PartnerSelecc"].ToString();

      // Make request 
      IContactRequest request = new IContactRequest(ProviderEnvironment.Production);
      // Set data
      request.ApiAppId = ApiAppId;
      request.ApiUserName = UserName;
      request.ApiUserPassword = UserPassword;
      request.AccountId = AccountId;
      request.ClientFolderId = ClientFolderId;

      //Encapsulate IContactRequest
      Session["IContactRequest"] = request;

      request.ApiAppId = ApiAppId;
      request.ApiUserName = UserName;
      request.ApiUserPassword = UserPassword;
      request.AccountId = AccountId;
      request.ClientFolderId = ClientFolderId;

      var site = _siteService.GetSiteByIdSite(idSite);
      bool result = false;
      bool _isPushCrew = false;
      bool _isSubscribers = false;
      bool _isMailChimp = false;
      bool _isCampaignMonitor = false;
      bool _isAweber = false;
      bool _isGetResponse = false;
      bool _isSendGrid = false;
      bool _isActiveCampaign = false;
      bool _isActiveIContact = false;

      IValidateProvider resultValidation = new ValidateProvider(EnumMailProviders.IContact);

      switch (mailingProvider)
      {
        case Utils.Constants.PROVIDER_ICONTACT:
          {
            IContactService<ICampaign, IContactGetListsResponse> contactService = new IContactService<ICampaign, IContactGetListsResponse>();
            resultValidation = contactService.ValidateApiToken(request);
            _isActiveIContact = true;
          }
          break;
      }

      return Json(new { message = resultValidation.Message, success = resultValidation.IsValid, isPushCrew = _isPushCrew, isSubscribers = _isSubscribers, isMailChimp = _isMailChimp, isCampaignMonitor = _isCampaignMonitor, isAweber = _isAweber, isGetResponse = _isGetResponse, isSendGrid = _isSendGrid, IsActiveCampaign = _isActiveCampaign, IsActiveIContact = _isActiveIContact });

    }

    public JsonResult ValidateApiToken(string ApiToken, string idSite, string idProvider, string wildCard)
    {
      if (!String.IsNullOrEmpty(ApiToken) && !String.IsNullOrEmpty(idSite))
      {
        Session["ApiToken"] = ApiToken;
        Session["WildCard"] = wildCard;
        string mailingProvider = Session["PartnerSelecc"].ToString();

        var site = _siteService.GetSiteByIdSite(idSite);
        bool result = false;
        bool _isPushCrew = false;
        bool _isSubscribers = false;
        bool _isMailChimp = false;
        bool _isCampaignMonitor = false;
        bool _isAweber = false;
        bool _isGetResponse = false;
        bool _isSendGrid = false;
        bool _isActiveCampaign = false;
        bool _isActiveIContact = false;
        bool _isSendinBlue = false;
        bool _isPushEngage = false;
        bool _isOneSignal = false;
        bool _isMailJet = false;

        switch (mailingProvider)
        {
          case Utils.Constants.PROVIDER_PUSH_CREW:
            result = Helpers.PushcrewHelper.validatePushCrewToken(ApiToken, site.URL);
            _isPushCrew = true;
            break;
          case Utils.Constants.PROVIDER_SUBSCRIBERS:
            result = Helpers.SubscribersHelper.ValidateKey(ApiToken, site.URL);
            _isSubscribers = true;
            break;
          case Utils.Constants.PROVIDER_MAIL_CHIMP:
            IMailChimpManager mailChimpManager = new MailChimp.MailChimpManager(ApiToken);
            var message = mailChimpManager.Ping();
            result = message != null;
            _isMailChimp = true;
            break;
          case Utils.Constants.PROVIDER_CAMPAIGN_MONITOR:
            AuthenticationDetails auth = new ApiKeyAuthenticationDetails(ApiToken);
            result = IsValidApiKeyCampaignMonitor(auth);
            _isCampaignMonitor = true;
            break;
          case Utils.Constants.PROVIDER_AWEBER:
            result = IsValidApiKeyAWeber(ApiToken).Success;
            _isAweber = true;
            break;
          case Utils.Constants.PROVIDER_GETRESPONSE:
            result = IsValidApiKeyGetResponse(ApiToken);
            _isGetResponse = true;
            break;
          case Utils.Constants.PROVIDER_SEND_GRID:
            result = IsValidApiKeySendGrid(ApiToken);
            _isSendGrid = true;
            break;
          case Utils.Constants.PROVIDER_ACTIVE_CAMPAIGN:
            result = IsValidApiKeyActiveCampaign(ApiToken, wildCard);
            _isActiveCampaign = true;
            break;
          case Utils.Constants.PROVIDER_SENDINBLUE:
            result = IsValidApiKeySendingBlue(ApiToken);
            _isSendinBlue = true;
            break;
          case Utils.Constants.PROVIDER_PUSH_ENGAGE:
            result = IsValidApiKeyPushEngage(ApiToken);
            _isPushEngage = true;
            break;
          case Utils.Constants.PROVIDER_ONE_SIGNAL:
            result = IsValidApiKeyOneSignal(ApiToken);
            _isOneSignal = true;
            break;
          case Utils.Constants.PROVIDER_MAILJET:
            result = IsValidApiKeyMailJet(ApiToken, wildCard);
            _isMailJet = true;
            break;

        }
        return Json(new { success = result, isPushCrew = _isPushCrew, isSubscribers = _isSubscribers, isMailChimp = _isMailChimp, isCampaignMonitor = _isCampaignMonitor,
          isAweber = _isAweber, isGetResponse = _isGetResponse, isSendGrid = _isSendGrid, IsActiveCampaign = _isActiveCampaign, IsActiveIContact = _isActiveIContact,
          IsSendinBlue = _isSendinBlue, IsPushEngage = _isPushEngage, IsOneSignal = _isOneSignal , IsMailJet = _isMailJet});
      }
      else
      {
        return Json(new { error = true });
      }
    }

    public JsonResult GetListMailChimp()
    {
      IMailChimpManager mailChimpManager = new MailChimp.MailChimpManager(Session["ApiToken"].ToString());
      var list = mailChimpManager.GetLists(null, 0, 100, "", "");
      return Json(new SelectList(list.Data, "Id", "Name"));
    }

    public static bool IsValidApiKeyCampaignMonitor(AuthenticationDetails auth)
    {
      try
      {
        return new General(auth).SystemDate() != null ? true : false;
      }
      catch (Exception)
      {
        return false;
      }
    }

    public ProviderAWeberApiResult IsValidApiKeyAWeber(string key)
    {
      ProviderAWeberApiResult result = new ProviderAWeberApiResult();
      try
      {
        var elements = key.Split('|');
        result.ApplicationKey = elements[0];
        result.ApplicationSecret = elements[1];
        result.RequestToken = elements[2];
        result.TokenSecret = elements[3];
        result.OauthVerifier = elements[4];

        // Create a new api instance
        API api = new API(result.ApplicationKey, result.ApplicationSecret);

        // Fill the tokens back from user session
        api.OAuthToken = result.RequestToken;
        api.OAuthTokenSecret = result.TokenSecret;

        // Get the returned oauth_verifier
        api.OAuthVerifier = result.OauthVerifier;

        // Get Access Token (this is the permanent token to be stored for future access)
        api.get_access_token();

        result.CallbackURL = api.CallbackUrl;
        result.OAuthToken = api.OAuthToken;
        result.OAuthTokenSecret = api.OAuthTokenSecret;
        result.OauthVerifier = api.OAuthVerifier;
        result.Success = !String.IsNullOrEmpty(result.OAuthToken) && !String.IsNullOrEmpty(result.OAuthTokenSecret);

        Aweber.Entity.Account account = api.getAccount();
        Session["ProviderAWeberApiResult"] = result;
        return result;
      }
      catch (Exception)
      {
        result.Success = false;
        return result;
      }
    }

    public bool IsValidApiKeyGetResponse(string key)
    {
      return _getResponseService.IsValid(key);
    }

    public bool IsValidApiKeySendGrid(string key)
    {
      return _sendGridService.IsValid(key);
    }

    public bool IsValidApiKeySendingBlue(string key)
    {
      bool result = false;
      SendinBlueManager sendinBlueManager = new SendinBlueManager(key);
      result = sendinBlueManager.ValidateApiKey();
      return result;
    }

    public bool IsValidApiKeyPushEngage(string key)
    {
      PushEngageManger pushEngageManger = new PushEngageManger();
      return pushEngageManger.IsValid(key);
    }

    public bool IsValidApiKeyOneSignal(string key)
    {
      OneSignalManager oneSignalManager = new OneSignalManager();
      return oneSignalManager.IsValid(key);
    }

    public bool IsValidApiKeyMailJet(string key, string privateKey)
    {
      MailJetManager mailJetManager = new MailJetManager();
      return mailJetManager.IsValidAsync(key, privateKey);
    }

    public bool IsValidApiKeyActiveCampaign(string key, string url)
    {
      return _activeCampaignService.IsValid(key, url);
    }

    public JsonResult GetListCampaignMonitor(string idClient)
    {
      if (!String.IsNullOrEmpty(idClient))
      {
        TempData["ClientCMSelecc"] = idClient;
        AuthenticationDetails auth = new ApiKeyAuthenticationDetails(Session["ApiToken"].ToString());
        Client client = new Client(auth, idClient);
        var list = client.Lists();
        return Json(new SelectList(list, "ListID", "Name"));
      }

      return Json(new SelectList(new List<BasicList>(), "ListID", "Name"));
    }

    public JsonResult GetSegmentCampaignMonitor(string idClient)
    {
      if (!String.IsNullOrEmpty(idClient))
      {
        TempData["ClientCMSelecc"] = idClient;
        AuthenticationDetails auth = new ApiKeyAuthenticationDetails(Session["ApiToken"].ToString());
        Client client = new Client(auth, idClient);
        var list = client.Segments();
        return Json(new SelectList(list, "SegmentID", "Title"));
      }

      return Json(new SelectList(new List<BasicList>(), "ListID", "Name"));
    }

    public JsonResult GetClientCampaignMonitor()
    {
      AuthenticationDetails auth = new ApiKeyAuthenticationDetails(Session["ApiToken"].ToString());
      General general = new General(auth);
      return Json(new SelectList(general.Clients(), "ClientID", "Name"));
    }


    public JsonResult GetTagMailChimp()
    {
      IMailChimpManager mailChimpManager = new MailChimp.MailChimpManager(Session["ApiToken"].ToString());
      MailChimp.Templates.TemplateTypes tt = new MailChimp.Templates.TemplateTypes() { Base = true, Gallery = true, User = true };
      MailChimp.Templates.TemplateFilters tf = new MailChimp.Templates.TemplateFilters() { IncludeDragAndDrop = true };

      var list = mailChimpManager.GetTemplates(tt, tf);
      return Json(new SelectList(list.UserTemplates, "TemplateID", "Name"));
    }

    public JsonResult GetListAWeber()
    {

      API api = GetDataApiKeyAweber();
      List<Aweber.Entity.List> lists = new List<Aweber.Entity.List>();
      Aweber.Entity.Account account = api.getAccount();

      foreach (Aweber.Entity.List list in account.lists().entries)
      {
        lists.Add(list);
      }
      return Json(new SelectList(lists, "id", "name"));
    }

    public JsonResult GetListActiveCampaign()
    {
      return Json(new SelectList(_activeCampaignService.GetLists(Session["ApiToken"].ToString(), Session["WildCard"].ToString()), "id", "name"));
    }

    public JsonResult GetListIContact()
    {
      IContactRequest request = (IContactRequest)Session["IContactRequest"];

      IContactService<ICampaign, IContactGetListsResponse> IContactProvider = new IContactService<ICampaign, IContactGetListsResponse>();
      IResponse response = IContactProvider.GetLists(request);
      IContactGetListsResponse data = (IContactGetListsResponse)response;
      SelectList lista = new SelectList(data.lists, "listId", "name");
      return Json(lista);
    }


    public JsonResult GetListFoldersSendinBlue()
    {
      string ApiKey = Session["ApiToken"].ToString();
      SendinBlueManager sendinBlueManager = new SendinBlueManager(ApiKey);
      List<Folder> folders = sendinBlueManager.GetAllFolders();
      SelectList lista = new SelectList(folders, "id", "name");
      return Json(lista);
    }


    public JsonResult GetListGetResponse()
    {
      var list = _getResponseService.GetLists(Session["ApiToken"].ToString());
      return Json(new SelectList(list, "id", "name"));
    }

    public JsonResult GetFromFieldGetResponse()
    {
      var list = _getResponseService.GetFromFields(Session["ApiToken"].ToString());
      return Json(new SelectList(list, "id", "name"));
    }

    public JsonResult GetListSendGrid()
    {
      var list = _sendGridService.GetLists(Session["ApiToken"].ToString());
      return Json(new SelectList(list, "Id", "name"));
    }

    public JsonResult GetSenderSendGrid()
    {
      var list = _sendGridService.GetSenders(Session["ApiToken"].ToString());
      return Json(new SelectList(list, "Id", "name"));
    }

    public JsonResult GetUnsubscribeGroupSendGrid()
    {
      var list = _sendGridService.GetUnsubscribeGroups(Session["ApiToken"].ToString());
      return Json(new SelectList(list, "Id", "name"));
    }

    public JsonResult GetListAppsOneSignal()
    {
      System.Threading.Thread.Sleep(1000);
      OneSignalManager oneSignalManager = new OneSignalManager();
      var list = oneSignalManager.GetApps(Session["ApiToken"].ToString());
      return Json(new SelectList(list, "Id", "name"));
    }

    public JsonResult GetListsMailJet()
    {
      MailJetManager mailJetManager = new MailJetManager();
      var list = mailJetManager.GetLists(Session["ApiToken"].ToString(), Session["WildCard"].ToString());
      return Json(new SelectList(list, "Id", "Name"));
    }

    public JsonResult GetSegmentMailJet()
    {
      MailJetManager mailJetManager = new MailJetManager();
      var list = mailJetManager.GetSegments(Session["ApiToken"].ToString(), Session["WildCard"].ToString());
      return Json(new SelectList(list, "Id", "Name"));
    }

    private void CleanSession()
    {
      Session["ProductTypeSelecc"] = null;
      Session["CategorySelecc"] = null;
      Session["TagSelecc"] = null;
      Session["PartnerSelecc"] = null;
      Session["PriceSelecc"] = null;
      Session["SiteSelecc"] = null;
    }

    public API GetDataApiKeyAweber()
    {
      var providerAWeberApiResult = (ProviderAWeberApiResult)Session["ProviderAWeberApiResult"];
      // Create a new api instance
      API api = new API(providerAWeberApiResult.ApplicationKey, providerAWeberApiResult.ApplicationSecret);
      // Fill the tokens back from user session
      api.OAuthToken = providerAWeberApiResult.OAuthToken;
      api.OAuthTokenSecret = providerAWeberApiResult.OAuthTokenSecret;

      // Get the returned oauth_verifier
      api.OAuthVerifier = providerAWeberApiResult.OauthVerifier;
      api.CallbackUrl = providerAWeberApiResult.CallbackURL;
      return api;
    }
  }
}
