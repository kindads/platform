using KindAds.Common.Models.Entities;
using DapperExtensions.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.DataAccess.Mappers
{
    public class PartnerMapper : ClassMapper<PartnerEntity>
    {
        public PartnerMapper()
        {
            //use a custom schema
            Schema("dbo");
            base.Table(@"PARTNERS");


            //Ignore this property entirely
            Map(x => x.IdPartner).Key(KeyType.Guid);

            Map(x => x.PARTNER_PRODUCT_SETTINGS).Ignore(); 
            Map(x => x.PRODUCT_TYPE).Ignore();
            Map(x => x.PRODUCTs).Ignore();
            Map(x => x.PARTNER_SETTINGS).Ignore();
            //optional, map all other columns
            AutoMap();
        }
    
    }
}
