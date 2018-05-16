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
    public class AspNetUserRepository : AGenericRepository<KindadsContext, AspNetUserEntity>, IAspNetUserRepository
    {
        public override IQueryable<AspNetUserEntity> GetAll()
        {
            return base.GetAll();
        }

        public override void Delete(AspNetUserEntity entity)
        {
            base.Delete(entity);
        }

        public override void Save()
        {
            base.Save();
        }

        public override void Add(AspNetUserEntity entity)
        {
            base.Add(entity);
        }

        public override void Edit(AspNetUserEntity entity)
        {
            base.Edit(entity);
        }

        public override IQueryable<AspNetUserEntity> FindBy(Expression<Func<AspNetUserEntity, bool>> predicate)
        {
            return base.FindBy(predicate);
        }

        public AspNetUserEntity GetByEmail(string email)
        {
            return (from r in Context.AspNetUsers where r.Email.Equals(email) select r).FirstOrDefault();
        }
    }
}
