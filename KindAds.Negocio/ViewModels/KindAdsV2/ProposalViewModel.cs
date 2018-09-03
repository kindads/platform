using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Negocio.ViewModels.KindAdsV2
{
    public class ProposalViewModel
    {
        public AudienceChannelDocument audienceChannel { set; get; }
        public AudienceDocument audience { set; get; }
        public string providerImageClass { set; get; }
        public List<ProposalQuestionViewModel> listQuestion { set; get; }
        public ProposalDocument proposal { set; get; }

        public string nameConversation { set; get; }

        public ProposalViewModel()
        {
            audience = new AudienceDocument();
            audienceChannel = new AudienceChannelDocument();
            providerImageClass = string.Empty;
            proposal = new ProposalDocument();
            listQuestion = new List<ProposalQuestionViewModel>();
        }

    }

    public class ProposalQuestionViewModel : QuestionAskToAudienceChannelDocument
    {
        public string Answer { set; get; }
    }
}
