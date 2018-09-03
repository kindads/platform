using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Negocio.ViewModels.KindAdsV2
{
    public class PublisherProposalDetailViewModel
    {
        public string ProposalId { get; set; }

        public string AdvertiserName { get; set; }

        public string AdvertiserImage { get; set; }

        public string AdvertiserLocation { get; set; }

        public string WebSite { get; set; }

        public string MemberSinceYear { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public double Price { get; set; }

        public string RejectDetail { get; set; }

        public bool? Accepted { get; set; }

        public List<ProposalAnswerDocument> QuestionsAndAnswers { get; set; }
    }
}
