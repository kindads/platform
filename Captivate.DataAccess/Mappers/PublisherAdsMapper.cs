using Captivate.Comun.Models.Entities;
using DapperExtensions.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.DataAccess.Mappers
{   

    class PublisherAdsMapper : ClassMapper<PublisherAdsEntity>
    {
        public PublisherAdsMapper()
        {
            //use a custom schema
            Schema("dbo");
            base.Table(@"PublisherAds");
            AutoMap();
        }

    }
}
