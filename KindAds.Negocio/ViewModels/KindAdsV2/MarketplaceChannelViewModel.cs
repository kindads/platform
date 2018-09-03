using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Negocio.ViewModels.KindAdsV2
{
    public class MarketplaceChannelViewModel
    {
        public List<AudienceChannelDocument> listChannel = new List<AudienceChannelDocument>();
    }

    public class ChannelViewModel
    {
        public AudienceDocument audience { set; get; }
        public AudienceChannelDocument audienceChannel { set; get; }
        


        public ChannelViewModel()
        {
            audienceChannel = new AudienceChannelDocument();
            audience = new AudienceDocument();
        }
    }
}
