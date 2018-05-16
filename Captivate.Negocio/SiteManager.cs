using Captivate.Business;
using Captivate.Common.Interfaces;
using Captivate.Comun.Models.Entities;
using Captivate.DataAccess;
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
            repository = new SiteRepository();
        }

        public List<SiteEntity> GetAll()
        {
            List<SiteEntity> sites = new List<SiteEntity>();
            sites = repository.GetAll().ToList();
            return sites;
        }

        
    }
}
