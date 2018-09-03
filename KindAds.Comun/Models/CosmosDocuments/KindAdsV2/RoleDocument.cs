using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models.CosmosDocuments.KindAdsV2
{
    public class RoleDocument : KindAdsV2Document
    {
        public string Name { set; get; }

        public RoleDocument()
        {
            Name = string.Format("<NULL>");
        }
    }
}
