using KindAds.Common.Utils.Partners.Mail.GetResponse;
using KindAds.Comun.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Negocio.PartnerServices
{
    public class GetResponseService
    {
        private static string baseUrl = "https://api.getresponse.com/v3/";

        
        public GetResponseService()
        {
           
        }

        public bool IsValid(string apiKey)
        {
            var result = MethodGet(apiKey, "accounts");
            return result.Code.Equals(HttpStatusCode.OK);
        }

        public List<GenericElement> GetLists(string key)
        {
            List<GenericElement> list = new List<GenericElement>();
            GenericElement element;
            var result = MethodGet(key, "campaigns");
            dynamic data = JsonConvert.DeserializeObject(result.Data);
            foreach (var items in data)
            {
                element = new GenericElement()
                {
                    Id = items["campaignId"],
                    Name = items["name"]
                };
                list.Add(element);
            }
            return list;
        }

        public List<GenericElement> GetFromFields(string key)
        {
            List<GenericElement> list = new List<GenericElement>();
            GenericElement element;
            var result = MethodGet(key, "from-fields");
            dynamic data = JsonConvert.DeserializeObject(result.Data);
            foreach (var items in data)
            {
                if (Convert.ToBoolean(items["isActive"]))
                {
                    element = new GenericElement()
                    {
                        Id = items["fromFieldId"],
                        Name = items["email"]
                    };
                    list.Add(element);
                }
            }
            return list;
        }

      

        public static ApiResult MethodGet(string apikey, string action)
        {
            string result = "";
            ApiResult apiResult = new ApiResult();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + action);
            request.Method = "GET";
            request.Headers.Add("X-Auth-Token", "api-key " + apikey);
            try
            {
                WebResponse webResponse = request.GetResponse();
                apiResult.Code = ((HttpWebResponse)webResponse).StatusCode;
                apiResult.Message = ((HttpWebResponse)webResponse).StatusDescription;
                using (Stream webStream = webResponse.GetResponseStream())
                {
                    if (webStream != null)
                    {
                        using (StreamReader responseReader = new StreamReader(webStream))
                        {
                            result = responseReader.ReadToEnd();
                            apiResult.Data = result;
                        }
                    }
                }
            }
            catch (WebException wex)
            {
                result = wex.Message;
            }
            return apiResult;
        }

        public static ApiResult MethodPost(string apikey, string action, IDictionary<string, object> parameters)
        {
            ApiResult apiResult = new ApiResult();

            var serialized = JsonConvert.SerializeObject(parameters);

            WebRequest request = WebRequest.Create(baseUrl + action);
            request.Method = "POST";
            request.Headers.Add("X-Auth-Token", "api-key " + apikey);
            byte[] byteArray = Encoding.UTF8.GetBytes(serialized);
            request.ContentType = "application/json";

            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            WebResponse response = request.GetResponse();
            apiResult.Code = ((HttpWebResponse)response).StatusCode;
            apiResult.Message = ((HttpWebResponse)response).StatusDescription;
            dataStream = response.GetResponseStream();

            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            apiResult.Data = responseFromServer;

            reader.Close();
            dataStream.Close();
            response.Close();

            return apiResult;
        }
    }
}
