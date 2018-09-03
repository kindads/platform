using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using KindAds.Negocio.Managersv2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Negocio.ViewModels.KindAdsV2
{
    public class AudienceListItemViewModel 
    {
        private readonly AudienceChannelManager _audienceManager;


        public AudienceDocument audience { set; get; }
        public List<AudienceChannelDocument> Channels
        {
            get
            {


                List<AudienceChannelDocument> audiencechannelVM = _audienceManager.GetAudienceChannelsLitsVMByAudience(audience.Id).ToList();

              

                return audiencechannelVM;
            }
        }
        

        public AudienceListItemViewModel()
        {
            audience = new AudienceDocument();
            _audienceManager = new AudienceChannelManager();
        }

        
    }
}
