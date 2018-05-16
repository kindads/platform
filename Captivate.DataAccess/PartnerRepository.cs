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
   
    public class PartnerRepository : AGenericRepository<KindadsContext, PartnerEntity>
    {
        public override IQueryable<PartnerEntity> GetAll()
        {
            return base.GetAll();
        }

        public override void Delete(PartnerEntity entity)
        {
            base.Delete(entity);
        }

        public override void Save()
        {
            base.Save();
        }

        public override void Add(PartnerEntity entity)
        {
            base.Add(entity);
        }

        public override void Edit(PartnerEntity entity)
        {
            base.Edit(entity);
        }

        public override IQueryable<PartnerEntity> FindBy(Expression<Func<PartnerEntity, bool>> predicate)
        {
            return base.FindBy(predicate);
        }
    }
}
