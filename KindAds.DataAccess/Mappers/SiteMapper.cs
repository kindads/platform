using KindAds.Common.Models.Entities;
using DapperExtensions.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.DataAccess.Mappers
{
    public class SiteMapper : ClassMapper<SiteEntity>
    {
        public SiteMapper()
        {
            //use a custom schema
            Schema("dbo");
            base.Table(@"SITES");


            //Ignore this property entirely
            Map(x => x.IdSite).Key(KeyType.Guid);

            Map(x => x.AspNetUser).Ignore();
            Map(x => x.PRODUCTs).Ignore();
            Map(x => x.CATEGORYs).Ignore();
            //optional, map all other columns
            AutoMap();
        }

    }

}
