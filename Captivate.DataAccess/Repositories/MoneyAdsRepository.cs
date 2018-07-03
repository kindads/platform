﻿using Captivate.Common.Models;
using Captivate.Comun.Models.Entities;
using Captivate.DataAccess.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.DataAccess.Repositories
{  
    public class MoneyAdsRepository : DGenericRepository<MoneyAdsEntity>
    {
        public MoneyAdsRepository(string connectionStringName = "KindAdsDefaultConnection") : base(connectionStringName)
        {
            LoadProfileMapper(typeof(SiteMapper));
        }

        public MoneyAdsEntity GetSingle(int Id)
        {
            var query = GetAll().FirstOrDefault(x => x.Id == Id);
            return query;
        }
    }
}