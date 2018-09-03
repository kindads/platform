using KindAds.Comun.Enums;
using KindAds.Comun.Models.CosmosDocuments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models
{    
    public class StickyImpressionInfo : CosmosDbDocument
    {
        public string Ip { set; get; }

        public StickyImpressionInfo()
        {
            Metric = AdsMonetizationMetrics.Impression;
            Type = AdsMonetizationTypes.Sticky;
        }
    }
}
