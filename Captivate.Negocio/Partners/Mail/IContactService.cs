using Captivate.Business;
using Captivate.Common.Interfaces;
using Captivate.Comun.Interfaces;
using Captivate.Comun.Models;
using Captivate.Comun.Models.Entities;
using Captivate.Negocio.Communication;
using Captivate.Negocio.Helper;
using Captivate.Negocio.Partners.IContact;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Captivate.Negocio.Partners.Mail
{
    public class IContacLogic<TCampaign, TR> : ITelemetria, IIContactService<TCampaign> where TCampaign : ICampaign where TR : IResponse, new()
    {
        #region properties
        public ITrace telemetria { set; get; }
        public TCampaign campaign { set; get; }
        //private Services.CampaignService campaignService;
        public RequestManager<IIContactRequest> requestManager { set; get; }
        public ResponseManager<TR> responseManager { set; get; }
        public IRequestSettings<IIContactRequest> config { set; get; }
        public IIContactRequest mailingProvider { set; get; }
        #endregion

        public IContacLogic()
        {
            telemetria = new Trace();

            //campaignService = new CampaignService();
            requestManager = new RequestManager<IIContactRequest>();
            responseManager = new ResponseManager<TR>();
            config = new Captivate.Comun.Models.RequestSettings<IIContactRequest>();
            mailingProvider = new IContactRequest(ProviderEnvironment.Production);
            config.mailingProvider = mailingProvider;
        }

        #region POST methods
        public IResponse CreateCampaign(IContactPostCampaignRequest requestBody, IContactRequest requestFrm)
        {
            IResponse response = new IContactPostCampaignsResponse();

            try
            {
                config.mailingProvider = requestFrm;
                IContactPostCampaignRequest[] requestArrayBody = new IContactPostCampaignRequest[] { requestBody };
                config.data = new JavaScriptSerializer().Serialize(requestArrayBody);
                HttpWebRequest request = GetRequest(Captivate.Comun.Utils.IContactRequest.AddCampaign, config, requestFrm);
                response = GetResponse(request);
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return response;
        }

        public static string CleanStripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }

        public string GetScheduleTime(DateTime? StartDate)
        {
            DateTime datetime = (DateTime)(StartDate == null ? DateTime.Now : StartDate);
            string startDateFormatted = string.Empty;
            var timeStamp = GetTimeStamp(datetime);
            return timeStamp;
        }

        public string GetTimeStamp(DateTime dateTime)
        {
            string timeStamp = string.Empty;

            try
            {
                string year = dateTime.Year.ToString();
                string month = dateTime.Month.ToString();
                string day = dateTime.Day.ToString();

                if (year == DateTime.Now.Year.ToString() && month == DateTime.Now.Month.ToString() && day == DateTime.Now.Day.ToString())
                {
                    timeStamp = string.Empty;
                }
                else
                {
                    timeStamp = DateTimeHelper.GetTimestamp(dateTime);
                }
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }

            return timeStamp;
        }

        public IContactRequest FillIContactRequest(ICollection<ProductSettingsEntity> settings)
        {
            IContactRequest request = new IContactRequest(ProviderEnvironment.Production);
            foreach (var item in settings)
            {

                switch (item.SettingName)
                {
                    case "icontactAccountId":
                        {
                            request.AccountId = item.SettingName.Equals("icontactAccountId") ? item.SettingValue : string.Empty;
                        }
                        break;
                    case "icontactApiAppId":
                        {
                            request.ApiAppId = item.SettingName.Equals("icontactApiAppId") ? item.SettingValue : string.Empty;
                        }
                        break;
                    case "icontactUserName":
                        {
                            request.ApiUserName = item.SettingName.Equals("icontactUserName") ? item.SettingValue : string.Empty;
                        }
                        break;
                    case "icontactUserPassword":
                        {
                            request.ApiUserPassword = item.SettingName.Equals("icontactUserPassword") ? item.SettingValue : string.Empty;
                        }
                        break;
                    case "icontactClientFolderId":
                        {
                            request.ClientFolderId = item.SettingName.Equals("icontactClientFolderId") ? item.SettingValue : string.Empty;
                        }
                        break;
                    case "icontactIdCampaign":
                        {
                            request.IdCampaign = item.SettingName.Equals("icontactIdCampaign") ? item.SettingValue : string.Empty;
                        }
                        break;
                    case "icontactIdList":
                        {
                            request.ListId = item.SettingName.Equals("icontactIdList") ? item.SettingValue : string.Empty;
                        }
                        break;
                }
            }

            return request;
        }

        public IResponse CreateSends(IContactPostSendsRequest requestBody, IContactRequest requestFrm)
        {
            config.mailingProvider = requestFrm;
            IResponse response = new IContactPostSendsResponse();

            try
            {
                IContactPostSendsRequestWithoutScheduledTime requestBodyWS = new IContactPostSendsRequestWithoutScheduledTime()
                {
                    messageId = requestBody.messageId,
                    includeListIds = requestBody.includeListIds
                };
                IContactPostSendsRequest[] requestArrayBody = new IContactPostSendsRequest[] { requestBody };
                IContactPostSendsRequestWithoutScheduledTime[] requestArrayBodyWithoutScheduleTime = new IContactPostSendsRequestWithoutScheduledTime[] { requestBodyWS };

                //Checamos si tiene fecha
                if (requestBody.scheduledTime == string.Empty)
                {
                    var jsonResolver = new PropertyRenameAndIgnoreSerializerContractResolver();
                    jsonResolver.IgnoreProperty(typeof(IContactPostSendsRequestWithoutScheduledTime), "scheduledTime");
                    jsonResolver.IgnoreProperty(typeof(IContactPostSendsRequestWithoutScheduledTime), "BaseUrl");
                    jsonResolver.IgnoreProperty(typeof(IContactPostSendsRequestWithoutScheduledTime), "scheduled");
                    var serializerSettings = new JsonSerializerSettings();
                    serializerSettings.ContractResolver = jsonResolver;
                    config.data = JsonConvert.SerializeObject(requestArrayBodyWithoutScheduleTime, serializerSettings);
                }
                else
                {
                    IContactPostSendsRequestWithoutScheduledTime[] requestArrayBodyWS = new IContactPostSendsRequestWithoutScheduledTime[] { requestBodyWS };
                    config.data = new JavaScriptSerializer().Serialize(requestArrayBodyWS);
                }

                //Realiza la peticion
                HttpWebRequest request = GetRequest(Captivate.Comun.Utils.IContactRequest.AddSends, config, requestFrm);
                response = GetResponse(request);
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }

            return response;
        }

        public IResponse CreateMessage(IContactPostMessageRequest requestBody, IContactRequest requestFrm)
        {
            config.mailingProvider = requestFrm;
            IResponse response = new IContactPostMessagesResponse();
            try
            {
                IContactPostMessageRequest[] requestArrayBody = new IContactPostMessageRequest[] { requestBody };
                config.data = new JavaScriptSerializer().Serialize(requestArrayBody);
                HttpWebRequest request = GetRequest(Captivate.Comun.Utils.IContactRequest.AddMessage, config, requestFrm);
                response = GetResponse(request);
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return response;
        }

        public Dictionary<string, string> CreateMessageDictionary(IContactPostMessageRequest requestBody)
        {
            Dictionary<string, string> messageDictionary = new Dictionary<string, string>();
            try
            {
                messageDictionary.Add("campaignId", requestBody.campaignId.ToString());
                messageDictionary.Add("messageType", requestBody.messageType.ToString());
                messageDictionary.Add("subject", requestBody.subject);
                messageDictionary.Add("textBody", requestBody.textBody);
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return messageDictionary;
        }

        public IResponse CreateMessageToHttpClient(IContactPostMessageRequest requestBody, IContactRequest requestFrm)
        {
            config.mailingProvider = requestFrm;
            IResponse response = new IContactPostMessagesResponse();

            try
            {
                IContactPostMessageRequest[] requestArrayBody = new IContactPostMessageRequest[] { requestBody };
                config.data = new JavaScriptSerializer().Serialize(requestArrayBody);
                Dictionary<string, string> messageDictionary = CreateMessageDictionary(requestBody);
                string uri = requestManager.GetUriRequest(Captivate.Comun.Utils.IContactRequest.AddMessage, config, requestFrm);
                HttpClient client = requestManager.GetHttpClient(config, Captivate.Comun.Utils.IContactRequest.AddMessage);
                response = GetResponseHttpClient(client, requestArrayBody, messageDictionary, uri);
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return response;
        }
        #endregion

        #region GET methods
        public IResponse GetCampaign(string Id)
        {
            IResponse response = new IContactGetCampaignResponse();
            //todo
            return response;
        }

        public IResponse GetSends(string Id)
        {

            IResponse response = new IContactGetSendsResponse();
            //todo
            return response;
        }

        public IResponse GetMessage(string Id)
        {

            IResponse response = new IContactGetMessageResponse();
            //Todo
            return response;
        }


        public IResponse GetLists(IContactRequest requestFrm)
        {
            config.mailingProvider = requestFrm;
            IResponse response = new IContactGetListsResponse();
            try
            {
                HttpWebRequest request = GetRequest(Captivate.Comun.Utils.IContactRequest.GetLists, config, requestFrm);
                response = GetResponse(request);
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return response;
        }

        public IResponse GetList(string Id, IContactRequest requestFrm)
        {
            config.mailingProvider = requestFrm;
            IResponse response = new IContactGetListResponse();
            HttpWebRequest request = GetRequest(Captivate.Comun.Utils.IContactRequest.GetLists, config, requestFrm);
            return response;
        }

        #endregion





        public IValidateProvider ValidateApiToken(IContactRequest requestFrm)
        {
            IValidateProvider validateProvider = new ValidateProvider(EnumMailProviders.IContact);

            bool resultado = false;
            try
            {
                config.mailingProvider = requestFrm;
                IResponse response = new IContactGetListsResponse();
                HttpWebRequest request = GetRequest(Captivate.Comun.Utils.IContactRequest.GetLists, config, requestFrm);
                response = GetResponse(request);
                resultado = response.StatusCode == 0 ? true : false;
                validateProvider.Validate(response.StatusCode == 0 ? true : false, response.StatusCode);
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }

            return validateProvider;
        }

        public HttpWebRequest GetRequest(Captivate.Comun.Utils.IContactRequest type, IRequestSettings<IIContactRequest> config, IContactRequest requestFrm)
        {
            HttpWebRequest request = requestManager.GetWebRequest(type, config, requestFrm);
            return request;
        }

        public IResponse GetResponse(HttpWebRequest request)
        {
            IResponse response = new IContactResponse();
            response = responseManager.GetResponse(request);
            return response;
        }

        public IResponse GetResponseHttpClient(HttpClient client, IContactPostMessageRequest[] requestBody, Dictionary<string, string> values, string uri)
        {
            IResponse response = new IContactResponse();
            response = responseManager.GetResponseFromHttpClient(client, requestBody, values, uri).Result;
            return response;
        }


    }
}
