using KindAds.Common.Models;
using KindAds.Common.Models.Entities;
using KindAds.DataAccess.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.DataAccess
{
    public class CampaignSettingsRepository : DGenericRepository<CampaignSettingsEntity>
    {
        public CampaignSettingsRepository(string connectionStringName = "KindAdsDefaultConnection") : base(connectionStringName)
        {
            LoadProfileMapper(typeof(CampaignSettingsMapper));
        }

        public List<CampaignSettingsEntity> GetCampaignSettingsByIdCampaign(Guid idCampaign)
        {
            return GetAll().Where(c => c.CAMPAIGNs1_IdCampaign == idCampaign).ToList();
        }

    }
}
