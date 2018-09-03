using KindAds.Common.Models;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Negocio.ViewModels.KindAdsV2
{
    public class VerifyAudienceViewModel
    {
        public TxtSiteValidation txt { set; get; }

        public AzureADSiteValidation ad { set; get; }

        public GoogleTagManagerValidation gtm { set; get; }

        public AudienceDocument audience { set; get; }

        public string TypeValidation { set; get; }

        public VerifyAudienceViewModel()
        {
            txt = new TxtSiteValidation();
            ad = new AzureADSiteValidation();
            gtm = new GoogleTagManagerValidation();

            audience = new AudienceDocument();
        }
    }
}
