using KindAds.Comun.Enums;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using KindAds.Negocio.ViewModels.KindAdsV2;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Negocio.Managersv2
{
    public class MarketplaceManager : BaseManager
    {
        private readonly AudienceChannelManager _audienceChannelManager;
        public MarketplaceManager()
        {
            databaseName = ConfigurationManager.AppSettings["azure-cosmos-databasename"];
            _audienceChannelManager = new AudienceChannelManager();
        }

        public bool DoYouHaveProfile(ProfilesTypes type, string userId)
        {
            bool result = false;
            if (ProfilesTypes.Advertiser == type)
            {
                collectionName = CosmosCollections.AdvertiserProfile.ToString();
                string query = $"SELECT * FROM {collectionName} WHERE {collectionName}.UserId='{userId}'";
                var profile = context.ExecuteQuery<AdvertiserProfileDocument>(databaseName, collectionName, query).FirstOrDefault();
                result = profile != null ? true : false;
            }
            else
            {
                collectionName = CosmosCollections.PublisherProfile.ToString();
                string query = $"SELECT * FROM {collectionName} WHERE {collectionName}.UserId='{userId}'";
                var profile = context.ExecuteQuery<PublisherProfileDocument>(databaseName, collectionName, query).FirstOrDefault();
                result = profile != null ? true : false;
            }
            return result;
        }

        public bool DoYouHaveALastOneAudience(string profileId)
        {
            bool result = false;
            collectionName = CosmosCollections.Audience.ToString();
            string query = $"SELECT * FROM {collectionName} WHERE {collectionName}.PublisherId='{profileId}' and {collectionName}.IsActive=true";
            var audience = context.ExecuteQuery<PublisherProfileDocument>(databaseName, collectionName, query).FirstOrDefault();
            result = audience != null ? true : false;
            return result;
        }

        

        public PublisherProfileDocument GetPublisherProfile(string userId)
        {
            collectionName = CosmosCollections.PublisherProfile.ToString();
            string query = $"SELECT * FROM {collectionName} WHERE {collectionName}.UserId='{userId}'";
            var profile = context.ExecuteQuery<PublisherProfileDocument>(databaseName, collectionName, query).FirstOrDefault();
            return profile;
        }

        public AdvertiserProfileDocument GetAdvertiserProfile(string userId)
        {
            collectionName = CosmosCollections.AdvertiserProfile.ToString();
            string query = $"SELECT * FROM {collectionName} WHERE {collectionName}.UserId='{userId}'";
            var profile = context.ExecuteQuery<AdvertiserProfileDocument>(databaseName, collectionName, query).FirstOrDefault();
            return profile;
        }

        public PublisherProfileDocument GetPublisherProfileById(string id)
        {
            collectionName = CosmosCollections.PublisherProfile.ToString();
            string query = $"SELECT * FROM {collectionName} WHERE {collectionName}.id='{id}'";
            var profile = context.ExecuteQuery<PublisherProfileDocument>(databaseName, collectionName, query).FirstOrDefault();
            return profile;
        }

        public bool AddProposal(ProposalViewModel model, string userId)
        {
            bool result = false;
            collectionName = CosmosCollections.Proposal.ToString();

            try
            {
                model.proposal.Id = Guid.NewGuid().ToString();
                model.proposal.AdvertiserProfileId = GetAdvertiserProfile(userId).Id;
                model.proposal.Price = _audienceChannelManager.GetAudienceChannelById(model.proposal.AudienceChannelId).Price;

                if (model.listQuestion != null && model.listQuestion.Any())
                {
                    foreach (var question in model.listQuestion)
                    {
                        model.proposal.Questions.Add(
                            new ProposalAnswerDocument {
                                Id = Guid.NewGuid().ToString(),
                                QuestionAskToAudienceChannelId = question.Id,
                                Question = question.Question,
                                Answer = question.Answer,
                                RegisterDate = DateTime.Now.ToString()});
                    }
                }

                context.AddDocument<ProposalDocument>(databaseName, CosmosCollections.Proposal.ToString(), model.proposal);

                result = true;
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return result;
        }

        public ProposalDocument GetProposalByChannelAndAdvetiserId(string AudienceChannelId, string AdvertiserProfileId)
        {
            var collectionProposal = CosmosCollections.Proposal.ToString();
            string query = $"SELECT * FROM {collectionProposal} WHERE {collectionProposal}.AudienceChannelId='{AudienceChannelId}' AND {collectionProposal}.AdvertiserProfileId='{AdvertiserProfileId}'";
            return context.ExecuteQuery<ProposalDocument>(databaseName, collectionProposal, query).FirstOrDefault();
        }

    }
}
