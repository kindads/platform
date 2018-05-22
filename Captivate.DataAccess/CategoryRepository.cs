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
    public class CategoryRepository : AGenericRepository<KindadsContext, CategoryEntity>, ICategoryRepository
    {
        public CategoryEntity GetSingle(short IdCategory)
        {
            var query = GetAll().FirstOrDefault(x => x.IdCategory == IdCategory);
            return query;
        }
        public override IQueryable<CategoryEntity> GetAll()
        {
            return base.GetAll();
        }

        public override void Delete(CategoryEntity entity)
        {
            base.Delete(entity);
        }

        public override void Save()
        {
            base.Save();
        }

        public override void Add(CategoryEntity entity)
        {
            base.Add(entity);
        }

        public override void Edit(CategoryEntity entity)
        {
            base.Edit(entity);
        }

        public override IQueryable<CategoryEntity> FindBy(Expression<Func<CategoryEntity, bool>> predicate)
        {
            return base.FindBy(predicate);
        }

        public List<CategoryEntity> GetByIdSite(Guid idSite)
        {
           return (from r in Context.CategorySites join c in Context.Categories on r.CATEGORY_IdCategory equals c.IdCategory where r.SITEs_IdSite.Equals(idSite) select c).ToList();
        }
    }
}