using KindAds.Comun.Enums;
using KindAds.Comun.Models.CosmosDocuments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models
{
    public class BrowserInfo : CosmosDbDocument
    {
        public string BrowserName { set; get; }
        public string BrowserVersion { set; get; }
        public string Language { set; get; }
        public string Platform { set; get; }
        public string UserAgent { set; get; }

        public BrowserInfo()
        {
            BrowserName = string.Empty;
            BrowserVersion = string.Empty;
            Language = string.Empty;
            Platform = string.Empty;
            UserAgent = string.Empty;

            Metric = AdsMonetizationMetrics.Impression;
            Type = AdsMonetizationTypes.Telemetry;
        }
    }
}
