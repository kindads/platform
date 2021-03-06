﻿using KindAds.Common.Models;
using KindAds.Common.Models.Entities;
using KindAds.DataAccess.Mappers;
using DapperExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperExtensions.Sql;
using Dapper;

namespace KindAds.DataAccess
{
    public class TagRepository : DGenericRepository<TagEntity>
    {
        public TagRepository(string connectionStringName = "KindAdsDefaultConnection") : base(connectionStringName)
        {
            LoadProfileMapper(typeof(TagPlainProfileMapper));

        }

        public override void Add(TagEntity entity)
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
