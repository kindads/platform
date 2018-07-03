using Captivate.Comun.Models.Entities;
using DapperExtensions.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.DataAccess.Mappers
{
  
    class MoneyAdsMapper : ClassMapper<MoneyAdsEntity>
    {
        public MoneyAdsMapper()
        {
            //use a custom schema
            Schema("dbo");
            base.Table(@"MoneyAds");
            AutoMap();
        }

    }
}
