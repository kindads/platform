using KindAds.Comun.Enums;
using KindAds.Comun.Models.CosmosDocuments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models
{
    public class StickyClicInfo : CosmosDbDocument
    {
        public StickyClicInfo()
        {
            Metric = AdsMonetizationMetrics.Click;
            Type = AdsMonetizationTypes.Sticky;
        }
    }
}
