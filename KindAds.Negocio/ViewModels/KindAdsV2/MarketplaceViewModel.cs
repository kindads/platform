using KindAds.Azure;
using KindAds.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Negocio.ViewModels.KindAdsV2
{
    public class MarketplaceViewModel : ITelemetria
    {
        public ITrace telemetria { set; get; }

        public bool needProfile { set; get; }

        public bool needAudience { set; get; }

        public bool needALastOneCampaign { set; get; }

        public bool needALastOneChannel { set; get; }

        public bool needAWallet { set; get; }

        public MarketplaceViewModel()
        {
            telemetria = new Trace();

            needProfile = true;
            needAudience = true;
            needALastOneCampaign = true;
            needALastOneChannel = true;
            needAWallet = true;
        }
    }
}
