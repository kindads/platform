using Captivate.Comun.Interfaces;
using Captivate.Comun.Models;
using Captivate.Comun.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.DataAccess
{
    public class CategorySiteRepository : AGenericRepository<KindadsContext, CategorySiteEntity>, ICategorySiteRepository
    {
        public CategorySiteEntity GetSingle(int CATEGORY_IdCategory, Guid SITEs_IdSite)
        {
            var query = GetAll().FirstOrDefault(x => x.CATEGORY_IdCategory == CATEGORY_IdCategory && x.SITEs_IdSite == SITEs_IdSite);
            return query;
        }
        public override IQueryable<CategorySiteEntity> GetAll()
        {
            return base.GetAll();
        }

        public override void Delete(CategorySiteEntity entity)
        {
            base.Delete(entity);
        }

        public override void Save()
        {
            base.Save();
        }

        public override void Add(CategorySiteEntity entity)
        {
            base.Add(entity);
        }

        public override void Edit(CategorySiteEntity entity)
        {
            base.Edit(entity);
        }

        public override IQueryable<CategorySiteEntity> FindBy(Expression<Func<CategorySiteEntity, bool>> predicate)
        {
            return base.FindBy(predicate);
        }
    }
}
