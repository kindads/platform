using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Common.Models.Entities
{
    [Table("PRODUCT_TYPE")]
    public class ProductTypeEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProductTypeEntity()
        {
            this.PARTNERS = new HashSet<PartnerEntity>();
            this.PRODUCTs = new HashSet<ProductEntity>();
            this.PARTNER_PRODUCT_SETTINGS = new HashSet<PartnerProductSettingsEntity>();
        }

        [Key]
        public System.Guid IdProductType { get; set; }
        public string Name { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PartnerEntity> PARTNERS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductEntity> PRODUCTs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PartnerProductSettingsEntity> PARTNER_PRODUCT_SETTINGS { get; set; }
    }
}
