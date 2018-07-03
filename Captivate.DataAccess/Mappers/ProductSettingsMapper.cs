using Captivate.Common.Models.Entities;
using DapperExtensions.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.DataAccess.Mappers
{
    class ProductSettingsMapper : ClassMapper<ProductSettingsEntity>
    {
        public ProductSettingsMapper()
        {
            //use a custom schema
            Schema("dbo");
            base.Table(@"PRODUCT_SETTINGS");


            //Ignore this property entirely
            Map(x => x.IdProductSetting).Key(KeyType.Guid);

            Map(x => x.PRODUCT).Ignore();
            //optional, map all other columns
            AutoMap();
        }

    }

}
