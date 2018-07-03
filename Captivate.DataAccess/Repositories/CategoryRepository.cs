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
    public class CategoryRepository : DGenericRepository<CategoryEntity>, ICategoryRepository
    {
        public CategoryRepository(string connectionStringName = "KindAdsDefaultConnection") : base(connectionStringName)
        {
            LoadProfileMapper(typeof(CategoryMapper));
        }
   
        public List<CategoryEntity> GetByIdSite(Guid idSite)
        {
            throw new NotImplementedException();
           //return (from r in Context.CategorySites join c in Context.Categories on r.CATEGORY_IdCategory equals c.IdCategory where r.SITEs_IdSite.Equals(idSite) select c).ToList();
        }
    }
}