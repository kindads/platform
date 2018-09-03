using createsend_dotnet;
using KindAds.Azure;
using KindAds.Business.Partners.Mail;
using KindAds.Business.Partners.Push;
using KindAds.Common.Interfaces;
using KindAds.Common.Partners.Mail.SendinBlue;
using KindAds.Comun.Enums;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using KindAds.Comun.Models.KindAdsV2;
using KindAds.Negocio.Managersv2;
using KindAds.Negocio.PartnerServices;
using MailChimp;
using MailChimp.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using KindAds.Comun.Models;
using KindAds.Common.Utils.Partners.Mail.Aweber;

namespace KindAds.Negocio.ViewModels.KindAdsV2
{
    public class AudienceChannelViewModel : ITelemetria
    {
        public AudienceChannelDocument channel { set; get; }
        public List<QuestionAskToAudienceChannelDocument> listQuestions { set; get; }
        public AudienceChannelManager manager { set; get; }
        public string EmailProviderSelected { set; get; }
        public string PushProviderSelected { set; get; }
        public string WebSpaceProviderSelected { set; get; }

        public EmailProductForms emailProductForm { set; get; }

        public PushProductForms pushProductForm { set; get; }

        public bool IsValidApiKey { set; get; }

        public bool ValidationQuestion { set; get; }



        #region Properties from AudienceChannel
        public string question { set; get; }
        public ITrace telemetria { set; get; }
        #endregion

        public AudienceChannelViewModel()
        {
            EmailProviderSelected = string.Empty;
            PushProviderSelected = string.Empty;
            WebSpaceProviderSelected = string.Empty;
            ValidationQuestion = false;

            telemetria = new Trace();
            channel = new AudienceChannelDocument();
            listQuestions = new List<QuestionAskToAudienceChannelDocument>();
            manager = new AudienceChannelManager();
            emailProductForm = new EmailProductForms();
            pushProductForm = new PushProductForms();
            IsValidApiKey = false;
        }

        //Metodo para obtener todos los email providers
        public List<EmailProductProviderDocument> GetEmailProviders(string type)
        {
            List<EmailProductProviderDocument> emailProviders = new List<EmailProductProviderDocument>();
            try
            {
                if (type == "choose")
                {
                    emailProviders.Add(new EmailProductProviderDocument { Id = "", Name = "Choose email provider ..." });
                    emailProviders.AddRange(manager.GetEmailProviders());
                }
                else
                {
                    emailProviders.AddRange(manager.GetEmailProviders());
                }
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return emailProviders;
        }

        //Metodo para obtener todos los push providers
        public List<PushProductProviderDocument> GetPushProviders(string type)
        {
            List<PushProductProviderDocument> pushProviders = new List<PushProductProviderDocument>();
            try
            {
                if (type == "choose")
                {
                    pushProviders.Add(new PushProductProviderDocument { Id = "", Name = "Choose push provider ..." });
                    pushProviders.AddRange(manager.GetPushProviders());
                }
                else
                {
                    pushProviders.AddRange(manager.GetPushProviders());
                }
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return pushProviders;
        }

        #region metodos de Mailchimp
        public SelectList GetListFromMailChimp(string ApiToken)
        {
            SelectList result = null;
            if (ApiToken != string.Empty)
            {
                IMailChimpManager mailChimpManager = new MailChimp.MailChimpManager(ApiToken);
                var list = mailChimpManager.GetLists(null, 0, 100, "", "");
                result = new SelectList(list.Data, "Id", "Name");
            }
            return result;
        }

        public SelectList GetListsSendGrid(string apiKey)
        {
            List<GenericElement> list = new List<GenericElement>();
            GenericElement genericElement;
            try
            {
                string apiHost = "https://api.sendgrid.com/";
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
                        genericElement = new GenericElement()
                        {
                            Id = id.Value.ToString(),
                            Name = name.Value.ToString()
                        };
                        list.Add(genericElement);
                    }

                    return new SelectList(list, "Id", "Name");
                }
            }
            catch (Exception) { }

            return null;
        }

        public string GetSubscribersMailChimp(string ApiToken, string Id)
        {
            string subscribers = string.Empty;
            if (ApiToken != string.Empty)
            {
                IMailChimpManager mailChimpManager = new MailChimp.MailChimpManager(ApiToken);
                var list = mailChimpManager.GetLists(null, 0, 100, "", "");
                foreach (var item in list.Data)
                {
                    if (item.Id == Id)
                    {
                        subscribers = item.Stats.MemberCount.ToString();
                    }
                }
            }
            return subscribers;
        }

        public string GetSubscribersCampaignMonitor(string ApiToken, string ListId)
        {
            AuthenticationDetails auth = new ApiKeyAuthenticationDetails(ApiToken);
            List list = new List(auth, ListId);
            return list.Stats().TotalActiveSubscribers.ToString();
        }

        public SelectList GetTemplatesFromMailChimp(string ApiToken)
        {
            SelectList result = null;

            if (ApiToken != string.Empty)
            {
                IMailChimpManager mailChimpManager = new MailChimp.MailChimpManager(ApiToken);
                MailChimp.Templates.TemplateTypes tt = new MailChimp.Templates.TemplateTypes() { Base = true, Gallery = true, User = true };
                MailChimp.Templates.TemplateFilters tf = new MailChimp.Templates.TemplateFilters() { IncludeDragAndDrop = true };

                var list = mailChimpManager.GetTemplates(tt, tf);
                result = new SelectList(list.UserTemplates, "TemplateID", "Name");
            }
            return result;
        }

        public SelectList GetAppsFromOneSignal(string ApiToken)
        {
            SelectList result = null;
            System.Threading.Thread.Sleep(1000);
            OneSignalManager oneSignalManager = new OneSignalManager();
            var list = oneSignalManager.GetApps(ApiToken);
            result = new SelectList(list, "Id", "name");

            return result;
        }

        public SelectList GetFoldersFromSendinBlue(string ApiToken)
        {
            SelectList result = null;
            SendinBlueManager sendinBlueManager = new SendinBlueManager(ApiToken);
            List<Folder> folders = sendinBlueManager.GetAllFolders();
            result = new SelectList(folders, "id", "name");
            return result;
        }


        public SelectList GetListFromSendinBlue(string ApiToken, string FolderId)
        {
            SelectList result = null;
            int id = Convert.ToInt32(FolderId);
            SendinBlueManager sendinBlueManager = new SendinBlueManager(ApiToken);
            List<Folder> folders = sendinBlueManager.GetAllFolders();
            List<Lista> listas = (from folder in folders
                                  where folder.id == id
                                  select folder.Listas).FirstOrDefault();

            result = new SelectList(listas, "id", "name");
            return result;
        }

        public SelectList GetListFromGetResponse(string ApiToken)
        {
            SelectList result = null;
            GetResponseService service = new GetResponseService();
            var list = service.GetLists(ApiToken);
            result = new SelectList(list, "id", "name");
            return result;
        }

        public SelectList GetFromFieldsFromGetResponse(string ApiToken)
        {
            SelectList result = null;
            GetResponseService service = new GetResponseService();
            var list = service.GetFromFields(ApiToken);
            result = new SelectList(list, "id", "name");
            return result;
        }

        public SelectList GetListFromMailJet(string ApiToken, string SecretKey)
        {
            SelectList result = null;
            MailJetManager mailJetManager = new MailJetManager();
            var list = mailJetManager.GetLists(ApiToken, SecretKey);
            result = new SelectList(list, "Id", "Name");
            return result;
        }

        public SelectList GetListAweber(ProviderAWeberApiResult ApiToken)
        {
            API api = GetConectionAweber(ApiToken);
            var account = api.getAccount();
            List<GenericElement> list = new List<GenericElement>();
            GenericElement element;

            foreach (var e in account.lists().entries)
            {
                element = new GenericElement()
                {
                    Id = e.id.ToString(),
                    Name = e.name
                };
                list.Add(element);
            }
            return new SelectList(list, "Id", "Name");

        }

        private API GetConectionAweber(ProviderAWeberApiResult ApiToken)
        {
            // Create a new api instance
            API api = new API(ApiToken.ApplicationKey, ApiToken.ApplicationSecret);
            // Fill the tokens back from user session
            api.OAuthToken = ApiToken.OAuthToken;
            api.OAuthTokenSecret = ApiToken.OAuthTokenSecret;

            // Get the returned oauth_verifier
            api.OAuthVerifier = ApiToken.OauthVerifier;
            api.CallbackUrl = ApiToken.CallbackURL;
            return api;
        }

        public SelectList GetSegmentsFromMailJet(string ApiToken, string SecretKey)
        {
            SelectList result = null;
            MailJetManager mailJetManager = new MailJetManager();
            var list = mailJetManager.GetSegments(ApiToken, SecretKey);
            result = new SelectList(list, "Id", "Name");
            return result;
        }
        #endregion

        public string GetSubscribersSendGrid(string ApiToken, string ListId)
        {
            string subscribers = string.Empty;
            if (ApiToken != string.Empty)
            {
                string apiHost = "https://api.sendgrid.com/";
                var headers = new Dictionary<string, string> { { "X-Mock", "200" } };
                var sg = new SendGridClient(ApiToken, apiHost, headers);
                var response = sg.RequestAsync(method: SendGridClient.Method.GET, urlPath: "contactdb/lists/" + ListId + "/recipients").GetAwaiter().GetResult();
                var deserializeBody = response.DeserializeResponseBody(response.Body);

                foreach (var item in deserializeBody)
                {
                    if (item.Key.Equals("recipient_count"))
                    {
                        return (item.Value != null) ? item.Value.ToString() : "0";
                    }
                }
            }
            return subscribers;
        }

        public string GetSubscribersAweber(ProviderAWeberApiResult ApiToken, string ListId)
        {
            API api = GetConectionAweber(ApiToken);
            var account = api.getAccount();

            foreach (var e in account.lists().entries)
            {
                if (e.id.ToString().Equals(ListId)){
                    return e.total_subscribed_subscribers.ToString();
                }
            }

            return "0";

        }
    }
}
