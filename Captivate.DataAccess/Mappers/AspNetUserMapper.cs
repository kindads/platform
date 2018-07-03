using Captivate.Common.Models.Entities;
using DapperExtensions.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.DataAccess.Mappers
{
    public class AspNetUserMapper : ClassMapper<AspNetUserEntity>
    {
        public AspNetUserMapper()
        {
            //use a custom schema
            Schema("dbo");
            base.Table(@"AspNetUsers");

            //Ignore this property entirely
            //Map(x => x.Id).Key(KeyType.Identity);

            Map(x => x.PRODUCTS).Ignore();
            Map(x => x.SITES).Ignore();
            Map(x => x.CAMPAIGNs).Ignore();
            Map(x => x.AspNetRoles).Ignore(); 
            Map(x => x.CAMPAIGN_CHAT).Ignore(); 

            //optional, map all other columns
            AutoMap();
        }

    }
}
