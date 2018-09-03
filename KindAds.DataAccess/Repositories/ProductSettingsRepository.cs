using KindAds.Common.Models;
using KindAds.Common.Models.Entities;
using KindAds.DataAccess.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.DataAccess
{

    public class ProductSettingsRepository : DGenericRepository<ProductSettingsEntity>
    {
        public ProductSettingsRepository(string connectionStringName = "KindAdsDefaultConnection") : base(connectionStringName)
        {
            LoadProfileMapper(typeof(ProductSettingsMapper));
        }

        public List<ProductSettingsEntity> GetProductSettingsByIdProduct(Guid idProducto)
        {
            return GetAll().Where(p => p.PRODUCT_IdProduct == idProducto).ToList();
        }
    }
}
