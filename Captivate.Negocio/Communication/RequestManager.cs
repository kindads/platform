using Captivate.Comun.Utils;
using Captivate.Comun.Interfaces;
using Captivate.Negocio.Partners.IContact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace Captivate.Negocio.Communication
{
  public class RequestManager<T>  where T: IRequest
  {
        Partners.IContact.IContactRequest requestFrm { set; get; }

    public RequestManager() { }

    public HttpWebRequest GetWebRequest(Comun.Utils.IContactRequest typeRequest, IRequestSettings<T> config, Partners.IContact.IContactRequest requestFrmOuter)
    {
      requestFrm = requestFrmOuter;
      ValidateRequestSettings(config, typeRequest);
      string uri = CreatetUriRequest(config.mailingProvider, typeRequest);
      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
      ConfigureRequest(request, config);
      return request;
    }

    public string GetUriRequest(Comun.Utils.IContactRequest typeRequest, IRequestSettings<T> config, Partners.IContact.IContactRequest requestFrmOuter)
    {
      requestFrm = requestFrmOuter;
      string uri = CreatetUriRequest(config.mailingProvider, typeRequest);
      return uri;
    }

    private string CreatetUriRequest(T mailingProvider, Comun.Utils.IContactRequest typeRequest)
    {
      string uri = string.Empty;

      TypeSwitch.Do(
         mailingProvider,
         TypeSwitch.Case<IIContactRequest>(() => { uri=IContactCreatetUriRequest(mailingProvider, typeRequest); }),
         TypeSwitch.Default(() => { uri = string.Empty; }));

      return uri;
    }

    private void ValidateRequestSettings(IRequestSettings<T> config, Comun.Utils.IContactRequest typeRequest)
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

    public HttpClient GetHttpClient(IRequestSettings<T> config, Comun.Utils.IContactRequest typeRequest)
    {
      HttpClient client = new HttpClient();

      string username = requestFrm.ApiUserName == string.Empty ? throw new ArgumentException("[Username] not found in settings, this key es required.") : requestFrm.ApiUserName;
      string password = requestFrm.ApiUserPassword == string.Empty ? throw new ArgumentException("[Password] not found in settings, this key es required.") : requestFrm.ApiUserPassword;
      string apiversion = requestFrm.ApiVersion == string.Empty ? throw new ArgumentException("[API-Version] not found in settings, this key es required.") : requestFrm.ApiVersion;
      string appid = requestFrm.ApiAppId == string.Empty ? throw new ArgumentException("[API-AppId] not found in settings, this key es required.") : requestFrm.ApiAppId;
      string accept = config.accept == string.Empty ? throw new ArgumentException("[Accept] not found in settings, this key es required.") : config.accept;
      string contenttype = config.contentType == string.Empty ? throw new ArgumentException("[contentType] not found in settings, this key es required.") : config.contentType;

      AuthenticationHeaderValue authorization= new AuthenticationHeaderValue(
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
      client.DefaultRequestHeaders.Authorization= authorization;

      

      return client;
    }
    
    private void IContactValidateRequestSettings(IRequestSettings<T> config, Comun.Utils.IContactRequest typeRequest)
    {
      //Tem var
      string username = requestFrm.ApiUserName == string.Empty ? throw new ArgumentException("[Username] not found in settings, this key es required.") : requestFrm.ApiUserName;
      string password = requestFrm.ApiUserPassword == string.Empty ? throw new ArgumentException("[Password] not found in settings, this key es required.") : requestFrm.ApiUserPassword;
      string apiversion = requestFrm.ApiVersion == string.Empty ? throw new ArgumentException("[API-Version] not found in settings, this key es required.") : requestFrm.ApiVersion;
      string appid = requestFrm.ApiAppId == string.Empty ? throw new ArgumentException("[API-AppId] not found in settings, this key es required.") : requestFrm.ApiAppId;
      string accept = config.accept == string.Empty ? throw new ArgumentException("[Accept] not found in settings, this key es required.") : config.accept;
      string contenttype = config.contentType == string.Empty ? throw new ArgumentException("[contentType] not found in settings, this key es required.") : config.contentType;

      //Add to header
      config.headers.Add("Username", username);
      config.headers.Add("Password", password);
      config.headers.Add("API-Version", apiversion);
      config.headers.Add("API-AppId", appid);     
      config.headers.Add("Accept", accept);
      config.headers.Add("contentType", contenttype);
     



      if (
          typeRequest == Comun.Utils.IContactRequest.AddMessage  ||
          typeRequest == Comun.Utils.IContactRequest.AddSends )
      {
        if (config.data == string.Empty)
        {
          throw new ArgumentException("[Data] not found in settings, this key es required.");
        }

        config.method = "POST";
      }
      else if (typeRequest == Comun.Utils.IContactRequest.AddCampaign )
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
      config.headers.Add("Method", method);
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

    private string IContactCreatetUriRequest(T providerSettings, Comun.Utils.IContactRequest typeRequest)
    {
      switch (typeRequest)
      {
        case Comun.Utils.IContactRequest.AddCampaign:
          {
            return CreateIContactPostCampaignRequest(providerSettings);
          }
        case Comun.Utils.IContactRequest.AddMessage:
          {
            return CreateIContactPostMessageRequest(providerSettings);
          }
        case Comun.Utils.IContactRequest.AddSends:
          {
            return CreateIContactPostSendsRequest(providerSettings);
          }
        case Comun.Utils.IContactRequest.GetCamapign:
          {
            return CreateIContactGetCampaignRequest(providerSettings);
          }
        case Comun.Utils.IContactRequest.GetLists:
          {
            return CreateIContactGetListsRequest(providerSettings);
          }
        case Comun.Utils.IContactRequest.GetMessage:
          {
            return CreateIContactGetMessageRequest(providerSettings);
          }
        case Comun.Utils.IContactRequest.GetSends:
          {
            return CreateIContactGetSendsRequest(providerSettings);
          }
        default:
          {
            return string.Empty;
          }
      }
    }

    private string CreateIContactPostMessageRequest( T providerSettings )
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

    private string CreateIContactPostSendsRequest( T providerSettings )
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

    private string CreateIContactGetListsRequest (T providerSettings)
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
