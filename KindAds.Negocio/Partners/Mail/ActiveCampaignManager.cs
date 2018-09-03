using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KindAds.Common.Interfaces;
using KindAds.Business;
using KindAds.DataAccess;
using KindAds.Common.Utils.Partners.Mail.ActiveCampaign;
using Newtonsoft.Json;
using KindAds.Common.Models.Entities;
using KindAds.Azure;
using KindAds.Comun.Models;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;

namespace KindAds.Business.Partners.Mail
{
    public class ActiveCampaignManager
    {
        public ITrace telemetria { set; get; }
        //public CampaignRepository CampaignRepository { set; get; }
        //public ProductRepository ProductRepository { set; get; }

        public IList<AudiencePropertieSetting> settings { set; get; }

        public ActiveCampaignManager()
        {

            telemetria = new Trace();
            //CampaignRepository = new CampaignRepository();
            //ProductRepository = new ProductRepository ();
        }

        public ActiveCampaignManager(IList<AudiencePropertieSetting> settings)
        {
            this.settings = settings;
        }

        public bool IsValid(string apiKey, string url)
        {
            ActiveCampaignClient activeCampaignClient = new ActiveCampaignClient(apiKey, url);
            Dictionary<string, string> parameters = new Dictionary<string, string>() { };
            var result = activeCampaignClient.ApiAsync("account_view", parameters);
            return Convert.ToBoolean(result.Code);
        }

        public List<ActiveCampaignGeneric> GetLists(string apiKey, string url)
        {
            ActiveCampaignGeneric element;
            List<ActiveCampaignGeneric> list = new List<ActiveCampaignGeneric>();
            ActiveCampaignClient activeCampaignClient = new ActiveCampaignClient(apiKey, url);
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            var result = activeCampaignClient.ApiAsync("list_paginator", parameters);
            dynamic stuff = JsonConvert.DeserializeObject(result.Data);
            dynamic rows = stuff.rows;
            foreach (var item in rows)
            {
                element = new ActiveCampaignGeneric()
                {
                    Id = item.id,
                    Name = item.name
                };
                list.Add(element);
            }
            return list;
        }

        public bool SettingsAreValid(string idCampaign)
        {
            // Possible need add more validations methods
            bool result = false;
            result = ( ApiTokenIsValid(idCampaign) && UrlIsValid(idCampaign) && ListIsValid(idCampaign)) ? true : false;
            return result;
        }

        public bool SettingsAreValid()
        {
            // Possible need add more validations methods
            bool result = false;
            result = (ApiTokenIsValid() && UrlIsValid() && ListIsValid()) ? true : false;
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
                            value = item.Name.Equals("activeCampaignApiToken") ? item.Value : value;
                        }
                    }
                    break;
                case "List":
                    {
                        foreach (var item in settings)
                        {
                            value = item.Name.Equals("activeCampaignList") ? item.Value : value;
                        }
                    }
                    break;
                case "Url":
                    {
                        foreach (var item in settings)
                        {
                            value = item.Name.Equals("activeCampaignUrl") ? item.Value : value;
                        }
                    }
                    break;

            }
            return value;
        }

        [Obsolete]
        public string GetSetting(string setting, string idCampaign)
        {
            //ProductSettingsRepository repository = new ProductSettingsRepository();
            //CampaignEntity campaign = CampaignRepository.FindById(new Guid(idCampaign));
            //ProductEntity product = ProductRepository.FindById(campaign.PRODUCT_IdProduct);
            //List<ProductSettingsEntity> settings = repository.GetProductSettingsByIdProduct(product.IdProduct);

            string value = string.Empty;
            //switch (setting)
            //{
            //    case "ApiKey":
            //        {
            //            foreach (var item in settings)
            //            {
            //                value = item.SettingName.Equals("activeCampaignApiToken") ? item.SettingValue : value;
            //            }
            //        }
            //        break;
            //    case "List":
            //        {
            //            foreach (var item in settings)
            //            {
            //                value = item.SettingName.Equals("activeCampaignList") ? item.SettingValue : value;
            //            }
            //        }
            //        break;
            //    case "Url":
            //        {
            //            foreach (var item in settings)
            //            {
            //                value = item.SettingName.Equals("activeCampaignUrl") ? item.SettingValue : value;
            //            }
            //        }
            //        break;
                
            //}
            return value;
        }

        #region nuevos metodos de validacion

        public bool ValidateApiToken(string ApiKey, string Url)
        {
            bool result = false;
            try
            {
                result = IsValid(ApiKey, Url);
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
            string url = string.Empty;

            try
            {
                apiKey = GetSettingFromCosmos("ApiKey");
                url = GetSettingFromCosmos("Url");

                result = IsValid(apiKey, url);
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return result;
        }

        private bool UrlIsValid()
        {
            bool result = false;
            string apiKey = string.Empty;
            string url = string.Empty;

            try
            {
                apiKey = GetSettingFromCosmos("ApiKey");
                url = GetSettingFromCosmos("Url");

                result = IsValid(apiKey, url);
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
            string apiKey = string.Empty;
            string url = string.Empty;
            string list = string.Empty;

            try
            {
                apiKey = GetSettingFromCosmos("ApiKey");
                url = GetSettingFromCosmos("Url");
                list = GetSettingFromCosmos("List");

                var lista = GetLists(apiKey, url);
                foreach (var item in lista)
                {
                    if (item.Id == list)
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
            string url = string.Empty;

            try
            {
                apiKey = GetSetting("ApiKey", idCampaign);
                url = GetSetting("Url", idCampaign);

                result = IsValid(apiKey, url);
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return result;
        }

        private bool UrlIsValid(string idCampaign)
        {
            bool result = false;
            string apiKey = string.Empty;
            string url = string.Empty;

            try
            {
                apiKey = GetSetting("ApiKey", idCampaign);
                url = GetSetting("Url", idCampaign);

                result = IsValid(apiKey, url);
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
            string apiKey = string.Empty;
            string url = string.Empty;
            string list = string.Empty;

            try
            {
                apiKey = GetSetting("ApiKey", idCampaign);
                url = GetSetting("Url", idCampaign);
                list = GetSetting("List", idCampaign);

                var lista = GetLists(apiKey, url);
                foreach(var item in lista)
                {
                    if(item.Id==list)
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

        public ActiveCampaignRequest FillSendinBlueRequestFromCosmos(List<CampaignSettingDocument> campaignSettings)
        {
            ActiveCampaignRequest request = new ActiveCampaignRequest();
            //todo
            try
            {
                // from products
                if (this.settings.Count > 0)
                {
                    foreach (var item in settings)
                    {

                        switch (item.Name)
                        {
                            case "activeCampaignApiToken":
                                {
                                    request.ApiKey = item.Name.Equals("activeCampaignApiToken") ? item.Value : string.Empty;
                                }
                                break;
                            case "activeCampaignList":
                                {
                                    request.ListId = item.Name.Equals("activeCampaignList") ? item.Value : string.Empty;
                                }
                                break;
                            case "activeCampaignUrl":
                                {
                                    request.Url = item.Name.Equals("activeCampaignUrl") ? item.Value : string.Empty;
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
                            case "activeCampaignSubject":
                                {
                                    request.Subject = setting.Name.Equals("activeCampaignSubject") ? setting.Value : string.Empty;
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

        public ActiveCampaignRequest getRequestData(CampaignDocument campaign, List<CampaignSettingDocument> settings)
        {
            ActiveCampaignRequest request = FillSendinBlueRequestFromCosmos(settings);
            request.Name = campaign.Name;
            request.Text = campaign.Text;
            return request;
        }

        public string SendCampaign(ActiveCampaignRequest request)
        {
            string Id = string.Empty;
            try
            {
                ActiveCampaignClient activeCampaignClient = new ActiveCampaignClient(request.ApiKey, request.Url);
                Dictionary<string, string> parameters = new Dictionary<string, string>() { };

                var result = activeCampaignClient.ApiAsync("account_view", parameters);
                dynamic data = JsonConvert.DeserializeObject(result.Data);
                request.FromEmail = data.email;
                request.FromName = data.fname;

                activeCampaignClient = new ActiveCampaignClient(request.ApiKey, request.Url);
                parameters = new Dictionary<string, string>
                {
                    { "format", "mime" },
                    { "subject", request.Subject },
                    { "fromemail", request.FromEmail },
                    { "fromname", request.FromName },
                    { "reply2", "" },
                    { "priority", "5" },
                    { "charset", "utf-8" },
                    { "encoding", "quoted-printable" },
                    { "htmlconstructor", "editor" },
                    { "html", request.Text },
                    { "textconstructor", "editor" },
                    { "p[" + request.ListId + "]", request.ListId }
                };
                result = activeCampaignClient.ApiAsync("message_add", parameters);
                data = JsonConvert.DeserializeObject(result.Data);
                string idMessage = data.id;

                activeCampaignClient = new ActiveCampaignClient(request.ApiKey, request.Url);
                parameters = new Dictionary<string, string>
                {
                    { "type", "text" },
                    { "segmentid", "0" },
                    { "bounceid", "-1" },
                    { "name", request.Name },
                    { "sdate", DateTime.Now.ToString() },
                    { "status", "1" },
                    { "public", "1" },
                    { "mailer_log_file", "4" },
                    { "tracklinks", "all" },
                    { "tracklinksanalytics", "" },
                    { "trackreads", "0" },
                    { "trackreplies", "0" },
                    { "htmlunsub", "1" },
                    { "textunsub", "1" },
                    { "p[" + request.ListId + "]", request.ListId },
                    { "m["+ idMessage +"]", "100" }
                };
                result = activeCampaignClient.ApiAsync("campaign_create", parameters);
                data = JsonConvert.DeserializeObject(result.Data);
                Id= data.id;

                Id = Convert.ToBoolean(result.Code) ? data.id : null;
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
      //      try
      //      {
      //          string apiKey = null;
      //          string idList = null;
      //          string url = null;
      //          string subject = null;
      //          string idCampaignActiveCampaign = null;
      //          string fromName = null;
      //          string fromEmail = null;
      //          string idMessage = null;

      //          CampaignEntity campaign = CampaignRepository.FindById(new Guid(idCampaign));
      //          ProductEntity product = ProductRepository.FindById(campaign.PRODUCT_IdProduct);

      //          if (product.ProductSettingsEntitys != null && product.ProductSettingsEntitys.Any())
      //          {
      //              foreach (var item in product.ProductSettingsEntitys)
      //              {
      //                  apiKey = item.SettingName.Equals("activeCampaignApiToken") ? item.SettingValue : apiKey;
      //                  idList = item.SettingName.Equals("activeCampaignList") ? item.SettingValue : idList;
      //                  url = item.SettingName.Equals("activeCampaignUrl") ? item.SettingValue : url;
      //              }
      //          }

      //          if (campaign.CAMPAIGN_SETTINGS != null && campaign.CAMPAIGN_SETTINGS.Any())
      //          {
      //              foreach (var setting in campaign.CAMPAIGN_SETTINGS)
      //              {
      //                  subject = setting.SettingName.Equals("activeCampaignSubject") ? setting.SettingValue : subject;
      //              }
      //          }

      //          ActiveCampaignClient activeCampaignClient = new ActiveCampaignClient(apiKey, url);
      //          Dictionary<string, string> parameters = new Dictionary<string, string>() { };

      //          var result = activeCampaignClient.ApiAsync("account_view", parameters);
      //          dynamic data = JsonConvert.DeserializeObject(result.Data);
      //          fromEmail = data.email;
      //          fromName = data.fname;

      //          activeCampaignClient = new ActiveCampaignClient(apiKey, url);
      //          parameters = new Dictionary<string, string>
      //{
      //  { "format", "mime" },
      //  { "subject", subject },
      //  { "fromemail", fromEmail },
      //  { "fromname", fromName },
      //  { "reply2", "" },
      //  { "priority", "5" },
      //  { "charset", "utf-8" },
      //  { "encoding", "quoted-printable" },
      //  { "htmlconstructor", "editor" },
      //  { "html", campaign.AdText },
      //  { "textconstructor", "editor" },
      //  { "p[" + idList + "]", idList }
      //};
      //          result = activeCampaignClient.ApiAsync("message_add", parameters);
      //          data = JsonConvert.DeserializeObject(result.Data);
      //          idMessage = data.id;
      //          //Console.WriteLine(result.Code);//1 successfull

      //          activeCampaignClient = new ActiveCampaignClient(apiKey, url);
      //          parameters = new Dictionary<string, string>
      //{
      //  { "type", "text" },
      //  { "segmentid", "0" },
      //  { "bounceid", "-1" },
      //  { "name", campaign.Name },
      //  { "sdate", DateTime.Now.ToString() },
      //  { "status", "1" },
      //  { "public", "1" },
      //  { "mailer_log_file", "4" },
      //  { "tracklinks", "all" },
      //  { "tracklinksanalytics", "" },
      //  { "trackreads", "0" },
      //  { "trackreplies", "0" },
      //  { "htmlunsub", "1" },
      //  { "textunsub", "1" },
      //  { "p[" + idList + "]", idList },
      //  { "m["+ idMessage +"]", "100" }
      //};
      //          result = activeCampaignClient.ApiAsync("campaign_create", parameters);
      //          data = JsonConvert.DeserializeObject(result.Data);
      //          idCampaignActiveCampaign = data.id;
      //          //Console.WriteLine(data.id);
      //          //Console.WriteLine(result.Code);//1 successfull

      //          idCampaignActiveCampaign = Convert.ToBoolean(result.Code) ? data.id : null;
      //          return idCampaignActiveCampaign;
      //      }
      //      catch (Exception ex)
      //      {
      //          var messageException = telemetria.MakeMessageException(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
      //          telemetria.Critical(messageException);
      //      }
            return null;
        }

    }


}
