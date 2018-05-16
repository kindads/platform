using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Comun.Models.Entities
{
    [Table("CAMPAIGN_SETTINGS")]
    public class CampaignSettingsEntity
    {
        [Key]
        public System.Guid IdCampaignSetting { get; set; }
        public string SettingName { get; set; }
        public string SettingValue { get; set; }

        [ForeignKey("CAMPAIGN")]
        public System.Guid CAMPAIGNs1_IdCampaign { get; set; }

        public virtual CampaignEntity CAMPAIGN { get; set; }
    }
}
