using Captivate.Comun.Models.Entities;
using DapperExtensions.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.DataAccess.Mappers
{
    public class AzureSubscriptionMapper : ClassMapper<AzureSupcriptionEntity>
    {
        public AzureSubscriptionMapper()
        {
            //use a custom schema
            Schema("dbo");
            base.Table(@"AzureSupcription");

            //Ignore this property entirely
            Map(x => x.IdSite).Key(KeyType.Identity);

            //Map(x => x.PRODUCTS).Ignore();
            //Map(x => x.SITES).Ignore();
            //Map(x => x.CAMPAIGNs).Ignore();
            //Map(x => x.AspNetRoles).Ignore();
            //Map(x => x.CAMPAIGN_CHAT).Ignore();

            //optional, map all other columns
            AutoMap();
        }

    }
}
