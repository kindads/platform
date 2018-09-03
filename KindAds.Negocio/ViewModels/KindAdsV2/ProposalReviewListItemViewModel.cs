using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Negocio.ViewModels.KindAdsV2
{
    public class ProposalReviewListItemViewModel
    {
        public string IdProposal { get; set; }

        public string ProductName { get; set; }

        public string AdvertiserImageSrc { get; set; } 

        public string TimeAgoReceived { get; set; }

        public string noMessages { get; set; }

        public string previewMessage { get; set; }

        public string AdvertiserName { get; set; }

        public string Price { get; set; }

        public string Location { get; set; }
    }
}
