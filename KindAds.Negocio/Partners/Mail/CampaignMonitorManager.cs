using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KindAds.Common.Interfaces;
using KindAds.Common.Models.Entities;
using KindAds.Business;
using KindAds.DataAccess;
using createsend_dotnet;
using KindAds.Azure;
using System.Net;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using KindAds.Comun.Models;

namespace KindAds.Business.Partners.Mail
{
    public class CampaignMonitorManager
    {
        public ITrace telemetria { set; get; }
        //public CampaignRepository CampaignRepository { set; get; }
        public ProductRepository ProductRepository { set; get; }

        //private readonly ProductSettingsRepository productSettingsRepository;
        //private readonly CampaignSettingsRepository campaignSettingsRepository;

        public IList<AudiencePropertieSetting> settings { set; get; }

        public CampaignMonitorManager()
        {
            telemetria = new Trace();
            //CampaignRepository = new CampaignRepository();
            //campaignSettingsRepository = new CampaignSettingsRepository();
            //ProductRepository = new ProductRepository();
            //productSettingsRepository = new ProductSettingsRepository();
        }

        public CampaignMonitorManager(IList<AudiencePropertieSetting> settings)
        {
            this.settings = settings;
            telemetria = new Trace();
        }

        public bool SettingsAreValid(string idCampaign)
        {
            bool result = false;
            result = (ApiTokenIsValid(idCampaign) && ListIsValid(idCampaign) && SegmentIsValid(idCampaign) && ClientIsValid(idCampaign)) ? true : false;
            return result;
        }

        public bool SettingsAreValid()
        {
            bool result = false;
            result = (ApiTokenIsValid() && ListIsValid() && SegmentIsValid() && ClientIsValid()) ? true : false;
            return result;
        }

        #region setting validation methods


        public string GetSettingFromCosmos(string setting)
        {
            string value = string.Empty;

            try
            {
                switch (setting)
                {
                    case "ApiKey":
                        {
                            foreach (var item in settings)
                            {
                                value = item.Name.Equals("campaignMonitorApiToken") ? item.Value : value;
                            }
                        }
                        break;
                    case "Client":
                        {
                            foreach (var item in settings)
                            {
                                value = item.Name.Equals("campaignMonitorClient") ? item.Value : value;
                            }
                        }
                        break;
                    case "List":
                        {
                            foreach (var item in settings)
                            {
                                value = item.Name.Equals("campaignMonitorList") ? item.Value : value;
                            }
                        }
                        break;
                    case "Segment":
                        {
                            foreach (var item in settings)
                            {
                                value = item.Name.Equals("campaignMonitorSegment") ? item.Value : value;
                            }
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }

            return value;
        }

        [Obsolete]
        public string GetSetting(string setting, string idCampaign)
        {
            string value = string.Empty;
            //ProductSettingsRepository repository = new ProductSettingsRepository();
            //CampaignEntity campaign = CampaignRepository.FindById(new Guid(idCampaign));
            //ProductEntity product = ProductRepository.FindById(campaign.PRODUCT_IdProduct);
            //List<ProductSettingsEntity> settings = repository.GetProductSettingsByIdProduct(product.IdProduct);

            //try
            //{
            //    switch (setting)
            //    {
            //        case "ApiKey":
            //            {
            //                foreach (var item in settings)
            //                {
            //                    value = item.SettingName.Equals("campaignMonitorApiToken") ? item.SettingValue : value;
            //                }
            //            }
            //            break;
            //        case "Client":
            //            {
            //                foreach (var item in settings)
            //                {
            //                    value = item.SettingName.Equals("campaignMonitorClient") ? item.SettingValue : value;
            //                }
            //            }
            //            break;
            //        case "List":
            //            {
            //                foreach (var item in settings)
            //                {
            //                    value = item.SettingName.Equals("campaignMonitorList") ? item.SettingValue : value;
            //                }
            //            }
            //            break;
            //        case "Segment":
            //            {
            //                foreach (var item in settings)
            //                {
            //                    value = item.SettingName.Equals("campaignMonitorSegment") ? item.SettingValue : value;
            //                }
            //            }
            //            break;
            //    }
            //}
            //catch (Exception e)
            //{
            //    var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
            //    telemetria.Critical(messageException);
            //}

            return value;
        }

        #region nuevos metodos de validacion

        private bool ApiTokenIsValid()
        {
            bool result = false;
            string apiKey = string.Empty;
            string client = string.Empty;
            string list = string.Empty;

            try
            {
                apiKey = GetSettingFromCosmos("ApiKey");
                client = GetSettingFromCosmos("Client");
                list = GetSettingFromCosmos("List");

                AuthenticationDetails auth = new ApiKeyAuthenticationDetails(apiKey);
                General general = new General(auth);
                var lista = general.Clients();
                if (lista != null)
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return result;
        }

        private bool ClientIsValid()
        {
            bool result = false;
            string apiKey = string.Empty;
            string client = string.Empty;
            string list = string.Empty;

            try
            {
                apiKey = GetSettingFromCosmos("ApiKey");
                client = GetSettingFromCosmos("Client");
                list = GetSettingFromCosmos("List");

                AuthenticationDetails auth = new ApiKeyAuthenticationDetails(apiKey);
                General general = new General(auth);
                var lista = general.Clients();
                if (lista != null)
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return result;
        }

        private bool ListIsValid()
        {
            bool result = false;
            string apiKey = string.Empty;
            string client = string.Empty;
            string list = string.Empty;

            try
            {
                apiKey = GetSettingFromCosmos("ApiKey");
                client = GetSettingFromCosmos("Client");
                list = GetSettingFromCosmos("List");

                AuthenticationDetails auth = new ApiKeyAuthenticationDetails(apiKey);
                Client monitorClient = new Client(auth, client);
                var lista = monitorClient.Lists();
                foreach (var item in lista)
                {
                    if (item.ListID == list)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return result;
        }

        public bool ValidateApiToken(string ApiToken)
        {
            bool result = false;
            AuthenticationDetails auth = new ApiKeyAuthenticationDetails(ApiToken);
            try
            {
                result= new General(auth).SystemDate() != null ? true : false;
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return result;
        }

        private bool SegmentIsValid()
        {
            bool result = false;
            string apiKey = string.Empty;
            string client = string.Empty;
            string segment = string.Empty;

            try
            {
                //Todo
                apiKey = GetSettingFromCosmos("ApiKey");
                segment = GetSettingFromCosmos("Segment");
                client = GetSettingFromCosmos("Client");

                AuthenticationDetails auth = new ApiKeyAuthenticationDetails(apiKey);
                Client monitorClient = new Client(auth, client);
                var list = monitorClient.Segments();
                foreach (var item in list)
                {
                    if (item.SegmentID == segment)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return result;
        }

        #endregion

        private bool ApiTokenIsValid(string idCampaign)
        {
            bool result = false;
            string apiKey = string.Empty;
            string client = string.Empty;
            string list = string.Empty;

            try
            {
                apiKey = GetSetting("ApiKey", idCampaign);
                client = GetSetting("Client", idCampaign);
                list = GetSetting("List", idCampaign);

                AuthenticationDetails auth = new ApiKeyAuthenticationDetails(apiKey);
                General general = new General(auth);
                var lista = general.Clients();
                if (lista != null)
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return result;
        }

        private bool ClientIsValid(string idCampaign)
        {
            bool result = false;
            string apiKey = string.Empty;
            string client = string.Empty;
            string list = string.Empty;

            try
            {
                apiKey = GetSetting("ApiKey", idCampaign);
                client = GetSetting("Client", idCampaign);
                list = GetSetting("List", idCampaign);

                AuthenticationDetails auth = new ApiKeyAuthenticationDetails(apiKey);
                General general = new General(auth);
                var lista = general.Clients();
                if(lista!=null)
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return result;
        }

        private bool ListIsValid(string idCampaign)
        {
            bool result = false;
            string apiKey = string.Empty;
            string client = string.Empty;
            string list = string.Empty;

            try
            {
                apiKey = GetSetting("ApiKey", idCampaign);
                client = GetSetting("Client", idCampaign);
                list = GetSetting("List", idCampaign);

                AuthenticationDetails auth = new ApiKeyAuthenticationDetails(apiKey);
                Client monitorClient = new Client(auth, client);
                var lista = monitorClient.Lists();
                foreach(var item in lista)
                {
                    if(item.ListID==list)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return result;
        }

        private bool SegmentIsValid(string idCampaign)
        {
            bool result = false;
            string apiKey = string.Empty;
            string client = string.Empty;
            string segment = string.Empty;

            try
            {
                //Todo
                apiKey = GetSetting("ApiKey", idCampaign);
                segment = GetSetting("Segment", idCampaign);
                client = GetSetting("Client", idCampaign);

                AuthenticationDetails auth = new ApiKeyAuthenticationDetails(apiKey);
                Client monitorClient = new Client(auth, client);
                var list = monitorClient.Segments();
                foreach(var item in list)
                {
                    if(item.SegmentID == segment)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return result;
        }

        #endregion

        public CampaignMonitorRequest FillSendinBlueRequestFromCosmos(List<CampaignSettingDocument> campaignSettings)
        {
            CampaignMonitorRequest request = new CampaignMonitorRequest();
            //todo
            try
            {
                // from products
                if (this.settings.Count > 0)
                {
                    foreach (var item in settings)
                    {

                        switch (item.Name)
                        {
                            case "campaignMonitorApiToken":
                                {
                                    request.ApiKey = item.Name.Equals("campaignMonitorApiToken") ? item.Value : string.Empty;
                                }
                                break;
                            case "campaignMonitorList":
                                {
                                    request.ListId = item.Name.Equals("campaignMonitorList") ? item.Value : string.Empty;
                                }
                                break;
                            case "campaignMonitorClient":
                                {
                                    request.ClientID = item.Name.Equals("campaignMonitorClient") ? item.Value : string.Empty;
                                }
                                break;
                            case "campaignMonitorSegment":
                                {
                                    request.SegmentIDs = item.Name.Equals("campaignMonitorSegment") ? item.Value : string.Empty;
                                }
                                break;
                        }
                    }
                }

                // from campaigns
                if (campaignSettings.Count > 0)
                {
                    foreach (var setting in campaignSettings)
                    {
                        switch (setting.Name)
                        {
                            case "campaignMonitorSubject":
                                {
                                    request.Subject = setting.Name.Equals("campaignMonitorSubject") ? setting.Value : string.Empty;
                                }
                                break;
                            case "campaignMonitorFromName":
                                {
                                    request.Subject = setting.Name.Equals("campaignMonitorFromName") ? setting.Value : string.Empty;
                                }
                                break;
                            case "campaignMonitorFromEmail":
                                {
                                    request.Subject = setting.Name.Equals("campaignMonitorFromEmail") ? setting.Value : string.Empty;
                                }
                                break;
                        }
                    }
                }

            }
            catch (Exception e)
            {
                string exceptionMessage = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(exceptionMessage);
            }
            return request;
        }

        public CampaignMonitorRequest getRequestData(CampaignDocument campaign, List<CampaignSettingDocument> settings)
        {
            CampaignMonitorRequest request = FillSendinBlueRequestFromCosmos(settings);
            request.Name = campaign.Name;
            request.Text = campaign.Text;
            return request;
        }

        public string SendCampaign(CampaignMonitorRequest request)
        {
            string Id = string.Empty;
            try
            {
                AuthenticationDetails auth = new ApiKeyAuthenticationDetails(request.ApiKey);
                List<string> listIDs = new List<string> { request.ListId };
                List<string> segmentIDs = new List<string> { request.SegmentIDs };

                TemplateContent templateContent = new TemplateContent();

                string campaignID = createsend_dotnet.Campaign.CreateFromTemplate(
                    auth,
                    request.ClientID,
                    request.Subject,
                    request.Name,
                    request.FromName + "-" + DateTime.Now.ToString("MMddyyyyhhmm"),
                    request.FromEmail,
                    request.FromEmail,
                    listIDs,
                    segmentIDs,
                    CreateTemplateFromCosmosDocument(auth, request.ClientID, request.campaign),
                    templateContent);

                createsend_dotnet.Campaign campaignMonitorCampaign = new createsend_dotnet.Campaign(auth, campaignID);
                campaignMonitorCampaign.Send(request.FromEmail);
                Id=campaignID;
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return Id;
        }

        [Obsolete]
        public string SendCampaign(string idCampaign)
        {
            //CampaignEntity campaign = CampaignRepository.FindById(new Guid(idCampaign));
            //campaign.CAMPAIGN_SETTINGS = campaignSettingsRepository.GetCampaignSettingsByIdCampaign(campaign.IdCampaign);
            //ProductEntity product = ProductRepository.FindById(campaign.PRODUCT_IdProduct);
            //product.ProductSettingsEntitys = productSettingsRepository.GetProductSettingsByIdProduct(product.IdProduct);
            //string idList = null;
            //string apiKey = null;
            //string clientID = null;
            //string subject = "";
            //string fromEmail = "";
            //string fromName = "";
            //string segment = "";


            //if (product.ProductSettingsEntitys != null && product.ProductSettingsEntitys.Any())
            //{
            //    foreach (var item in product.ProductSettingsEntitys)
            //    {
            //        apiKey = item.SettingName.Equals("campaignMonitorApiToken") ? item.SettingValue : apiKey;
            //        idList = item.SettingName.Equals("campaignMonitorList") ? item.SettingValue : idList;
            //        clientID = item.SettingName.Equals("campaignMonitorClient") ? item.SettingValue : clientID;
            //        segment = item.SettingName.Equals("campaignMonitorSegment") ? item.SettingValue : segment;
            //    }
            //}

            //if (campaign.CAMPAIGN_SETTINGS != null && campaign.CAMPAIGN_SETTINGS.Any())
            //{
            //    foreach (var setting in campaign.CAMPAIGN_SETTINGS)
            //    {
            //        subject = setting.SettingName.Equals("campaignMonitorSubject") ? setting.SettingValue : subject;
            //        fromName = setting.SettingName.Equals("campaignMonitorFromName") ? setting.SettingValue : fromName;
            //        fromEmail = setting.SettingName.Equals("campaignMonitorFromEmail") ? setting.SettingValue : fromEmail;
            //    }
            //}

            //try
            //{
            //    AuthenticationDetails auth = new ApiKeyAuthenticationDetails(apiKey);
            //    List<string> listIDs = new List<string> { idList };
            //    List<string> segmentIDs = new List<string> { segment };

            //    TemplateContent templateContent = new TemplateContent();

            //    string campaignID = createsend_dotnet.Campaign.CreateFromTemplate(
            //        auth,
            //        clientID,
            //        subject,
            //        campaign.Name,
            //        fromName + "-" + DateTime.Now.ToString("MMddyyyyhhmm"),
            //        fromEmail,
            //        fromEmail,
            //        listIDs,
            //        segmentIDs,
            //        CreateTemplate(auth, clientID, campaign),
            //        templateContent);

            //    createsend_dotnet.Campaign campaignMonitorCampaign = new createsend_dotnet.Campaign(auth, campaignID);
            //    campaignMonitorCampaign.Send(fromEmail);
            //    return campaignID;
            //}
            //catch (Exception e)
            //{
            //    var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
            //    telemetria.Critical(messageException);
            //}

            return null;
        }

        private string CreateTemplate(AuthenticationDetails auth, string clientID, CampaignEntity campaign)
        {
            string template = string.Empty;
            try
            {
                var htmlText = WebUtility.HtmlDecode(GenerateTemplateCampaignMonitor(campaign));
                var templateID = BlobManager.CreateHtmlTemplateAzure(htmlText);
                template = Template.Create(auth, clientID, "TEMPLATE " + campaign.Name, templateID, "");
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return template;
        }

        private string GenerateTemplateCampaignMonitor(CampaignEntity campaign)
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


        private string CreateTemplateFromCosmosDocument(AuthenticationDetails auth, string clientID, CampaignDocument campaign)
        {
            string template = string.Empty;
            try
            {
                var htmlText = WebUtility.HtmlDecode(GenerateTemplateCampaignMonitorFromCosmosDocument(campaign));
                var templateID = BlobManager.CreateHtmlTemplateAzure(htmlText);
                template = Template.Create(auth, clientID, "TEMPLATE " + campaign.Name, templateID, "");
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return template;
        }

        private string GenerateTemplateCampaignMonitorFromCosmosDocument(CampaignDocument campaign)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                sb.Append("<html><head><title>").Append(campaign.Name).Append("</title></head>");
                sb.Append("<body><p><singleline>").Append(campaign.Name).Append("</singleline></p>");
                sb.Append("<div><multiline>").Append(campaign.Text).Append("</multiline></div>");
                sb.Append("<p><unsubscribe>Unsubscribe</unsubscribe></p></body></html>");
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return sb.ToString();
        }
    }
}
