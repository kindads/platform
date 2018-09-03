using KindAds.Business.Partners.Mail;
using KindAds.Comun.Enums;
using KindAds.Comun.Interfaces;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using KindAds.Negocio.Managersv2;
using KindAdsV2.Azure;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Negocio.Partnersv2.Mail
{
    public class SendinBlueManagerv2 : BaseManager, IMailChannel
    {
        #region properties
        public IList<AudiencePropertieSetting> settings { set; get; }
        public SendinBlueManager manager { set; get; }

        public bool AreSettingsValid {set; get;}

        #endregion

        #region public methods

        public IMailChannel CreateChannel(IList<AudiencePropertieSetting> settings, string emailProductId)
        {
            return new SendinBlueManagerv2(settings, emailProductId);
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
                var request = manager.getRequestData(campaign);
                apiCampaignId = manager.SendCampaign(request);

                // update campaign
                if(apiCampaignId!=string.Empty)
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
                var request = manager.getRequestData(campaign);
                apiCampaignId = manager.SendCampaign(request);

                // update campaign
                if (apiCampaignId != string.Empty)
                {
                    campaign.ApiCampaignId = apiCampaignId;
                    collectionName= CosmosCollections.Campaign.ToString();
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

        public bool ValidateApiToken(string ApiToken)
        {
            bool result = false;
            manager = new SendinBlueManager();
            manager.ApiKey = ApiToken;
            result = manager.ValidateApiKey(ApiToken);
            return result;
        }

        public bool SettingsAreValid()
        {
            bool result = false;
            if (settings.Count > 0)
            {
                result=manager.SettingsAreValid();
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

        #endregion

        #region constructors

        public SendinBlueManagerv2(IList<AudiencePropertieSetting> settings, string emailProductId)
        {           
            this.settings = settings;
            this.manager = new SendinBlueManager(settings);

            // command pattern
            AreSettingsValid = SettingsAreValid();
            if (AreSettingsValid)
            {
                Save(emailProductId);
            }
        }

        public SendinBlueManagerv2(string emailProductId)
        {
            LoadSettings(emailProductId);
            this.manager = new SendinBlueManager(settings);

            // command pattern
            AreSettingsValid = SettingsAreValid();
            if (AreSettingsValid)
            {
                Save(emailProductId);
            }
        }

        public SendinBlueManagerv2() { }

        #endregion
    }
}
