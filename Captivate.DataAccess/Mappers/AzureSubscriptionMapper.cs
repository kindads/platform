using Captivate.Common.Models.Entities;
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

            

            //optional, map all other columns
            AutoMap();
        }

    }
}
