using Captivate.Common.Interfaces;
using Captivate.Common.Models;
using Captivate.Common.Models.Entities;
using Captivate.Common.Models.ViewModel;
using Captivate.DataAccess.Mappers;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.DataAccess
{
    public class ProductRepository : DGenericRepository<ProductEntity>, IProductRepository
    {
        public ProductRepository(string connectionStringName = "KindAdsDefaultConnection") : base(connectionStringName)
        {
            LoadProfileMapper(typeof(ProductMapper));
        }


        public override void Delete(ProductEntity product)
        {
            product.IsActive = false;
            Edit(product);            
        }

        public bool DeleteById(Guid Id)
        {
            bool result = false;
            ProductEntity product = FindById(Id);
            product.IsActive = false;
            Edit(product);

            result = true;
            return result;
        }

   


        public ProductEntity FindById(Guid Id)
        {
            using (var cnn = DBConnection)
            {
                cnn.Open();
                return cnn.Query<ProductEntity>("sp_Products_FindActiveById", new { idProduct = Id }, commandType: CommandType.StoredProcedure).SingleOrDefault();
            }
        }



        public List<ProductEntity> GetActiveProducts()
        {
            using (var cnn = DBConnection)
            {
                cnn.Open();
                return cnn.Query<ProductEntity>("sp_Products_FindActiveProducts",null, commandType: CommandType.StoredProcedure).ToList();
            }
        }

        public List<ProductListEntityViewModel> GetActiveProductsByUserId(Guid idUser)
        {
            using (var cnn = DBConnection)
            {
                cnn.Open();
                return cnn.Query<ProductListEntityViewModel>("sp_ProductsList", new { @idUser = idUser.ToString() }, commandType: CommandType.StoredProcedure).Where(p => p.IsActive == true). ToList();
            }
        }
    }
}
