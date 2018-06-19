using Captivate.Comun.Models.Entities;
using DapperExtensions.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.DataAccess.Mappers
{
    public class CategorySiteMapper : ClassMapper<CategorySiteEntity>
    {
        public CategorySiteMapper()
        {
            //use a custom schema
            Schema("dbo");
            base.Table(@"CATEGORYSITE");

            //Ignore this property entirely
            //Map(x => x.CATEGORY_IdCategory).Key(KeyType.Identity);
            //Map(x => x.TAGs).Ignore();
            //Map(x => x.SITEs).Ignore();
            
            //optional, map all other columns
            AutoMap();
        }
     
    }
}
