using Captivate.Common.Models.Entities;
using DapperExtensions.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.DataAccess.Mappers
{
    public class CampaignSettingsMapper : ClassMapper<CampaignSettingsEntity>
    {
        public CampaignSettingsMapper()
        {
            //use a custom schema
            Schema("dbo");
            base.Table(@"CAMPAIGNs1");

            //Ignore this property entirely
            Map(x => x.IdCampaignSetting).Key(KeyType.Guid);





            Map(x => x.CAMPAIGN).Ignore();
            

            //optional, map all other columns
            AutoMap();
        }
     
    }
}
