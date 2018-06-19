using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Captivate.Business;
using Captivate.Common.Interfaces;
using Captivate.Comun.Models.Entities;
using Captivate.DataAccess;
using Newtonsoft.Json;

namespace Captivate.Negocio.Partners.Push
{
    public class OneSignalManager
    {
        public ITrace telemetria { set; get; }
        public CampaignRepository CampaignRepository { set; get; }
        public ProductRepository ProductRepository { set; get; }
        private static readonly string api_url = "https://onesignal.com/api/v1/";
        public OneSignalManager()
        {
            
            telemetria = new Trace();
            CampaignRepository = new CampaignRepository();
            ProductRepository = new ProductRepository();
        }

        public string ValidateCampaign(string idCampaign)
        {
            string idCampaignISignal = null;

            try
            {
                var campaign = CampaignRepository.FindById(new Guid(idCampaign));

                var request = WebRequest.Create(api_url+ "notifications") as HttpWebRequest;

                request.KeepAlive = true;
                request.Method = "POST";
                request.ContentType = "application/json; charset=utf-8";

                request.Headers.Add("authorization", "Basic " + GetProductSetting(campaign.PRODUCT.ProductSettingsEntitys, "oneSignalAppKey"));

                byte[] byteArray = Encoding.UTF8.GetBytes("{"
                                                        + "\"app_id\": \"" + GetProductSetting(campaign.PRODUCT.ProductSettingsEntitys, "oneSignalAppId") + "\","
                                                        + "\"contents\": {\"en\": \"" + campaign.AdText + "\"},"
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

                idCampaignISignal = responseContent.id;
                if(idCampaignISignal == null)
                {
                    telemetria.Critical(responseContent.errors[0]);
                }

            }
            catch (Exception ex)
            {
                var messageException = telemetria.MakeMessageException(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }


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
