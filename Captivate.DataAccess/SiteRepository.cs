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
    public class SiteRepository : AGenericRepository<KindadsContext, SiteEntity>, ISiteRepository
    {
        public SiteEntity GetSingle(Guid IdSite)
        {
            var query = GetAll().FirstOrDefault(x => x.IdSite == IdSite);
            return query;
        }
        public override IQueryable<SiteEntity> GetAll()
        {
            return base.GetAll();
        }

        public override void Delete(SiteEntity entity)
        {
            base.Delete(entity);
        }

        public override void Save()
        {
            base.Save();
        }

        public override void Add(SiteEntity entity)
        {
            base.Add(entity);
        }

        public override void Edit(SiteEntity entity)
        {
            base.Edit(entity);
        }

        public override IQueryable<SiteEntity> FindBy(Expression<Func<SiteEntity, bool>> predicate)
        {
            return base.FindBy(predicate);
        }
    }
}
