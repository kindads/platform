using Captivate.Comun.Interfaces;
using Captivate.Comun.Models;
using Captivate.Comun.Models.Entities;
using Captivate.DataAccess.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.DataAccess
{
    public class CategorySiteRepository : DGenericRepository<CategorySiteEntity>, ICategorySiteRepository
    {
        public CategorySiteRepository(string connectionStringName = "KindAdsDefaultConnection") : base(connectionStringName)
        {
            LoadProfileMapper(typeof(CategorySiteMapper));
        }

        public CategorySiteEntity GetSingle(int CATEGORY_IdCategory, Guid SITEs_IdSite)
        {
            var query = GetAll().FirstOrDefault(x => x.CATEGORY_IdCategory == CATEGORY_IdCategory && x.SITEs_IdSite == SITEs_IdSite);
            return query;
        }
      
      

      
    }
}
