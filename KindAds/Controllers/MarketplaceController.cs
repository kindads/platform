using KindAds.AuthorizeAttributes;
using KindAds.Business;
using KindAds.Comun.Models;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using KindAds.Comun.Models.ViewModel;
using KindAds.Negocio.Managersv2;
using KindAds.Negocio.ViewModels.KindAdsV2;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KindAds.Controllers {
    [Authorize]
    [ValidProfile]
    public class MarketplaceController : Controller {
        MarketplaceManager manager { set; get; }
        AudienceChannelManager audienceChannelManager { set; get; }
        AudienceManager audienceManager { set; get; }
        NotificationManager notificationManager { set; get; }

        public MarketplaceController()
        {
            manager = new MarketplaceManager();
            audienceChannelManager = new AudienceChannelManager();
            audienceManager = new AudienceManager();
        }

        // GET: Marketplace
        public ActionResult Advertiser()
        {
            MarketplaceViewModel viewModel = new MarketplaceViewModel();
            string userId = User.Identity.GetUserId();

            //Checamos si ya tiene un advertiser profile
            bool existProfile = manager.DoYouHaveProfile(Comun.Enums.ProfilesTypes.Advertiser, userId);

            ////if(existProfile) {
            ////    var profile = manager.GetAdvertiserProfile(userId);
            ////    //Checamos si tiene al menos una audiencia
            ////    var existAudience = manager.DoYouHaveALastOneAudience(profile.Id);
            ////    viewModel.needAudience = !existAudience;
            ////}


            //llenamos el modelo
            viewModel.needProfile = !existProfile;
            return View(viewModel);
        }


        public ActionResult Publisher()
        {
            MarketplaceViewModel viewModel = new MarketplaceViewModel();
            string userId = User.Identity.GetUserId();

            //Checar si ya tiene un publihserProfileMediante el UserId
            bool existProfile = manager.DoYouHaveProfile(Comun.Enums.ProfilesTypes.Publisher, userId);

            if (existProfile) {
                var profile = manager.GetPublisherProfile(userId);
                // checamos si tiene al menos una audiencia
                var existAudience = manager.DoYouHaveALastOneAudience(profile.Id);
                viewModel.needAudience = !existAudience;
            }


            //llenamos el model
            viewModel.needProfile = !existProfile;
            return View(viewModel);
        }

        public ActionResult GetMarketplaceChannelsAdvertiser()
        {
            MarketplaceChannelViewModel model = new MarketplaceChannelViewModel() {
                listChannel = audienceChannelManager.GetAudienceChannelForAdvertiser()
            };

            ViewBag.NoChannelNoPremium = (model.listChannel != null && model.listChannel.Any()) ? (from r in model.listChannel where r.IsPremium == false select r).Count().ToString() : "0";
            ViewBag.NoChannelPremium = (model.listChannel != null && model.listChannel.Any()) ? (from r in model.listChannel where r.IsPremium == true select r).Count().ToString() : "0";
            return PartialView("_MarketplaceChannels", model);
        }

        public ActionResult GetMarketplaceChannelsPublisher()
        {
            string userId = User.Identity.GetUserId();
            MarketplaceChannelViewModel model = new MarketplaceChannelViewModel() {
                listChannel = audienceChannelManager.GetAudienceChannelForPublisher(userId)
            };

            ViewBag.NoChannelNoPremium = (model.listChannel != null && model.listChannel.Any()) ? (from r in model.listChannel where r.IsPremium == false select r).Count().ToString() : "0";
            ViewBag.NoChannelPremium = (model.listChannel != null && model.listChannel.Any()) ? (from r in model.listChannel where r.IsPremium == true select r).Count().ToString() : "0";
            return PartialView("_MarketplaceChannels", model);
        }

        // GET: Marketplace
        public ActionResult ViewChannel(string idAudienceChannel)
        {            
            var audienceChannel = audienceChannelManager.GetAudienceChannelById(idAudienceChannel);
            var audience = audienceManager.GetAudienceById(audienceChannel.AudienceId);
            var imageProviderCss = audienceChannelManager.GetProviderImageCss(audienceChannel.ProductProviderId);

            ViewChannelViewModel viewModel = new ViewChannelViewModel() {
                audienceChannel = audienceChannel,
                audience = audience,
                providerImageClass = imageProviderCss
            };
            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Proposal(string idAudienceChannel)
        {
            TempData["idAudienceChannel"] = idAudienceChannel;
            var audienceChannel = audienceChannelManager.GetAudienceChannelById(idAudienceChannel);
            var audience = audienceManager.GetAudienceById(audienceChannel.AudienceId);
            var imageProviderCss = audienceChannelManager.GetProviderImageCss(audienceChannel.ProductProviderId);
            var proposal = ExistProposal(idAudienceChannel);
            bool existProposal = proposal != null && !string.IsNullOrEmpty(proposal.Id) && proposal.Questions.Any();
            ViewBag.existProposal = existProposal;

            ProposalViewModel viewModel = new ProposalViewModel() {
                audienceChannel = audienceChannel,
                audience = audience,
                providerImageClass = imageProviderCss,
                listQuestion = existProposal ? ParseExistProposalQuestions(proposal.Questions) : ParseProposalQuestions(audienceChannelManager.GetQuestions(audienceChannel.Id)),
                proposal = existProposal ? proposal : new ProposalDocument()
            };

            return View(viewModel);
        }

        private ProposalDocument ExistProposal(string idAudienceChannel)
        {
            var advertiserProfile = manager.GetAdvertiserProfile(User.Identity.GetUserId());

            if(advertiserProfile != null && !string.IsNullOrEmpty(advertiserProfile.Id))
            {
              return manager.GetProposalByChannelAndAdvetiserId(idAudienceChannel, advertiserProfile.Id);
            }

            return null;
        }

        [HttpPost]
        public ActionResult Proposal(ProposalViewModel viewModel)
        {
            ConversationParametersViewModel conversationParameters = new ConversationParametersViewModel();
            viewModel.proposal.AudienceChannelId = TempData["idAudienceChannel"].ToString();
            var operationResult = manager.AddProposal(viewModel, User.Identity.GetUserId());
            string conversationParametersJson = string.Empty;
            if (operationResult) {
                string serviceBusQueueName = "proposaltopicdev";
                var audienceChannel = audienceChannelManager.GetAudienceChannelById(viewModel.proposal.AudienceChannelId);
                var audience = audienceManager.GetAudienceById(audienceChannel.AudienceId);
                var publisherProfile = manager.GetPublisherProfileById(audience.PublisherId);
                var advertiserProfile = manager.GetAdvertiserProfile(User.Identity.GetUserId());

                conversationParameters.NameConversation = publisherProfile.Name +"_" + audienceChannel.Name + "_" + advertiserProfile.Title;
                conversationParameters.AdvertiserId = advertiserProfile.Id;
                conversationParameters.PublisherId = publisherProfile.Id;
                conversationParameters.AudienceChannelId = audienceChannel.Id;

                conversationParametersJson = JsonConvert.SerializeObject(conversationParameters);

                ProposalNotification proposalNotification = new ProposalNotification() {
                    idUser = publisherProfile.UserId,
                    message = "Proposal created successfully"
                };

                string message = JsonConvert.SerializeObject(proposalNotification);
                notificationManager = new NotificationManager(serviceBusQueueName);
                notificationManager.ProcessProposalNotification(message, new Dictionary<string, int>());
                notificationManager.Close();
            }
            return operationResult ? Json(new { success = true, message = "Proposal created successfully", conversationParameters = conversationParametersJson, proposalId = viewModel.proposal.Id }) : Json(new { error = "Error creating Proposal" });
        }

        private List<ProposalQuestionViewModel> ParseProposalQuestions(List<QuestionAskToAudienceChannelDocument> listQuestion)
        {
            List<ProposalQuestionViewModel> listProposalQuestion = new List<ProposalQuestionViewModel>();
            ProposalQuestionViewModel proposalQuestion;
            if (listQuestion != null && listQuestion.Any()) {
                foreach (var question in listQuestion) {
                    proposalQuestion = new ProposalQuestionViewModel() {
                        Id = question.Id,
                        AudienceChannelId = question.AudienceChannelId,
                        Question = question.Question,
                        RegisterDate = question.RegisterDate
                    };

                    listProposalQuestion.Add(proposalQuestion);
                }
            }
            return listProposalQuestion;
        }

        private List<ProposalQuestionViewModel> ParseExistProposalQuestions(List<ProposalAnswerDocument> listQuestion)
        {
            List<ProposalQuestionViewModel> listProposalQuestion = new List<ProposalQuestionViewModel>();
            ProposalQuestionViewModel proposalQuestion;
            if (listQuestion != null && listQuestion.Any()) {
                foreach (var question in listQuestion) {
                    proposalQuestion = new ProposalQuestionViewModel() {
                        Question = question.Question,
                        Answer = question.Answer
                    };

                    listProposalQuestion.Add(proposalQuestion);
                }
            }
            return listProposalQuestion;
        }

    }
}
