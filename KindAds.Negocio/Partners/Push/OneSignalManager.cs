using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using KindAds.Azure;
using KindAds.Business;
using KindAds.Common.Interfaces;
using KindAds.Common.Models.Entities;
using KindAds.Comun.Models;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using KindAds.DataAccess;
using Newtonsoft.Json;

namespace KindAds.Business.Partners.Push
{
    public class OneSignalManager
    {
        public ITrace telemetria { set; get; }
        //public CampaignRepository CampaignRepository { set; get; }
        //public ProductRepository ProductRepository { set; get; }
        private static readonly string api_url = "https://onesignal.com/api/v1/";

        public IList<AudiencePropertieSetting> settings { set; get; }

        public OneSignalManager()
        {
            
            telemetria = new Trace();
            //CampaignRepository = new CampaignRepository();
            //ProductRepository = new ProductRepository();
        }

        public OneSignalManager(IList<AudiencePropertieSetting> settings)
        {
            this.settings = settings;
            telemetria = new Trace();
        }

        public bool SettingsAreValid(string idCampaign)
        {
            bool result = false;
            result = (ApiTokenIsValid(idCampaign) && AppIsValid(idCampaign)) ? true : false;
            return result;
        }

        public bool SettingsAreValid()
        {
            bool result = false;
            result = (ApiTokenIsValid() && AppIsValid()) ? true : false;
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
                            value = item.Name.Equals("oneSignalApiKey") ? item.Value : value;
                        }
                    }
                    break;
                case "AppKey":
                    {
                        foreach (var item in settings)
                        {
                            value = item.Name.Equals("oneSignalAppKey") ? item.Value : value;
                        }
                    }
                    break;
                case "AppId":
                    {
                        foreach (var item in settings)
                        {
                            value = item.Name.Equals("oneSignalAppId") ? item.Value : value;
                        }
                    }
                    break;
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

            //switch (setting)
            //{
            //    case "ApiKey":
            //        {
            //            foreach (var item in settings)
            //            {
            //                value = item.SettingName.Equals("oneSignalApiKey") ? item.SettingValue : value;
            //            }
            //        }
            //        break;
            //    case "AppKey":
            //        {
            //            foreach (var item in settings)
            //            {
            //                value = item.SettingName.Equals("oneSignalAppKey") ? item.SettingValue : value;
            //            }
            //        }
            //        break;
            //    case "AppId":
            //        {
            //            foreach (var item in settings)
            //            {
            //                value = item.SettingName.Equals("oneSignalAppId") ? item.SettingValue : value;
            //            }
            //        }
            //        break;
            //}

            return value;
        }

        #region nuevas validaciones
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

        private bool AppIsValid()
        {
            bool result = false;
            string apiKey = string.Empty;
            string app = string.Empty;

            try
            {
                apiKey = GetSettingFromCosmos("ApiKey");
                app = GetSettingFromCosmos("AppId");

                List<OneSingalAppResult> apps = JsonConvert.DeserializeObject<List<OneSingalAppResult>>(MethodGet("apps", apiKey));
                foreach (var item in apps)
                {
                    if (item.id == app)
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

        public OneSignalCampaignRequest FillSendinBlueRequestFromCosmos()
        {
            OneSignalCampaignRequest request = new OneSignalCampaignRequest();
            // todo
            try
            {
                if (this.settings.Count > 0)
                {
                    foreach (var item in settings)
                    {

                        switch (item.Name)
                        {
                            case "oneSignalAppKey":
                                {
                                    request.AppKey = item.Name.Equals("oneSignalAppKey") ? item.Value : string.Empty;
                                }
                                break;
                            case "oneSignalAppId":
                                {
                                    request.AppId = item.Name.Equals("oneSignalAppId") ? item.Value : string.Empty;
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

        public OneSignalCampaignRequest getRequestData(CampaignDocument campaign)
        {
            OneSignalCampaignRequest request = FillSendinBlueRequestFromCosmos();
            request.Text = campaign.Text;
            request.Name = campaign.Name;
            return request;
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

        private bool AppIsValid(string idCampaign)
        {
            bool result = false;
            string apiKey = string.Empty;
            string app = string.Empty;

            try
            {
                apiKey = GetSetting("ApiKey", idCampaign);
                app = GetSetting("AppId", idCampaign);

                List<OneSingalAppResult> apps = JsonConvert.DeserializeObject<List<OneSingalAppResult>>(MethodGet("apps", apiKey));
                foreach(var item in apps)
                {
                    if(item.id==app)
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

        public string SendCampaign(OneSignalCampaignRequest requestData)
        {
            string Id = string.Empty;
            try
            {
                var request = WebRequest.Create(api_url + "notifications") as HttpWebRequest;

                request.KeepAlive = true;
                request.Method = "POST";
                request.ContentType = "application/json; charset=utf-8";

                request.Headers.Add("authorization", "Basic " + requestData.AppKey);

                byte[] byteArray = Encoding.UTF8.GetBytes("{"
                                                        + "\"app_id\": \"" + requestData.AppId + "\","
                                                        + "\"contents\": {\"en\": \"" + requestData.Text + "\"},"
                                                        + "\"included_segments\": [\"All\"]}");

                dynamic responseContent = null;

                using (var writer = request.GetRequestStream())
                {
                    writer.Write(byteArray, 0, byteArray.Length);
                }

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseContent = JsonConvert.DeserializeObject(reader.ReadToEnd());
                    }
                }

                Id = responseContent.id;
            }
            catch (Exception ex)
            {
                var messageException = telemetria.MakeMessageException(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return Id;
        }

        [Obsolete]
        public string SendCampaign(string idCampaign)
        {
            string idCampaignISignal = null;

            //try
            //{
            //    var campaign = CampaignRepository.FindById(new Guid(idCampaign));

            //    var request = WebRequest.Create(api_url+ "notifications") as HttpWebRequest;

            //    request.KeepAlive = true;
            //    request.Method = "POST";
            //    request.ContentType = "application/json; charset=utf-8";

            //    request.Headers.Add("authorization", "Basic " + GetProductSetting(campaign.PRODUCT.ProductSettingsEntitys, "oneSignalAppKey"));

            //    byte[] byteArray = Encoding.UTF8.GetBytes("{"
            //                                            + "\"app_id\": \"" + GetProductSetting(campaign.PRODUCT.ProductSettingsEntitys, "oneSignalAppId") + "\","
            //                                            + "\"contents\": {\"en\": \"" + campaign.AdText + "\"},"
            //                                            + "\"included_segments\": [\"All\"]}");

            //    dynamic responseContent = null;

            //    using (var writer = request.GetRequestStream())
            //    {
            //        writer.Write(byteArray, 0, byteArray.Length);
            //    }

            //    using (var response = request.GetResponse() as HttpWebResponse)
            //    {
            //        using (var reader = new StreamReader(response.GetResponseStream()))
            //        {
            //            responseContent = JsonConvert.DeserializeObject(reader.ReadToEnd());
            //        }
            //    }

            //    idCampaignISignal = responseContent.id;
            //    if(idCampaignISignal == null)
            //    {
            //        telemetria.Critical(responseContent.errors[0]);
            //    }

            //}
            //catch (Exception ex)
            //{
            //    var messageException = telemetria.MakeMessageException(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            //    telemetria.Critical(messageException);
            //}


            return idCampaignISignal;
        }

        private string MethodGet(string action, string key)
        {
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(api_url + action);
            request.Method = "GET";
            request.ContentType = "application/json; charset=utf-8";
            request.Headers.Add("Authorization", "Basic " + key);
            try
            {
                System.Net.HttpWebResponse webResponse = (System.Net.HttpWebResponse)request.GetResponse();
                if (webResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    using (System.IO.Stream webStream = webResponse.GetResponseStream())
                    {
                        if (webStream != null)
                        {
                            using (System.IO.StreamReader responseReader = new System.IO.StreamReader(webStream))
                            {
                                return responseReader.ReadToEnd();

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var messageException = telemetria.MakeMessageException(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }

            return null;
        }

        public List<OneSingalAppResult> GetApps(string authKey)
        {
            return JsonConvert.DeserializeObject<List<OneSingalAppResult>>(MethodGet("apps", authKey));
        }

        public OneSingalAppResult GetApp(string authKey, string appId)
        {
            return JsonConvert.DeserializeObject<OneSingalAppResult>(MethodGet("apps/" + appId, authKey));
        }

        public bool IsValid(string authKey)
        {
            return (MethodGet("apps", authKey)) != null;
        }

        public class OneSingalAppResult
        {
            public string id { get; set; }
            public string basic_auth_key { get; set; }
            public string name { get; set; }
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
