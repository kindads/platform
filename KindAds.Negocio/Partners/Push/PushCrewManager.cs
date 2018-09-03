using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using KindAds.Common.Interfaces;
using KindAds.Business;
using KindAds.DataAccess;
using KindAds.Common.Partners.Push;
using KindAds.Azure;
using KindAds.Comun.Models;
using KindAds.Common.Models.Entities;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;

namespace KindAds.Business.Partners.Push
{
    public class PushCrewManager
    {
        public ITrace telemetria { set; get; }
        //public CampaignRepository CampaignRepository { set; get; }
        //public ProductRepository ProductRepository { set; get; }
        //public ProductSettingsRepository ProductSettingsRepository { set; get; }
        public IList<AudiencePropertieSetting> settings { set; get; }
        public IList<CampaignSettingDocument> campaignSettings { set; get; }

        public PushCrewManager()
        {            
            telemetria = new Trace();
            //CampaignRepository = new CampaignRepository ();
            //ProductRepository = new ProductRepository ();
            //ProductSettingsRepository = new ProductSettingsRepository();
        }

        public PushCrewManager(IList<AudiencePropertieSetting> settings)
        {
            this.settings = settings;
            telemetria = new Trace();
        }

        public  bool validatePushCrewToken(string _apiKey, string _siteUrl)
        {
            if (_apiKey.Length > 0)
            {
                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("https://pushcrew.com/api/v1/segments");
                request.Method = "GET";
                request.Headers.Add("Authorization", _apiKey);
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
                                        ValidKeyResponse _response = new ValidKeyResponse();
                                        _response = Newtonsoft.Json.JsonConvert.DeserializeObject<ValidKeyResponse>(_apiresponse);
                                        if (_response != null)
                                        {
                                            if (_response.status != null)
                                            {
                                                if ((_response.message != null && !_response.message.Contains("not authorized")) || _response.status == "success")
                                                {
                                                    return true;
                                                }
                                                else
                                                {
                                                    return false;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception Ex)
                {
                    return false;
                }
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
            if(campaignSettings!=null || campaignSettings.Count>0)
            {
                result = (ApiTokenIsValid()) ? true : false;
            }          
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

        #region nuevas validaciones

        public bool ValidateApiToken(string ApiKey,string Url)
        {
            bool result = false;

            try
            {
                result = validatePushCrewToken(ApiKey, Url);
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return result;
        }

        private bool ApiTokenIsValid()
        {
            bool result = false;
            string apiKey = string.Empty;
            string siteUrl = string.Empty;

            try
            {
                apiKey = GetSettingFromCosmos("ApiKey");
                siteUrl = GetSettingFromCosmos("Url");

                result = validatePushCrewToken(apiKey, siteUrl);
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
            string siteUrl = string.Empty;

            try
            {
                apiKey = GetSetting("ApiKey", idCampaign);
                siteUrl = GetSetting("Url", idCampaign);

                result = validatePushCrewToken( apiKey, siteUrl);
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return result;
        }


        public PushCrewCampaignRequest FillSendinBlueRequestFromCosmos()
        {
            PushCrewCampaignRequest request = new PushCrewCampaignRequest();

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

                if (this.campaignSettings.Count >0)
                {

                    foreach (var item in settings)
                    {

                        switch (item.Name)
                        {
                            case "pushUrl":
                                {
                                    request.Url = item.Name.Equals("pushUrl") ? item.Value : string.Empty;
                                }
                                break;
                            case "pushImage":
                                {
                                    request.Image = item.Name.Equals("pushImage") ? item.Value : string.Empty;
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

        public PushCrewCampaignRequest getRequestData(CampaignDocument campaign)
        {
            PushCrewCampaignRequest request = FillSendinBlueRequestFromCosmos();
            request.Text = campaign.Text;
            request.Name = campaign.Name;
            return request;
        }

        public string SendCampaign(PushCrewCampaignRequest request)
        {
            string Id = string.Empty;
            try
            {
                //todo
                PushcrewModel.MessageRequest _message = new PushcrewModel.MessageRequest();

                _message.title = request.Name;
                _message.message = request.Text;
                _message.url = request.Url;
                _message.image_url = request.Image;

                string newMessage = "title=" + _message.title + "&";
                newMessage += "message=" + _message.message + "&";
                newMessage += "url=" + _message.url + "&";
                newMessage += "image_url=" + _message.image_url + "";

                try
                {
                    using (WebClient wc = new WebClient())
                    {
                        wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                        wc.Headers[HttpRequestHeader.Authorization] = request.ApiKey;
                        byte[] bret = wc.UploadData("https://pushcrew.com/api/v1/send/all/", "POST", System.Text.Encoding.UTF8.GetBytes(newMessage));
                        string HtmlResult = System.Text.Encoding.UTF8.GetString(bret);
                        string key = "";

                        if (HtmlResult.Length > 0)
                        {
                            dynamic dyn = JsonConvert.DeserializeObject(HtmlResult);
                            key = dyn.request_id;
                            return key;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    var messageException = telemetria.MakeMessageException(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    telemetria.Critical(messageException);
                }
            }
            catch (Exception e)
            {

            }
            return Id;
        }

        [Obsolete]
        public string SendCampaign(string idCampaign)
        {
            //var _campaign = CampaignRepository.FindById(new Guid(idCampaign));
            //_campaign.PRODUCT = ProductRepository.FindById(_campaign.PRODUCT_IdProduct);
            //_campaign.PRODUCT.ProductSettingsEntitys = ProductSettingsRepository.GetProductSettingsByIdProduct(_campaign.PRODUCT_IdProduct);

            //if (_campaign != null)
            //{
            //    var _apikey = (from d in _campaign.PRODUCT.ProductSettingsEntitys where d.SettingName.Equals("pushApiToken") where d.PRODUCT_IdProduct.Equals(_campaign.PRODUCT.IdProduct) select d).FirstOrDefault();

            //    if (_apikey != null)
            //    {
            //        PushcrewModel.MessageRequest _message = new PushcrewModel.MessageRequest();

            //        _message.title = _campaign.Name;
            //        _message.message = _campaign.AdText;
            //        _message.url = _campaign.AdURL;
            //        _message.image_url = _campaign.AdImage;

            //        string newMessage = "title=" + _message.title + "&";
            //        newMessage += "message=" + _message.message + "&";
            //        newMessage += "url=" + _message.url + "&";
            //        newMessage += "image_url=" + _message.image_url + "";

            //        try
            //        {
            //            using (WebClient wc = new WebClient())
            //            {
            //                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            //                wc.Headers[HttpRequestHeader.Authorization] = _apikey.SettingValue.ToString();
            //                byte[] bret = wc.UploadData("https://pushcrew.com/api/v1/send/all/", "POST", System.Text.Encoding.UTF8.GetBytes(newMessage));
            //                string HtmlResult = System.Text.Encoding.UTF8.GetString(bret);
            //                string key = "";

            //                if (HtmlResult.Length > 0)
            //                {
            //                    dynamic dyn = JsonConvert.DeserializeObject(HtmlResult);
            //                    key = dyn.request_id;
            //                    return key;
            //                }
            //                else
            //                {
            //                    return null;
            //                }
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            var messageException = telemetria.MakeMessageException(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            //            telemetria.Critical(messageException);
            //        }
            //    }
            //}
            return null;
        }
    }
}
