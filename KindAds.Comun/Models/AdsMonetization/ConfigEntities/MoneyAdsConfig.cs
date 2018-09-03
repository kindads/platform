using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models
{
    public class MoneyAdsConfig
    {
        public string IdUser { set; get; }

        public int EnableDeviceMetrics { set; get; }

        public int EnableGeoLocationMetrics { set; get; }

        public int EnableTimeMetricsScript { set; get; }

        public int EnableInjectedAds { set; get; }

        public MoneyAdsConfig()
        {
            EnableDeviceMetrics = 1;
            EnableGeoLocationMetrics = 1;
            EnableTimeMetricsScript = 1;
            EnableInjectedAds = 1;
        }
    }
}
