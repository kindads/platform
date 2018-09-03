using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models.CosmosDocuments.KindAdsV2
{
    public class IndustryDocument: KindAdsV2Document
    {
        public string Name { set; get; }

        public List<IndustrySubcategoryDocument> SubIndustries { get; set; }
        public IndustryDocument()
        {
            SubIndustries = new List<IndustrySubcategoryDocument>();
            Name = string.Format("<NULL>");
        }
    }


    public class ProtocolDocument : KindAdsV2Document
    {
        public string Name { set; get; }

        public ProtocolDocument()
        {
            Name = string.Empty;
            Id = string.Empty;
        }
    }
}
