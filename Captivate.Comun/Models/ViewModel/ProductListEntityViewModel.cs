using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Common.Models.ViewModel
{
    public class ProductListEntityViewModel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProductListEntityViewModel()
        {
        }

 
        public System.Guid IdProduct { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }

        public bool IsActive { set; get; }


        #region FK
    
        public string AspNetUsers_Id { get; set; }

    
        public Guid SITE_IdSite { set; get; }

       
        public Guid PARTNER_IdPartner { set; get; }

       
        public Guid PRODUCT_TYPE_IdProductType { set; get; }
        #endregion


        public double Price { get; set; }
        public string Image { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public Nullable<System.DateTime> RegistrationDate { get; set; }

        public string PartnerName { get; set; }

        public string SiteName { get; set; }
    }
}
