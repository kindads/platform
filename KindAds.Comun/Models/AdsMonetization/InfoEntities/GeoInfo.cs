using KindAds.Comun.Enums;
using KindAds.Comun.Models.CosmosDocuments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models
{
    public class GeoInfo : CosmosDbDocument
    {
        public string latitude { set; get; }

        public string longitude { set; get; }

        public string Address { set; get; }

        public GeoInfo()
        {
            latitude = string.Empty;
            longitude = string.Empty;
            Address = string.Empty;

            Metric = AdsMonetizationMetrics.Impression;
            Type = AdsMonetizationTypes.Telemetry;
        }
    }
}
