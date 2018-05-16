using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Comun.Models.Entities
{
    [Table("PARTNER_PRODUCT_SETTINGS")]
    public class PartnerProductSettingsEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PartnerProductSettingsEntity()
        {
            this.PARTNERS = new HashSet<PartnerEntity>();
        }

        [Key]
        public System.Guid IdSetting { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        [ForeignKey("PRODUCT_TYPE")]
        public Guid PRODUCT_TYPE_IdProductType { set; get; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PartnerEntity> PARTNERS { get; set; }
        public virtual ProductTypeEntity PRODUCT_TYPE { get; set; }
    }
}
