﻿using KindAds.Common.Models;
using KindAds.Comun.Models.Entities;
using KindAds.DataAccess.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.DataAccess.Repositories
{
    public class AdsTypeRepository : DGenericRepository<AdsTypeEntity>
    {
        public AdsTypeRepository(string connectionStringName = "KindAdsDefaultConnection") : base(connectionStringName)
        {
            LoadProfileMapper(typeof(SiteMapper));
        }

        public AdsTypeEntity GetSingle(int Id)
        {
            var query = GetAll().FirstOrDefault(x => x.Id == Id);
            return query;
        }
    }
}
