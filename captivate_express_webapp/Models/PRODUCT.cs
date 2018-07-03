//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace captivate_express_webapp.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class PRODUCT
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PRODUCT()
        {
            this.CAMPAIGNs = new HashSet<CAMPAIGN>();
            this.PRODUCT_SETTINGS = new HashSet<PRODUCT_SETTINGS>();
        }
    
        public System.Guid IdProduct { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string AspNetUsers_Id { get; set; }
        public double Price { get; set; }
        public string Image { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public Nullable<System.DateTime> RegistrationDate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public bool IsPremium { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual SITE SITE { get; set; }
        public virtual PARTNER PARTNER { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CAMPAIGN> CAMPAIGNs { get; set; }
        public virtual PRODUCT_TYPE PRODUCT_TYPE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUCT_SETTINGS> PRODUCT_SETTINGS { get; set; }
    }
}
