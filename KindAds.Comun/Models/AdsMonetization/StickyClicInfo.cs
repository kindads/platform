using Captivate.Comun.Enums;
using Captivate.Comun.Models.CosmosDocuments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Comun.Models
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
