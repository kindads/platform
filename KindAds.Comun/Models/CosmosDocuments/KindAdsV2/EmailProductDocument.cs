using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models.CosmosDocuments.KindAdsV2
{
    public class EmailProductDocument : KindAdsV2Document
    {
        public string AudiencePropertieId { set; get; }

        // tipo de proveedor
        public string EmailProductProviderId { set; get; }

        //public string Name { set; get; }

        public string SettingId { set; get; }

        public string Detail { get; set; }
        public DateTime? LastDetailUpdated { get; set; }
    }
}
