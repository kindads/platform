using KindAds.Azure;
using KindAds.Business;
using KindAds.Common.Interfaces;
using KindAds.Common.Models.Entities;
using KindAds.Common.Partners.Mail.SendinBlue;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using KindAds.DataAccess;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace KindAds.Business.Partners.Mail
{
    public class SendinBlueManager : ITelemetria
    {
        #region properties
        //public CampaignRepository CampaignRepository { set; get; }
        //public ProductRepository ProductRepository { set; get; }

        public string ApiKey { set; get; }
        public ITrace telemetria { set; get; }
        public NotificationManager notificationManager { set; get; }

        public IList<AudiencePropertieSetting> settings { set; get; }
        #endregion

        #region Version 2


        public SendinBlueManager(IList<AudiencePropertieSetting> settings)
        {
            this.settings = settings;
            telemetria = new Trace();
        }

        private bool ApiTolenIsValid()
        {
            bool result = false;
            try
            {
                string key = GetSettingFromCosmos("Key");
                APISendingBlue sendinBlue = new APISendingBlue(key);
                dynamic account = sendinBlue.get_account();
                if (account.code == "success")
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

        private bool FolderIsValid()
        {
            bool result = false;
            try
            {
                string key = GetSettingFromCosmos("Key");
                APISendingBlue sendinBlue = new APISendingBlue(key);
                Dictionary<string, int> foldersData = new Dictionary<string, int>();
                Dictionary<string, int> folderData = new Dictionary<string, int>();
                foldersData.Add("page", 1);
                foldersData.Add("page_limit", 1);

                //Todo
                dynamic folders = sendinBlue.get_folders(foldersData);

                // get result
                if (folders.code == "success")
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
            try
            {
                string key = GetSettingFromCosmos("Key");
                APISendingBlue sendinBlue = new APISendingBlue(key);
                int IdList = Convert.ToInt32(GetSettingFromCosmos("List"));
                Dictionary<string, int> listData = new Dictionary<string, int>();
                listData.Add("id", IdList);
                dynamic list = sendinBlue.get_list(listData);
                // get result
                if (list.code == "success")
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

        public bool SettingsAreValid()
        {
            bool result = false;
            result = (ApiTolenIsValid() && FolderIsValid() && ListIsValid()) ? true : false;
            return result;
        }

        public string GetSettingFromCosmos(string settingName)
        {
            string value = string.Empty;
            switch (settingName)
            {
                case "Key":
                    {
                        foreach (var setting in settings)
                        {
                            if (setting.Name == "sendinBlueApiKey")
                            {
                                value = setting.Value;
                            }
                        }
                    }
                    break;
                case "Folder":
                    {
                        foreach (var setting in settings)
                        {
                            if (setting.Name == "sendinBlueFolder")
                            {
                                value = setting.Value;
                            }
                        }
                    }
                    break;
                case "List":
                    {
                        foreach (var setting in settings)
                        {
                            if (setting.Name == "sendinBlueListId")
                            {
                                value = setting.Value;
                            }
                        }
                    }
                    break;
            }
            return value;
        }

        #endregion

        #region validation settings methods

        private string GetSetting(string settingName, string IdCampaign)
        {
            string value = string.Empty;
            ProductSettingsRepository repository = new ProductSettingsRepository();
            CampaignRepository cmprepository = new CampaignRepository();
            Guid IdProduct = (from cam in cmprepository.GetAll() where cam.IdCampaign == new Guid(IdCampaign) select cam.PRODUCT_IdProduct).FirstOrDefault();
            List<ProductSettingsEntity> settings = repository.GetProductSettingsByIdProduct(IdProduct);

            switch (settingName)
            {
                case "Key":
                    {
                        foreach (var setting in settings)
                        {
                            if (setting.SettingName == "sendinBlueApiKey")
                            {
                                value = setting.SettingValue;
                            }
                        }
                    }
                    break;
                case "Folder":
                    {
                        foreach (var setting in settings)
                        {
                            if (setting.SettingName == "sendinBlueFolder")
                            {
                                value = setting.SettingValue;
                            }
                        }
                    }
                    break;
                case "List":
                    {
                        foreach (var setting in settings)
                        {
                            if (setting.SettingName == "sendinBlueListId")
                            {
                                value = setting.SettingValue;
                            }
                        }
                    }
                    break;
            }
            return value;
        }

        private bool ApiTolenIsValid(string IdCampaign)
        {
            bool result = false;
            try
            {
                string key = GetSetting("Key", IdCampaign);
                APISendingBlue sendinBlue = new APISendingBlue(key);
                dynamic account = sendinBlue.get_account();
                if (account.code == "success")
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

        private bool FolderIsValid(string IdCampaign)
        {
            bool result = false;
            try
            {
                string key = GetSetting("Key", IdCampaign);
                APISendingBlue sendinBlue = new APISendingBlue(key);
                Dictionary<string, int> foldersData = new Dictionary<string, int>();
                Dictionary<string, int> folderData = new Dictionary<string, int>();
                foldersData.Add("page", 1);
                foldersData.Add("page_limit", 1);

                //Todo
                dynamic folders = sendinBlue.get_folders(foldersData);

                // get result
                if (folders.code == "success")
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

        private bool ListIsValid(string IdCampaign)
        {
            bool result = false;
            try
            {
                string key = GetSetting("Key", IdCampaign);
                APISendingBlue sendinBlue = new APISendingBlue(key);
                int IdList = Convert.ToInt32(GetSetting("List", IdCampaign));
                Dictionary<string, int> listData = new Dictionary<string, int>();
                listData.Add("id", IdList);
                dynamic list = sendinBlue.get_list(listData);
                // get result
                if (list.code == "success")
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
        #endregion

        #region Version 1
        [Obsolete]
        public SendinBlueManager()
        {
            telemetria = new Trace();
            //CampaignRepository = new CampaignRepository();
            //ProductRepository = new ProductRepository();
            ApiKey = string.Empty;
            notificationManager = new NotificationManager();
        }

        [Obsolete]
        public SendinBlueManager(string ApiKey)
        {

            telemetria = new Trace();
            //CampaignRepository = new CampaignRepository();
            //ProductRepository = new ProductRepository();
            this.ApiKey = string.Empty;
            this.ApiKey = ApiKey;
            notificationManager = new NotificationManager();
        }

        public void GetAllLists(string IdCampaign,int IdFolder)
        {
            try
            {
                if (ApiKey != string.Empty)
                {
                    APISendingBlue sendinBlue = new APISendingBlue(ApiKey);
                    Dictionary<string, int> data = new Dictionary<string, int>();
                    data.Add("list_parent", 1);
                    data.Add("page", 1);
                    data.Add("page_limit", 10);
                    Object getLists = sendinBlue.get_lists(data);
                    Console.WriteLine(getLists);
                }
            }
            catch (Exception e)
            {
                string exceptionMessage = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(exceptionMessage);
            }           
        }

        public List<Folder> GetAllFolders()
        {
            List<Folder> folders = new List<Folder>{ };

            try
            {
                if (ApiKey != string.Empty)
                {
                    APISendingBlue sendinBlue = new APISendingBlue(ApiKey);
                    Dictionary<string, int> data = new Dictionary<string, int>();
                    data.Add("page", 1);
                    data.Add("page_limit", 10);

                    dynamic foldersResponse = sendinBlue.get_folders(data);
                    if(foldersResponse.code== "success")
                    {
                        folders = foldersResponse.data.folders.ToObject<List<Folder>>();
                        for(var i=0; i<=(folders.Count-1);i++)
                        {
                            folders[i].Listas=folders[i].lists.ToObject<List<Lista>>();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string exceptionMessage = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(exceptionMessage);
            }
            return folders.ToList();
        }

        public string GetAccountEmail()
        {
            string email = string.Empty;
            try
            {
                if (ApiKey != string.Empty)
                {
                    APISendingBlue sendinBlue = new APISendingBlue(ApiKey);
                    dynamic authentication = sendinBlue.get_account();
                    if (authentication.code == "success")
                    {
                        email = authentication.data[2].email;
                    }
                }
            }
            catch (Exception e)
            {
                string exceptionMessage = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(exceptionMessage);
            }
            return email;
        }

        public bool ValidateApiKey()
        {
            bool result = false;
            try
            {
                if (ApiKey != string.Empty)
                {
                    APISendingBlue sendinBlue = new APISendingBlue(ApiKey);
                    dynamic authentication = sendinBlue.get_account();
                    if(authentication.code== "success")
                    {
                        result = true;
                    }
                }
            }
            catch (Exception e)
            {
                string exceptionMessage = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(exceptionMessage);
            }
            return result;
        }

        public bool ValidateApiKey(string Key)
        {
            bool result = false;
            try
            {
                if (Key != string.Empty)
                {
                    APISendingBlue sendinBlue = new APISendingBlue(Key);
                    dynamic authentication = sendinBlue.get_account();
                    if (authentication.code == "success")
                    {
                        result = true;
                    }
                }
            }
            catch (Exception e)
            {
                string exceptionMessage = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(exceptionMessage);
            }
            return result;
        }

        [System.Obsolete("Este método ya no esta en uso favor de usar CreateCampaign(SendinBlueCampaignRequest request)")]
        public void CreateCampaignTest()
        {
            if (ApiKey != string.Empty)
            {
                APISendingBlue sendinBlue = new APISendingBlue(ApiKey);
                Dictionary<string, Object> data = new Dictionary<string, Object>();
                List<int> listid = new List<int>();
                listid.Add(2);
                data.Add("category", "My category");
                data.Add("from_name", "[DEFAULT_FROM_NAME]");
                data.Add("name", "My Campaign 1");
                data.Add("bat", "");
                data.Add("html_content", "<html><body><pre> Corramos con el peje este 4 de julio a festejar a su depa </pre></body></html>");
                data.Add("html_url", "");
                data.Add("listid", listid);
                data.Add("scheduled_date", "2018-05-16 16:05:01");
                data.Add("subject", "My Subject");
                data.Add("from_email", "angel.alvarado@blockbliss.com");
                data.Add("reply_to", "[DEFAULT_REPLY_TO]");
                data.Add("to_field", "[PRENOM] [NOM]");
                data.Add("exclude_list", new List<int>());
                data.Add("attachment_url", "");
                data.Add("inline_image", 1);
                data.Add("mirror_active", 0);
                data.Add("send_now", 0);
                data.Add("utm_campaign", "My UTM Value1");

                Object createCampaign = sendinBlue.create_campaign(data);
                Console.WriteLine(createCampaign);
            }
        }

        public SendinBlueCampaignRequest FillSendinBlueRequestFromCosmos()
        {
            SendinBlueCampaignRequest request = new SendinBlueCampaignRequest();

            try
            {
                if (this.settings.Count > 0)
                {
                    foreach (var item in settings)
                    {

                        switch (item.Name)
                        {
                            case "sendinBlueApiKey":
                                {
                                    request.ApiKey = item.Name.Equals("sendinBlueApiKey") ? item.Value : string.Empty;
                                }
                                break;
                            case "sendinBlueCategory":
                                {
                                    request.Category = item.Name.Equals("sendinBlueCategory") ? item.Value : string.Empty;
                                }
                                break;
                            case "sendinBlueFromEmail":
                                {
                                    SendinBlueManager manager = new SendinBlueManager(request.ApiKey);
                                    request.FromEmail = manager.GetAccountEmail();
                                }
                                break;
                            case "sendinBlueListId":
                                {
                                    string listId = item.Name.Equals("sendinBlueListId") ? item.Value : string.Empty;
                                    request.ListIds = new List<int> { Convert.ToInt32(listId) };
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

        public SendinBlueCampaignRequest FillSendinBlueRequest(ICollection<ProductSettingsEntity> settings)
        {
            SendinBlueCampaignRequest request = new SendinBlueCampaignRequest();
            
            foreach (var item in settings)
            {

                switch (item.SettingName)
                {
                    case "sendinBlueApiKey":
                        {
                            request.ApiKey= item.SettingName.Equals("sendinBlueApiKey") ? item.SettingValue : string.Empty;
                        }
                        break;
                    case "sendinBlueCategory":
                        {
                            request.Category= item.SettingName.Equals("sendinBlueCategory") ? item.SettingValue : string.Empty;
                        }
                        break;
                    case "sendinBlueFromEmail":
                        {
                            SendinBlueManager manager = new SendinBlueManager(request.ApiKey);
                            request.FromEmail = manager.GetAccountEmail();
                        }
                        break;
                    case "sendinBlueListId":
                        {
                            string listId= item.SettingName.Equals("sendinBlueListId") ? item.SettingValue : string.Empty;
                            request.ListIds=new List<int> { Convert.ToInt32(listId) };
                        }break;                   
                }
            }
            return request;
        }

        public bool SettingsAreValid(string IdCampaign)
        {
            bool result = false;
            result = (ApiTolenIsValid(IdCampaign) && FolderIsValid(IdCampaign) && ListIsValid(IdCampaign)) ? true : false;
            return result;
        }


        public SendinBlueCampaignRequest getRequestData(CampaignDocument campaign)
        {
            SendinBlueCampaignRequest request = FillSendinBlueRequestFromCosmos();
            request.Subject = campaign.Name;
            request.HtmlContent = campaign.Text;
            request.Schedule = campaign.StartDate == null ? DateTime.Now : CheckStartDate(Convert.ToDateTime(campaign.StartDate));
            request.Name = campaign.Name;
            return request;
        }


        public string SendCampaign(SendinBlueCampaignRequest request)
        {
            string Id = string.Empty;
            try
            {
                Id = CreateCampaign(request);
            }
            catch (Exception e)
            {
                string exceptionMessage = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(exceptionMessage);
            }
            return Id;
        }

        [Obsolete]
        public  string SendCampaign(string IdCampaig)
        {
            //string Id = string.Empty;
            //CampaignManager campaignManager = new CampaignManager();
            //NotificationManager notificationManager = new NotificationManager();

            ////Obtenemos los datos
            //CampaignEntity campaign = CampaignRepository.FindById(new Guid(IdCampaig));
            //ProductEntity product = ProductRepository.FindById(campaign.PRODUCT_IdProduct);

            ////Creamos el objecto de peticion
            //SendinBlueCampaignRequest request = FillSendinBlueRequest(product.ProductSettingsEntitys);
            //request.Subject = campaign.Name;
            //request.HtmlContent = campaign.AdText;
            //request.Schedule = campaign.StartDate == null ? DateTime.Now: CheckStartDate((DateTime)campaign.StartDate);
            //request.Name = campaign.Name;

            ////Creamos la campaña
            ////Id = CreateCampaign(request, campaign);
            //Id = CreateCampaign(request);
            //campaign.IdCampaign3rdParty = Id;

            //return Id;

            return null;
        }

        public DateTime CheckStartDate(DateTime startDate)
        {
            DateTime now = DateTime.Now;

            int yearNow = now.Year;
            int monthNow = now.Month;
            int dayNow = now.Day;
            int hourNow = now.Hour;
            int minutesNow = now.Minute;

            int yearStartDate = startDate.Year;
            int monthStartDate = startDate.Month;
            int dayStartDate = startDate.Day;
            int hourStartNow = startDate.Hour;
            int minuteStartNow = startDate.Minute;

            int difYear = yearStartDate - yearNow;
            int difMonth = monthStartDate - monthNow;
            int difDay = dayStartDate - dayNow;
            int difHour = hourStartNow - hourNow;
            int difMinutes = minuteStartNow-minutesNow;

            if( difYear==0 && difMonth==0 && difDay==0 && difHour<0 )
            {
                now=now.AddMinutes(5);
                return now;
            }
            else if(difYear<0 || difMonth<0 || difDay<0 || difDay<0 )
            {
                return startDate;
            }
            else
            {
                return startDate;
            }
        }

        public string CreateCampaign(SendinBlueCampaignRequest request)
        {
            string IdCampaign = string.Empty;
            try
            {
                ApiKey = request.ApiKey;

                if (ApiKey != string.Empty)
                {
                    APISendingBlue sendinBlue = new APISendingBlue(ApiKey);
                    var data = request.GetSendinBlueCampaignRequestObject();
                    dynamic createCampaignResponse = sendinBlue.create_campaign(data);
                    if (createCampaignResponse.code == "success")
                    {
                        IdCampaign = createCampaignResponse.data.id;
                    }
                    else
                    {
                        SendinBlueResponse wrong = new SendinBlueResponse();
                        wrong = createCampaignResponse;
                        string error = wrong.message;

                        string messageError = string.Format("Something goes wrong, detail:{0}", error);
                    }
                }
            }
            catch (Exception e)
            {
                string exceptionMessage = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(exceptionMessage);
            }           
            return IdCampaign;
        }

        #endregion
    }

    public class APISendingBlue
    {
        public string base_url = "https://api.sendinblue.com/v2.0/";
        public string accessId = "";
        public int timeout;

        public APISendingBlue(string accessId)
        {
            this.accessId = accessId;
            this.timeout = 30000; //default timeout: 30 secs
        }
        public APISendingBlue(string accessId, int timeout)
        {
            this.accessId = accessId;
            this.timeout = timeout;
        }
        private dynamic auth_call(string resource, string method, string content)
        {
            Stream stream = new MemoryStream();
            string url = base_url + resource;
            string content_type = "application/json";
            // Create request

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            try
            {
                if (timeout != null && (timeout <= 0 || timeout > 60000))
                {
                    throw new Exception("value not allowed for timeout");
                }
            }
            catch (System.Net.WebException ex)
            {
                stream = ex.Response.GetResponseStream() as Stream;
            }

            // Set method
            request.Method = method;
            request.ContentType = content_type;
            request.Timeout = timeout;
            request.Headers.Add("api-key", accessId);

            if (method == "POST" || method == "PUT")
            {
                using (System.IO.Stream s = request.GetRequestStream())
                {
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(s))
                        sw.Write(content);
                }
            }
            try
            {
                HttpWebResponse response;
                response = request.GetResponse() as HttpWebResponse;
                // read the response stream and put it into a byte array
                stream = response.GetResponseStream() as Stream;
            }
            catch (System.Net.WebException ex)
            {
                if (ex.Response == null)
                {
                    throw new Exception("Request failed");
                }
                // read the response stream If status code is other than 200 and put it into a byte array
                stream = ex.Response.GetResponseStream() as Stream;
            }

            byte[] buffer = new byte[32 * 1024];
            int nRead = 0;
            MemoryStream ms = new MemoryStream();
            do
            {
                nRead = stream.Read(buffer, 0, buffer.Length);
                ms.Write(buffer, 0, nRead);
            } while (nRead > 0);
            // convert read bytes into string
            ASCIIEncoding encoding = new ASCIIEncoding();
            String responseString = encoding.GetString(ms.ToArray());
            if (responseString == "")
            {
                throw new Exception("Unable to read response");
            }
            return JObject.Parse(responseString);
        }
        private dynamic get_request(string resource, string content)
        {
            return auth_call(resource, "GET", "");
        }
        private dynamic post_request(string resource, string content)
        {
            return auth_call(resource, "POST", content);
        }
        private dynamic delete_request(string resource, string content)
        {
            return auth_call(resource, "DELETE", "");
        }
        private dynamic put_request(string resource, string content)
        {
            return auth_call(resource, "PUT", content);
        }

        /*
            Get SMTP details.
            No input required
        */
        public dynamic get_account()
        {
            return get_request("account", "");
        }

        /*
            Get SMTP details.
            No input required
        */
        public dynamic get_smtp_details()
        {
            return get_request("account/smtpdetail", "");
        }

        public dynamic get_reseller_child(Object data)
        {
            return post_request("account/getchildv2", JsonConvert.SerializeObject(data));
        }

        public dynamic add_remove_child_credits(Object data)
        {
            return post_request("account/addrmvcredit", JsonConvert.SerializeObject(data));
        }

        public dynamic delete_child_account(Dictionary<string, string> data)
        {
            String child_authkey = data["auth_key"];
            return delete_request("account/" + child_authkey, "");
        }

        public dynamic create_child_account(Object data)
        {
            return post_request("account", JsonConvert.SerializeObject(data));
        }

       
        public dynamic update_child_account(Object data)
        {
            return post_request("account", JsonConvert.SerializeObject(data));
        }

       
        public dynamic get_campaigns_v2(Dictionary<string, Object> data)
        {
            string type = data["type"].ToString(); string status = data["status"].ToString(); string page = data["page"].ToString(); string page_limit = data["page_limit"].ToString();
            string url = "";
            if (type == "" && status == "" && page == "" && page_limit == "")
            {
                url = "campaign/detailsv2/";
            }
            else
            {
                url = "campaign/detailsv2/type/" + type + "/status/" + status + "/page/" + page + "/page_limit/" + page_limit + "/";
            }
            return get_request(url, "");
        }

      
        public dynamic get_campaign_v2(Dictionary<string, int> data)
        {
            string id = data["id"].ToString();
            return get_request("campaign/" + id + "/detailsv2/", "");
        }

       
        public dynamic create_campaign(Object data)
        {
            return post_request("campaign", JsonConvert.SerializeObject(data));
        }

       
        public dynamic delete_campaign(Dictionary<string, int> data)
        {
            string id = data["id"].ToString();
            return delete_request("campaign/" + id, "");
        }

       
        public dynamic update_campaign(Dictionary<string, Object> data)
        {
            string id = data["id"].ToString();
            return put_request("campaign/" + id, JsonConvert.SerializeObject(data));
        }

       
        public dynamic campaign_report_email(Dictionary<string, Object> data)
        {
            string id = data["id"].ToString();
            return post_request("campaign/" + id + "/report", JsonConvert.SerializeObject(data));
        }

       
        public dynamic campaign_recipients_export(Dictionary<string, Object> data)
        {
            string id = data["id"].ToString();
            return post_request("campaign/" + id + "/recipients", JsonConvert.SerializeObject(data));
        }

       
        public dynamic send_bat_email(Dictionary<string, Object> data)
        {
            string id = data["id"].ToString();
            return put_request("campaign/" + id + "/test", JsonConvert.SerializeObject(data));
        }

       
        public dynamic create_trigger_campaign(Object data)
        {
            return post_request("campaign", JsonConvert.SerializeObject(data));
        }

        
        public dynamic update_trigger_campaign(Dictionary<string, Object> data)
        {
            string id = data["id"].ToString();
            return put_request("campaign/" + id, JsonConvert.SerializeObject(data));
        }

       
        public dynamic share_campaign(Object data)
        {
            return post_request("campaign/sharelinkv2", JsonConvert.SerializeObject(data));
        }

      
        public dynamic update_campaign_status(Dictionary<string, Object> data)
        {
            string id = data["id"].ToString();
            return put_request("campaign/" + id + "/updatecampstatus", JsonConvert.SerializeObject(data));
        }

       
        public dynamic get_processes(Dictionary<string, int> data)
        {
            string page = data["page"].ToString(); string page_limit = data["page_limit"].ToString();
            String url = "page/" + page + "/page_limit/" + page_limit;
            return get_request("process/index/" + url, "");
        }

       
        public dynamic get_process(Dictionary<string, int> data)
        {
            string id = data["id"].ToString();
            return get_request("process/" + id, "");
        }

        public dynamic get_lists(Dictionary<string, int> data)
        {
            string list_parent = data["list_parent"].ToString(); string page = data["page"].ToString(); string page_limit = data["page_limit"].ToString();
            String url = "page/" + page + "/page_limit/" + page_limit + "/list_parent/" + list_parent;
            return get_request("list/index/" + url, "");
        }

     
        public dynamic get_list(Dictionary<string, int> data)
        {
            string id = data["id"].ToString();
            return get_request("list/" + id, "");
        }

       
        public dynamic create_list(Object data)
        {
            return post_request("list", JsonConvert.SerializeObject(data));
        }

      
        public dynamic delete_list(Dictionary<string, int> data)
        {
            string id = data["id"].ToString();
            return delete_request("list/" + id, "");
        }

       
        public dynamic update_list(Dictionary<string, Object> data)
        {
            string id = data["id"].ToString();
            return put_request("list/" + id, JsonConvert.SerializeObject(data));
        }

        
        public dynamic display_list_users(Object data)
        {
            return put_request("list/display", JsonConvert.SerializeObject(data));
        }


      
        public dynamic add_users_list(Dictionary<string, Object> data)
        {
            string id = data["id"].ToString();
            return post_request("list/" + id + "/users", JsonConvert.SerializeObject(data));
        }

       
        public dynamic delete_users_list(Dictionary<string, Object> data)
        {
            string id = data["id"].ToString();
            return put_request("list/" + id + "/delusers", JsonConvert.SerializeObject(data));
        }

        
        public dynamic send_email(Object data)
        {
            return post_request("email", JsonConvert.SerializeObject(data));
        }

       
        public dynamic get_webhooks(Dictionary<string, string> data)
        {
            string is_plat = data["is_plat"].ToString();
            return get_request("webhook/index/is_plat/" + is_plat, "");
        }

        
        public dynamic get_webhook(Dictionary<string, int> data)
        {
            string id = data["id"].ToString();
            return get_request("webhook/" + id, "");
        }

        
        public dynamic create_webhook(Object data)
        {
            return post_request("webhook", JsonConvert.SerializeObject(data));
        }

       
        public dynamic delete_webhook(Dictionary<string, int> data)
        {
            string id = data["id"].ToString();
            return delete_request("webhook/" + id, "");
        }

       
        public dynamic update_webhook(Dictionary<string, Object> data)
        {
            string id = data["id"].ToString();
            return put_request("webhook/" + id, JsonConvert.SerializeObject(data));
        }

        
        public dynamic get_statistics(Object data)
        {
            return post_request("statistics", JsonConvert.SerializeObject(data));
        }

        
        public dynamic get_user(Dictionary<string, string> data)
        {
            string email = data["email"];
            return get_request("user/" + email.Trim(), "");
        }

        
        public dynamic delete_user(Dictionary<string, string> data)
        {
            string email = data["email"];
            return delete_request("user/" + email.Trim(), "");
        }

       
        public dynamic create_update_user(Object data)
        {
            return put_request("user/createdituser", JsonConvert.SerializeObject(data));
        }

       
        public dynamic import_users(Object data)
        {
            return post_request("user/import", JsonConvert.SerializeObject(data));
        }

       
        public dynamic export_users(Object data)
        {
            return post_request("user/export", JsonConvert.SerializeObject(data));
        }

       
        public dynamic get_attributes()
        {
            return get_request("attribute", "");
        }

        
        public dynamic get_attribute(Dictionary<string, string> data)
        {
            string type = data["type"];
            return get_request("attribute/" + type, "");
        }

        
        public dynamic create_attribute(Object data)
        {
            return post_request("attribute", JsonConvert.SerializeObject(data));
        }

        
        public dynamic delete_attribute(Dictionary<string, Object> data)
        {
            string type = data["type"].ToString();
            return post_request("attribute/" + type, JsonConvert.SerializeObject(data));
        }


       
        public dynamic get_report(Object data)
        {
            return post_request("report", JsonConvert.SerializeObject(data));
        }

       
        public dynamic get_folders(Dictionary<string, int> data)
        {
            string page = data["page"].ToString(); string page_limit = data["page_limit"].ToString();
            String url = "page/" + page + "/page_limit/" + page_limit;
            return get_request("folder/index/" + url, "");
        }

       
        public dynamic get_folder(Dictionary<string, int> data)
        {
            string id = data["id"].ToString();
            return get_request("folder/" + id, "");
        }

        
        public dynamic create_folder(Object data)
        {
            return post_request("folder", JsonConvert.SerializeObject(data));
        }

        
        public dynamic delete_folder(Dictionary<string, int> data)
        {
            string id = data["id"].ToString();
            return delete_request("folder/" + id, "");
        }

       
        public dynamic update_folder(Dictionary<string, Object> data)
        {
            string id = data["id"].ToString();
            return put_request("folder/" + id, JsonConvert.SerializeObject(data));
        }

       
        public dynamic delete_bounces(Object data)
        {
            return post_request("bounces", JsonConvert.SerializeObject(data));
        }

       
        public dynamic send_transactional_template(Dictionary<string, Object> data)
        {
            string id = data["id"].ToString();
            return put_request("template/" + id, JsonConvert.SerializeObject(data));
        }

       
        public dynamic create_sender(Object data)
        {
            return post_request("advanced", JsonConvert.SerializeObject(data));
        }

        
        public dynamic delete_sender(Dictionary<string, int> data)
        {
            string id = data["id"].ToString();
            return delete_request("advanced/" + id, "");
        }

        
        public dynamic update_sender(Dictionary<string, Object> data)
        {
            string id = data["id"].ToString();
            return put_request("advanced/" + id, JsonConvert.SerializeObject(data));
        }

        
        public dynamic get_senders(Dictionary<string, string> data)
        {
            string option = data["option"];
            return get_request("advanced/index/option/" + option, "");
        }


        
        public dynamic send_bat_sms(Dictionary<string, Object> data)
        {
            string id = data["id"].ToString();
            string mobilephone = data["to"].ToString();
            String phone = HttpUtility.UrlEncode(mobilephone);
            return get_request("sms/" + id + "/" + phone, "");

        }

        
        public dynamic send_sms(Object data)
        {
            return post_request("sms", JsonConvert.SerializeObject(data));
        }

       
        public dynamic create_sms_campaign(Object data)
        {
            return post_request("sms", JsonConvert.SerializeObject(data));
        }

       
        public dynamic update_sms_campaign(Dictionary<string, Object> data)
        {
            string id = data["id"].ToString();
            return put_request("sms/" + id, JsonConvert.SerializeObject(data));
        }

        
        public dynamic create_template(Object data)
        {
            return post_request("template", JsonConvert.SerializeObject(data));
        }

        
        public dynamic update_template(Dictionary<string, Object> data)
        {
            string id = data["id"].ToString();
            return post_request("template/" + id, JsonConvert.SerializeObject(data));
        }

    }
}
