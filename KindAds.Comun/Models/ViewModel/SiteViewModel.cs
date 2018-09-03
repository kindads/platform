using KindAds.Common.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KindAds.Common.Models.ViewModel
{
    public class SiteViewModel : CreateSite
    {
        public TxtSiteValidation txt { set; get; }

        public AzureADSiteValidation ad { set; get; }

        public GoogleTagManagerValidation gtm { set; get; }

        public int type { set; get; }

        #region session properties
        public string ProductTypeSelecc { set; get; }
        public string CategorySelecc { set; get; }

        public string TagSelecc { set; get; }

        public string PartnerSelecc { set; get; }

        public string SiteSelecc { set; get; }

        public string IdSite { set; get; }
       
        #endregion

        #region view properties
        [Display(Name = "Categories")]
        [Required(ErrorMessage = "Category is required")]
        public short categorySelected { set; get; }

        [Display(Name= "Protocols")]
        public string protocoloSelected { set; get; }
        #endregion



        public SiteViewModel()
        {
            txt = new TxtSiteValidation();
            ad = new AzureADSiteValidation();
            gtm = new GoogleTagManagerValidation();
        }


       
    }
}
