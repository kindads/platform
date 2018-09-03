using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KindAds.Business.Managers;
using KindAds.Comun.Enums;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using KindAds.Comun.Utils;
using KindAds.Negocio.ViewModels.KindAdsV2;

namespace KindAds.Negocio.Managersv2
{
    public class ProposalManager : BaseManager
    {
        private readonly AdvertiserProfileManager _advertiserProfileManager;
        private readonly CosmosIdentityManager _identityManager;
        private readonly AudienceChannelManager _audienceChannelManager;
        private readonly CatalogManager _catalogManager;

        public ProposalManager() : base()
        {
            databaseName = ConfigurationManager.AppSettings["azure-cosmos-databasename"];
            _advertiserProfileManager = new AdvertiserProfileManager();
            _identityManager = new CosmosIdentityManager();
            _audienceChannelManager = new AudienceChannelManager();
            _catalogManager = new CatalogManager();
        }


        public List<ProposalReviewListItemViewModel> GetProposalsByAudienceChannelId(string audienceChannelId)
        {
            string collectionName = CosmosCollections.Proposal.ToString();
            string queryGetProposal = $"SELECT * FROM {collectionName} WHERE {collectionName}.AudienceChannelId='{audienceChannelId}'";
            List<ProposalDocument> proposals = context.ExecuteQuery<ProposalDocument>(this.databaseName, CosmosCollections.Proposal.ToString(), queryGetProposal);
            AudienceChannelDocument audienceChannel = _audienceChannelManager.GetAudienceChannelById(audienceChannelId);

            List<ProposalReviewListItemViewModel> results = proposals.Select(p => MapToProposalReviewListItemViewModel(p, audienceChannel)).ToList();
            return results;
        }

        public int GetNoProposalsByAudienceChannelId(string audienceChannelId)
        {
            string collectionName = CosmosCollections.Proposal.ToString();
            string queryGetProposal = $"SELECT * FROM {collectionName} WHERE {collectionName}.AudienceChannelId='{audienceChannelId}'";
            List<ProposalDocument> proposals = context.ExecuteQuery<ProposalDocument>(this.databaseName, CosmosCollections.Proposal.ToString(), queryGetProposal); 
            return proposals.Count();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="proposal"></param>
        /// <param name="audienceChannel">Si se le pasa un audienceChannel este se usara para setear price and productName, sino se consultara uno a la BD</param>
        /// <param name=""></param>
        /// <returns></returns>
        private ProposalReviewListItemViewModel MapToProposalReviewListItemViewModel(ProposalDocument proposal, AudienceChannelDocument audienceChannel=null)
        {
            if (audienceChannel == null)
            {
                audienceChannel = _audienceChannelManager.GetAudienceChannelById(proposal.AudienceChannelId);
            }

            
            AdvertiserProfileDocument advertiserProfile = _advertiserProfileManager.FindProfileByProfileId(proposal.AdvertiserProfileId);
            ApplicationUser user = _identityManager.FindUserByUserId(advertiserProfile.UserId);
            CountryDocument country = _catalogManager.FindCountryById(advertiserProfile.CountryBusinessInId);


            DateTime registerProposal = Convert.ToDateTime(proposal.RegisterDate);

            string messageDaysAgo = DateUtils.GetTextDaysAgo(registerProposal);
           

            return new ProposalReviewListItemViewModel()
            {
                TimeAgoReceived = messageDaysAgo,
                AdvertiserImageSrc = advertiserProfile.IconUrl,
                AdvertiserName = user.Name,
                IdProposal= proposal.Id,
                Location = country!=null?country.Name:"",
                Price = audienceChannel.Price.ToString() + " kind",
                ProductName = audienceChannel.Name

            };
        }
    }
}
