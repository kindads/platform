using Captivate.Comun.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperExtensions.Mapper;

namespace Captivate.DataAccess.Mappers
{
    public class TagPlainProfileMapper : ClassMapper<TagEntity2>
    {
        public TagPlainProfileMapper()
        {
            //use a custom schema
            Schema("dbo");
            base.Table(@"TAGS");

            
            //Ignore this property entirely
            Map(x => x.IdTag).Key(KeyType.Identity);

            //optional, map all other columns
            AutoMap();
        }
    }
}
