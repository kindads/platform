using KindAds.Comun.Models.CosmosDocuments;
using KindAds.Comun.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models.AdsMonetization
{
    public class DefaultClicInfo : CosmosDbDocument
    {
        public DefaultClicInfo()
        {
            Metric = AdsMonetizationMetrics.Click;
            Type = AdsMonetizationTypes.Default;
        }
    }
}
