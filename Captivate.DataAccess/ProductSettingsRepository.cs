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
     
    public class ProductSettingsRepository : AGenericRepository<KindadsContext, ProductSettingsEntity>
    {
       
        public override IQueryable<ProductSettingsEntity> GetAll()
        {
            return base.GetAll();
        }

        public override void Delete(ProductSettingsEntity entity)
        {
            base.Delete(entity);
        }

        public override void Save()
        {
            base.Save();
        }

        public override void Add(ProductSettingsEntity entity)
        {
            base.Add(entity);
        }

        public override void Edit(ProductSettingsEntity entity)
        {
            base.Edit(entity);
        }

        public override IQueryable<ProductSettingsEntity> FindBy(Expression<Func<ProductSettingsEntity, bool>> predicate)
        {
            return base.FindBy(predicate);
        }
    }
}
