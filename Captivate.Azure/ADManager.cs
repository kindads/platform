using Captivate.Common.Interfaces;
using Captivate.Common.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Captivate.Azure
{
    public static class ADManager 
    {
       
        private static void EnableHTTPS()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls   |
                                                   SecurityProtocolType.Tls11 |
                                                   SecurityProtocolType.Tls12;
        }


        public static SitesAD GetSites(string Token, string SubscriptionId)
        {
            SitesAD result = new SitesAD();
            string url = string.Format("https://management.azure.com/subscriptions/{0}/providers/Microsoft.Web/sites?api-version=2016-08-01", SubscriptionId);

            WebRequest request = WebRequest.Create(url);
            request.Method = "GET";
            request.Headers.Add("Authorization", string.Format("Bearer {0}", Token));

            EnableHTTPS();
            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    var StatusCode = response.StatusCode;
                    if (StatusCode == HttpStatusCode.OK)
                    {
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            var jsonData = reader.ReadToEnd();
                            var serializer = new JavaScriptSerializer();
                            result = serializer.Deserialize<SitesAD>(jsonData);
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
                //todo
            }
            return result;
        }

        public static string GetToken(string TenantId, string ApplicationId, string Key)
        {
            string token = string.Empty;
            try
            {
                var authenticationContext = new AuthenticationContext(string.Format("https://login.microsoftonline.com/{0}/oauth2/authorize?client_id={1}&response_type=code", TenantId, ApplicationId));
                var credential = new ClientCredential(clientId: ApplicationId, clientSecret: Key);
                var result = authenticationContext.AcquireTokenAsync(resource: "https://management.azure.com/", clientCredential: credential).Result;

                if (result == null)
                {
                    throw new InvalidOperationException("Failed to obtain the JWT token");
                }

                token = result.AccessToken;
            }
            catch (Exception e)
            {
                //Todo
            }
            return token;
        }

        public static bool ValidateSite(string SiteUrl, string Token, string SubscriptionId)
        {
            bool result = false;
            SitesAD sitesData = GetSites(Token, SubscriptionId);

            foreach(var siteData in sitesData.value)
            {
                foreach(var host in siteData.properties.hostNames)
                {
                    string cleanSiteUrl = SiteUrl.Replace("https://", "").Replace("http://","").Replace("www.","");
                    string cleanHost = host.Replace("htpp://", "").Replace("https://", "").Replace("www.","");

                    if (cleanHost.Equals(cleanSiteUrl))
                    {
                        result = true;
                        break;
                    }
                }
                if (result == true) break;
            }
            return result;
        }
    }
}
