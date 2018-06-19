using Captivate.Comun.Models.Entities;
using DapperExtensions.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.DataAccess.Mappers
{
    public class CampaignMapper : ClassMapper<CampaignEntity>
    {
        public CampaignMapper()
        {
            //use a custom schema
            Schema("dbo");
            base.Table(@"CAMPAIGNs1");

            //Ignore this property entirely
            Map(x => x.IdCampaign).Key(KeyType.Guid);





            Map(x => x.AspNetUser).Ignore();
            Map(x => x.PRODUCT).Ignore();
            Map(x => x.TRANSACTIONS_CAPT).Ignore();
            Map(x => x.CAT_CAMPAIGN_STATUS).Ignore();
            Map(x => x.CAMPAIGN_SETTINGS).Ignore();
            Map(x => x.CAMPAIGN_CHAT).Ignore();


            //optional, map all other columns
            AutoMap();
        }
     
    }
}
