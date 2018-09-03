using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KindAds.Common.Interfaces;
using KindAds.Business;
using KindAds.DataAccess;
using Mailjet;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Newtonsoft.Json;
using KindAds.Common.Models.Entities;
using KindAds.Azure;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using KindAds.Comun.Models;

namespace KindAds.Business.Partners.Mail
{

    public class MailJetManager
    {
        public ITrace telemetria { set; get; }
        //public CampaignRepository CampaignRepository { set; get; }
        //public ProductRepository ProductRepository { set; get; }

        public IList<AudiencePropertieSetting> settings { set; get; }

        public MailJetManager()
        {

            telemetria = new Trace();
            //CampaignRepository = new CampaignRepository ();
            //ProductRepository = new ProductRepository ();
        }

        public MailJetManager(IList<AudiencePropertieSetting> settings)
        {
            this.settings = settings;
            telemetria = new Trace();
        }

        public bool IsValidAsync(string apiKey, string apiSecret)
        {
            try
            {
                MailjetClient client = new MailjetClient(apiKey, apiSecret);
                MailjetRequest request = new MailjetRequest
                {
                    Resource = Sender.Resource,
                };

                MailjetResponse response = Task.Run(() => client.GetAsync(request)).Result;
                return response.IsSuccessStatusCode;
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
            result = (ApiTokenIsValid(idCampaign) && SecretKeyIsValid(idCampaign) &&
                       SegmentIsValid(idCampaign) && ListIsValid(idCampaign)) ? true : false;
            return result;
        }

        public bool SettingsAreValid()
        {
            bool result = false;
            result = (ApiTokenIsValid() && SecretKeyIsValid() &&
                       SegmentIsValid() && ListIsValid()) ? true : false;
            return result;
        }

        #region settings validation methods


        public string GetSettingFromCosmos(string setting)
        {
            string value = string.Empty;
           
            switch (setting)
            {
                case "ApiKey":
                    {
                        foreach (var item in settings)
                        {
                            value = item.Name.Equals("mailJetApiKey") ? item.Value : value;
                        }
                    }
                    break;
                case "SecretKey":
                    {
                        foreach (var item in settings)
                        {
                            value = item.Name.Equals("mailJetSecretKey") ? item.Value : value;
                        }
                    }
                    break;
                case "List":
                    {
                        foreach (var item in settings)
                        {
                            value = item.Name.Equals("mailJetList") ? item.Value : value;
                        }
                    }
                    break;
                case "Segment":
                    {
                        foreach (var item in settings)
                        {
                            value = item.Name.Equals("mailJetSegment") ? item.Value : value;
                        }
                    }
                    break;
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

            //switch (setting)
            //{
            //    case "ApiKey":
            //        {
            //            foreach (var item in settings)
            //            {
            //                value = item.SettingName.Equals("mailJetApiKey") ? item.SettingValue : value;
            //            }
            //        }
            //        break;
            //    case "SecretKey":
            //        {
            //            foreach (var item in settings)
            //            {
            //                value = item.SettingName.Equals("mailJetSecretKey") ? item.SettingValue : value;
            //            }
            //        }
            //        break;
            //    case "List":
            //        {
            //            foreach (var item in settings)
            //            {
            //                value = item.SettingName.Equals("mailJetList") ? item.SettingValue : value;
            //            }
            //        }
            //        break;
            //    case "Segment":
            //        {
            //            foreach (var item in settings)
            //            {
            //                value = item.SettingName.Equals("mailJetSegment") ? item.SettingValue : value;
            //            }
            //        }
            //        break;
            //}
            //return value;

            return null;
        }

        #region new validations
        private bool ApiTokenIsValid()
        {
            bool result = false;
            string apiKey = string.Empty;
            string secretKey = string.Empty;

            try
            {
                //Todo
                apiKey = GetSettingFromCosmos("ApiKey");
                secretKey = GetSettingFromCosmos("SecretKey");

                var lista = GetLists(apiKey, secretKey);
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

        private bool SecretKeyIsValid()
        {
            bool result = false;
            string apiKey = string.Empty;
            string secretKey = string.Empty;

            try
            {
                //Todo
                apiKey = GetSettingFromCosmos("ApiKey");
                secretKey = GetSettingFromCosmos("SecretKey");

                var lista = GetLists(apiKey, secretKey);
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

        private bool ListIsValid()
        {
            bool result = false;
            string apiKey = string.Empty;
            string secretKey = string.Empty;
            string list = string.Empty;

            try
            {
                //Todo
                apiKey = GetSettingFromCosmos("ApiKey");
                secretKey = GetSettingFromCosmos("SecretKey");
                list = GetSettingFromCosmos("List");

                var lista = GetLists(apiKey, secretKey);
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

        private bool SegmentIsValid()
        {
            bool result = false;
            string apiKey = string.Empty;
            string secretKey = string.Empty;
            string segment = string.Empty;

            try
            {
                apiKey = GetSettingFromCosmos("ApiKey");
                secretKey = GetSettingFromCosmos("SecretKey");
                segment = GetSettingFromCosmos("Segment");

                var segmentos = GetSegments(apiKey, secretKey);
                foreach (var segmento in segmentos)
                {
                    if (segmento.Id == segment)
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
            string secretKey = string.Empty;

            try
            {
                //Todo
                apiKey = GetSetting("ApiKey", idCampaign);
                secretKey = GetSetting("SecretKey", idCampaign);

                var lista = GetLists(apiKey, secretKey);
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

        private bool SecretKeyIsValid(string idCampaign)
        {
            bool result = false;
            string apiKey = string.Empty;
            string secretKey = string.Empty;

            try
            {
                //Todo
                apiKey = GetSetting("ApiKey", idCampaign);
                secretKey = GetSetting("SecretKey", idCampaign);

                var lista = GetLists(apiKey, secretKey);
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

        private bool ListIsValid(string idCampaign)
        {
            bool result = false;
            string apiKey = string.Empty;
            string secretKey = string.Empty;
            string list = string.Empty;

            try
            {
                //Todo
                apiKey = GetSetting("ApiKey", idCampaign);
                secretKey = GetSetting("SecretKey", idCampaign);
                list = GetSetting("List", idCampaign);

                var lista = GetLists(apiKey, secretKey);
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

        private bool SegmentIsValid(string idCampaign)
        {
            bool result = false;
            string apiKey = string.Empty;
            string secretKey = string.Empty;
            string segment = string.Empty;

            try
            {
                apiKey = GetSetting("ApiKey", idCampaign);
                secretKey = GetSetting("SecretKey", idCampaign);
                segment = GetSetting("Segment", idCampaign);

                var segmentos = GetSegments(apiKey, secretKey);
                foreach(var segmento in segmentos)
                {
                    if(segmento.Id==segment)
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

        public MailJetCampaignRequest FillSendinBlueRequestFromCosmos(List<CampaignSettingDocument> campaignSettings)
        {
            MailJetCampaignRequest request = new MailJetCampaignRequest();
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
                            case "mailJetApiKey":
                                {
                                    request.ApiKey = item.Name.Equals("mailJetApiKey") ? item.Value : string.Empty;
                                }
                                break;
                            case "mailJetSecretKey":
                                {
                                    request.SecretKey = item.Name.Equals("mailJetSecretKey") ? item.Value : string.Empty;
                                }
                                break;
                            case "mailJetList":
                                {
                                    request.ListId = item.Name.Equals("mailJetList") ? item.Value : string.Empty;
                                }
                                break;
                            case "mailJetSegment":
                                {
                                    request.SegmentId = item.Name.Equals("mailJetSegment") ? item.Value : string.Empty;
                                }
                                break;
                        }
                    }
                }

                // from campaigns
                if(campaignSettings.Count>0)
                {
                    foreach (var setting in campaignSettings)
                    {
                        switch (setting.Name)
                        {
                            case "mailJetSubject":
                                {
                                    request.Subject = setting.Name.Equals("mailJetSubject") ? setting.Value : string.Empty;
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

        public MailJetCampaignRequest getRequestData(CampaignDocument campaign, List<CampaignSettingDocument> settings)
        {
            MailJetCampaignRequest request = FillSendinBlueRequestFromCosmos(settings);
            request.Name = campaign.Name;
            request.Text = campaign.Text;
            return request;
        }

        public string SendCampaign(MailJetCampaignRequest requestData)
        {
            string Id = string.Empty;
            try
            {
                MailjetClient client = new MailjetClient(requestData.ApiKey, requestData.SecretKey);

                var sender = GetSender(client);

                MailjetRequest request = new MailjetRequest
                {
                    Resource = Campaigndraft.Resource,
                }
                   .Property(Campaigndraft.Locale, "en_US")
                   .Property(Campaigndraft.Sender, GetMyProfile(client).CompanyName)
                   .Property(Campaigndraft.SenderEmail, sender.Email)
                   .Property(Campaigndraft.Subject, requestData.Subject)
                   .Property("ContactsListID", requestData.ListId)
                   .Property(Campaigndraft.Title, requestData.Name) //campaign
                   .Property("SegmentationID", requestData.SegmentId);

                MailjetResponse response = Task.Run(() => client.PostAsync(request)).Result;
                if (response.IsSuccessStatusCode)
                {
                    List<MailJetCampaign> listCampaigncampaign = JsonConvert.DeserializeObject<List<MailJetCampaign>>(response.GetData().ToString());
                    var campaignMailJet = listCampaigncampaign.FirstOrDefault();
                    bool actionAddBody = AddBody(client, Convert.ToInt64(campaignMailJet.Id), requestData.Text); // campaign
                    bool actionSendCampaign = SendCampaign(client, Convert.ToInt64(campaignMailJet.Id));
                    return actionAddBody && actionSendCampaign ? campaignMailJet.Id : null;
                }
                else
                {
                    telemetria.Critical(string.Format("StatusCode: {0}\n", response.StatusCode) + " " + string.Format("ErrorInfo: {0}\n", response.GetErrorInfo()) + " " + string.Format("ErrorMessage: {0}\n", response.GetErrorMessage()));
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
            //    CampaignEntity campaign = CampaignRepository.FindById(new Guid(idCampaign));
            //    string apiKey = GetProductSetting(campaign.PRODUCT.ProductSettingsEntitys, "mailJetApiKey");
            //    string secretKey = GetProductSetting(campaign.PRODUCT.ProductSettingsEntitys, "mailJetSecretKey");
            //    string listId = GetProductSetting(campaign.PRODUCT.ProductSettingsEntitys, "mailJetList");
            //    string segmentId = GetProductSetting(campaign.PRODUCT.ProductSettingsEntitys, "mailJetSegment");
            //    string subject = GetCampaignSetting(campaign.CAMPAIGN_SETTINGS, "mailJetSubject");

            //    MailjetClient client = new MailjetClient(apiKey, secretKey);

            //    var sender = GetSender(client);

            //    MailjetRequest request = new MailjetRequest
            //    {
            //        Resource = Campaigndraft.Resource,
            //    }
            //       .Property(Campaigndraft.Locale, "en_US")
            //       .Property(Campaigndraft.Sender, GetMyProfile(client).CompanyName)
            //       .Property(Campaigndraft.SenderEmail, sender.Email)
            //       .Property(Campaigndraft.Subject, subject)
            //       .Property("ContactsListID", listId)
            //       .Property(Campaigndraft.Title, campaign.Name)
            //       .Property("SegmentationID", segmentId);

            //    MailjetResponse response = Task.Run(() => client.PostAsync(request)).Result;
            //    if (response.IsSuccessStatusCode)
            //    {
            //        List<MailJetCampaign> listCampaigncampaign = JsonConvert.DeserializeObject<List<MailJetCampaign>>(response.GetData().ToString());
            //        var campaignMailJet = listCampaigncampaign.FirstOrDefault();
            //        bool actionAddBody = AddBody(client, Convert.ToInt64(campaignMailJet.Id), campaign.AdText);
            //        bool actionSendCampaign = SendCampaign(client, Convert.ToInt64(campaignMailJet.Id));
            //        return actionAddBody && actionSendCampaign ? campaignMailJet.Id : null;
            //    }
            //    else
            //    {
            //        telemetria.Critical(string.Format("StatusCode: {0}\n", response.StatusCode) + " " + string.Format("ErrorInfo: {0}\n", response.GetErrorInfo()) + " " + string.Format("ErrorMessage: {0}\n", response.GetErrorMessage()));
            //    }
            //}
            //catch (Exception ex)
            //{

            //    var messageException = telemetria.MakeMessageException(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            //    telemetria.Critical(messageException);
            //}
            return null;

        }

        private bool AddBody(MailjetClient client, long idCampaign, string message)
        {
            try
            {
                MailjetRequest request = new MailjetRequest
                {
                    Resource = CampaigndraftDetailcontent.Resource,
                    ResourceId = ResourceId.Numeric(idCampaign)
                }
               .Property(CampaigndraftDetailcontent.Htmlpart, message)
               .Property(CampaigndraftDetailcontent.Textpart, "");
                MailjetResponse response = Task.Run(() => client.PostAsync(request)).Result;
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    telemetria.Critical(string.Format("StatusCode: {0}\n", response.StatusCode) + " " + string.Format("ErrorInfo: {0}\n", response.GetErrorInfo()) + " " + string.Format("ErrorMessage: {0}\n", response.GetErrorMessage()));
                }
            }
            catch (Exception ex)
            {
                var messageException = telemetria.MakeMessageException(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }


            return false;
        }

        private bool SendCampaign(MailjetClient client, long idCampaign)
        {
            try
            {
                MailjetRequest request = new MailjetRequest
                {
                    Resource = CampaigndraftSend.Resource,
                    ResourceId = ResourceId.Numeric(idCampaign)
                };


                MailjetResponse response = Task.Run(() => client.PostAsync(request)).Result;
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    telemetria.Critical(string.Format("StatusCode: {0}\n", response.StatusCode) + " " + string.Format("ErrorInfo: {0}\n", response.GetErrorInfo()) + " " + string.Format("ErrorMessage: {0}\n", response.GetErrorMessage()));
                }
            }
            catch (Exception ex)
            {
                var messageException = telemetria.MakeMessageException(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }

            return false;

        }

        private MailJetSender GetSender(MailjetClient client)
        {
            try
            {
                MailjetRequest request = new MailjetRequest
                {
                    Resource = Sender.Resource,
                };

                MailjetResponse response = Task.Run(() => client.GetAsync(request)).Result;

                if (response.IsSuccessStatusCode)
                {
                    List<MailJetSender> listSender = JsonConvert.DeserializeObject<List<MailJetSender>>(response.GetData().ToString());
                    return listSender.FirstOrDefault();
                }
                else
                {
                    telemetria.Critical(string.Format("StatusCode: {0}\n", response.StatusCode) + " " + string.Format("ErrorInfo: {0}\n", response.GetErrorInfo()) + " " + string.Format("ErrorMessage: {0}\n", response.GetErrorMessage()));
                }
            }
            catch (Exception ex)
            {
                var messageException = telemetria.MakeMessageException(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }

            return null;
        }

        public MailJetProfile GetMyProfile(MailjetClient client)
        {
            try
            {
                MailjetRequest request = new MailjetRequest
                {
                    Resource = Myprofile.Resource,
                };

                MailjetResponse response = Task.Run(() => client.GetAsync(request)).Result;

                if (response.IsSuccessStatusCode)
                {
                    List<MailJetProfile> listSender = JsonConvert.DeserializeObject<List<MailJetProfile>>(response.GetData().ToString());
                    return listSender.FirstOrDefault();
                }
                else
                {
                    telemetria.Critical(string.Format("StatusCode: {0}\n", response.StatusCode) + " " + string.Format("ErrorInfo: {0}\n", response.GetErrorInfo()) + " " + string.Format("ErrorMessage: {0}\n", response.GetErrorMessage()));
                }
            }
            catch (Exception ex)
            {
                var messageException = telemetria.MakeMessageException(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            
            return null;
        }

        public List<MailJetListContacts> GetLists(string apiKey, string apiSecret)
        {
            try
            {
                MailjetClient client = new MailjetClient(apiKey, apiSecret);
                MailjetRequest request = new MailjetRequest
                {
                    Resource = Contactslist.Resource,
                }
                .Filter(Contact.ContactsList, "$ContactsListID");
                MailjetResponse response = Task.Run(() => client.GetAsync(request)).Result;
                if (response.IsSuccessStatusCode)
                {
                    List<MailJetListContacts> contactsList = JsonConvert.DeserializeObject<List<MailJetListContacts>>(response.GetData().ToString());
                    return contactsList;
                }
                else
                {
                    telemetria.Critical(string.Format("StatusCode: {0}\n", response.StatusCode) + " " + string.Format("ErrorInfo: {0}\n", response.GetErrorInfo()) + " " + string.Format("ErrorMessage: {0}\n", response.GetErrorMessage()));
                }
            }
            catch (Exception ex)
            {
                var messageException = telemetria.MakeMessageException(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }

            return null;

        }

        public List<MailJetSegment> GetSegments(string apiKey, string apiSecret)
        {
            try
            {
                MailjetClient client = new MailjetClient(apiKey,apiSecret);
                MailjetRequest request = new MailjetRequest
                {
                    Resource = Contactfilter.Resource,
                }
                .Filter(Contact.ContactsList, "$ContactsListID");
                MailjetResponse response = Task.Run(() => client.GetAsync(request)).Result;
                if (response.IsSuccessStatusCode)
                {
                    List<MailJetSegment> segmentsList = JsonConvert.DeserializeObject<List<MailJetSegment>>(response.GetData().ToString());
                    return segmentsList;
                }
                else
                {
                    telemetria.Critical(string.Format("StatusCode: {0}\n", response.StatusCode) + " " + string.Format("ErrorInfo: {0}\n", response.GetErrorInfo()) + " " + string.Format("ErrorMessage: {0}\n", response.GetErrorMessage()));
                }
            }
            catch (Exception ex)
            {
                var messageException = telemetria.MakeMessageException(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return null;
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

    public class MailJetSegment
    {
        public string Name { get; set; }
        public string Id { get; set; }
    }

    public class MailJetListContacts
    {
        public string Name { get; set; }
        public string Id { get; set; }
    }

    public class MailJetCampaign
    {
        public string Title { get; set; }
        public string Id { get; set; }
    }

    public class MailJetSender
    {
        public string Email { get; set; }
        public string Id { get; set; }
    }

    public class MailJetProfile
    {
        public string CompanyName { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Id { get; set; }
    }
}
