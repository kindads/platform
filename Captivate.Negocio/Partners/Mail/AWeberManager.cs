using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Captivate.Common.Interfaces;
using Captivate.Business;
using Captivate.DataAccess;
using Captivate.Common.Models.Entities;
using Captivate.Common.Utils.Partners.Mail.Aweber;
using System.Web;
using Captivate.Common.Utils.Partners.Mail.Aweber.OAuth;
using System.Net;
using Newtonsoft.Json;
using Captivate.Comun;
using Captivate.Azure;

namespace Captivate.Business.Partners.Mail
{
    public class AWeberManager
    {
        public ITrace telemetria { set; get; }
        public CampaignRepository CampaignRepository { set; get; }
        public ProductRepository ProductRepository { set; get; }

        public AWeberManager()
        {

            telemetria = new Trace();
            CampaignRepository = new CampaignRepository();
            ProductRepository = new ProductRepository();
        }

        public string ValidateCampaign(string idCampaign)
        {
            CampaignEntity campaign = CampaignRepository.FindById(new Guid(idCampaign));
            ProductEntity product = ProductRepository.FindById(campaign.PRODUCT_IdProduct);
            string campaignID = null;
            string appKey = null;
            string appSecret = null;
            string aList = null;
            string oAuthToken = null;
            string oAuthSecret = null;
            string oAuthVerify = null;

            string subject = null;
            string bodyHTML = null;
            string isArchived = null;

            if (product.ProductSettingsEntitys != null && product.ProductSettingsEntitys.Any())
            {
                foreach (var item in product.ProductSettingsEntitys)
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

        public string SetBroadcastSchedule(string key, string secret, string oAuth, string oAuthSecret, string oAuthVerifier, string list, string idBroadcast)
        {
            string result = null;
            try
            {
                API api = GetDataApiKeyAweber(key, secret, oAuth, oAuthSecret, oAuthVerifier);
                Captivate.Common.Utils.Partners.Mail.Aweber.Entity.Account account = api.getAccount();
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

        public void GetDataClientAweber(Guid idProduct)
        {
            var product = ProductRepository.FindById(idProduct);
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
                if (product.ProductSettingsEntitys != null && product.ProductSettingsEntitys.Any())
                {
                    foreach (var item in product.ProductSettingsEntitys)
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
                Captivate.Common.Utils.Partners.Mail.Aweber.Entity.Account account = api.getAccount();
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
    }
}
