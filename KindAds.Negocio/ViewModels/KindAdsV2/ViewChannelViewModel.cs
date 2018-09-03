using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Negocio.ViewModels.KindAdsV2
{
    public class ViewChannelViewModel
    {
        public AudienceChannelDocument audienceChannel { set; get; }
        public AudienceDocument audience { set; get; }

        public string providerImageClass { set; get; }

        public ViewChannelViewModel()
        {
            audience = new AudienceDocument();
            audienceChannel = new AudienceChannelDocument();
            providerImageClass = string.Empty;
        }
    }
}
