using Captivate.Comun.Models.CosmosDocuments;
using Captivate.Comun.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Comun.Models.AdsMonetization
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
