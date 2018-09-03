using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KindAds.Common.Interfaces;
using KindAds.Business;
using KindAds.DataAccess;
using KindAds.Common.Models.Entities;
using KindAds.Common.Utils.Partners.Mail.Aweber;
using System.Web;
using KindAds.Common.Utils.Partners.Mail.Aweber.OAuth;
using System.Net;
using Newtonsoft.Json;
using KindAds.Comun;
using KindAds.Azure;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using KindAds.Comun.Models;

namespace KindAds.Business.Partners.Mail
{
    public class AWeberManager
    {
        public ITrace telemetria { set; get; }
        //public CampaignRepository CampaignRepository { set; get; }
        //public ProductRepository ProductRepository { set; get; }

        public IList<AudiencePropertieSetting> settings { set; get; }

        public AWeberManager()
        {

            telemetria = new Trace();
            //CampaignRepository = new CampaignRepository();
            //ProductRepository = new ProductRepository();
        }

        public AWeberManager(IList<AudiencePropertieSetting> settings)
        {
            this.settings = settings;
            telemetria = new Trace();
        }


        public bool SettingsAreValid(string idCampaign)
        {
            bool result = false;
            result = ListIsValid(idCampaign);
            return result;
        }

        public bool SettingsAreValid()
        {
            bool result = false;
            result = ListIsValid();
            return result;
        }


        public string GetSettingFromCosmos(string setting)
        {            
            string value = string.Empty;
            switch (setting)
            {
                case "ApiKey":
                    {
                        foreach (var item in settings)
                        {
                            value = item.Name.Equals("aweberApplicationKey") ? item.Value : value;
                        }
                    }
                    break;
                case "List":
                    {
                        foreach (var item in settings)
                        {
                            value = item.Name.Equals("aweberList") ? item.Value : value;
                        }
                    }
                    break;
                case "Secret":
                    {
                        foreach (var item in settings)
                        {
                            value = item.Name.Equals("aweberApplicationSecret") ? item.Value : value;
                        }
                    }
                    break;
                case "OAuth":
                    {
                        foreach (var item in settings)
                        {
                            value = item.Name.Equals("aweberOAuthToken") ? item.Value : value;
                        }
                    }
                    break;
                case "OAuthSecret":
                    {
                        foreach (var item in settings)
                        {
                            value = item.Name.Equals("aweberOAuthTokenSecret") ? item.Value : value;
                        }
                    }
                    break;
                case "OAuthVerifier":
                    {
                        foreach (var item in settings)
                        {
                            value = item.Name.Equals("aweberOauthVerifier") ? item.Value : value;
                        }
                    }
                    break;
            }
            return value;
        }

        [Obsolete]
        public string GetSetting(string setting, string idCampaign)
        {
        //    ProductSettingsRepository repository = new ProductSettingsRepository();
        //    CampaignEntity campaign = CampaignRepository.FindById(new Guid(idCampaign));
        //    ProductEntity product = ProductRepository.FindById(campaign.PRODUCT_IdProduct);
        //    List<ProductSettingsEntity> settings = repository.GetProductSettingsByIdProduct(product.IdProduct);

            string value = string.Empty;
            //switch (setting)
            //{
            //    case "ApiKey":
            //        {
            //            foreach (var item in settings)
            //            {
            //                value = item.SettingName.Equals("aweberApplicationKey") ? item.SettingValue : value;
            //            }
            //        }
            //        break;
            //    case "List":
            //        {
            //            foreach (var item in settings)
            //            {
            //                value = item.SettingName.Equals("aweberList") ? item.SettingValue : value;
            //            }
            //        }
            //        break;
            //    case "Secret":
            //        {
            //            foreach (var item in settings)
            //            {
            //                value = item.SettingName.Equals("aweberApplicationSecret") ? item.SettingValue : value;
            //            }
            //        }
            //        break;
            //    case "OAuth":
            //        {
            //            foreach (var item in settings)
            //            {
            //                value = item.SettingName.Equals("aweberOAuthToken") ? item.SettingValue : value;
            //            }
            //        }
            //        break;
            //    case "OAuthSecret":
            //        {
            //            foreach (var item in settings)
            //            {
            //                value = item.SettingName.Equals("aweberOAuthTokenSecret") ? item.SettingValue : value;
            //            }
            //        }
            //        break;
            //    case "OAuthVerifier":
            //        {
            //            foreach (var item in settings)
            //            {
            //                value = item.SettingName.Equals("aweberOauthVerifier") ? item.SettingValue : value;
            //            }
            //        }
            //        break;
            //}
            return value;
        }

        public bool ListIsValid()
        {
            bool result = false;
            string key = string.Empty;
            string secret = string.Empty;
            string oAuth = string.Empty;
            string oAuthSecret = string.Empty;
            string oAuthVerifier = string.Empty;
            string listId = string.Empty;

            try
            {
                key = GetSettingFromCosmos("ApiKey");
                secret = GetSettingFromCosmos("Secret");
                oAuth = GetSettingFromCosmos("OAuth");
                oAuthSecret = GetSettingFromCosmos("OAuthSecret");
                oAuthVerifier = GetSettingFromCosmos("OAuthVerifier");
                listId = GetSettingFromCosmos("List");

                API api = GetDataApiKeyAweber(key, secret, oAuth, oAuthSecret, oAuthVerifier);
                Common.Utils.Partners.Mail.Aweber.Entity.Account account = api.getAccount();

                foreach (Common.Utils.Partners.Mail.Aweber.Entity.List list in account.lists().entries)
                {
                    if (list.id.ToString() == listId)
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

        public bool ListIsValid(string idCampaign)
        {
            bool result = false;
            string key = string.Empty;
            string secret = string.Empty;
            string oAuth = string.Empty;
            string oAuthSecret = string.Empty;
            string oAuthVerifier = string.Empty;
            string listId = string.Empty;

            try
            {
                key = GetSetting("ApiKey", idCampaign);
                secret= GetSetting("Secret", idCampaign);
                oAuth= GetSetting("OAuth", idCampaign);
                oAuthSecret= GetSetting("OAuthSecret", idCampaign); 
                oAuthVerifier= GetSetting("OAuthVerifier", idCampaign);
                listId = GetSetting("List", idCampaign);

                API api = GetDataApiKeyAweber(key, secret, oAuth, oAuthSecret, oAuthVerifier);
                Common.Utils.Partners.Mail.Aweber.Entity.Account account = api.getAccount();

                foreach (Common.Utils.Partners.Mail.Aweber.Entity.List list in account.lists().entries)
                {
                    if(list.id.ToString()==listId)
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

        public AWeberCampaignRequest FillSendinBlueRequestFromCosmos(List<CampaignSettingDocument> campaignSettings)
        {
            AWeberCampaignRequest request = new AWeberCampaignRequest();
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
                            case "aweberApplicationKey":
                                {
                                    request.AppKey = item.Name.Equals("aweberApplicationKey") ? item.Value : string.Empty;
                                }
                                break;
                            case "aweberApplicationSecret":
                                {
                                    request.AppSecret = item.Name.Equals("aweberApplicationSecret") ? item.Value : string.Empty;
                                }
                                break;
                            case "aweberOAuthToken":
                                {
                                    request.OAuthToken = item.Name.Equals("aweberOAuthToken") ? item.Value : string.Empty;
                                }
                                break;
                            case "aweberOAuthTokenSecret":
                                {
                                    request.OAuthSecret = item.Name.Equals("aweberOAuthTokenSecret") ? item.Value : string.Empty;
                                }
                                break;
                            case "aweberOauthVerifier":
                                {
                                    request.OAuthVerify = item.Name.Equals("aweberOauthVerifier") ? item.Value : string.Empty;
                                }
                                break;
                            case "aweberList":
                                {
                                    request.AList = item.Name.Equals("aweberList") ? item.Value : string.Empty;
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
                            case "aweberSubject":
                                {
                                    request.Subject = setting.Name.Equals("aweberSubject") ? setting.Value : string.Empty;
                                }
                                break;
                            case "aweberBodyHtml":
                                {
                                    request.BodyHTML = setting.Name.Equals("aweberBodyHtml") ? setting.Value : string.Empty;
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

        public AWeberCampaignRequest getRequestData(CampaignDocument campaign, List<CampaignSettingDocument> settings)
        {
            AWeberCampaignRequest request = FillSendinBlueRequestFromCosmos(settings);
            request.Name = campaign.Name;
            request.Text = campaign.Text;
            return request;
        }


        public string SendCampaign(AWeberCampaignRequest request)
        {
            string Id = string.Empty;
            try
            {
                Id = SetBroadcast(request.AppKey, request.AppSecret, request.OAuthToken, request.OAuthSecret, request.OAuthVerify, request.AList, request.BodyHTML, request.Subject);
                string schedule = SetBroadcastSchedule(request.AppKey, request.AppSecret, request.OAuthToken, request.OAuthSecret, request.OAuthVerify, request.AList, Id);
                string broadcast = GetBroadcast(request.AppKey, request.AppSecret, request.OAuthToken, request.OAuthSecret, request.OAuthVerify, request.AList, Id);
                if (broadcast == "scheduled")
                    return Id;
                else
                    return null;
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
            //ProductEntity product = ProductRepository.FindById(campaign.PRODUCT_IdProduct);
            //string campaignID = null;
            //string appKey = null;
            //string appSecret = null;
            //string aList = null;
            //string oAuthToken = null;
            //string oAuthSecret = null;
            //string oAuthVerify = null;

            //string subject = null;
            //string bodyHTML = null;
            //string isArchived = null;

            //if (product.ProductSettingsEntitys != null && product.ProductSettingsEntitys.Any())
            //{
            //    foreach (var item in product.ProductSettingsEntitys)
            //    {
            //        appKey = item.SettingName.Equals("aweberApplicationKey") ? item.SettingValue : appKey;
            //        appSecret = item.SettingName.Equals("aweberApplicationSecret") ? item.SettingValue : appSecret;
            //        oAuthToken = item.SettingName.Equals("aweberOAuthToken") ? item.SettingValue : oAuthToken;
            //        oAuthSecret = item.SettingName.Equals("aweberOAuthTokenSecret") ? item.SettingValue : oAuthSecret;
            //        oAuthVerify = item.SettingName.Equals("aweberOauthVerifier") ? item.SettingValue : oAuthVerify;
            //        aList = item.SettingName.Equals("aweberList") ? item.SettingValue : aList;
            //    }
            //}

            //if (campaign.CAMPAIGN_SETTINGS != null && campaign.CAMPAIGN_SETTINGS.Any())
            //{
            //    foreach (var setting in campaign.CAMPAIGN_SETTINGS)
            //    {
            //        subject = setting.SettingName.Equals("aweberSubject") ? setting.SettingValue : subject;
            //        bodyHTML = setting.SettingName.Equals("aweberBodyHtml") ? setting.SettingValue : bodyHTML;
            //        isArchived = setting.SettingName.Equals("aweberIsArchived") ? setting.SettingValue : isArchived;
            //    }
            //}

            //try
            //{
            //    campaignID = SetBroadcast(appKey, appSecret, oAuthToken, oAuthSecret, oAuthVerify, aList, bodyHTML, subject);
            //    string schedule = SetBroadcastSchedule(appKey, appSecret, oAuthToken, oAuthSecret, oAuthVerify, aList, campaignID);
            //    string broadcast = GetBroadcast(appKey, appSecret, oAuthToken, oAuthSecret, oAuthVerify, aList, campaignID);
            //    if (broadcast == "scheduled")
            //        return campaignID;
            //    else
            //        return null;
            //}
            //catch (Exception e)
            //{
            //    var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
            //    telemetria.Critical(messageException);
            //    campaignID = e.Message;
            //}
            return null;
        }

        public string SetBroadcastSchedule(string key, string secret, string oAuth, string oAuthSecret, string oAuthVerifier, string list, string idBroadcast)
        {
            string result = null;
            try
            {
                API api = GetDataApiKeyAweber(key, secret, oAuth, oAuthSecret, oAuthVerifier);
                KindAds.Common.Utils.Partners.Mail.Aweber.Entity.Account account = api.getAccount();
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

        [Obsolete]
        public void GetDataClientAweber(Guid idProduct)
        {
            //var product = ProductRepository.FindById(idProduct);
            //string apiKey = null;
            //string aweberList = null;
            //string appKey = null;
            //string appSecret = null;
            //string oAuthToken = null;
            //string oAuthSecret = null;
            //string oAuthVerify = null;
            //string callBackUrl = null;

            //try
            //{
            //    if (product.ProductSettingsEntitys != null && product.ProductSettingsEntitys.Any())
            //    {
            //        foreach (var item in product.ProductSettingsEntitys)
            //        {
            //            apiKey = item.SettingName.Equals("aweberApiToken") ? item.SettingValue : apiKey;
            //            aweberList = item.SettingName.Equals("aweberList") ? item.SettingValue : aweberList;
            //            appKey = item.SettingName.Equals("aweberApplicationKey") ? item.SettingValue : appKey;
            //            appSecret = item.SettingName.Equals("aweberApplicationSecret") ? item.SettingValue : appSecret;
            //            oAuthToken = item.SettingName.Equals("aweberOAuthToken") ? item.SettingValue : oAuthToken;
            //            oAuthSecret = item.SettingName.Equals("aweberOAuthTokenSecret") ? item.SettingValue : oAuthSecret;
            //            oAuthVerify = item.SettingName.Equals("aweberOauthVerifier") ? item.SettingValue : oAuthVerify;
            //            callBackUrl = item.SettingName.Equals("aweberCallbackURL") ? item.SettingValue : callBackUrl;
            //        }
            //    }
            //}
            //catch (Exception e)
            //{
            //    var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
            //    telemetria.Critical(messageException);
            //}
        }

        public string SetBroadcast(string key, string secret, string oAuth, string oAuthSecret, string oAuthVerifier, string list, string bodyHtml, string subject)
        {
            string result = null;

            try
            {
                API api = GetDataApiKeyAweber(key, secret, oAuth, oAuthSecret, oAuthVerifier);
                Common.Utils.Partners.Mail.Aweber.Entity.Account account = api.getAccount();
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
                var broadcast = JsonConvert.DeserializeObject<Broadcast>(result);
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

        public string GetBroadcast(string key, string secret, string oAuth, string oAuthSecret, string oAuthVerifier, string list, string idBroadcast)
        {
            string result = null;
            try
            {
                API api = GetDataApiKeyAweber(key, secret, oAuth, oAuthSecret, oAuthVerifier);
                KindAds.Common.Utils.Partners.Mail.Aweber.Entity.Account account = api.getAccount();
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
                var broadcast = JsonConvert.DeserializeObject<Broadcast>(result);
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

        public bool ValidateApiToken(string ApiToken)
        {
            bool isValid = false;
            ProviderAWeberApiResult result = new ProviderAWeberApiResult();
            try
            {
                var elements = ApiToken.Split('|');
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

                isValid = true;
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return isValid;
        }
    }
}
