using KindAds.Business.Partners.Mail;
using KindAds.Comun.Enums;
using KindAds.Comun.Interfaces;
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

    public class IContactManagerv2 : BaseManager, IMailChannel
    {
        #region properties
        public IList<AudiencePropertieSetting> settings { set; get; }
        public bool AreSettingsValid { set; get; }
        #endregion

        #region public methods

        public IMailChannel CreateChannel(IList<AudiencePropertieSetting> settings, string emailProductId)
        {
            return new IContactManagerv2(settings, emailProductId);
        }

        public void LoadSettings(string emailProductId)
        {
            string collectionName = CosmosCollections.EmailProductSetting.ToString();
            string query = $"SELECT * FROM {collectionName} WHERE {collectionName}.ProductId='{emailProductId}'";
            this.settings = context.ExecuteQuery<AudiencePropertieSetting>(databaseName, collectionName, query);
        }

        public string Send(string emailProductId)
        {
            string apiCampaignId = string.Empty;
            if (settings.Count > 0)
            {
                //no obtenemos los datos de bd
                //todo
            }
            else
            {
                LoadSettings(emailProductId);
                //todo
            }
            return apiCampaignId;
        }

        public bool SettingsAreValid(string emailProductId)
        {
            bool result = false;

            if (settings.Count > 0)
            {
                //no obtenemos los datos de bd
                result = true;
            }
            else
            {
                LoadSettings(emailProductId);
                result = true;
            }
            return result;
        }

        public bool SettingsAreValid()
        {
            bool result = false;
            if (settings.Count > 0)
            {
                //todo
                result = true;
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
            //todo
            return result;
        }

        #endregion

        #region constructors

        public IContactManagerv2(IList<AudiencePropertieSetting> settings, string emailProductId)
        {
            this.settings = settings;

            // command pattern
            AreSettingsValid = SettingsAreValid();
            if (AreSettingsValid)
            {
                Save(emailProductId);
            }
        }

        public IContactManagerv2(string emailProductId)
        {
            LoadSettings(emailProductId);

            // command pattern
            AreSettingsValid = SettingsAreValid();
            if (AreSettingsValid)
            {
                Save(emailProductId);
            }
        }

        public IContactManagerv2() { }

        #endregion
    }
}
