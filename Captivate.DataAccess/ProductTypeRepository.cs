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
    public class ProductTypeRepository : AGenericRepository<KindadsContext, ProductTypeEntity>
    {
        public override IQueryable<ProductTypeEntity> GetAll()
        {
            return base.GetAll();
        }

        public override void Delete(ProductTypeEntity entity)
        {
            base.Delete(entity);
        }

        public override void Save()
        {
            base.Save();
        }

        public override void Add(ProductTypeEntity entity)
        {
            base.Add(entity);
        }

        public override void Edit(ProductTypeEntity entity)
        {
            base.Edit(entity);
        }

        public override IQueryable<ProductTypeEntity> FindBy(Expression<Func<ProductTypeEntity, bool>> predicate)
        {
            return base.FindBy(predicate);
        }
    }
}
