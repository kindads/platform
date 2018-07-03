using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Common.Models.Entities
{
    public class SiteEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SiteEntity()
        {
        
        }


        public System.Guid IdSite { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }

        public string Token { set; get; }

    
        public string AspNetUsers_Id { get; set; }
        public Nullable<bool> Verified { get; set; }
        public string VerificationString { get; set; }
        public Nullable<System.DateTime> RegistrationDate { get; set; }
        public Nullable<bool> IsActive { get; set; }

        public virtual AspNetUserEntity AspNetUser { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductEntity> PRODUCTs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CategoryEntity> CATEGORYs { get; set; }
    }
}
