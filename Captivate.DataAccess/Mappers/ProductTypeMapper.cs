using Captivate.Common.Models.Entities;
using DapperExtensions.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.DataAccess.Mappers
{
    class ProductTypeMapper : ClassMapper<ProductTypeEntity>
    {
        public ProductTypeMapper()
        {
            //use a custom schema
            Schema("dbo");
            base.Table(@"PRODUCT_TYPE");


            //Ignore this property entirely
            Map(x => x.IdProductType).Key(KeyType.Guid);

            Map(x => x.PARTNERS).Ignore();

            Map(x => x.PRODUCTs).Ignore();

            Map(x => x.PARTNER_PRODUCT_SETTINGS).Ignore();
            //optional, map all other columns
            AutoMap();
        }

    }

}
