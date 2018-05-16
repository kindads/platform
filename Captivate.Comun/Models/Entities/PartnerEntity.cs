using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Comun.Models.Entities
{
    [Table("PARTNER")]
    public class PartnerEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PartnerEntity()
        {
            this.PARTNER_PRODUCT_SETTINGS = new HashSet<PartnerProductSettingsEntity>();
            this.PRODUCT_TYPE = new HashSet<ProductTypeEntity>();
            this.PRODUCTs = new HashSet<ProductEntity>();
            this.PARTNER_SETTINGS = new HashSet<PartnerSettingsEntity>();
        }

        [Key]
        public System.Guid IdPartner { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PartnerProductSettingsEntity> PARTNER_PRODUCT_SETTINGS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductTypeEntity> PRODUCT_TYPE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductEntity> PRODUCTs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PartnerSettingsEntity> PARTNER_SETTINGS { get; set; }
    }
}
