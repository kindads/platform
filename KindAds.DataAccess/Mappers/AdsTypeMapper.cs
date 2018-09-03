using KindAds.Comun.Models.Entities;
using DapperExtensions.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.DataAccess.Mappers
{   

    class AdsTypeMapper : ClassMapper<AdsTypeEntity>
    {
        public AdsTypeMapper()
        {
            //use a custom schema
            Schema("dbo");
            base.Table(@"AdsType");
            AutoMap();
        }
    }
}
