using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KindAds.Common.Interfaces;
using KindAds.Business;
using KindAds.DataAccess;
using SendGrid;
using Newtonsoft.Json;
using System.Net;
using KindAds.Common.Models.Entities;
using KindAds.Azure;
using Newtonsoft.Json.Linq;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using KindAds.Comun.Models;

namespace KindAds.Business.Partners.Mail
{
    public class SendGridManager
    {
        public ITrace telemetria { set; get; }
        //public CampaignRepository CampaignRepository { set; get; }
        //public ProductRepository ProductRepository { set; get; }

        private static string apiHost = "https://api.sendgrid.com/";

        public IList<AudiencePropertieSetting> settings { set; get; }

        public string ApiToken { set; get; }

        public SendGridManager()
        {
            telemetria = new Trace();
            //CampaignRepository = new CampaignRepository ();
            //ProductRepository = new ProductRepository ();
        }

        public SendGridManager(IList<AudiencePropertieSetting> settings)
        {
            this.settings = settings;
            telemetria = new Trace();
        }

        public bool SettingsAreValid(string idCampaign)
        {
            bool result = false;
            result = ( ApiTokenIsValid(idCampaign) && UnsubscriberGroupIsValid(idCampaign) &&
                       SenderIsValid(idCampaign)   && ListIsValid(idCampaign)) ? true : false;
            return result;
        }

        public bool SettingsAreValid()
        {
            bool result = false;
            result = (ApiTokenIsValid() && UnsubscriberGroupIsValid() &&
                       SenderIsValid() && ListIsValid()) ? true : false;
            return result;
        }

        [Obsolete]
        public string GetSetting(string setting,string idCampaign)
        {
            //ProductSettingsRepository repository = new ProductSettingsRepository();
            //CampaignEntity campaign = CampaignRepository.FindById(new Guid(idCampaign));
            //ProductEntity product = ProductRepository.FindById(campaign.PRODUCT_IdProduct);
            //List<ProductSettingsEntity> settings = repository.GetProductSettingsByIdProduct(product.IdProduct);

            //string value = string.Empty;
            //switch(setting)
            //{
            //    case "ApiKey":
            //        {
            //            foreach (var item in settings)
            //            {
            //                value = item.SettingName.Equals("sendGridApiToken") ? item.SettingValue : value;
            //            }
            //        }
            //        break;
            //    case "List":
            //        {
            //            foreach (var item in settings)
            //            {
            //                value = item.SettingName.Equals("sendGridList") ? item.SettingValue : value;
            //            }
            //        }
            //        break;
            //    case "Sender":
            //        {
            //            foreach (var item in settings)
            //            {
            //                value = item.SettingName.Equals("sendGridSender") ? item.SettingValue : value;
            //            }
            //        }
            //        break;
            //    case "Unsubscribe":
            //        {
            //            foreach (var item in settings)
            //            {
            //                value = item.SettingName.Equals("sendGridUnsubscribeGroup") ? item.SettingValue : value;
            //            }
            //        }
            //        break;
            //}
            //return value;
            return null;
        }

        public string GetSettingFromCosmos(string setting )
        {
            string value = string.Empty;
            switch (setting)
            {
                case "ApiKey":
                    {
                        foreach (var item in this.settings)
                        {
                            value = item.Name.Equals("sendGridApiToken") ? item.Value : value;
                        }
                    }
                    break;
                case "List":
                    {
                        foreach (var item in this.settings)
                        {
                            value = item.Name.Equals("sendGridList") ? item.Value : value;
                        }
                    }
                    break;
                case "Sender":
                    {
                        foreach (var item in this.settings)
                        {
                            value = item.Name.Equals("sendGridSender") ? item.Value : value;
                        }
                    }
                    break;
                case "Unsubscribe":
                    {
                        foreach (var item in this.settings)
                        {
                            value = item.Name.Equals("sendGridUnsubscribeGroup") ? item.Value : value;
                        }
                    }
                    break;
            }
            return value;
        }


        #region CosmosValidation

        public bool ApiTokenIsValid()
        {
            bool result = false;
            string apiKey = string.Empty;

            try
            {
                apiKey = GetSettingFromCosmos("ApiKey");

                var headers = new Dictionary<string, string> { { "X-Mock", "200" } };
                var sg = new SendGridClient(apiKey, apiHost, headers);
                var response = sg.RequestAsync(method: SendGridClient.Method.GET, urlPath: "contactdb/lists").GetAwaiter().GetResult();
                result = response.StatusCode.Equals(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return result;
        }

        public bool TokenIsValid(string apiKey)
        {
            bool result = false;

            try
            {
                var headers = new Dictionary<string, string> { { "X-Mock", "200" } };
                var sg = new SendGridClient(apiKey, apiHost, headers);
                var response = sg.RequestAsync(method: SendGridClient.Method.GET, urlPath: "contactdb/lists").GetAwaiter().GetResult();
                result = response.StatusCode.Equals(HttpStatusCode.OK);
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
            string list = string.Empty;
            string apiKey = string.Empty;

            try
            {
                apiKey = GetSettingFromCosmos("ApiKey");
                list = GetSettingFromCosmos("List");

                var headers = new Dictionary<string, string> { { "X-Mock", "200" } };
                var sg = new SendGridClient(apiKey, apiHost, headers);
                var response = sg.RequestAsync(method: SendGridClient.Method.GET, urlPath: "contactdb/lists").GetAwaiter().GetResult();
                var deserializeBody = response.DeserializeResponseBody(response.Body);
                foreach (var item in deserializeBody)
                {
                    var arrayJson = (JArray)JsonConvert.DeserializeObject(Convert.ToString(item.Value));
                    foreach (var element in arrayJson.Children())
                    {
                        var itemProperties = element.Children<JProperty>();
                        var id = itemProperties.FirstOrDefault(x => x.Name == "id");
                        var name = itemProperties.FirstOrDefault(x => x.Name == "name");
                        if (id.Value.ToString() == list)
                        {
                            result = true;
                        }
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

        private bool SenderIsValid()
        {
            bool result = false;
            string sender = string.Empty;
            string apiKey = string.Empty;

            try
            {
                apiKey = GetSettingFromCosmos("ApiKey");
                sender = GetSettingFromCosmos("Sender");

                var headers = new Dictionary<string, string> { { "X-Mock", "200" } };
                var sg = new SendGridClient(apiKey, apiHost, headers);
                var response = sg.RequestAsync(method: SendGridClient.Method.GET, urlPath: "senders").GetAwaiter().GetResult();
                var deserializeBody = response.Body.ReadAsStringAsync().GetAwaiter().GetResult();

                var arrayJson = (JArray)JsonConvert.DeserializeObject(Convert.ToString(deserializeBody));
                foreach (var element in arrayJson.Children())
                {
                    var itemProperties = element.Children<JProperty>();
                    var id = itemProperties.FirstOrDefault(x => x.Name == "id");
                    var nickname = itemProperties.FirstOrDefault(x => x.Name == "nickname");
                    var verified = itemProperties.FirstOrDefault(x => x.Name == "verified");
                    JObject verifiedObject = JObject.Parse(verified.Value.ToString());
                    if (id.Value.ToString() == sender)
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

        private bool UnsubscriberGroupIsValid()
        {
            bool result = false;
            string unsubscriber = string.Empty;
            string apiKey = string.Empty;

            try
            {
                apiKey = GetSettingFromCosmos("ApiKey");
                unsubscriber = GetSettingFromCosmos("Unsubscribe");

                var headers = new Dictionary<string, string> { { "X-Mock", "200" } };
                var sg = new SendGridClient(apiKey, apiHost, headers);
                var queryParams = @"{}";
                var response = sg.RequestAsync(method: SendGridClient.Method.GET, urlPath: "asm/groups", queryParams: queryParams).GetAwaiter().GetResult();
                var deserializeBody = response.Body.ReadAsStringAsync().GetAwaiter().GetResult();

                var arrayJson = (JArray)JsonConvert.DeserializeObject(Convert.ToString(deserializeBody));
                foreach (var element in arrayJson.Children())
                {
                    var itemProperties = element.Children<JProperty>();
                    var id = itemProperties.FirstOrDefault(x => x.Name == "id");
                    var name = itemProperties.FirstOrDefault(x => x.Name == "name");
                    if (id.Value.ToString() == unsubscriber)
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

        private bool ApiTokenIsValid(string idCampaign)
        {
            bool result = false;
            string apiKey = string.Empty;

            try
            {
                apiKey = GetSetting("ApiKey", idCampaign);

                var headers = new Dictionary<string, string> { { "X-Mock", "200" } };
                var sg = new SendGridClient(apiKey, apiHost, headers);
                var response = sg.RequestAsync(method: SendGridClient.Method.GET, urlPath: "contactdb/lists").GetAwaiter().GetResult();
                result= response.StatusCode.Equals(HttpStatusCode.OK);
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
            string list = string.Empty;
            string apiKey = string.Empty;

            try
            {
                apiKey = GetSetting("ApiKey", idCampaign);
                list =GetSetting("List", idCampaign);

                var headers = new Dictionary<string, string> { { "X-Mock", "200" } };
                var sg = new SendGridClient(apiKey, apiHost, headers);
                var response = sg.RequestAsync(method: SendGridClient.Method.GET, urlPath: "contactdb/lists").GetAwaiter().GetResult();
                var deserializeBody = response.DeserializeResponseBody(response.Body);
                foreach (var item in deserializeBody)
                {
                    var arrayJson = (JArray)JsonConvert.DeserializeObject(Convert.ToString(item.Value));
                    foreach (var element in arrayJson.Children())
                    {
                        var itemProperties = element.Children<JProperty>();
                        var id = itemProperties.FirstOrDefault(x => x.Name == "id");
                        var name = itemProperties.FirstOrDefault(x => x.Name == "name");
                        if (id.Value.ToString() == list)
                        {
                            result = true;
                        }
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

        private bool SenderIsValid(string idCampaign)
        {
            bool result = false;
            string sender = string.Empty;
            string apiKey = string.Empty;

            try
            {
                apiKey = GetSetting("ApiKey", idCampaign);
                sender = GetSetting("Sender", idCampaign);

                var headers = new Dictionary<string, string> { { "X-Mock", "200" } };
                var sg = new SendGridClient(apiKey, apiHost, headers);
                var response = sg.RequestAsync(method: SendGridClient.Method.GET, urlPath: "senders").GetAwaiter().GetResult();
                var deserializeBody = response.Body.ReadAsStringAsync().GetAwaiter().GetResult();

                var arrayJson = (JArray)JsonConvert.DeserializeObject(Convert.ToString(deserializeBody));
                foreach (var element in arrayJson.Children())
                {
                    var itemProperties = element.Children<JProperty>();
                    var id = itemProperties.FirstOrDefault(x => x.Name == "id");
                    var nickname = itemProperties.FirstOrDefault(x => x.Name == "nickname");
                    var verified = itemProperties.FirstOrDefault(x => x.Name == "verified");
                    JObject verifiedObject = JObject.Parse(verified.Value.ToString());
                    if(id.Value.ToString()==sender)
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

        private bool UnsubscriberGroupIsValid(string idCampaign)
        {
            bool result = false;
            string unsubscriber = string.Empty;
            string apiKey = string.Empty;

            try
            {
                apiKey = GetSetting("ApiKey", idCampaign);
                unsubscriber = GetSetting("Unsubscribe", idCampaign);

                var headers = new Dictionary<string, string> { { "X-Mock", "200" } };
                var sg = new SendGridClient(apiKey, apiHost, headers);
                var queryParams = @"{}";
                var response = sg.RequestAsync(method: SendGridClient.Method.GET, urlPath: "asm/groups", queryParams: queryParams).GetAwaiter().GetResult();
                var deserializeBody = response.Body.ReadAsStringAsync().GetAwaiter().GetResult();

                var arrayJson = (JArray)JsonConvert.DeserializeObject(Convert.ToString(deserializeBody));
                foreach (var element in arrayJson.Children())
                {
                    var itemProperties = element.Children<JProperty>();
                    var id = itemProperties.FirstOrDefault(x => x.Name == "id");
                    var name = itemProperties.FirstOrDefault(x => x.Name == "name");
                    if(id.Value.ToString()==unsubscriber)
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

        // nueva funcionalidad
        public SendGridCampaignRequest FillSendinBlueRequestFromCosmos(List<CampaignSettingDocument> campaignSettings)
        {
            SendGridCampaignRequest request = new SendGridCampaignRequest();

            // get data from product settings
            if (this.settings != null && this.settings.Any())
            {
                foreach (var item in this.settings)
                {
                    request.ApiKey = item.Name.Equals("sendGridApiToken") ? item.Value : request.ApiKey;
                    request.ListId = item.Name.Equals("sendGridList") ? item.Value : request.ListId;
                    request.SenderId = item.Name.Equals("sendGridSender") ? item.Value : request.SenderId;
                    request.UnsubscriberGroupId = item.Name.Equals("sendGridUnsubscribeGroup") ? item.Value : request.UnsubscriberGroupId;
                }
            }

            // get data from campaign settings
            if (campaignSettings != null && campaignSettings.Any())
            {
                foreach (var setting in campaignSettings)
                {
                    request.Subject = setting.Name.Equals("sendGridSubject") ? setting.Value : request.Subject;
                }
            }

            return request;
        }

        // nueva funcionalidad
        public SendGridCampaignRequest getRequestData(CampaignDocument campaign, List<CampaignSettingDocument> campaignSettings)
        {
            SendGridCampaignRequest request = FillSendinBlueRequestFromCosmos(campaignSettings);
            request.Text = campaign.Text;
            request.Name = campaign.Name;
            return request;
        }

        // nueva funcionalidad
        public string SendCampaign(SendGridCampaignRequest request)
        {
            string Id = string.Empty;
            try
            {
                var headers = new Dictionary<string, string> { { "X-Mock", "201" } };
                var sg = new SendGridClient(request.ApiKey, apiHost, headers);
                JsonCampaign jsonCampaign = new JsonCampaign()
                {
                    categories = new List<string>(),
                    custom_unsubscribe_url = "",
                    html_content = "<html><head><title></title></head><body>" + request.Text + "[unsubscribe]</body></html>",
                    list_ids = new List<int>() { Convert.ToInt32(request.ListId) },
                    plain_content = "",
                    segment_ids = new List<int>(),
                    sender_id = Convert.ToInt32(request.SenderId),
                    subject = request.Subject,
                    suppression_group_id = Convert.ToInt32(request.UnsubscriberGroupId),
                    title = request.Name
                };
                var data = JsonConvert.SerializeObject(jsonCampaign);
                var response = sg.RequestAsync(method: SendGridClient.Method.POST, urlPath: "campaigns", requestBody: data).GetAwaiter().GetResult();
                var deserializeBody = response.DeserializeResponseBody(response.Body);

                if (response.StatusCode.Equals(HttpStatusCode.Created))
                {
                    Id = Convert.ToString(deserializeBody["id"]);
                    SendCampaign(request.ApiKey, Id);
                }
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
            //try
            //{
            //    string apiKey = null;
            //    string idList = null;
            //    string idSender = null;
            //    string idUnsubscriberGroup = null;
            //    string subject = null;
            //    string idCampaignSendGrid = null;

            //    CampaignEntity campaign = CampaignRepository.FindById(new Guid(idCampaign));
            //    ProductEntity product = ProductRepository.FindById(campaign.PRODUCT_IdProduct);

            //    if (product.ProductSettingsEntitys != null && product.ProductSettingsEntitys.Any())
            //    {
            //        foreach (var item in product.ProductSettingsEntitys)
            //        {
            //            apiKey = item.SettingName.Equals("sendGridApiToken") ? item.SettingValue : apiKey;
            //            idList = item.SettingName.Equals("sendGridList") ? item.SettingValue : idList;
            //            idSender = item.SettingName.Equals("sendGridSender") ? item.SettingValue : idSender;
            //            idUnsubscriberGroup = item.SettingName.Equals("sendGridUnsubscribeGroup") ? item.SettingValue : idUnsubscriberGroup;
            //        }
            //    }

            //    if (campaign.CAMPAIGN_SETTINGS != null && campaign.CAMPAIGN_SETTINGS.Any())
            //    {
            //        foreach (var setting in campaign.CAMPAIGN_SETTINGS)
            //        {
            //            subject = setting.SettingName.Equals("sendGridSubject") ? setting.SettingValue : subject;
            //        }
            //    }

            //    var headers = new Dictionary<string, string> { { "X-Mock", "201" } };
            //    var sg = new SendGridClient(apiKey, apiHost, headers);
            //    JsonCampaign jsonCampaign = new JsonCampaign()
            //    {
            //        categories = new List<string>(),
            //        custom_unsubscribe_url = "",
            //        html_content = "<html><head><title></title></head><body>" + campaign.AdText + "[unsubscribe]</body></html>",
            //        list_ids = new List<int>() { Convert.ToInt32(idList) },
            //        plain_content = "",
            //        segment_ids = new List<int>(),
            //        sender_id = Convert.ToInt32(idSender),
            //        subject = subject,
            //        suppression_group_id = Convert.ToInt32(idUnsubscriberGroup),
            //        title = campaign.Name
            //    };
            //    var data = JsonConvert.SerializeObject(jsonCampaign);
            //    var response = sg.RequestAsync(method: SendGridClient.Method.POST, urlPath: "campaigns", requestBody: data).GetAwaiter().GetResult();
            //    var deserializeBody = response.DeserializeResponseBody(response.Body);

            //    if (response.StatusCode.Equals(HttpStatusCode.Created))
            //    {
            //        idCampaignSendGrid = Convert.ToString(deserializeBody["id"]);
            //        SendCampaign(apiKey, idCampaignSendGrid);
            //    }

            //    return idCampaignSendGrid;
            //}
            //catch(Exception ex)
            //{
            //    var messageException = telemetria.MakeMessageException(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            //    telemetria.Critical(messageException);
            //}
            return null;
        }

        private HttpStatusCode SendCampaign(string apiKey, string idCampaignSendGrid)
        {
            var headers = new Dictionary<string, string> { { "X-Mock", "200" } };
            var sg = new SendGridClient(apiKey, apiHost, headers);
            var response = sg.RequestAsync(method: SendGridClient.Method.POST, urlPath: "campaigns/" + idCampaignSendGrid + "/schedules/now").GetAwaiter().GetResult();
            var deserializeBody = response.DeserializeResponseBody(response.Body);
            return response.StatusCode;
        }

    }

    public class JsonCampaign
    {
        public List<string> categories { get; set; }
        public string custom_unsubscribe_url { get; set; }
        public string html_content { get; set; }
        public List<int> list_ids { get; set; }
        public string plain_content { get; set; }
        public List<int> segment_ids { get; set; }
        public int sender_id { get; set; }
        public string subject { get; set; }
        public string title { get; set; }
        public int suppression_group_id { get; set; }
    }

}
