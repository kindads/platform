using Captivate.Common.Interfaces;
using Captivate.Common.Models;
using Captivate.Common.Models.Entities;
using Captivate.DataAccess.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.DataAccess
{
    public class SiteRepository : DGenericRepository< SiteEntity>, ISiteRepository
    {
        public SiteRepository(string connectionStringName = "KindAdsDefaultConnection") : base(connectionStringName)
        {
            LoadProfileMapper(typeof(SiteMapper));
        }

        public SiteEntity GetSingle(Guid IdSite)
        {
            var query = GetAll().FirstOrDefault(x => x.IdSite == IdSite);
            return query;
        }
       
        
    }
}
