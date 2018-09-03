using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KindAds.DataAccess;
using KindAds.Common.Models.Entities;
using KindAds.Common.Partners.Push;
using System.Net;
using System.IO;
using KindAds.Common.Interfaces;
using KindAds.Business;
using KindAds.Azure;
using KindAds.Comun.Models;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;

namespace KindAds.Business.Partners.Push
{
    public class SubscribersManager
    {
        public ITrace telemetria { set; get; }
        //public CampaignRepository CampaignRepository { set; get; }
        //public ProductRepository ProductRepository { set; get; }

        public IList<AudiencePropertieSetting> settings { set; get; }

        public IList<CampaignSettingDocument> campaignSettings { set; get; }

        public SubscribersManager()
        {            
            telemetria = new Trace();
            //CampaignRepository = new CampaignRepository ();
            //ProductRepository = new ProductRepository ();
        }

        public SubscribersManager(IList<AudiencePropertieSetting> settings)
        {
            this.settings = settings;
            telemetria = new Trace();
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
                case "Url":
                    {
                        foreach (var item in campaignSettings)
                        {
                            value = item.Name.Equals("pushNotifUrl") ? item.Value : value;
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
            //CampaignSettingsRepository campaignRepository = new CampaignSettingsRepository();

            //CampaignEntity campaign = CampaignRepository.FindById(new Guid(idCampaign));
            //ProductEntity product = ProductRepository.FindById(campaign.PRODUCT_IdProduct);
            //List<ProductSettingsEntity> settings = repository.GetProductSettingsByIdProduct(product.IdProduct);
            
            //switch (setting)
            //{
            //    case "ApiKey":
            //        {
            //            foreach (var item in settings)
            //            {
            //                value = item.SettingName.Equals("pushApiToken") ? item.SettingValue : value;
            //            }
            //        }
            //        break;
            //    case "Url":
            //        {
            //            List<CampaignSettingsEntity> campaignSettings = campaignRepository.GetCampaignSettingsByIdCampaign(campaign.IdCampaign);

            //            foreach (var item in campaignSettings)
            //            {
            //                value = item.SettingName.Equals("pushNotifUrl") ? item.SettingValue : value;
            //            }
            //        }
            //        break;
            //}

            return value;
        }

        public bool ValidateKey(string apikey, string urlsite)
        {
            if (apikey.Length > 0)
            {
                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("http://app.subscribers.com/api/v1/site");
                request.Method = "GET";
                request.Headers.Add("X-API-Key", apikey);

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
                                    var _apiresponse = responseReader.ReadToEnd();
                                    if (_apiresponse != null)
                                    {
                                        VerifyKeyResponse _response = new VerifyKeyResponse();
                                        _response = Newtonsoft.Json.JsonConvert.DeserializeObject<VerifyKeyResponse>(_apiresponse);
                                        if (_response != null)
                                        {
                                            string _authorizedsite = _response.url.Replace("http://www.", "").Replace("https://www.", "").Replace("http://", "").Replace("https://", "");
                                            urlsite = urlsite.Replace("http://www.", "").Replace("https://www.", "").Replace("http://", "").Replace("https://", "");
                                            if (urlsite == _authorizedsite)
                                            {
                                                return true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    telemetria.Critical(messageException);
                    return false;
                }
            }
            return false;
        }


        private bool ApiTokenIsValid()
        {
            bool result = false;
            string apiKey = string.Empty;
            string urlSite = string.Empty;

            try
            {
                apiKey = GetSettingFromCosmos("ApiKey");
                urlSite = GetSettingFromCosmos("Url");
                result = ValidateKey(apiKey, urlSite);
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
            string urlSite = string.Empty;

            try
            {
                apiKey = GetSetting("ApiKey", idCampaign);
                urlSite = GetSetting("Url", idCampaign);
                result=ValidateKey(apiKey, urlSite);
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return result;
        }

        public SubscribersCampaignRequest FillSendinBlueRequestFromCosmos()
        {
            SubscribersCampaignRequest request = new SubscribersCampaignRequest();
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
                                    request.ApiKey = item.Name.Equals("pushApiToken") ? item.Value : string.Empty;
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
                            case "pushImage":
                                {
                                    request.Image = item.Name.Equals("pushImage") ? item.Value : string.Empty;
                                }
                                break;
                            case "Utm":
                                {
                                    request.Utm = item.Name.Equals("Utm") ? item.Value : string.Empty;
                                }
                                break;
                            case "UtmMedium":
                                {
                                    request.UtmMedium = item.Name.Equals("UtmMedium") ? item.Value : string.Empty;
                                }
                                break;
                            case "UtmSource":
                                {
                                    request.UtmSource = item.Name.Equals("UtmSource") ? item.Value : string.Empty;
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

        public SubscribersCampaignRequest getRequestData(CampaignDocument campaign)
        {
            SubscribersCampaignRequest request = FillSendinBlueRequestFromCosmos();
            request.Text = campaign.Text;
            request.Name = campaign.Name;
            return request;
        }

        public string SendCampaign(SubscribersCampaignRequest requestData)
        {
            string Id = string.Empty;
            try
            {
                SubscribersModels.MessageRequest _message = new SubscribersModels.MessageRequest();

                _message.body = requestData.Text;
                _message.title = requestData.Name;
                _message.landing_page_url = requestData.Url;
                _message.image_url = requestData.Image;
                _message.utm = new SubscribersModels.UTM() { campaign = requestData.Utm, medium = requestData.UtmMedium , source = requestData.UtmSource };
                _message.metadata = new SubscribersModels.METADATA() { additionalProp1 = "", additionalProp2 = "", additionalProp3 = "" };

                string sdata = Newtonsoft.Json.JsonConvert.SerializeObject(_message);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://app.subscribers.com/api/v1/messages");
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers.Add("X-API-Key", requestData.ApiKey );

                //POST
                request.ContentLength = sdata.Length;

                using (Stream webStream = request.GetRequestStream())
                using (StreamWriter requestWriter = new StreamWriter(webStream, System.Text.Encoding.ASCII))
                {
                    requestWriter.Write(sdata);
                }

                SubscribersModels.MessageResponse _response = new SubscribersModels.MessageResponse();

                WebResponse webResponse = request.GetResponse();
                using (Stream webStream = webResponse.GetResponseStream())
                {
                    if (webStream != null)
                    {
                        using (StreamReader responseReader = new StreamReader(webStream))
                        {
                            _response = Newtonsoft.Json.JsonConvert.DeserializeObject<SubscribersModels.MessageResponse>(responseReader.ReadToEnd());
                            return _response.uuid;
                        }
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

        [Obsolete]
        public string SendCampaign(string idCampaign)
        {
            //CampaignEntity _campaign = CampaignRepository.FindById(new Guid(idCampaign));
            //ProductEntity product = ProductRepository.FindById(_campaign.PRODUCT_IdProduct);
            //if (_campaign != null)
            //{
            //    var _apikey = (from d in _campaign.PRODUCT.ProductSettingsEntitys where d.SettingName.Equals("pushApiToken") where d.PRODUCT_IdProduct.Equals(_campaign.PRODUCT.IdProduct) select d).FirstOrDefault();
            //    var _url = (from r in _campaign.CAMPAIGN_SETTINGS where r.SettingName.Equals("pushNotifUrl") select r).FirstOrDefault();
            //    if (_apikey != null)
            //    {
            //        SubscribersModels.MessageRequest _message = new SubscribersModels.MessageRequest();

            //        _message.body = _campaign.AdText;
            //        _message.title = _campaign.Name;
            //        _message.landing_page_url = _url.SettingValue;
            //        _message.image_url = _campaign.AdImage;
            //        _message.utm = new SubscribersModels.UTM() { campaign = _campaign.UTM_Campaign, medium = _campaign.UTM_Medium, source = _campaign.UTM_Source };
            //        _message.metadata = new SubscribersModels.METADATA() { additionalProp1 = "", additionalProp2 = "", additionalProp3 = "" };

            //        string sdata = Newtonsoft.Json.JsonConvert.SerializeObject(_message);

            //        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://app.subscribers.com/api/v1/messages");
            //        request.Method = "POST";
            //        request.ContentType = "application/json";
            //        request.Headers.Add("X-API-Key", _apikey.SettingValue.ToString());

            //        //POST
            //        request.ContentLength = sdata.Length;

            //        using (Stream webStream = request.GetRequestStream())
            //        using (StreamWriter requestWriter = new StreamWriter(webStream, System.Text.Encoding.ASCII))
            //        {
            //            requestWriter.Write(sdata);
            //        }

            //        SubscribersModels.MessageResponse _response = new SubscribersModels.MessageResponse();
            //        try
            //        {
            //            WebResponse webResponse = request.GetResponse();
            //            using (Stream webStream = webResponse.GetResponseStream())
            //            {
            //                if (webStream != null)
            //                {
            //                    using (StreamReader responseReader = new StreamReader(webStream))
            //                    {
            //                        _response = Newtonsoft.Json.JsonConvert.DeserializeObject<SubscribersModels.MessageResponse>(responseReader.ReadToEnd());
            //                        return _response.uuid;
            //                    }
            //                }
            //            }
            //        }
            //        catch (WebException e)
            //        {
            //            var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
            //            telemetria.Critical(messageException);
            //        }
            //    }
            //}
            return null;
        }
    }
}
