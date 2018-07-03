using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Captivate.Common.Interfaces;
using Captivate.Business;
using Captivate.DataAccess;
using Mailjet;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Newtonsoft.Json;
using Captivate.Common.Models.Entities;
using Captivate.Azure;

namespace Captivate.Business.Partners.Mail
{

    public class MailJetManager
    {
        public ITrace telemetria { set; get; }
        public CampaignRepository CampaignRepository { set; get; }
        public ProductRepository ProductRepository { set; get; }
        public MailJetManager()
        {

            telemetria = new Trace();
            CampaignRepository = new CampaignRepository ();
            ProductRepository = new ProductRepository ();
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

        public string ValidateCampaign(string idCampaign)
        {
            try
            {
                CampaignEntity campaign = CampaignRepository.FindById(new Guid(idCampaign));
                string apiKey = GetProductSetting(campaign.PRODUCT.ProductSettingsEntitys, "mailJetApiKey");
                string secretKey = GetProductSetting(campaign.PRODUCT.ProductSettingsEntitys, "mailJetSecretKey");
                string listId = GetProductSetting(campaign.PRODUCT.ProductSettingsEntitys, "mailJetList");
                string segmentId = GetProductSetting(campaign.PRODUCT.ProductSettingsEntitys, "mailJetSegment");
                string subject = GetCampaignSetting(campaign.CAMPAIGN_SETTINGS, "mailJetSubject");

                MailjetClient client = new MailjetClient(apiKey, secretKey);

                var sender = GetSender(client);

                MailjetRequest request = new MailjetRequest
                {
                    Resource = Campaigndraft.Resource,
                }
                   .Property(Campaigndraft.Locale, "en_US")
                   .Property(Campaigndraft.Sender, GetMyProfile(client).CompanyName)
                   .Property(Campaigndraft.SenderEmail, sender.Email)
                   .Property(Campaigndraft.Subject, subject)
                   .Property("ContactsListID", listId)
                   .Property(Campaigndraft.Title, campaign.Name)
                   .Property("SegmentationID", segmentId);

                MailjetResponse response = Task.Run(() => client.PostAsync(request)).Result;
                if (response.IsSuccessStatusCode)
                {
                    List<MailJetCampaign> listCampaigncampaign = JsonConvert.DeserializeObject<List<MailJetCampaign>>(response.GetData().ToString());
                    var campaignMailJet = listCampaigncampaign.FirstOrDefault();
                    bool actionAddBody = AddBody(client, Convert.ToInt64(campaignMailJet.Id), campaign.AdText);
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
