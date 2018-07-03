using Captivate.Common.Models;
using Captivate.Comun.Models.Entities;
using Captivate.DataAccess.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.DataAccess.Repositories
{
    public class MoneyAdsSettingsRepository : DGenericRepository<MoneyAdsSettingsEntity>
    {
        public MoneyAdsSettingsRepository(string connectionStringName = "KindAdsDefaultConnection") : base(connectionStringName)
        {
            LoadProfileMapper(typeof(SiteMapper));
        }

        public MoneyAdsSettingsEntity GetSingle(int Id)
        {
            var query = GetAll().FirstOrDefault(x => x.Id == Id);
            return query;
        }
    }
}
