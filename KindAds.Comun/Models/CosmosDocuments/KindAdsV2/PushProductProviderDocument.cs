using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models.CosmosDocuments.KindAdsV2
{
    public class PushProductProviderDocument : KindAdsV2Document
    {
        public string Name { set; get; }

        public PushProductProviderDocument()
        {
            Name = string.Format("<NULL>");
        }
    }
}
