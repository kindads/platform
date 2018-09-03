using KindAds.Azure;
using KindAds.Business;
using KindAds.Common.Interfaces;
using KindAds.Common.Models;
using KindAds.Common.Models.Entities;
using KindAds.Common.Utils;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using KindAds.DataAccess;
using KindAds.Negocio.Managersv2;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Business
{
    public class SiteManager : ITelemetria
    {
        public ITrace telemetria { set; get; }
        public AudienceManager repository { set; get; }

   

        public SiteManager()
        {
        
            telemetria = new Trace();
            repository = new AudienceManager() ;
        }

        public bool ValidateToken(string messageToken)
        {
            bool result = false;
            try
            {
                // Obtenemos el objeto JSON
                SiteValidationToken siteToVerified = JsonConvert.DeserializeObject<SiteValidationToken>(messageToken);
                Guid IdAudience = new Guid(siteToVerified.AudienceId);
                AudienceDocument audience = repository.GetAudienceById(IdAudience.ToString());

                //Creamos el token con los datos del sitio
                //SiteToken siteToken = new SiteToken
                //{
                //    Name = audience.Title,
                //    Url = siteToVerified.SiteUrl,
                //    SiteId = new Guid(siteToVerified.AudienceId)
                //};

                //string siteTokenRow = JsonConvert.SerializeObject(siteToken);
                //string token = Security.GetSha256(siteTokenRow);

                string siteclean = audience.WebSiteUrl.Replace("https://", "").Replace("http://", "");
                string siteToVerifiedClean= siteToVerified.SiteUrl.Replace("https://", "").Replace("http://", "");
                // Realizamos la validacion de la url del sitio y del token
                if (siteclean == siteToVerifiedClean)
                {
                    audience.Verified = true;
                    result = true;
                    repository.UpdateAudience(audience);
                    //Enviar notificacion del resultado
                }
                else
                {
                    //Enviar notificacion del resultado
                }
            }
            catch (Exception e)
            {
                string messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }           
            return result;
        }
        
    }
}
