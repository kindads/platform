using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Captivate.Common.Interfaces;
using Captivate.Business;
using Captivate.DataAccess;
using Captivate.Comun.Utils.Partners.Mail.GetResponse;
using Captivate.Comun.Models.Entities;

namespace Captivate.Negocio.Partners.Mail
{
    public class GetResponseManager
    {
        public ITrace telemetria { set; get; }
        public CampaignRepository CampaignRepository { set; get; }
        public ProductRepository ProductRepository { set; get; }
        private static string baseUrl = "https://api.getresponse.com/v3/";

        public GetResponseManager()
        {
            KindadsContext context = new KindadsContext();
            telemetria = new Trace();
            CampaignRepository = new CampaignRepository { Context = context };
            ProductRepository = new ProductRepository { Context = context };
        }

        public string ValidateCampaign(string idCampaign)
        {
            try
            {
                dynamic json = new ExpandoObject();
                dynamic fromField = new ExpandoObject();
                dynamic campaign = new ExpandoObject();
                dynamic content = new ExpandoObject();
                dynamic sendSettings = new ExpandoObject();
                dynamic selectedCampaigns = new List<string>();

                string apiKey = null;
                string idList = null;
                string idFromField = null;
                string subject = null;

                CampaignEntity _campaign = CampaignRepository.FindBy(c => c.IdCampaign == new Guid(idCampaign)).FirstOrDefault();
                ProductEntity _product = ProductRepository.FindBy(p => p.IdProduct == _campaign.PRODUCT_IdProduct).FirstOrDefault();

                if (_product.ProductSettingsEntitys != null && _product.ProductSettingsEntitys.Any())
                {
                    foreach (var item in _product.ProductSettingsEntitys)
                    {
                        apiKey = item.SettingName.Equals("getResponseApiToken") ? item.SettingValue : apiKey;
                        idList = item.SettingName.Equals("getResponseList") ? item.SettingValue : idList;
                        idFromField = item.SettingName.Equals("getResponseFromField") ? item.SettingValue : idFromField;
                    }
                }

                if (_campaign.CAMPAIGN_SETTINGS != null && _campaign.CAMPAIGN_SETTINGS.Any())
                {
                    foreach (var setting in _campaign.CAMPAIGN_SETTINGS)
                    {
                        subject = setting.SettingName.Equals("getResponseSubject") ? setting.SettingValue : subject;
                    }
                }

                json.name = _campaign.Name;
                json.type = "broadcast";
                json.subject = subject;

                fromField.fromFieldId = idFromField;
                json.fromField = fromField;

                campaign.campaignId = idList;
                json.campaign = campaign;

                content.html = _campaign.AdText;
                content.plain = null;
                json.content = content;

                json.replyTo = fromField;

                selectedCampaigns.Add(idList);
                sendSettings.timeTravel = "false";
                sendSettings.perfectTiming = "true";
                sendSettings.selectedCampaigns = selectedCampaigns;
                sendSettings.selectedSegments = new List<string>();
                sendSettings.selectedSuppressions = new List<string>();
                sendSettings.excludedCampaigns = new List<string>();
                sendSettings.excludedSegments = new List<string>();
                sendSettings.selectedContacts = new List<string>();

                json.sendSettings = sendSettings;
                var result = MethodPost(apiKey, "newsletters", (IDictionary<string, object>)json);
                dynamic data = JsonConvert.DeserializeObject(result.Data);
                return (string)data.newsletterId;
            }
            catch (Exception ex)
            {
                var messageException = telemetria.MakeMessageException(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return null;
        }

        public static ApiResult MethodGet(string apikey, string action)
        {
            string result = "";
            ApiResult apiResult = new ApiResult();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + action);
            request.Method = "GET";
            request.Headers.Add("X-Auth-Token", "api-key " + apikey);
            try
            {
                WebResponse webResponse = request.GetResponse();
                apiResult.Code = ((HttpWebResponse)webResponse).StatusCode;
                apiResult.Message = ((HttpWebResponse)webResponse).StatusDescription;
                using (Stream webStream = webResponse.GetResponseStream())
                {
                    if (webStream != null)
                    {
                        using (StreamReader responseReader = new StreamReader(webStream))
                        {
                            result = responseReader.ReadToEnd();
                            apiResult.Data = result;
                        }
                    }
                }
            }
            catch (WebException wex)
            {
                result = wex.Message;
            }
            return apiResult;
        }

        public static ApiResult MethodPost(string apikey, string action, IDictionary<string, object> parameters)
        {
            ApiResult apiResult = new ApiResult();

            var serialized = JsonConvert.SerializeObject(parameters);

            WebRequest request = WebRequest.Create(baseUrl + action);
            request.Method = "POST";
            request.Headers.Add("X-Auth-Token", "api-key " + apikey);
            byte[] byteArray = Encoding.UTF8.GetBytes(serialized);
            request.ContentType = "application/json";

            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            WebResponse response = request.GetResponse();
            apiResult.Code = ((HttpWebResponse)response).StatusCode;
            apiResult.Message = ((HttpWebResponse)response).StatusDescription;
            dataStream = response.GetResponseStream();

            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            apiResult.Data = responseFromServer;

            reader.Close();
            dataStream.Close();
            response.Close();

            return apiResult;
        }
    }
}
