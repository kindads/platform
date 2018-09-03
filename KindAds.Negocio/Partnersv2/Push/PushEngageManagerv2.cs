using KindAds.Business.Partners.Push;
using KindAds.Comun.Enums;
using KindAds.Comun.Interfaces;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using KindAds.Negocio.Managersv2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Negocio.Partnersv2.Push
{
    public class PushEngageManagerv2 : BaseManager,IPushChannel
    {
        #region properties
        public IList<AudiencePropertieSetting> settings { set; get; }
        public bool AreSettingsValid { set; get; }

        public PushEngageManger manager { set; get; }

        #endregion

        #region public methods
        public IPushChannel CreateChannel(IList<AudiencePropertieSetting> settings, string emailProductId)
        {
            return new PushEngageManagerv2(settings, emailProductId);
        }

        public void LoadSettings(string emailProductId)
        {
            string collectionName = CosmosCollections.PushProductSetting.ToString();
            string query = $"SELECT * FROM {collectionName} WHERE {collectionName}.ProductId='{emailProductId}'";
            this.settings = context.ExecuteQuery<AudiencePropertieSetting>(databaseName, collectionName, query);
        }

        public bool Save(string emailProductId)
        {
            bool result = false;
            string collectionName = CosmosCollections.PushProductSetting.ToString();

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

        public IList<CampaignSettingDocument> GetCampaignSetting(string campaignId)
        {
            IList<CampaignSettingDocument> settings = new List<CampaignSettingDocument>();
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


        public string Send(string pushProductId)
        {
            string apiCampaignId = string.Empty;
            if (settings.Count > 0)
            {
                // no obtenemos los datos de bd
                manager.settings = this.settings;
                CampaignDocument campaign = GetCampaignDocument(pushProductId);
                IList<CampaignSettingDocument> campaignSettings = GetCampaignSetting(campaign.Id);
                manager.campaignSettings = campaignSettings;
                var request = manager.getRequestData(campaign);

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
                LoadSettings(pushProductId);
                manager.settings = this.settings;
                CampaignDocument campaign = GetCampaignDocument(pushProductId);
                IList<CampaignSettingDocument> campaignSettings = GetCampaignSetting(campaign.Id);
                manager.campaignSettings = campaignSettings;
                var request = manager.getRequestData(campaign);
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

        public bool ValidateApiToken(string ApiToken)
        {
            bool result = false;
            manager = new PushEngageManger();
            result = manager.IsValid(ApiToken);
            return result;
        }

        #endregion

        #region constructors
        public PushEngageManagerv2() { }

        public PushEngageManagerv2(string emailProductId)
        {
            LoadSettings(emailProductId);

            if(settings!=null && settings.Count>0)
            {
                this.manager = new PushEngageManger(settings);
            }
            
            // command pattern
            AreSettingsValid = SettingsAreValid();
            if (AreSettingsValid)
            {
                Save(emailProductId);
            }
        }

        public PushEngageManagerv2(IList<AudiencePropertieSetting> settings, string emailProductId)
        {
            this.settings = settings;
            if (settings != null && settings.Count > 0)
            {
                this.manager = new PushEngageManger(settings);
            }

            // command pattern
            AreSettingsValid = SettingsAreValid();
            if (AreSettingsValid)
            {
                Save(emailProductId);
            }
        }
        #endregion
    }
}
