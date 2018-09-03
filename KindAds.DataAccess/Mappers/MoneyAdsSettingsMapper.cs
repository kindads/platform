using KindAds.Comun.Models.Entities;
using DapperExtensions.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.DataAccess.Mappers
{
    class MoneyAdsSettingsMapper : ClassMapper<MoneyAdsSettingsEntity>
    {
        public MoneyAdsSettingsMapper()
        {
            //use a custom schema
            Schema("dbo");
            base.Table(@"MoneyAdsSettings");
            AutoMap();
        }
    }
}
