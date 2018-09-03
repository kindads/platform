using KindAds.Azure;
using KindAds.Business;
using KindAds.Common.Interfaces;
using KindAds.Common.Models.Entities;
using KindAds.Comun.Models;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using KindAds.DataAccess;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Business.Partners.Push
{
    public class PushEngageManger
    {
        public ITrace telemetria { set; get; }
        public CampaignRepository CampaignRepository { set; get; }
        public ProductRepository ProductRepository { set; get; }
        private static readonly string api_url = "https://api.pushengage.com/apiv1/notifications";
        public IList<AudiencePropertieSetting> settings { set; get; }

        public IList<CampaignSettingDocument> campaignSettings { set; get; }

        public PushEngageManger()
        {            
            telemetria = new Trace();
            CampaignRepository = new CampaignRepository ();
            ProductRepository = new ProductRepository ();
        }

        public PushEngageManger(IList<AudiencePropertieSetting> settings)
        {
            this.settings = settings;
            telemetria = new Trace();
        }

        public bool IsValid(string apiKey)
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    wc.Headers.Add("api_key", apiKey);
                    byte[] bret = wc.DownloadData(api_url + "?status=sent");
                    string HtmlResult = System.Text.Encoding.UTF8.GetString(bret);

                    if (HtmlResult.Length > 0)
                    {
                        dynamic dyn = JsonConvert.DeserializeObject(HtmlResult);
                        return dyn.success;
                    }
                }
            }
            catch (Exception ex)
            {
                var messageException = telemetria.MakeMessageException(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }

            return false;
        }

        public bool SettingsAreValid(string idCampaign)
        {
            bool result = false;
            result = (ApiTokenIsValid(idCampaign)) ? true : false;
            return result;
        }

        public bool SettingsAreValid()
        {
            bool result = false;
            result = (ApiTokenIsValid()) ? true : false;
            return result;
        }

        public string GetSetting(string setting,string idCampaign)
        {
            string value = string.Empty;
            ProductSettingsRepository repository = new ProductSettingsRepository();
            CampaignEntity campaign = CampaignRepository.FindById(new Guid(idCampaign));
            ProductEntity product = ProductRepository.FindById(campaign.PRODUCT_IdProduct);
            List<ProductSettingsEntity> settings = repository.GetProductSettingsByIdProduct(product.IdProduct);

            switch(setting)
            {
                case "ApiKey":
                    {
                        foreach (var item in settings)
                        {
                            value = item.SettingName.Equals("pushApiToken") ? item.SettingValue : value;
                        }
                    }
                    break;
            }

            return value;
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
                            value = item.Name.Equals("pushApiToken") ? item.Value : value;
                        }
                    }
                    break;
            }

            return value;
        }

        private bool ApiTokenIsValid()
        {
            bool result = false;
            string apiKey = string.Empty;

            try
            {
                apiKey = GetSettingFromCosmos("ApiKey");
                result = IsValid(apiKey);
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return result;
        }

        private bool ApiTokenIsValid(string idCampaign)
        {
            bool result = false;
            string apiKey = string.Empty;

            try
            {
                apiKey = GetSetting("ApiKey", idCampaign);
                result = IsValid(apiKey);
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return result;
        }

        public PushEngageCampaignRequest FillSendinBlueRequestFromCosmos()
        {
            PushEngageCampaignRequest request = new PushEngageCampaignRequest();
            //todo
            try
            {
                if (this.settings.Count > 0)
                {
                    foreach (var item in settings)
                    {

                        switch (item.Name)
                        {
                            case "pushApiToken":
                                {
                                    request.ApiToken = item.Name.Equals("pushApiToken") ? item.Value : string.Empty;
                                }
                                break;
                        }
                    }
                }

                if (this.campaignSettings.Count > 0)
                {

                    foreach (var item in settings)
                    {

                        switch (item.Name)
                        {
                            case "pushNotifUrl":
                                {
                                    request.Url = item.Name.Equals("pushNotifUrl") ? item.Value : string.Empty;
                                }
                                break;
                            case "pushNotifImage":
                                {
                                    request.Image = item.Name.Equals("pushNotifImage") ? item.Value : string.Empty;
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

        public PushEngageCampaignRequest getRequestData(CampaignDocument campaign)
        {
            PushEngageCampaignRequest request = FillSendinBlueRequestFromCosmos();
            request.Text = campaign.Text;
            request.Name = campaign.Name;
            return request;
        }

        public string SendCampaign(PushEngageCampaignRequest requestData)
        {
            string Id = string.Empty;
            try
            {
                EngageMessage.Title = requestData.Name;
                EngageMessage.Message = requestData.Text;
                EngageMessage.Url = requestData.Url;
                EngageMessage.Image_url = requestData.Image;
                EngageMessage.Key = requestData.ApiToken;

                string newMessage = "notification_title=" + EngageMessage.Title + "&";
                newMessage += "notification_message=" + EngageMessage.Message + "&";
                newMessage += "notification_url=" + EngageMessage.Url + "&";
                newMessage += "image_url=" + EngageMessage.Image_url + "";

                using (WebClient wc = new WebClient())
                {
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    wc.Headers.Add("api_key", EngageMessage.Key);
                    byte[] bret = wc.UploadData(api_url, "POST", System.Text.Encoding.UTF8.GetBytes(newMessage));
                    string HtmlResult = Encoding.UTF8.GetString(bret);

                    if (HtmlResult.Length > 0)
                    {
                        dynamic dyn = JsonConvert.DeserializeObject(HtmlResult);
                        Id = dyn.notification_id;
                    }
                }
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return Id;
        }


        public string SendCampaign(string idCampaign)
        {
            string idCampaignEngage = null;

            var _campaign = CampaignRepository.FindById(new Guid(idCampaign));

            try
            {
                if (_campaign != null)
                {
                    EngageMessage.Title = _campaign.Name;
                    EngageMessage.Message = _campaign.AdText;
                    EngageMessage.Url = GetCampaignSetting(_campaign.CAMPAIGN_SETTINGS, "pushNotifUrl");
                    EngageMessage.Image_url = GetCampaignSetting(_campaign.CAMPAIGN_SETTINGS, "pushNotifImage");
                    EngageMessage.Key = GetProductSetting(_campaign.PRODUCT.ProductSettingsEntitys, "pushApiToken");

                    string newMessage = "notification_title=" + EngageMessage.Title + "&";
                    newMessage += "notification_message=" + EngageMessage.Message + "&";
                    newMessage += "notification_url=" + EngageMessage.Url + "&";
                    newMessage += "image_url=" + EngageMessage.Image_url + "";

                    using (WebClient wc = new WebClient())
                    {
                        wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                        wc.Headers.Add("api_key", EngageMessage.Key);
                        byte[] bret = wc.UploadData(api_url, "POST", System.Text.Encoding.UTF8.GetBytes(newMessage));
                        string HtmlResult = Encoding.UTF8.GetString(bret);

                        if (HtmlResult.Length > 0)
                        {
                            dynamic dyn = JsonConvert.DeserializeObject(HtmlResult);
                            idCampaignEngage = dyn.notification_id;

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                var messageException = telemetria.MakeMessageException(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }

            return idCampaignEngage;
        }

        private class EngageMessage
        {
            internal static string Title;
            internal static string Message;
            internal static string Url;
            internal static string Image_url;
            internal static string Key;
        }

        private string GetCampaignSetting(ICollection<CampaignSettingsEntity> settings, string key)
        {
            return (from r in settings where r.SettingName.Equals(key) select r).FirstOrDefault().SettingValue;
        }

        private string GetProductSetting(ICollection<ProductSettingsEntity> settings, string key)
        {
            return (from r in settings where r.SettingName.Equals(key) select r).FirstOrDefault().SettingValue;
        }
    }
}
