using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using KindAds.Common.Interfaces;
using KindAds.Business;
using KindAds.DataAccess;
using KindAds.Common.Utils.Partners.Mail.GetResponse;
using KindAds.Common.Models.Entities;
using KindAds.Azure;
using KindAds.Comun.Models;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;

namespace KindAds.Business.Partners.Mail
{
    public class GetResponseManager
    {
        public ITrace telemetria { set; get; }
        //public CampaignRepository CampaignRepository { set; get; }
        //public ProductRepository ProductRepository { set; get; }
        private static string baseUrl = "https://api.getresponse.com/v3/";

        public IList<AudiencePropertieSetting> settings { set; get; }

        public GetResponseManager()
        {
            telemetria = new Trace();
            //CampaignRepository = new CampaignRepository();
            //ProductRepository = new ProductRepository();
        }

        public GetResponseManager(IList<AudiencePropertieSetting> settings)
        {
            this.settings = settings;
            telemetria = new Trace();
        }

        public bool SettingsAreValid(string idCampaign)
        {
            bool result = false;
            result = (ApiTokeIsValid(idCampaign) && FromFieldIsValid(idCampaign) && ListIsValid(idCampaign)) ? true : false;
            return result;
        }

        public bool SettingsAreValid()
        {
            bool result = false;
            result = (ApiTokeIsValid() && FromFieldIsValid() && ListIsValid()) ? true : false;
            return result;
        }

        #region settings validation methods

        public bool IsValid(string apiKey)
        {
            var result = MethodGet(apiKey, "accounts");
            return result.Code.Equals(HttpStatusCode.OK);
        }

        #region nuevas validaciones
        private bool ApiTokeIsValid()
        {
            bool result = false;
            string apiKey = string.Empty;

            try
            {
                apiKey = GetSettingFromCosmos("ApiToken");
                var response = MethodGet(apiKey, "accounts");
                result = response.Code.Equals(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return result;
        }

        private bool ListIsValid()
        {
            bool result = false;
            string idList = string.Empty;
            string apiKey = string.Empty;

            try
            {
                idList = GetSettingFromCosmos("List");
                apiKey = GetSettingFromCosmos("ApiToken");
                var response = MethodGet(apiKey, "campaigns");

                dynamic data = JsonConvert.DeserializeObject(response.Data);
                foreach (var item in data)
                {
                    if (item.campaignId == idList)
                    {
                        result = true;
                        break;
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

        private bool FromFieldIsValid()
        {
            bool result = false;
            string idFromField = string.Empty;
            string apiKey = string.Empty;

            try
            {
                idFromField = GetSettingFromCosmos("FromField");
                apiKey = GetSettingFromCosmos("ApiToken");

                var response = MethodGet(apiKey, "from-fields");
                dynamic data = JsonConvert.DeserializeObject(response.Data);

                foreach (var item in data)
                {
                    if (item.fromFieldId == idFromField)
                    {
                        result = true;
                        break;
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

        private bool ApiTokeIsValid(string idCampaign)
        {
            bool result = false;
            string apiKey = string.Empty;

            try
            {
                apiKey = GetSetting("ApiToken", idCampaign);
                var response = MethodGet(apiKey, "accounts");
                result = response.Code.Equals(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return result;
        }

        private bool ListIsValid(string idCampaign)
        {
            bool result = false;
            string idList = string.Empty;
            string apiKey = string.Empty;

            try
            {
                idList = GetSetting("List", idCampaign);
                apiKey = GetSetting("ApiToken", idCampaign);
                var response = MethodGet(apiKey, "campaigns");

                dynamic data = JsonConvert.DeserializeObject(response.Data);
                foreach (var item in data)
                {
                    if (item.campaignId == idList)
                    {
                        result = true;
                        break;
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

        private bool FromFieldIsValid(string idCampaign)
        {
            bool result = false;
            string idFromField = string.Empty;
            string apiKey = string.Empty;

            try
            {
                idFromField = GetSetting("FromField", idCampaign);
                apiKey = GetSetting("ApiToken", idCampaign);

                var response = MethodGet(apiKey, "from-fields");
                dynamic data = JsonConvert.DeserializeObject(response.Data);

                foreach (var item in data)
                {
                    if (item.fromFieldId == idFromField)
                    {
                        result = true;
                        break;
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

        public string GetSettingFromCosmos(string setting)
        {
            string value = string.Empty;           

            switch (setting)
            {
                case "ApiToken":
                    {
                        foreach (var item in settings)
                        {
                            value = item.Name.Equals("getResponseApiToken") ? item.Value : value;
                        }
                    }
                    break;
                case "List":
                    {
                        foreach (var item in settings)
                        {
                            value = item.Name.Equals("getResponseList") ? item.Value : value;
                        }
                    }
                    break;
                case "FromField":
                    {
                        foreach (var item in settings)
                        {
                            value = item.Name.Equals("getResponseFromField") ? item.Value : value;
                        }
                    }
                    break;
            }

            return value;
        }

        public string GetSetting(string setting, string idCampaign)
        {
            string value = string.Empty;
            //ProductSettingsRepository productSettingRepository = new ProductSettingsRepository();
            //CampaignEntity _campaign = CampaignRepository.FindById(new Guid(idCampaign));
            //ProductEntity _product = ProductRepository.FindById(_campaign.PRODUCT_IdProduct);
            //List<ProductSettingsEntity> settings = productSettingRepository.GetProductSettingsByIdProduct(_product.IdProduct);

            //switch (setting)
            //{
            //    case "ApiToken":
            //        {
            //            foreach (var item in settings)
            //            {
            //                value = item.SettingName.Equals("getResponseApiToken") ? item.SettingValue : value;
            //            }
            //        }
            //        break;
            //    case "List":
            //        {
            //            foreach (var item in settings)
            //            {
            //                value = item.SettingName.Equals("getResponseList") ? item.SettingValue : value;
            //            }
            //        }
            //        break;
            //    case "FromField":
            //        {
            //            foreach (var item in settings)
            //            {
            //                value = item.SettingName.Equals("getResponseFromField") ? item.SettingValue : value;
            //            }
            //        }
            //        break;
            //}

            return value;
        }

        public GetResponseCampaignRequest FillSendinBlueRequestFromCosmos(List<CampaignSettingDocument> campaignSettings)
        {
            GetResponseCampaignRequest request = new GetResponseCampaignRequest();
            try
            {
                // from products
                if (this.settings.Count > 0)
                {
                    foreach (var item in settings)
                    {

                        switch (item.Name)
                        {
                            case "getResponseApiToken":
                                {
                                    request.ApiKey = item.Name.Equals("getResponseApiToken") ? item.Value : string.Empty;
                                }
                                break;
                            case "getResponseList":
                                {
                                    request.ListId = item.Name.Equals("getResponseList") ? item.Value : string.Empty;
                                }
                                break;
                            case "getResponseFromField":
                                {
                                    request.FromFiledId = item.Name.Equals("getResponseFromField") ? item.Value : string.Empty;
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
                            case "getResponseSubject":
                                {
                                    request.Subject = setting.Name.Equals("getResponseSubject") ? setting.Value : string.Empty;
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

        public GetResponseCampaignRequest getRequestData(CampaignDocument campaign, List<CampaignSettingDocument> settings)
        {
            GetResponseCampaignRequest request = FillSendinBlueRequestFromCosmos(settings);
            request.Name = campaign.Name;
            request.Text = campaign.Text;
            return request;
        }

        public string SendCampaign(GetResponseCampaignRequest request)
        {
            string Id = string.Empty;
            try
            {
                //main objects
                dynamic json = new ExpandoObject();
                dynamic fromField = new ExpandoObject();
                dynamic campaign = new ExpandoObject();
                dynamic content = new ExpandoObject();
                dynamic sendSettings = new ExpandoObject();
                dynamic selectedCampaigns = new List<string>();

                //set values
                json.name = request.Name;
                json.type = "broadcast";
                json.subject = request.Subject;

                fromField.fromFieldId = request.FromFiledId;
                json.fromField = fromField;

                campaign.campaignId = request.ListId;
                json.campaign = campaign;

                content.html = request.Text;
                content.plain = null;
                json.content = content;

                json.replyTo = fromField;

                selectedCampaigns.Add(request.ListId);
                sendSettings.timeTravel = "false";
                sendSettings.perfectTiming = "true";
                sendSettings.selectedCampaigns = selectedCampaigns;
                sendSettings.selectedSegments = new List<string>();
                sendSettings.selectedSuppressions = new List<string>();
                sendSettings.excludedCampaigns = new List<string>();
                sendSettings.excludedSegments = new List<string>();
                sendSettings.selectedContacts = new List<string>();

                json.sendSettings = sendSettings;
                var result = MethodPost(request.ApiKey, "newsletters", (IDictionary<string, object>)json);
                dynamic data = JsonConvert.DeserializeObject(result.Data);
                Id=(string)data.newsletterId;
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
            //try
            //{
            //    dynamic json = new ExpandoObject();
            //    dynamic fromField = new ExpandoObject();
            //    dynamic campaign = new ExpandoObject();
            //    dynamic content = new ExpandoObject();
            //    dynamic sendSettings = new ExpandoObject();
            //    dynamic selectedCampaigns = new List<string>();

            //    string apiKey = null;
            //    string idList = null;
            //    string idFromField = null;
            //    string subject = null;

            //    CampaignEntity _campaign = CampaignRepository.FindById(new Guid(idCampaign));
            //    ProductEntity _product = ProductRepository.FindById(_campaign.PRODUCT_IdProduct);

            //    if (_product.ProductSettingsEntitys != null && _product.ProductSettingsEntitys.Any())
            //    {
            //        foreach (var item in _product.ProductSettingsEntitys)
            //        {
            //            apiKey = item.SettingName.Equals("getResponseApiToken") ? item.SettingValue : apiKey;
            //            idList = item.SettingName.Equals("getResponseList") ? item.SettingValue : idList;
            //            idFromField = item.SettingName.Equals("getResponseFromField") ? item.SettingValue : idFromField;
            //        }
            //    }

            //    if (_campaign.CAMPAIGN_SETTINGS != null && _campaign.CAMPAIGN_SETTINGS.Any())
            //    {
            //        foreach (var setting in _campaign.CAMPAIGN_SETTINGS)
            //        {
            //            subject = setting.SettingName.Equals("getResponseSubject") ? setting.SettingValue : subject;
            //        }
            //    }

            //    json.name = _campaign.Name;
            //    json.type = "broadcast";
            //    json.subject = subject;

            //    fromField.fromFieldId = idFromField;
            //    json.fromField = fromField;

            //    campaign.campaignId = idList;
            //    json.campaign = campaign;

            //    content.html = _campaign.AdText;
            //    content.plain = null;
            //    json.content = content;

            //    json.replyTo = fromField;

            //    selectedCampaigns.Add(idList);
            //    sendSettings.timeTravel = "false";
            //    sendSettings.perfectTiming = "true";
            //    sendSettings.selectedCampaigns = selectedCampaigns;
            //    sendSettings.selectedSegments = new List<string>();
            //    sendSettings.selectedSuppressions = new List<string>();
            //    sendSettings.excludedCampaigns = new List<string>();
            //    sendSettings.excludedSegments = new List<string>();
            //    sendSettings.selectedContacts = new List<string>();

            //    json.sendSettings = sendSettings;
            //    var result = MethodPost(apiKey, "newsletters", (IDictionary<string, object>)json);
            //    dynamic data = JsonConvert.DeserializeObject(result.Data);
            //    return (string)data.newsletterId;
            //}
            //catch (Exception ex)
            //{
            //    var messageException = telemetria.MakeMessageException(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            //    telemetria.Critical(messageException);
            //}
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
