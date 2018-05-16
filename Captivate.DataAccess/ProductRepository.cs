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
    public class ProductRepository :  AGenericRepository<KindadsContext, ProductEntity>, IProductRepository
    {

        public ProductEntity GetSingle(Guid Id)
        {

            var query = GetAll().FirstOrDefault(x => x.IdProduct == Id);
            return query;
        }

        public override IQueryable<ProductEntity> GetAll()
        {
            return base.GetAll();
        }

        public override void Delete(ProductEntity product)
        {
            product.IsActive = false;
            Edit(product);
            Save();
        }

        public bool DeleteById(Guid Id)
        {
            bool result = false;
            ProductEntity product = FindById(Id);
            product.IsActive = false;
            Edit(product);
            Save();
            result = true;
            return result;
        }

        public override void Save()
        {
            base.Save();
        }

        public override void Add(ProductEntity entity)
        {
            base.Add(entity);
        }

        public override void Edit(ProductEntity entity)
        {
            base.Edit(entity);
        }

        public override IQueryable<ProductEntity> FindBy(Expression<Func<ProductEntity, bool>> predicate)
        {
            return base.FindBy(predicate);
        }


        public ProductEntity FindById(Guid Id)
        {
            return FindBy(o => o.IdProduct == Id && o.IsActive==true).FirstOrDefault();
        }
    }
}
