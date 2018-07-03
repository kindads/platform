using Captivate.Comun.Enums;
using Captivate.Comun.Models.CosmosDocuments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Comun.Models
{
    public class MobileInfo : CosmosDbDocument
    {
        public string Mobile { set; get; }
        public string Phone { set; get; }
        public string Tablet { set; get; }
        public string Os { set; get; }

        public MobileInfo()
        {
            Mobile = string.Empty;
            Phone = string.Empty;
            Tablet = string.Empty;
            Os = string.Empty;
            Ip = string.Empty;

            Metric = AdsMonetizationMetrics.Impression;
            Type = AdsMonetizationTypes.Telemetry;
        }
    }
}
