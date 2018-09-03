using KindAds.Common.Utils;
using KindAds.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace KindAds.Negocio.Communication
{
    public class RequestManager<T> where T : IRequest
    {
        Common.Partners.IContact.IContactRequest requestFrm { set; get; }

        public RequestManager() { }

        public HttpWebRequest GetWebRequest(Common.Utils.IContactRequest typeRequest, IRequestSettings<T> config, Common.Partners.IContact.IContactRequest requestFrmOuter)
        {
            requestFrm = requestFrmOuter;
            ValidateRequestSettings(config, typeRequest);
            string uri = CreatetUriRequest(config.mailingProvider, typeRequest);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            ConfigureRequest(request, config);
            return request;
        }

        public string GetUriRequest(Common.Utils.IContactRequest typeRequest, IRequestSettings<T> config, Common.Partners.IContact.IContactRequest requestFrmOuter)
        {
            requestFrm = requestFrmOuter;
            string uri = CreatetUriRequest(config.mailingProvider, typeRequest);
            return uri;
        }

        private string CreatetUriRequest(T mailingProvider, Common.Utils.IContactRequest typeRequest)
        {
            string uri = string.Empty;

            TypeSwitch.Do(
               mailingProvider,
               TypeSwitch.Case<IIContactRequest>(() => { uri = IContactCreatetUriRequest(mailingProvider, typeRequest); }),
               TypeSwitch.Default(() => { uri = string.Empty; }));

            return uri;
        }

        private void ValidateRequestSettings(IRequestSettings<T> config, Common.Utils.IContactRequest typeRequest)
        {

            TypeSwitch.Do(
              config.mailingProvider,
              TypeSwitch.Case<IIContactRequest>(() => { IContactValidateRequestSettings(config, typeRequest); }),
              TypeSwitch.Default(() => { }));

        }


        private void ConfigureRequest(HttpWebRequest request, IRequestSettings<T> config)
        {
            TypeSwitch.Do(
           config.mailingProvider,
           TypeSwitch.Case<IIContactRequest>(() => {
         //IContactConfigureRequest(request, config);
         IContactConfigureRequestJson(request, config);
           }),
           TypeSwitch.Default(() => { }));
        }


        #region IContact methods

        public HttpClient GetHttpClient(IRequestSettings<T> config, Common.Utils.IContactRequest typeRequest)
        {
            HttpClient client = new HttpClient();

            string username = requestFrm.ApiUserName == string.Empty ? throw new ArgumentException("[Username] not found in settings, this key es required.") : requestFrm.ApiUserName;
            string password = requestFrm.ApiUserPassword == string.Empty ? throw new ArgumentException("[Password] not found in settings, this key es required.") : requestFrm.ApiUserPassword;
            string apiversion = requestFrm.ApiVersion == string.Empty ? throw new ArgumentException("[API-Version] not found in settings, this key es required.") : requestFrm.ApiVersion;
            string appid = requestFrm.ApiAppId == string.Empty ? throw new ArgumentException("[API-AppId] not found in settings, this key es required.") : requestFrm.ApiAppId;
            string accept = config.accept == string.Empty ? throw new ArgumentException("[Accept] not found in settings, this key es required.") : config.accept;
            string contenttype = config.contentType == string.Empty ? throw new ArgumentException("[contentType] not found in settings, this key es required.") : config.contentType;

            AuthenticationHeaderValue authorization = new AuthenticationHeaderValue(
             "Basic",
             Convert.ToBase64String(
                 System.Text.ASCIIEncoding.ASCII.GetBytes(
                     string.Format("{0}:{1}", username, password))));
            //Todo
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contenttype));
            client.DefaultRequestHeaders.Add("Username", username);
            client.DefaultRequestHeaders.Add("Password", password);
            client.DefaultRequestHeaders.Add("API-Version", apiversion);
            client.DefaultRequestHeaders.Add("API-AppId", appid);
            client.DefaultRequestHeaders.Add("Accept", accept);
            client.DefaultRequestHeaders.Add("API-Username", username);
            client.DefaultRequestHeaders.Add("API-Password", password);
            client.DefaultRequestHeaders.Authorization = authorization;



            return client;
        }

        private void IContactValidateRequestSettings(IRequestSettings<T> config, Common.Utils.IContactRequest typeRequest)
        {
            //Tem var
            string username = requestFrm.ApiUserName == string.Empty ? throw new ArgumentException("[Username] not found in settings, this key es required.") : requestFrm.ApiUserName;
            string password = requestFrm.ApiUserPassword == string.Empty ? throw new ArgumentException("[Password] not found in settings, this key es required.") : requestFrm.ApiUserPassword;
            string apiversion = requestFrm.ApiVersion == string.Empty ? throw new ArgumentException("[API-Version] not found in settings, this key es required.") : requestFrm.ApiVersion;
            string appid = requestFrm.ApiAppId == string.Empty ? throw new ArgumentException("[API-AppId] not found in settings, this key es required.") : requestFrm.ApiAppId;
            string accept = config.accept == string.Empty ? throw new ArgumentException("[Accept] not found in settings, this key es required.") : config.accept;
            string contenttype = config.contentType == string.Empty ? throw new ArgumentException("[contentType] not found in settings, this key es required.") : config.contentType;

            //checar que no esten las claves.
            if (config.headers.ContainsKey("Username") == false)
            {
                config.headers.Add("Username", username);
            }
            if (config.headers.ContainsKey("Password") == false)
            {
                config.headers.Add("Password", password);
            }
            if (config.headers.ContainsKey("API-Version") == false)
            {
                config.headers.Add("API-Version", apiversion);
            }
            if (config.headers.ContainsKey("API-AppId") == false)
            {
                config.headers.Add("API-AppId", appid);
            }
            if (config.headers.ContainsKey("Accept") == false)
            {
                config.headers.Add("Accept", accept);
            }
            if (config.headers.ContainsKey("contentType") == false)
            {
                config.headers.Add("contentType", contenttype);
            }

            if (
                typeRequest == Common.Utils.IContactRequest.AddMessage ||
                typeRequest == Common.Utils.IContactRequest.AddSends)
            {
                if (config.data == string.Empty)
                {
                    throw new ArgumentException("[Data] not found in settings, this key es required.");
                }

                config.method = "POST";
            }
            else if (typeRequest == Common.Utils.IContactRequest.AddCampaign)
            {
                config.method = "POST";
            }
            else
            {
                config.method = "GET";
            }

            // Config
            string method = config.method == string.Empty ? throw new ArgumentException("[Method] not found in settings, this key es required.") : config.method;
            config.authorization = string.Format("{0} {1}", username, password);
            if (config.headers.ContainsKey("Method") == false)
            {
                config.headers.Add("Method", method);
            }
        }

        private void IContactConfigureRequest(HttpWebRequest request, IRequestSettings<T> config)
        {
            // Get data from config.headers dictionary
            string Username = config.headers["Username"];
            string Password = config.headers["Password"];
            string ApiVersion = config.headers["API-Version"];
            string AppId = config.headers["API-AppId"];
            string Data = config.data;

            // config request with IRequestSettings
            request.KeepAlive = false;
            request.Method = config.method;
            request.Accept = config.accept;
            request.Headers.Add("API-Version", ApiVersion);
            request.Headers.Add("API-AppId", AppId);
            request.Headers.Add("API-Username", Username);
            request.Headers.Add("API-Password", Password);
            request.Headers.Add("Authorization", string.Format("{0} {1}", Username, Password));
            request.ContentType = config.contentType;
            request.Credentials = new NetworkCredential(Username, Password);

            // config body of request
            if (Data != null)
            {
                var bytes = Encoding.UTF8.GetBytes(Data);
                request.ContentLength = bytes.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
            }
        }


        private void IContactConfigureRequestJson(HttpWebRequest request, IRequestSettings<T> config)
        {
            // Get data from config.headers dictionary
            string Username = config.headers["Username"];
            string Password = config.headers["Password"];
            string ApiVersion = config.headers["API-Version"];
            string AppId = config.headers["API-AppId"];
            string Data = config.data;


            // config request with IRequestSettings
            request.KeepAlive = false;
            request.Method = config.method;
            request.Accept = config.accept;
            request.Headers.Add("API-Version", ApiVersion);
            request.Headers.Add("API-AppId", AppId);
            request.Headers.Add("API-Username", Username);
            request.Headers.Add("API-Password", Password);
            request.Headers.Add("Authorization", string.Format("{0} {1}", Username, Password));
            request.ContentType = config.contentType;
            request.Credentials = new NetworkCredential(Username, Password);

            // config body of request
            if (Data != null)
            {
                byte[] DataBytes = Encoding.ASCII.GetBytes(Data);
                request.ContentLength = DataBytes.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(DataBytes, 0, DataBytes.Length);
                }
            }
        }

        private string IContactCreatetUriRequest(T providerSettings, Common.Utils.IContactRequest typeRequest)
        {
            switch (typeRequest)
            {
                case Common.Utils.IContactRequest.AddCampaign:
                    {
                        return CreateIContactPostCampaignRequest(providerSettings);
                    }
                case Common.Utils.IContactRequest.AddMessage:
                    {
                        return CreateIContactPostMessageRequest(providerSettings);
                    }
                case Common.Utils.IContactRequest.AddSends:
                    {
                        return CreateIContactPostSendsRequest(providerSettings);
                    }
                case Common.Utils.IContactRequest.GetCamapign:
                    {
                        return CreateIContactGetCampaignRequest(providerSettings);
                    }
                case Common.Utils.IContactRequest.GetLists:
                    {
                        return CreateIContactGetListsRequest(providerSettings);
                    }
                case Common.Utils.IContactRequest.GetMessage:
                    {
                        return CreateIContactGetMessageRequest(providerSettings);
                    }
                case Common.Utils.IContactRequest.GetSends:
                    {
                        return CreateIContactGetSendsRequest(providerSettings);
                    }
                case IContactRequest.GetUsers:
                    {
                        return CreateIContactGetUsersRequest(providerSettings);
                    }
                case IContactRequest.GetFolders:
                    {
                        return CreateIContactGetFolderRequest(providerSettings);
                    }
                default:
                    {
                        return string.Empty;
                    }
            }
        }

        private string CreateIContactPostMessageRequest(T providerSettings)
        {
            IIContactRequest config = (IIContactRequest)providerSettings;
            string uri = string.Empty;
            //Todo
            uri += config.BaseUrl;
            uri += "/a/";
            uri += config.AccountId;
            uri += "/c/";
            uri += config.ClientFolderId;
            uri += "/messages";
            return uri;
        }

        private string CreateIContactPostSendsRequest(T providerSettings)
        {
            IIContactRequest config = (IIContactRequest)providerSettings;
            string uri = string.Empty;
            //Todo
            uri += config.BaseUrl;
            uri += "/a/";
            uri += config.AccountId;
            uri += "/c/";
            uri += config.ClientFolderId;
            uri += "/sends";
            return uri;
        }

        private string CreateIContactGetSendsRequest(T providerSettings)
        {
            IIContactRequest config = (IIContactRequest)providerSettings;
            string uri = string.Empty;
            //Todo
            uri += config.BaseUrl;
            uri += "/a/";
            uri += config.AccountId;
            uri += "/c/";
            uri += config.ClientFolderId;
            uri += "/lists";
            return uri;
        }

        private string CreateIContactGetUsersRequest(T providerSettings)
        {
            // https://app.sandbox.icontact.com/icp/a/[accountid]/users/[userid]
            IIContactRequest config = (IIContactRequest)providerSettings;
            string uri = string.Empty;
            uri += config.BaseUrl;
            uri += "/a/";
            uri += config.AccountId;
            uri += "/users/";
            return uri;
        }

        private string CreateIContactGetFolderRequest( T providerSettings)
        {
            // https://app.sandbox.icontact.com/icp/a/{accountid}/c/
            IIContactRequest config = (IIContactRequest)providerSettings;
            string uri = string.Empty;
            uri += config.BaseUrl;
            uri += "/a/";
            uri += config.AccountId;
            uri += "/c/";
            return uri;
        }

        private string CreateIContactGetMessageRequest(T providerSettings)
        {
            IIContactRequest config = (IIContactRequest)providerSettings;
            string uri = string.Empty;
            //Todo
            uri += config.BaseUrl;
            uri += "/a/";
            uri += config.AccountId;
            uri += "/c/";
            uri += config.ClientFolderId;
            uri += "/lists";
            return uri;
        }

        private string CreateIContactGetListsRequest(T providerSettings)
        {
            IIContactRequest config = (IIContactRequest)providerSettings;
            string uri = string.Empty;
            //Todo
            uri += config.BaseUrl;
            uri += "/a/";
            uri += config.AccountId;
            uri += "/c/";
            uri += config.ClientFolderId;
            uri += "/lists";
            return uri;
        }

        private string CreateIContactGetCampaignRequest(T providerSettings)
        {
            IIContactRequest config = (IIContactRequest)providerSettings;
            string uri = string.Empty;
            //Todo
            uri += config.BaseUrl;
            uri += "/a/";
            uri += config.AccountId;
            uri += "/c/";
            uri += config.ClientFolderId;
            uri += "/campaigns/";
            return uri;
        }

        private string CreateIContactPostCampaignRequest(T providerSettings)
        {
            IIContactRequest config = (IIContactRequest)providerSettings;
            string uri = string.Empty;
            //Todo
            uri += config.BaseUrl;
            uri += "/a/";
            uri += config.AccountId;
            uri += "/c/";
            uri += config.ClientFolderId;
            uri += "/campaigns/";
            return uri;
        }
        #endregion


    }
}
