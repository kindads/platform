using Captivate.Comun.Models.Entities;
using DapperExtensions.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.DataAccess.Mappers
{
    class ProductMapper : ClassMapper<ProductEntity>
    {
        public ProductMapper()
        {
            //use a custom schema
            Schema("dbo");
            base.Table(@"PRODUCTs");

            //Ignore this property entirely
            Map(x => x.IdProduct).Key(KeyType.Guid);

            Map(x => x.CampaignEntitys).Ignore();

            Map(x => x.ProductSettingsEntitys).Ignore();

            Map(x => x.AspNetUser).Ignore();

            Map(x => x.PARTNER).Ignore();

            Map(x => x.PRODUCT_TYPE).Ignore();

            Map(x => x.SITE).Ignore();
            //optional, map all other columns
            AutoMap();
        }

    }
}
