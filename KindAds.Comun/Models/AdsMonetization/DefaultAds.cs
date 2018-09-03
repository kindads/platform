using KindAds.Comun.Enums;
using KindAds.Comun.Models.CosmosDocuments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models
{
    public class DefaultAds : CosmosDbDocument
    {
        public string Name { set; get; }

        public string Site { set; get; }

        public string Status { set; get; }

        public string JSId { set; get; }

        public DefaultAds()
        {
            Name = string.Empty;
            Type = string.Empty;
            Status = string.Empty;
            JSId = string.Empty;

            Type = AdsMonetizationTypes.Default;
        }
    }
}
