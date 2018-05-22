using captivate_express_webapp.Models;
using captivate_express_webapp.Models.Core;
using captivate_express_webapp.Models.Campaign;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq.Dynamic;
using captivate_express_webapp.Utils.Enums;
using captivate_express_webapp.Models.Advertiser;
using MailChimp.Campaigns;
using MailChimp;
using createsend_dotnet;
using System.Web;
using Aweber;
using System.Net;
using Aweber.OAuth;
using Newtonsoft.Json;
using Captivate.Common.Interfaces;
using Captivate.Business;
//using captivate_express_webapp.Models;

namespace captivate_express_webapp.Services
{
  public class CampaignService : ITelemetria
  {
    private static int PAGE_SIZE = 8;
    private KindadsEntities _context;

    public ITrace telemetria { set; get; }

    public CampaignService()
    {
      telemetria = new Trace();
      _context = new KindadsEntities();
    }

    public async Task<PRODUCT> GetProductByIdProductAsync(Guid idProduct)
    {
      return await (from r in _context.PRODUCTS where r.IdProduct.Equals(idProduct) select r).FirstOrDefaultAsync();
    }

    public PRODUCT GetProductByIdProduct(Guid idProduct)
    {
      return (from r in _context.PRODUCTS where r.IdProduct.Equals(idProduct) select r).FirstOrDefault();
    }

    public bool RegisterCampaign(CAMPAIGN campaign, List<CAMPAIGN_SETTINGS> listCampaignSettings, FileUpload fileUpload)
    {
      try
      {
        campaign.IdCampaign = Guid.NewGuid();
        campaign.RegisterDate = DateTime.Now;
        PRODUCT p = (from r in _context.PRODUCTS where r.IdProduct.Equals(campaign.PRODUCT.IdProduct) select r).FirstOrDefault();
        AspNetUser u = (from r in _context.AspNetUsers where r.Email.Equals(campaign.AspNetUser.Email) select r).FirstOrDefault();
        campaign.PRODUCT = p;
        campaign.AspNetUser = u;
        campaign.CAT_CAMPAIGN_STATUS_IdStatus = (int)CatCampaignStatusEnum.In_Review;

        _context.Set<CAMPAIGN>().Add(campaign);
        if (listCampaignSettings != null && listCampaignSettings.Any())
        {
          foreach (var setting in listCampaignSettings)
          {
            setting.IdCampaignSetting = Guid.NewGuid();
            setting.CAMPAIGNs1 = campaign;
            setting.CAMPAIGNs1_IdCampaign = campaign.IdCampaign;
            if (setting.SettingName.Equals("pushNotifImage")) { setting.SettingValue = GetImageAzure(fileUpload); }
            _context.Set<CAMPAIGN_SETTINGS>().Add(setting);
          }
        }
      }
      catch (Exception e)
      {
        var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
        telemetria.Critical(messageException);
      }
      return _context.SaveChanges() > 0;
    }

    public bool ModifyCampaign(CAMPAIGN campaign, ICollection<CAMPAIGN_SETTINGS> listCampaignSettings, FileUpload fileUpload)
    {
      _context.Entry(campaign).State = EntityState.Modified;
      try
      {
        if (listCampaignSettings != null && listCampaignSettings.Any())
        {
          foreach (var setting in listCampaignSettings)
          {
            if (setting.SettingName.Equals("pushNotifImage"))
            {
              if (fileUpload != null && fileUpload.FileData != null && fileUpload.Filextension != null) { setting.SettingValue = GetImageAzure(fileUpload); }
            }
            _context.Entry(setting).State = EntityState.Modified;
          }
        }
      }
      catch (Exception e)
      {
        var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
        telemetria.Critical(messageException);
      }
      return _context.SaveChanges() > 0;
    }

    private string GetImageAzure(FileUpload fileUpload)
    {
      if (fileUpload != null) { return Helpers.AzureStorageHelper.CreateBlobFile(fileUpload.FileData, fileUpload.Filextension); } else { return ""; }
    }

    [Obsolete]
    public string CreateCampaignMailChimp(string idCampaign)
    {
      string idList = null;
      string apiKey = null;
      int idTemplate = 0;
      string subject = "";
      string fromEmail = "";
      string fromName = "";

      var campaign = GetCampaignById(idCampaign);
      var product = GetProductByIdProduct(campaign.PRODUCT.IdProduct);

      try
      {
        if (product.PRODUCT_SETTINGS != null && product.PRODUCT_SETTINGS.Any())
        {
          foreach (var item in product.PRODUCT_SETTINGS)
          {
            apiKey = item.SettingName.Equals("mailChimpApiToken") ? item.SettingValue : apiKey;
            idList = item.SettingName.Equals("mailChimpList") ? item.SettingValue : idList;
            idTemplate = item.SettingName.Equals("mailChimpTemplate") ? Convert.ToInt32(item.SettingValue) : idTemplate;
          }
        }

        if (campaign.CAMPAIGN_SETTINGS != null && campaign.CAMPAIGN_SETTINGS.Any())
        {
          foreach (var setting in campaign.CAMPAIGN_SETTINGS)
          {
            subject = setting.SettingName.Equals("mailChimpSubject") ? setting.SettingValue : subject;
            fromName = setting.SettingName.Equals("mailChimpFromName") ? setting.SettingValue : fromName;
            fromEmail = setting.SettingName.Equals("mailChimpFromEmail") ? setting.SettingValue : fromEmail;
          }
        }

        CampaignCreateOptions cco = new CampaignCreateOptions()
        {
          ListId = idList,
          Title = campaign.Name,
          Subject = subject,
          FromEmail = fromEmail,
          FromName = fromName
        };

        Dictionary<string, string> sections = new Dictionary<string, string>();
        if (product.SITE.CATEGORY != null && product.SITE.CATEGORY.Any())
        {
          foreach (var item in product.SITE.CATEGORY)
          {
            sections.Add(item.IdCategory.ToString(), item.Description);
          }
        }

        CampaignCreateContent ccc = new CampaignCreateContent()
        {
          Sections = sections,
          HTML = campaign.AdText
        };

        IMailChimpManager mailChimpManager = new MailChimpManager(apiKey);
        var campaignMailChimp = mailChimpManager.CreateCampaign("regular", cco, ccc, null, null);

        if (campaign != null && !String.IsNullOrEmpty(campaignMailChimp.Id))
        {
          var resultSendCampaign = mailChimpManager.SendCampaign(campaignMailChimp.Id);
          return resultSendCampaign.Complete ? campaignMailChimp.Id : null;
        }
      }
      catch (Exception e)
      {
        var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
        telemetria.Critical(messageException);
      }
      return null;
    }

    [Obsolete]
    public string CreateCampaignMonitor(string idCampaign)
    {
      var campaign = GetCampaignById(idCampaign);
      var product = GetProductByIdProduct(campaign.PRODUCT.IdProduct);
      string idList = null;
      string apiKey = null;
      string clientID = null;
      string subject = "";
      string fromEmail = "";
      string fromName = "";
      string segment = "";


      if (product.PRODUCT_SETTINGS != null && product.PRODUCT_SETTINGS.Any())
      {
        foreach (var item in product.PRODUCT_SETTINGS)
        {
          apiKey = item.SettingName.Equals("campaignMonitorApiToken") ? item.SettingValue : apiKey;
          idList = item.SettingName.Equals("campaignMonitorList") ? item.SettingValue : idList;
          clientID = item.SettingName.Equals("campaignMonitorClient") ? item.SettingValue : clientID;
          segment = item.SettingName.Equals("campaignMonitorSegment") ? item.SettingValue : segment;
        }
      }

      if (campaign.CAMPAIGN_SETTINGS != null && campaign.CAMPAIGN_SETTINGS.Any())
      {
        foreach (var setting in campaign.CAMPAIGN_SETTINGS)
        {
          subject = setting.SettingName.Equals("campaignMonitorSubject") ? setting.SettingValue : subject;
          fromName = setting.SettingName.Equals("campaignMonitorFromName") ? setting.SettingValue : fromName;
          fromEmail = setting.SettingName.Equals("campaignMonitorFromEmail") ? setting.SettingValue : fromEmail;
        }
      }

      try
      {
        AuthenticationDetails auth = new ApiKeyAuthenticationDetails(apiKey);
        List<string> listIDs = new List<string> { idList };
        List<string> segmentIDs = new List<string> { segment };

        TemplateContent templateContent = new TemplateContent();

        string campaignID = createsend_dotnet.Campaign.CreateFromTemplate(
            auth,
            clientID,
            subject,
            campaign.Name,
            fromName + "-" + DateTime.Now.ToString("MMddyyyyhhmm"),
            fromEmail,
            fromEmail,
            listIDs,
            segmentIDs,
            CreateTemplate(auth, clientID, campaign),
            templateContent);

        createsend_dotnet.Campaign campaignMonitorCampaign = new createsend_dotnet.Campaign(auth, campaignID);
        campaignMonitorCampaign.Send(fromEmail);
        return campaignID;
      }
      catch (Exception e)
      {
        var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
        telemetria.Critical(messageException);
      }

      return null;
    }

    [Obsolete]
    public string CreateAweberCampaign(string idCampaign)
    {
      string campaignID = null;
      var campaign = GetCampaignById(idCampaign);
      var product = GetProductByIdProduct(campaign.PRODUCT.IdProduct);
      string appKey = null;
      string appSecret = null;
      string aList = null;
      string oAuthToken = null;
      string oAuthSecret = null;
      string oAuthVerify = null;

      string subject = null;
      string bodyHTML = null;
      string isArchived = null;

      if (product.PRODUCT_SETTINGS != null && product.PRODUCT_SETTINGS.Any())
      {
        foreach (var item in product.PRODUCT_SETTINGS)
        {
          appKey = item.SettingName.Equals("aweberApplicationKey") ? item.SettingValue : appKey;
          appSecret = item.SettingName.Equals("aweberApplicationSecret") ? item.SettingValue : appSecret;
          oAuthToken = item.SettingName.Equals("aweberOAuthToken") ? item.SettingValue : oAuthToken;
          oAuthSecret = item.SettingName.Equals("aweberOAuthTokenSecret") ? item.SettingValue : oAuthSecret;
          oAuthVerify = item.SettingName.Equals("aweberOauthVerifier") ? item.SettingValue : oAuthVerify;
          aList = item.SettingName.Equals("aweberList") ? item.SettingValue : aList;
        }
      }

      if (campaign.CAMPAIGN_SETTINGS != null && campaign.CAMPAIGN_SETTINGS.Any())
      {
        foreach (var setting in campaign.CAMPAIGN_SETTINGS)
        {
          subject = setting.SettingName.Equals("aweberSubject") ? setting.SettingValue : subject;
          bodyHTML = setting.SettingName.Equals("aweberBodyHtml") ? setting.SettingValue : bodyHTML;
          isArchived = setting.SettingName.Equals("aweberIsArchived") ? setting.SettingValue : isArchived;
        }
      }

      try
      {
        campaignID = SetBroadcast(appKey, appSecret, oAuthToken, oAuthSecret, oAuthVerify, aList, bodyHTML, subject);
        string schedule = SetBroadcastSchedule(appKey, appSecret, oAuthToken, oAuthSecret, oAuthVerify, aList, campaignID);
        string broadcast = GetBroadcast(appKey, appSecret, oAuthToken, oAuthSecret, oAuthVerify, aList, campaignID);
        if (broadcast == "scheduled")
          return campaignID;
        else
          return null;
      }
      catch (Exception e)
      {
        var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
        telemetria.Critical(messageException);
        campaignID = e.Message;
      }
      return null;
    }

    [Obsolete]
    private string CreateTemplate(AuthenticationDetails auth, string clientID, CAMPAIGN campaign)
    {
      string template = string.Empty;
      try
      {
        var htmlText = HttpUtility.HtmlDecode(GenerateTemplateCampaignMonitor(campaign));
        var templateID = Helpers.AzureStorageHelper.CreateHtmlTemplateAzure(htmlText);
        template = Template.Create(auth, clientID, "TEMPLATE " + campaign.Name, templateID, "");
      }
      catch (Exception e)
      {
        var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
        telemetria.Critical(messageException);
      }
      return template;
    }

    private string GenerateTemplateCampaignMonitor(CAMPAIGN campaign)
    {
      System.Text.StringBuilder sb = new System.Text.StringBuilder();
      try
      {
        sb.Append("<html><head><title>").Append(campaign.Name).Append("</title></head>");
        sb.Append("<body><p><singleline>").Append(campaign.Name).Append("</singleline></p>");
        sb.Append("<div><multiline>").Append(campaign.AdText).Append("</multiline></div>");
        sb.Append("<p><unsubscribe>Unsubscribe</unsubscribe></p></body></html>");
      }
      catch (Exception e)
      {
        var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
        telemetria.Critical(messageException);
      }
      return sb.ToString();
    }

    public void ObtainDefaultValues(Guid idProduct, CreateCampaingModel model)
    {
      var product = GetProductByIdProduct(idProduct);
      string idList = null;
      string apiKey = null;

      try
      {
        if (product.PRODUCT_SETTINGS != null && product.PRODUCT_SETTINGS.Any())
        {
          foreach (var item in product.PRODUCT_SETTINGS)
          {
            apiKey = item.SettingName.Equals("mailChimpApiToken") ? item.SettingValue : apiKey;
            idList = item.SettingName.Equals("mailChimpList") ? item.SettingValue : idList;
          }

          IMailChimpManager mailChimpManager = new MailChimpManager(apiKey);
          MailChimp.Lists.ListFilter filter = new MailChimp.Lists.ListFilter();
          filter.ListId = idList;
          foreach (var item in mailChimpManager.GetLists(filter).Data)
          {
            model.FromNameMC = item.DefaultFromName;
            model.FromEmailMC = item.DefaultFromEmail;
          }
        }
      }
      catch (Exception e)
      {
        var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
        telemetria.Critical(messageException);
      }
    }

    public void GetDataClientCampaignMonitor(Guid idProduct, CreateCampaingModel model)
    {
      var product = GetProductByIdProduct(idProduct);
      string clientID = null;
      string apiKey = null;

      try
      {
        if (product.PRODUCT_SETTINGS != null && product.PRODUCT_SETTINGS.Any())
        {
          foreach (var item in product.PRODUCT_SETTINGS)
          {
            apiKey = item.SettingName.Equals("campaignMonitorApiToken") ? item.SettingValue : apiKey;
            clientID = item.SettingName.Equals("campaignMonitorClient") ? item.SettingValue : clientID;
          }
          AuthenticationDetails auth = new ApiKeyAuthenticationDetails(apiKey);
          Account account = new Account(auth);
          foreach (var item in account.Administrators())
          {
            model.FromNameMC = item.Name;
            model.FromEmailMC = item.EmailAddress;
          }

        }
      }
      catch (Exception e)
      {
        var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
        telemetria.Critical(messageException);
      }

    }

    public void GetDataClientAweber(Guid idProduct, CreateCampaingModel model)
    {
      var product = GetProductByIdProduct(idProduct);
      string apiKey = null;
      string aweberList = null;
      string appKey = null;
      string appSecret = null;
      string oAuthToken = null;
      string oAuthSecret = null;
      string oAuthVerify = null;
      string callBackUrl = null;

      try
      {
        if (product.PRODUCT_SETTINGS != null && product.PRODUCT_SETTINGS.Any())
        {
          foreach (var item in product.PRODUCT_SETTINGS)
          {
            apiKey = item.SettingName.Equals("aweberApiToken") ? item.SettingValue : apiKey;
            aweberList = item.SettingName.Equals("aweberList") ? item.SettingValue : aweberList;
            appKey = item.SettingName.Equals("aweberApplicationKey") ? item.SettingValue : appKey;
            appSecret = item.SettingName.Equals("aweberApplicationSecret") ? item.SettingValue : appSecret;
            oAuthToken = item.SettingName.Equals("aweberOAuthToken") ? item.SettingValue : oAuthToken;
            oAuthSecret = item.SettingName.Equals("aweberOAuthTokenSecret") ? item.SettingValue : oAuthSecret;
            oAuthVerify = item.SettingName.Equals("aweberOauthVerifier") ? item.SettingValue : oAuthVerify;
            callBackUrl = item.SettingName.Equals("aweberCallbackURL") ? item.SettingValue : callBackUrl;
          }
        }
      }
      catch (Exception e)
      {
        var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
        telemetria.Critical(messageException);
      }
    }

    public List<CAMPAIGN> GetAllCampaigns()
    {
      return (from r in _context.CAMPAIGNs1 join p in _context.PRODUCTS on r.PRODUCT.IdProduct equals p.IdProduct select r).ToList();
    }

    public List<CAMPAIGN> GetCampaignByIdUser(Guid idUser)
    {
      return (from r in _context.CAMPAIGNs1 join p in _context.PRODUCTS on r.PRODUCT.IdProduct equals p.IdProduct where r.AspNetUser.Id.Equals(idUser.ToString()) select r).ToList();
    }

    public CAMPAIGN GetCampaignById(string idCampaign)
    {
      return (from r in _context.CAMPAIGNs1 where r.IdCampaign.Equals(new Guid(idCampaign)) select r).FirstOrDefault();
    }

    public TableCampaignViewModel GetTableCampaignPending(int idUserRole, string idUser, int page = 1, string sort = "RegisterDate", string sortdir = "DESC")
    {
      List<int> status = new List<int>() { (int)CatCampaignStatusEnum.In_Review, (int)CatCampaignStatusEnum.Waiting_For_Response };
      TableCampaignViewModel m = new TableCampaignViewModel();
      m.PageSize = PAGE_SIZE;

      try
      {
        if (idUserRole == (int)Roles.Advertiser)
        {
          m.TotalRecord = _context.CAMPAIGNs1
            .OrderBy(sort + " " + sortdir)
            .Where(p => p.AspNetUser.Id.Equals(idUser) && (p.CAT_CAMPAIGN_STATUS_IdStatus != null && status.Contains((int)p.CAT_CAMPAIGN_STATUS_IdStatus))).Count();
          m.ListCampaigns = _context.CAMPAIGNs1
            .OrderBy(sort + " " + sortdir)
            .Where(p => p.AspNetUser.Id.Equals(idUser) && (p.CAT_CAMPAIGN_STATUS_IdStatus != null && status.Contains((int)p.CAT_CAMPAIGN_STATUS_IdStatus)))
            .Skip((page - 1) * m.PageSize).Take(m.PageSize).ToList();
        }
        else if (idUserRole == (int)Roles.Publisher)
        {
          m.TotalRecord = _context.CAMPAIGNs1
            .OrderBy(sort + " " + sortdir)
            .Where(p => p.PRODUCT.AspNetUsers_Id.Equals(idUser) && (p.CAT_CAMPAIGN_STATUS_IdStatus != null && status.Contains((int)p.CAT_CAMPAIGN_STATUS_IdStatus))).Count();
          m.ListCampaigns = _context.CAMPAIGNs1
            .OrderBy(sort + " " + sortdir)
            .Where(p => p.PRODUCT.AspNetUsers_Id.Equals(idUser) && (p.CAT_CAMPAIGN_STATUS_IdStatus != null && status.Contains((int)p.CAT_CAMPAIGN_STATUS_IdStatus)))
            .Skip((page - 1) * m.PageSize).Take(m.PageSize).ToList();
        }
        m.NoOfPages = (m.TotalRecord / m.PageSize) + ((m.TotalRecord % m.PageSize) > 0 ? 1 : 0);
      }
      catch (Exception e)
      {
        var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
        telemetria.Critical(messageException);
      }
      return m;
    }

    public TableCampaignViewModel GetTableCampaignVerify(int idUserRole, string idUser, int page = 1, string sort = "RegisterDate", string sortdir = "DESC")
    {
      List<int> status = new List<int>() { (int)CatCampaignStatusEnum.Authorized };
      TableCampaignViewModel m = new TableCampaignViewModel();
      m.PageSize = PAGE_SIZE;

      try
      {
        if (idUserRole == (int)Roles.Advertiser)
        {
          m.TotalRecord = _context.CAMPAIGNs1
            .OrderBy(sort + " " + sortdir)
            .Where(p => p.AspNetUser.Id.Equals(idUser) && (p.CAT_CAMPAIGN_STATUS_IdStatus != null && status.Contains((int)p.CAT_CAMPAIGN_STATUS_IdStatus))).Count();
          m.ListCampaigns = _context.CAMPAIGNs1
            .OrderBy(sort + " " + sortdir)
            .Where(p => p.AspNetUser.Id.Equals(idUser) && (p.CAT_CAMPAIGN_STATUS_IdStatus != null && status.Contains((int)p.CAT_CAMPAIGN_STATUS_IdStatus)))
            .Skip((page - 1) * m.PageSize).Take(m.PageSize).ToList();

        }
        else if (idUserRole == (int)Roles.Publisher)
        {
          m.TotalRecord = _context.CAMPAIGNs1
            .OrderBy(sort + " " + sortdir)
            .Where(p => p.PRODUCT.AspNetUsers_Id.Equals(idUser) && (p.CAT_CAMPAIGN_STATUS_IdStatus != null && status.Contains((int)p.CAT_CAMPAIGN_STATUS_IdStatus))).Count();
          m.ListCampaigns = _context.CAMPAIGNs1
            .OrderBy(sort + " " + sortdir)
            .Where(p => p.PRODUCT.AspNetUsers_Id.Equals(idUser) && (p.CAT_CAMPAIGN_STATUS_IdStatus != null && status.Contains((int)p.CAT_CAMPAIGN_STATUS_IdStatus)))
            .Skip((page - 1) * m.PageSize).Take(m.PageSize).ToList();
        }
        m.NoOfPages = (m.TotalRecord / m.PageSize) + ((m.TotalRecord % m.PageSize) > 0 ? 1 : 0);
      }
      catch (Exception e)
      {
        var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
        telemetria.Critical(messageException);
      }
      return m;
    }

    public List<PRODUCT> GetProductsByIdCampaign(Guid idCampaign)
    {
      try
      {
        return (from p in _context.PRODUCTS
                from c in p.CAMPAIGNs
                where c.IdCampaign.Equals(idCampaign)
                select p).ToList();
      }
      catch (Exception e)
      {
        var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
        telemetria.Critical(messageException);
      }
      return new List<PRODUCT>();
    }

    public bool AutorizeCampaign(Models.CAMPAIGN campaign)
    {
      var product = campaign.PRODUCT;
      var user = (from r in _context.AspNetUsers where r.Email.Equals(campaign.AspNetUser.Email) select r).FirstOrDefault();
      var _createTransaction = Helpers.AsyncHelpers.RunSync<Models.Wallet.CreateTransactionModel>(() => Helpers.NethereumHelper.DoTransaction(user.WalletAddress, product.AspNetUser.WalletAddress, ((Double)product.Price * (Double)100000000).ToString()));


      try
      {
        if (campaign != null && campaign.IdCampaign != Guid.Empty && _createTransaction != null && _createTransaction.hashTransaction.Length > 5)
        {
          campaign.CAT_CAMPAIGN_STATUS_IdStatus = (int)CatCampaignStatusEnum.Authorized;
          _context.Entry(campaign).State = EntityState.Modified;
          TRANSACTIONS_CAPT _transaction = new TRANSACTIONS_CAPT()
          {
            Amount = _createTransaction.amount,
            HashFrom = _createTransaction.hashFrom,
            HashTo = _createTransaction.hashTo,
            HashTransaction = _createTransaction.hashTransaction,
            TRANSACTION_TYPE_IdTransactionType = 1,
            BlockDate = Helpers.DateTimeHelper.CurrentDateTimeString(),
            Gas = "0",
            RegisterDate = Helpers.DateTimeHelper.CurrentDateTimeString(),
            CAMPAIGN = campaign
          };
          _context.Set<TRANSACTIONS_CAPT>().Add(_transaction);

          campaign.CAT_CAMPAIGN_STATUS_IdStatus = (int)CatCampaignStatusEnum.Authorized;
          return _context.SaveChanges() > 0;
        }
      }
      catch (Exception e)
      {
        var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
        telemetria.Critical(messageException);
      }
      return false;
    }

    public bool RegisterCampaignMessage(CAMPAIGN_CHAT c)
    {
      try
      {
        c.IdCampaignMessage = Guid.NewGuid();
        c.RegisterDate = DateTime.Now;
        c.CampaignChatStatus = 1;
        _context.CAMPAIGN_CHAT.Add(c);
      }
      catch (Exception e)
      {
        var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
        telemetria.Critical(messageException);
      }
      return _context.SaveChanges() > 0;
    }

    public List<CAMPAIGN_CHAT> GetCampaignChatByIdCampaign(string idCampaign)
    {
      return (from r in _context.CAMPAIGN_CHAT where r.CAMPAIGN_IdCampaign.Equals(new Guid(idCampaign)) orderby r.RegisterDate descending select r).ToList();
    }

    private bool CreatePaymentTransaction(string idCampaign)
    {
      var campaign = GetCampaignById(idCampaign);
      var product = campaign.PRODUCT;
      var user = (from r in _context.AspNetUsers where r.Email.Equals(campaign.AspNetUser.Email) select r).FirstOrDefault();
      var _createTransaction = Helpers.AsyncHelpers.RunSync<Models.Wallet.CreateTransactionModel>(() => Helpers.NethereumHelper.DoTransaction(user.WalletAddress, product.AspNetUser.WalletAddress, ((Double)product.Price * (Double)100000000).ToString()));

      try
      {
        if (_createTransaction != null && _createTransaction.hashTransaction.Length > 5)
        {
          TRANSACTIONS_CAPT _transaction = new TRANSACTIONS_CAPT()
          {
            Amount = _createTransaction.amount,
            HashFrom = _createTransaction.hashFrom,
            HashTo = _createTransaction.hashTo,
            HashTransaction = _createTransaction.hashTransaction,
            TRANSACTION_TYPE_IdTransactionType = 1,
            BlockDate = Helpers.DateTimeHelper.CurrentDateTimeString(),
            Gas = "0",
            RegisterDate = Helpers.DateTimeHelper.CurrentDateTimeString(),
            CAMPAIGN = campaign
          };
          _context.Set<TRANSACTIONS_CAPT>().Add(_transaction);
          return _context.SaveChanges() > 0;
        }
      }
      catch (Exception e)
      {
        var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
        telemetria.Critical(messageException);
      }
      return false;
    }



    public string SetBroadcast(string key, string secret, string oAuth, string oAuthSecret, string oAuthVerifier, string list, string bodyHtml, string subject)
    {
      string result = null;

      try
      {
        API api = GetDataApiKeyAweber(key, secret, oAuth, oAuthSecret, oAuthVerifier);
        Aweber.Entity.Account account = api.getAccount();
        string endpoint = string.Format(Settings.createBroadcast, account.id, list);
        Request request = new Request
        {
          oauth_consumer_key = key,
          oauth_consumer_secret = secret,
          oauth_token = api.OAuthToken,
          oauth_token_secret = api.OAuthTokenSecret
        };

        SortedList<string, string> parameters = new SortedList<string, string>();
        parameters.Add("click_tracking_enabled", "True");
        parameters.Add("is_archived", "True");
        parameters.Add("notify_on_send", "True");
        parameters.Add("body_html", bodyHtml);
        parameters.Add("subject", subject);
        request.Build(parameters, endpoint);
        WebClient client = new WebClient();
        client.Headers["Content-type"] = "application/x-www-form-urlencoded";
        result = client.UploadString(endpoint, request.Parameters);
        var broadcast = JsonConvert.DeserializeObject<Aweber.Broadcast>(result);
        result = broadcast.broadcast_id;
      }
      catch (Exception e)
      {
        var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
        telemetria.Critical(messageException);
      }
      finally
      {
        result = result == "" ? null : result;
      }
      return result;
    }

    public string SetBroadcastSchedule(string key, string secret, string oAuth, string oAuthSecret, string oAuthVerifier, string list, string idBroadcast)
    {
      string result = null;
      try
      {
        API api = GetDataApiKeyAweber(key, secret, oAuth, oAuthSecret, oAuthVerifier);
        Aweber.Entity.Account account = api.getAccount();
        int idAccount = account.id;
        string endpoint = string.Format(Settings.scheduleBroadcast, idAccount, list, idBroadcast);
        Request request = new Request
        {
          oauth_consumer_key = key,
          oauth_consumer_secret = secret,
          oauth_token = api.OAuthToken,
          oauth_token_secret = api.OAuthTokenSecret
        };
        string date = DateTime.UtcNow.ToString("s") + "Z";
        SortedList<string, string> parameters = new SortedList<string, string>();
        parameters.Add("scheduled_for", date);
        request.Build(parameters, endpoint);
        WebClient client = new WebClient();
        client.Headers["Content-type"] = "application/x-www-form-urlencoded";
        result = client.UploadString(endpoint, request.Parameters);
      }
      catch (Exception e)
      {
        var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
        telemetria.Critical(messageException);
      }
      finally
      {
        result = result == "" ? null : result;
      }
      return result;
    }

    public string GetBroadcast(string key, string secret, string oAuth, string oAuthSecret, string oAuthVerifier, string list, string idBroadcast)
    {
      string result = null;
      try
      {
        API api = GetDataApiKeyAweber(key, secret, oAuth, oAuthSecret, oAuthVerifier);
        Aweber.Entity.Account account = api.getAccount();
        string endpoint = string.Format(Settings.getBroadcast, account.id, list, idBroadcast);
        Request request = new Request
        {
          oauth_consumer_key = key,
          oauth_consumer_secret = secret,
          oauth_token = api.OAuthToken,
          oauth_token_secret = api.OAuthTokenSecret
        };
        SortedList<string, string> parameters = new SortedList<string, string>();
        request.Build(parameters, endpoint, "GET");
        WebClient client = new WebClient();
        client.Headers["Content-type"] = "application/x-www-form-urlencoded";
        result = client.DownloadString(endpoint + "?" + request.Parameters);
        var broadcast = JsonConvert.DeserializeObject<Aweber.Broadcast>(result);
        result = broadcast.status;
      }
      catch (Exception e)
      {
        var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
        telemetria.Critical(messageException);
      }
      finally
      {
        result = result == "" ? null : result;
      }
      return result;
    }

    public API GetDataApiKeyAweber(string key, string secret, string oAuth, string oAuthSecret, string oAuthVerifier)
    {
      // Create a new api instance
      API api = new API(key, secret);
      try
      {
        api.OAuthToken = oAuth;
        api.OAuthTokenSecret = oAuthSecret;
        api.OAuthVerifier = oAuthVerifier;
        api.CallbackUrl = "http://localhost:11575/product/validateapitoken";
      }
      catch (Exception e)
      {
        var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
        telemetria.Critical(messageException);
      }
      return api;
    }


  }
}
