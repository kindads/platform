using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Comun.Models.Entities
{

    [Table("PRODUCT")]
    public partial class ProductEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProductEntity()
        {
            //this.CampaignEntitys = new HashSet<CampaignEntity>();
            //this.ProductSettingsEntitys = new HashSet<ProductSettingsEntity>();
        }

        [Key]
        public System.Guid IdProduct { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }

        public bool IsActive { set; get; }


        #region FK
        [ForeignKey("AspNetUser")]
        public string AspNetUsers_Id { get; set; }

        [ForeignKey("SITE")]
        public Guid SITE_IdSite { set; get; }

        [ForeignKey("PARTNER")]
        public Guid PARTNER_IdPartner { set; get; }

        [ForeignKey("PRODUCT_TYPE")]
        public Guid PRODUCT_TYPE_IdProductType { set; get; }
        #endregion


        public double Price { get; set; }
        public string Image { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public Nullable<System.DateTime> RegistrationDate { get; set; }

        public virtual SiteEntity SITE { set; get; }

        public virtual AspNetUserEntity AspNetUser { get; set; }
      
        public virtual PartnerEntity PARTNER { get; set; }
        public virtual ProductTypeEntity PRODUCT_TYPE { get; set; }


        #region Navigation property
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CampaignEntity> CampaignEntitys { get; set; }
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductSettingsEntity> ProductSettingsEntitys { get; set; }

        #endregion
    }
}
