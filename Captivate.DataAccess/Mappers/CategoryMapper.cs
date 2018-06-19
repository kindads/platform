using Captivate.Comun.Models.Entities;
using DapperExtensions.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.DataAccess.Mappers
{
    public class CategoryMapper : ClassMapper<CategoryEntity>
    {
        public CategoryMapper()
        {
            //use a custom schema
            Schema("dbo");
            base.Table(@"CATEGORIES");

            //Ignore this property entirely
            Map(x => x.IdCategory).Key(KeyType.Identity);
            Map(x => x.TAGs).Ignore();
            Map(x => x.SITEs).Ignore();
            
            //optional, map all other columns
            AutoMap();
        }
     
    }
}
