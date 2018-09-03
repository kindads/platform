using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Negocio.ViewModels.KindAdsV2
{
    public class CampaignViewModel
    {
        public CampaignDocument campaign { set; get; }
        public List<PublisherPreferenceQuestionDocument> listQuestions { set; get; }

        public bool isWebSiteChecked { set; get; }
        public bool isEmailChecked { set; get; }
        public bool isPushNotificationChecked { set; get; }
        public string question { set; get; }

        public CampaignViewModel()
        {
            campaign = new CampaignDocument();
            listQuestions = new List<PublisherPreferenceQuestionDocument>();
        }

    }
}
