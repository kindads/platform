using Captivate.Business;
using Captivate.Common.Interfaces;
using Captivate.Comun.Interfaces;
using Captivate.Negocio.Partners.IContact;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace Captivate.Negocio.Communication
{
    public class ResponseManager<T> : ITelemetria where T : IResponse,   new()
  {
    public ITrace telemetria { set; get; }
    
    public ResponseManager()
    {
      telemetria = new Trace();
    }

    public HttpStatusCode GetHttpStatusCode(string message)
    {
      HttpStatusCode statusCode = HttpStatusCode.Ambiguous;

      if(message.Contains("401"))
      {
        statusCode = HttpStatusCode.Unauthorized;
      }

      return statusCode;
    }

    public T GetResponse(HttpWebRequest request)
    {
      T result = new T();

      EnableHTTPS();

      try
      {
        using (var response = (HttpWebResponse)request.GetResponse())
        {
          result.StatusCode = response.StatusCode;
          if (result.StatusCode == HttpStatusCode.OK)
          {
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
              var jsonData = reader.ReadToEnd();
              var serializer = new JavaScriptSerializer();
              result = serializer.Deserialize<T>(jsonData);
              return result;
            }
          }
          else
          {
            return result;
          }
        }
      }      
      catch (Exception e)
      {
        var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
        telemetria.Critical(messageException);

        result.StatusCode = GetHttpStatusCode(e.Message);
      }

      return result;
    }

    public async Task<T> GetResponseFromHttpClient(HttpClient client,IContactPostMessageRequest[] requestBody,Dictionary<string,string> values, string uri)
    {
      T result = new T();

      try
      {     
        var response = await client.PostAsJsonAsync(uri, requestBody).ConfigureAwait(false);
        var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var serializer = new JavaScriptSerializer();
        result = serializer.Deserialize<T>(responseString);
      
      }
      catch (Exception e)
      {
        //Todo
      }
      return result;
    }

    private void EnableHTTPS()
    {
      ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
                                             SecurityProtocolType.Tls11 |
                                             SecurityProtocolType.Tls12;
    }

  }
}
