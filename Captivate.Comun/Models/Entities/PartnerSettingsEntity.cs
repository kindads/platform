using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Comun.Models.Entities
{
    [Table("PARTNER_SETTINGS")]
    public class PartnerSettingsEntity
    {
        [Key]
        public System.Guid IdPartnerSetting { get; set; }
        public string SettingName { get; set; }
        public string SettingValue { get; set; }

        [ForeignKey("PARTNER")]
        public System.Guid PARTNER_IdPartner { get; set; }

        public virtual PartnerEntity PARTNER { get; set; }
    }
}
