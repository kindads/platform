using KindAds.Business.Partners.Mail;
using KindAds.Common.Utils.Partners.Mail.Aweber;
using KindAds.Comun.Enums;
using KindAds.Comun.Interfaces;
using KindAds.Comun.Models;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using KindAds.Negocio.Managersv2;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Negocio.Partnersv2.Mail
{
    public class AWeberManagerv2 : BaseManager, IMailChannel
    {
        #region properties
        public IList<AudiencePropertieSetting> settings { set; get; }
        public bool AreSettingsValid { set; get; }

        public AWeberManager manager { set; get; }

        #endregion

        #region public methods

        public IMailChannel CreateChannel(IList<AudiencePropertieSetting> settings, string emailProductId)
        {
            return new AWeberManagerv2(settings, emailProductId);
        }

        public void LoadSettings(string emailProductId)
        {
            string collectionName = CosmosCollections.EmailProductSetting.ToString();
            string query = $"SELECT * FROM {collectionName} WHERE {collectionName}.ProductId='{emailProductId}'";
            this.settings = context.ExecuteQuery<AudiencePropertieSetting>(databaseName, collectionName, query);
        }

        #region Helpers
        public EmailProductDocument GetEmailProduct(string emailProductId)
        {
            EmailProductDocument emailProduct = new EmailProductDocument();
            string collectionName = CosmosCollections.EmailProduct.ToString();
            string query = $"SELECT * FROM {collectionName} WHERE {collectionName}.id='{emailProductId}'";
            emailProduct = context.ExecuteQuery<EmailProductDocument>(databaseName, collectionName, query).FirstOrDefault();
            return emailProduct;
        }

        public CampaignDocument GetCampaign(string audiencePropertieId)
        {
            CampaignDocument campaign = new CampaignDocument();
            string collectionName = CosmosCollections.Campaign.ToString();
            string query = $"SELECT * FROM {collectionName} WHERE {collectionName}.AudiencePropertieId='{audiencePropertieId}'";
            campaign = context.ExecuteQuery<CampaignDocument>(databaseName, collectionName, query).FirstOrDefault();
            return campaign;
        }

        public List<CampaignSettingDocument> GetCampaignSetting(string campaignId)
        {
            List<CampaignSettingDocument> settings = new List<CampaignSettingDocument>();
            string collectionName = CosmosCollections.CampaignSettings.ToString();
            string query = $"SELECT * FROM {collectionName} WHERE {collectionName}.CampaignId='{campaignId}'";
            settings = context.ExecuteQuery<CampaignSettingDocument>(databaseName, collectionName, query);
            return settings;
        }

        public CampaignDocument GetCampaignDocument(string emailProductId)
        {
            CampaignDocument campaign = new CampaignDocument();
            EmailProductDocument emailProduct = GetEmailProduct(emailProductId);
            campaign = GetCampaign(emailProduct.AudiencePropertieId);
            return campaign;
        }

        #endregion


        public string Send(string emailProductId)
        {
            string apiCampaignId = string.Empty;
            if (settings.Count > 0)
            {
                //no obtenemos los datos de bd
                manager.settings = this.settings;
                CampaignDocument campaign = GetCampaignDocument(emailProductId);
                List<CampaignSettingDocument> campaignSettings = GetCampaignSetting(campaign.Id);
                var request = manager.getRequestData(campaign, campaignSettings);
                apiCampaignId = manager.SendCampaign(request);

                // update campaign
                if (apiCampaignId != string.Empty)
                {
                    campaign.ApiCampaignId = apiCampaignId;
                    collectionName = CosmosCollections.Campaign.ToString();
                    context.AddDocument<CampaignDocument>(databaseName, collectionName, campaign);
                }
            }
            else
            {
                LoadSettings(emailProductId);
                manager.settings = this.settings;
                CampaignDocument campaign = GetCampaignDocument(emailProductId);
                List<CampaignSettingDocument> campaignSettings = GetCampaignSetting(campaign.Id);
                var request = manager.getRequestData(campaign, campaignSettings);
                apiCampaignId = manager.SendCampaign(request);

                // update campaign
                if (apiCampaignId != string.Empty)
                {
                    campaign.ApiCampaignId = apiCampaignId;
                    collectionName = CosmosCollections.Campaign.ToString();
                    context.AddDocument<CampaignDocument>(databaseName, collectionName, campaign);
                }
            }
            return apiCampaignId;
        }

        public bool SettingsAreValid(string emailProductId)
        {
            bool result = false;

            if (settings.Count > 0)
            {
                manager.settings = this.settings;
                result = manager.SettingsAreValid();
                result = true;
            }
            else
            {
                LoadSettings(emailProductId);
                manager.settings = this.settings;
                result = manager.SettingsAreValid();
            }
            return result;
        }

        public bool SettingsAreValid()
        {
            bool result = false;
            if (settings.Count > 0)
            {
                result = manager.SettingsAreValid();
            }
            return result;
        }

        public bool Save(string emailProductId)
        {
            bool result = false;
            string collectionName = CosmosCollections.EmailProductSetting.ToString();

            try
            {
                foreach (var setting in settings)
                {
                    EmailProductSettingDocument emailSetting = new EmailProductSettingDocument()
                    { Value = setting.Value, Name = setting.Name, ProductId = emailProductId };
                    context.AddDocument<EmailProductSettingDocument>(databaseName, collectionName, emailSetting);
                }
                result = true;
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
            manager = new AWeberManager();
            result = manager.ValidateApiToken(ApiToken);
            return result;
        }

        #endregion

        #region constructor

        public AWeberManagerv2(IList<AudiencePropertieSetting> settings, string emailProductId)
        {
            this.settings = settings;
            this.manager = new AWeberManager(settings);

            // command pattern
            AreSettingsValid = SettingsAreValid();
            if (AreSettingsValid)
            {
                Save(emailProductId);
            }
        }

        public AWeberManagerv2(string emailProductId)
        {
            LoadSettings(emailProductId);
            this.manager = new AWeberManager(settings);

            // command pattern
            AreSettingsValid = SettingsAreValid();
            if (AreSettingsValid)
            {
                Save(emailProductId);
            }
        }

        public ProviderAWeberApiResult GetAweberApiToken(string ApiKey)
        {
            ProviderAWeberApiResult result = new ProviderAWeberApiResult();
            try
            {
                var elements = ApiKey.Split('|');
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

                var account = api.getAccount();
                
                return result;
            }
            catch (Exception)
            {
                result.Success = false;
                return result;
            }
        }

        public AWeberManagerv2() { }

        #endregion
    }
}
