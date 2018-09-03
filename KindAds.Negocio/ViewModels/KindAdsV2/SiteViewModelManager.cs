using KindAds.Azure;
using KindAds.Common.Models;
using KindAds.Common.Models.Entities;
using KindAds.Common.Models.ViewModel;
using KindAds.Common.Utils;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using KindAds.DataAccess;
using KindAds.Negocio.Managersv2;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KindAds.Comun.Models.ViewModel.KindAdsV2
{
    public class SiteViewModelManager
    {
        private readonly AudienceManager _audienceManager;
        public SiteViewModelManager()
        {
            _audienceManager = new AudienceManager();
        }

        public FileStreamResult CreateVerificationFile(Guid IdAudience)
        {
            AudienceDocument _audience = GetAudienceById(IdAudience.ToString());

            if (_audience != null)
            {
                string datafile = _audience.VerificationString;

                if (datafile.Length > 0)
                {
                    var byteArray = System.Text.Encoding.ASCII.GetBytes(datafile);
                    var stream = new System.IO.MemoryStream(byteArray);

                    System.Web.Mvc.FileStreamResult _sfile = new System.Web.Mvc.FileStreamResult(stream, "text/plain");
                    _sfile.FileDownloadName = "kindads.txt";

                    return _sfile;
                }
                else
                {
                    return null;
                }
            }
            return null;
        }

        public FileStreamResult CreateScriptFile(Guid IdAudience)
        {
            AudienceManager repository = new AudienceManager();
            AudienceDocument _audience = repository.GetAudienceById(IdAudience.ToString());

            if (_audience != null)
            {
                string datafile = CreateScriptGtm(IdAudience);

                if (datafile.Length > 0)
                {
                    var byteArray = System.Text.Encoding.ASCII.GetBytes(datafile);
                    var stream = new System.IO.MemoryStream(byteArray);

                    System.Web.Mvc.FileStreamResult _sfile = new System.Web.Mvc.FileStreamResult(stream, "text/plain");
                    _sfile.FileDownloadName = "scriptGTM.txt";

                    return _sfile;
                }
                else
                {
                    return null;
                }
            }
            return null;
        }

        public AudienceDocument GetAudienceById(string IdAudience)
        {
            AudienceManager repository = new AudienceManager();
            AudienceDocument audience = repository.GetAudienceById(IdAudience);
            return audience;
        }

        public List<string> CreateScriptGtmByRows(Guid audienceId)
        {
            string token = Guid.NewGuid().ToString();
            string kindadsapi = ConfigurationManager.AppSettings["kindads-api-url"];
            string kindadsapiurl = string.Empty;

            List<string> rows = new List<string>();
            rows.Add("");
#if DEV
            rows.Add("<script src=\"https://kindadsscripts.blob.core.windows.net/site-validation-dev/KindAdsSites.js\"></script>");
#elif QA
            rows.Add("<script src=\"https://kindadsscripts.blob.core.windows.net/site-validation-qa/KindAdsSites.js\"></script>");
#elif STAGING
            rows.Add("<script src=\"https://kindadsscripts.blob.core.windows.net/site-validation-staging/KindAdsSites.js\"></script>");
#else
            rows.Add("<script src=\"https://kindadsscripts.blob.core.windows.net/site-validation-prod/KindAdsSites.js\"></script>");
#endif

            rows.Add("<script type=\"text/javascript\">");
#if DEV 
            kindadsapiurl = string.Format(kindadsapi, "-dev");
            string urlLine = string.Format("var url='{0}';", kindadsapiurl);
            rows.Add(urlLine);
#elif QA
            kindadsapiurl = string.Format(kindadsapi, "-qa");
            string urlLine = string.Format("var url='{0}';", kindadsapiurl);
            rows.Add(urlLine);
#elif STAGING
            kindadsapiurl = string.Format(kindadsapi, "-staging");
            string urlLine = string.Format("var url='{0}';", kindadsapiurl);
            rows.Add(urlLine);
#else
            kindadsapiurl = string.Format(kindadsapi, "");
            string urlLine = string.Format("var url='{0}';", kindadsapiurl);
            rows.Add(urlLine);
#endif
            rows.Add(string.Format(" var apiToken = '{0}';", token));
            rows.Add(string.Format(" var idAudience = '{0}';", audienceId.ToString().ToUpper()));
            rows.Add(" kindAds.validateSite(url, apiToken, idAudience);");
            rows.Add(" </script>");

            rows.Add("");
            return rows;
        }

        public string CreateScriptGtm(Guid IdAudience)
        {
            StringBuilder scriptM = new StringBuilder();
            //todo
            string token = Guid.NewGuid().ToString();
            string kindadsapi = ConfigurationManager.AppSettings["kindads-api-url"];
            string kindadsapiurl = string.Empty;

            scriptM.AppendLine();

#if DEV || DEBUG
            scriptM.AppendLine(" <script src=\"https://kindadsscripts.blob.core.windows.net/site-validation-dev/KindAdsSites.js\"></script>");
#elif QA
            scriptM.AppendLine(" <script src=\"https://kindadsscripts.blob.core.windows.net/site-validation-qa/KindAdsSites.js\"></script>");
#elif STAGING
            scriptM.AppendLine(" <script src=\"https://kindadsscripts.blob.core.windows.net/site-validation-staging/KindAdsSites.js\"></script>");
#else
            scriptM.AppendLine(" <script src=\"https://kindadsscripts.blob.core.windows.net/site-validation-prod/KindAdsSites.js\"></script>");
#endif

            scriptM.AppendLine(" <script type=\"text/javascript\">");

#if DEV || DEBUG
            kindadsapiurl = string.Format(kindadsapi, "");
            string urlLine = string.Format(" var url='{0}';", kindadsapiurl);
            scriptM.AppendLine(urlLine);
#elif QA
            kindadsapiurl = string.Format(kindadsapi, "-qa");
            string urlLine = string.Format(" var url='{0}';", kindadsapiurl);
            scriptM.AppendLine(urlLine);
#elif STAGING
            kindadsapiurl = string.Format(kindadsapi, "-staging");
            string urlLine = string.Format(" var url='{0}';", kindadsapiurl);
            scriptM.AppendLine(urlLine);
#else
            kindadsapiurl = string.Format(kindadsapi, "");
            string urlLine = string.Format(" var url='{0}';", kindadsapiurl);
            scriptM.AppendLine(urlLine);
#endif
            scriptM.AppendLine(string.Format(" var apiToken = '{0}';", token));
            scriptM.AppendLine(string.Format(" var idAudience = '{0}';", IdAudience.ToString().ToUpper()));
            scriptM.AppendLine(" kindAds.validateSite(url, apiToken, idAudience);");
            scriptM.AppendLine(" </script>");

            return scriptM.ToString();
        }

        public bool VerifySite(Guid IdAudience, int Type)
        {
            bool result = false;
            switch (Type)
            {
                case (int)EnumTypeSiteValidation.Gtm:
                    {
                        result = SiteWithTokenVerified(IdAudience);
                    }
                    break;
                case (int)EnumTypeSiteValidation.Txt:
                    {
                        result = SiteWithTxtVerified(IdAudience);
                    }
                    break;
            }

            if (result)
            {
                _audienceManager.EnqueueAudienceChange(new AudienceChangeNotification
                {
                    ChangeType = TypeAudienceChange.ActiveChannels,
                    idAudience = IdAudience.ToString()
                });
            }
            return result;
        }

        public bool SiteWithTxtVerified(Guid IdAudience)
        {
            bool result = false;
            AudienceManager repository = new AudienceManager();
            AudienceDocument audience = repository.GetAudienceById(IdAudience.ToString());
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(audience.WebSiteUrl + "/kindads.txt");
            request.Method = "GET";

            try
            {
                System.Net.WebResponse webResponse = request.GetResponse();
                using (System.IO.Stream webStream = webResponse.GetResponseStream())
                {
                    if (webStream != null)
                    {
                        using (System.IO.StreamReader responseReader = new System.IO.StreamReader(webStream))
                        {
                            var _response = responseReader.ReadToEnd();
                            if (_response != null)
                            {
                                if (_response.Trim() == audience.VerificationString.Trim())
                                {
                                    //Verify Site
                                    audience.Verified = true;
                                    repository.UpdateAudience(audience);
                                    result = true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                //Do nothing
            }
            return result;
        }

        public bool SiteWithTokenVerified(Guid IdAudience)
        {
            bool result = false;
            AudienceDocument _audience = GetAudienceById(IdAudience.ToString());
            result = (bool)_audience.Verified;
            return result;
        }



    }
}
