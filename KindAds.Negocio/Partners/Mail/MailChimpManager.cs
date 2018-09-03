using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailChimp.Campaigns;
using MailChimp;
using KindAds.Common.Interfaces;
using KindAds.Business;
using KindAds.DataAccess;
using KindAds.Common.Models.Entities;
using KindAds.Azure;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using KindAds.Comun.Models;

namespace KindAds.Business.Partners.Mail
{
    public class MailChimpManager
    {
        public ITrace telemetria { set; get; }
        //public CampaignRepository CampaignRepository { set; get; }
        //public ProductRepository ProductRepository { set; get; }
        //public CategoryRepository CategoryRepository { set; get; }

        public IList<AudiencePropertieSetting> settings { set; get; }

        public MailChimpManager()
        {

            telemetria = new Trace();
            //CampaignRepository = new CampaignRepository();
            //ProductRepository = new ProductRepository();
            //CategoryRepository = new CategoryRepository();
        }

        public MailChimpManager(IList<AudiencePropertieSetting> settings)
        {
            this.settings = settings;
            telemetria = new Trace();
        }


        public bool SettingsAreValid(string idCampaign)
        {
            bool result = false;
            result = (ApiTokenIsValid(idCampaign) && ListIsValid(idCampaign) && TemplateIsValid(idCampaign)) ? true : false;
            return result;
        }

        public bool SettingsAreValid()
        {
            bool result = false;
            result = (ApiTokenIsValid() && ListIsValid() && TemplateIsValid()) ? true : false;
            return result;
        }

        #region settings validation methods


        public string GetSettingFromCosmos(string setting)
        {
            string value = string.Empty;

            try
            {
                switch (setting)
                {
                    case "ApiKey":
                        {
                            foreach (var item in settings)
                            {
                                value = item.Name.Equals("mailChimpApiToken") ? item.Value : value;
                            }
                        }
                        break;
                    case "Template":
                        {
                            foreach (var item in settings)
                            {
                                value = item.Name.Equals("mailChimpTemplate") ? item.Value : value;
                            }
                        }
                        break;
                    case "List":
                        {
                            foreach (var item in settings)
                            {
                                value = item.Name.Equals("mailChimpList") ? item.Value : value;
                            }
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return value;
        }

        [Obsolete]
        public string GetSetting(string setting,string idCampaign)
        {
            //string value = string.Empty;
            //ProductSettingsRepository repository = new ProductSettingsRepository();
            //CampaignEntity campaign = CampaignRepository.FindById(new Guid(idCampaign));
            //ProductEntity product = ProductRepository.FindById(campaign.PRODUCT_IdProduct);
            //List<ProductSettingsEntity> settings = repository.GetProductSettingsByIdProduct(product.IdProduct);

            //try
            //{
            //    switch (setting)
            //    {
            //        case "ApiKey":
            //            {
            //                foreach (var item in settings)
            //                {
            //                    value = item.SettingName.Equals("mailChimpApiToken") ? item.SettingValue : value;
            //                }
            //            }
            //            break;
            //        case "Template":
            //            {
            //                foreach (var item in settings)
            //                {
            //                    value = item.SettingName.Equals("mailChimpTemplate") ? item.SettingValue : value;
            //                }
            //            }
            //            break;
            //        case "List":
            //            {
            //                foreach (var item in settings)
            //                {
            //                    value = item.SettingName.Equals("mailChimpList") ? item.SettingValue : value;
            //                }
            //            }
            //            break;
            //    }
            //}
            //catch (Exception e)
            //{
            //    var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
            //    telemetria.Critical(messageException);
            //}

            //return value;

            return null;
        }

        public bool ValidateApiToken(string ApiToken)
        {
            bool result = false;
            IMailChimpManager mailChimpManager = new MailChimp.MailChimpManager(ApiToken);
            var message = mailChimpManager.Ping();
            result = message != null;
            return result;
        }

        #region nuevas validaciones

      
        public bool ApiTokenIsValid()
        {
            bool result = false;
            string apiKey = string.Empty;

            try
            {
                apiKey = GetSettingFromCosmos("ApiKey");
                IMailChimpManager mailChimpManager = new MailChimp.MailChimpManager(apiKey);
                var lista = mailChimpManager.GetLists(null, 0, 100, "", "");
                if (lista != null)
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return result;
        }

        private bool TemplateIsValid()
        {
            bool result = false;
            string apiKey = string.Empty;
            string template = string.Empty;

            try
            {
                apiKey = GetSettingFromCosmos("ApiKey");
                template = GetSettingFromCosmos("Template");

                IMailChimpManager mailChimpManager = new MailChimp.MailChimpManager(apiKey);
                MailChimp.Templates.TemplateTypes tt = new MailChimp.Templates.TemplateTypes() { Base = true, Gallery = true, User = true };
                MailChimp.Templates.TemplateFilters tf = new MailChimp.Templates.TemplateFilters() { IncludeDragAndDrop = true };

                var list = mailChimpManager.GetTemplates(tt, tf);
                foreach (var tem in list.UserTemplates)
                {
                    if (tem.TemplateID.ToString() == template)
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

        private bool ListIsValid()
        {
            bool result = false;
            string apiKey = string.Empty;
            string list = string.Empty;

            try
            {
                apiKey = GetSettingFromCosmos("ApiKey");
                list = GetSettingFromCosmos("List");

                IMailChimpManager mailChimpManager = new MailChimp.MailChimpManager(apiKey);
                var lista = mailChimpManager.GetLists(null, 0, 100, "", "");
                foreach (var item in lista.Data)
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

        public bool ApiTokenIsValid(string idCampaign)
        {
            bool result = false;
            string apiKey = string.Empty;

            try
            {
                apiKey = GetSetting("ApiKey", idCampaign);
                IMailChimpManager mailChimpManager = new MailChimp.MailChimpManager(apiKey);
                var lista = mailChimpManager.GetLists(null, 0, 100, "", "");
                if(lista!=null)
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return result;
        }

        private bool TemplateIsValid(string idCampaign)
        {
            bool result = false;
            string apiKey = string.Empty;
            string template = string.Empty;

            try
            {
                apiKey = GetSetting("ApiKey", idCampaign);
                template = GetSetting("Template", idCampaign);

                IMailChimpManager mailChimpManager = new MailChimp.MailChimpManager(apiKey);
                MailChimp.Templates.TemplateTypes tt = new MailChimp.Templates.TemplateTypes() { Base = true, Gallery = true, User = true };
                MailChimp.Templates.TemplateFilters tf = new MailChimp.Templates.TemplateFilters() { IncludeDragAndDrop = true };

                var list = mailChimpManager.GetTemplates(tt, tf);
                foreach(var tem in list.UserTemplates)
                {
                    if(tem.TemplateID.ToString()==template)
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

        private bool ListIsValid(string idCampaign)
        {
            bool result = false;
            string apiKey = string.Empty;
            string list = string.Empty;

            try
            {
                apiKey = GetSetting("ApiKey", idCampaign);
                list = GetSetting("List", idCampaign);

                IMailChimpManager mailChimpManager = new MailChimp.MailChimpManager(apiKey);
                var lista = mailChimpManager.GetLists(null, 0, 100, "", "");
                foreach(var item in lista.Data)
                {
                    if(item.Id == list)
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

        #endregion+

        public MailChimpCampaignRequest FillSendinBlueRequestFromCosmos(List<CampaignSettingDocument> campaignSettings)
        {
            MailChimpCampaignRequest request = new MailChimpCampaignRequest();

            if (this.settings.Count > 0)
            {
                foreach (var item in settings)
                {

                    switch (item.Name)
                    {
                        case "mailChimpApiToken":
                            {
                                request.ApiKey = item.Name.Equals("mailChimpApiToken") ? item.Value : string.Empty;
                            }
                            break;
                        case "mailChimpList":
                            {
                                request.ListId = item.Name.Equals("mailChimpList") ? item.Value : string.Empty;
                            }
                            break;
                        case "mailChimpTemplate":
                            {
                                request.TemplateId = item.Name.Equals("mailChimpTemplate") ? item.Value : string.Empty;
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
                        case "mailChimpSubject":
                            {
                                request.Subject = setting.Name.Equals("mailChimpSubject") ? setting.Value : string.Empty;
                            }
                            break;
                        case "mailChimpFromName":
                            {
                                request.FromName = setting.Name.Equals("mailChimpFromName") ? setting.Value : string.Empty;
                            }
                            break;
                        case "mailChimpFromEmail":
                            {
                                request.FromEmail = setting.Name.Equals("mailChimpFromEmail") ? setting.Value : string.Empty;
                            }
                            break;
                    }
                }
            }

            return request;
        }

        public string SendCampaign(MailChimpCampaignRequest request)
        {
            string Id = string.Empty;
            try
            {
                CampaignCreateOptions cco = new CampaignCreateOptions()
                {
                    ListId = request.ListId,
                    Title = request.Name,
                    Subject = request.Subject,
                    FromEmail = request.FromEmail,
                    FromName = request.FromName
                };

                Dictionary<string, string> sections = new Dictionary<string, string>();
              

                CampaignCreateContent ccc = new CampaignCreateContent()
                {
                    Sections = sections,
                    HTML = request.Text
                };

                IMailChimpManager mailChimpManager = new MailChimp.MailChimpManager(request.ApiKey);
                var campaignMailChimp = mailChimpManager.CreateCampaign("regular", cco, ccc, null, null);

                if (!String.IsNullOrEmpty(campaignMailChimp.Id))
                {
                    var resultSendCampaign = mailChimpManager.SendCampaign(campaignMailChimp.Id);
                    Id= resultSendCampaign.Complete ? campaignMailChimp.Id : string.Empty;
                }
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return Id;
        }

        public MailChimpCampaignRequest getRequestData(CampaignDocument campaign, List<CampaignSettingDocument> settings)
        {
            MailChimpCampaignRequest request = FillSendinBlueRequestFromCosmos(settings);
            request.Name = campaign.Name;
            request.Text = campaign.Text;
            return request;
        }

        [Obsolete]
        public string SendCampaign(string idCampaign)
        {
            //string idList = null;
            //string apiKey = null;
            //int idTemplate = 0;
            //string subject = "";
            //string fromEmail = "";
            //string fromName = "";

            //CampaignEntity campaign = CampaignRepository.FindById(new Guid(idCampaign));
            //ProductEntity product = ProductRepository.FindById(campaign.PRODUCT_IdProduct);

            //try
            //{
            //    if (product.ProductSettingsEntitys != null && product.ProductSettingsEntitys.Any())
            //    {
            //        foreach (var item in product.ProductSettingsEntitys)
            //        {
            //            apiKey = item.SettingName.Equals("mailChimpApiToken") ? item.SettingValue : apiKey;
            //            idList = item.SettingName.Equals("mailChimpList") ? item.SettingValue : idList;
            //            idTemplate = item.SettingName.Equals("mailChimpTemplate") ? Convert.ToInt32(item.SettingValue) : idTemplate;
            //        }
            //    }

            //    if (campaign.CAMPAIGN_SETTINGS != null && campaign.CAMPAIGN_SETTINGS.Any())
            //    {
            //        foreach (var setting in campaign.CAMPAIGN_SETTINGS)
            //        {
            //            subject = setting.SettingName.Equals("mailChimpSubject") ? setting.SettingValue : subject;
            //            fromName = setting.SettingName.Equals("mailChimpFromName") ? setting.SettingValue : fromName;
            //            fromEmail = setting.SettingName.Equals("mailChimpFromEmail") ? setting.SettingValue : fromEmail;
            //        }
            //    }

            //    CampaignCreateOptions cco = new CampaignCreateOptions()
            //    {
            //        ListId = idList,
            //        Title = campaign.Name,
            //        Subject = subject,
            //        FromEmail = fromEmail,
            //        FromName = fromName
            //    };

            //    Dictionary<string, string> sections = new Dictionary<string, string>();
            //    var categorys = CategoryRepository.GetByIdSite(campaign.PRODUCT.SITE.IdSite);
            //    if (categorys != null && categorys.Any())
            //    {
            //        foreach (var item in categorys)
            //        {
            //            sections.Add(item.IdCategory.ToString(), item.Description);
            //        }
            //    }

            //    CampaignCreateContent ccc = new CampaignCreateContent()
            //    {
            //        Sections = sections,
            //        HTML = campaign.AdText
            //    };

            //    IMailChimpManager mailChimpManager = new MailChimp.MailChimpManager(apiKey);
            //    var campaignMailChimp = mailChimpManager.CreateCampaign("regular", cco, ccc, null, null);

            //    if (campaign != null && !String.IsNullOrEmpty(campaignMailChimp.Id))
            //    {
            //        var resultSendCampaign = mailChimpManager.SendCampaign(campaignMailChimp.Id);
            //        return resultSendCampaign.Complete ? campaignMailChimp.Id : null;
            //    }
            //}
            //catch (Exception e)
            //{
            //    var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
            //    telemetria.Critical(messageException);
            //}
            return null;
        }

        public bool addUserToList(string apiKey, string idList, string email)
        {
            MailChimp.Net.Models.Member member = new MailChimp.Net.Models.Member()
            {
                EmailAddress = email,
                Status = MailChimp.Net.Models.Status.Subscribed
            };

            MailChimp.Net.Interfaces.IMailChimpManager mailChimpManager = new MailChimp.Net.MailChimpManager(apiKey);
            var result =  Task.Run(() =>mailChimpManager.Members.AddOrUpdateAsync(idList, member)).Result;
            
            return result.Id != null;


        }
    }
}
