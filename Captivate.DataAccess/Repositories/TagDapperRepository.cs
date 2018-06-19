using Captivate.Comun.Models;
using Captivate.Comun.Models.Entities;
using Captivate.DataAccess.Mappers;
using DapperExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperExtensions.Sql;
using Dapper;

namespace Captivate.DataAccess
{
    public class TagDapperRepository : DGenericRepository<TagEntity2>
    {
        public TagDapperRepository(string connectionStringName = "KindAdsDefaultConnection") : base(connectionStringName)
        {
            LoadProfileMapper(typeof(TagPlainProfileMapper));

        }

        public override void Add(TagEntity2 entity)
        {
            using (var con = DBConnection)
            {
                con.Open();
                var query = string.Format( "Insert into TAGS (Description) values ('{0}')", entity.Description );
                con.Execute(query);
                con.Close();
            }
        }
    }
}
