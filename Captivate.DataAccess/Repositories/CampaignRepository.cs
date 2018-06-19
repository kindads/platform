using Captivate.Comun.Models;
using Captivate.Comun.Models.Entities;
using Captivate.DataAccess.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.DataAccess
{
    public class CampaignRepository : DGenericRepository<CampaignEntity>
    {
        public CampaignRepository(string connectionStringName = "KindAdsDefaultConnection") : base(connectionStringName)
        {
            LoadProfileMapper(typeof(CampaignMapper));
        }
    }
}
