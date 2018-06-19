using Captivate.Business;
using Captivate.Common.Interfaces;
using Captivate.Comun.Models;
using Captivate.Comun.Models.Entities;
using Captivate.Comun.Utils;
using Captivate.DataAccess;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Negocio
{
    public class SiteManager : ITelemetria
    {
        public ITrace telemetria { set; get; }
        public SiteRepository repository { set; get; }

   

        public SiteManager()
        {
        
            telemetria = new Trace();
            repository = new SiteRepository() ;
        }

        public bool ValidateToken(string messageToken)
        {
            bool result = false;
            try
            {
                // Obtenemos el objeto JSON
                SiteValidationToken siteToVerified = JsonConvert.DeserializeObject<SiteValidationToken>(messageToken);
                Guid IdSite = new Guid(siteToVerified.SiteId);
                SiteEntity site = repository.FindById(IdSite);

                //Creamos el token con los datos del sitio
                SiteToken siteToken = new SiteToken
                {
                    Name = site.Name,
                    Url = siteToVerified.SiteUrl,
                    SiteId = new Guid(siteToVerified.SiteId)
                };

                string siteTokenRow = JsonConvert.SerializeObject(siteToken);
                string token = Security.GetSha256(siteTokenRow);


                // Realizamos la validacion de la url del sitio y del token
                if ( site.URL.Replace("https://","").Replace("http://","") == siteToVerified.SiteUrl )
                {
                    site.Verified = true;
                    result = true;
                    repository.Edit(site);
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
