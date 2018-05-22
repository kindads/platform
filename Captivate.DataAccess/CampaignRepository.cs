using Captivate.Comun.Models;
using Captivate.Comun.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.DataAccess
{
    public class CampaignRepository : AGenericRepository<KindadsContext, CampaignEntity>
    {
        public CampaignEntity GetById(Guid idCampaign)
        {
            return (from c in Context.Campaigns where c.IdCampaign == idCampaign select c).FirstOrDefault();
        }
    }
}
