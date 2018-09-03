using KindAds.Azure;
using KindAds.Business;
using KindAds.Common.Interfaces;
using KindAds.Comun.Models;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using KindAds.Comun.Models.ViewModel;
using KindAds.Negocio.Managersv2;
using KindAds.Negocio.ViewModels.KindAdsV2;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace KindAds.Controllers
{
    public class ConversationController : Controller
    {
        ConversationManager Manager { set; get; }
        ITrace Telemetry { set; get; }
        NotificationManager notificationManager { set; get; }

        public ConversationController()
        {
            Manager = new ConversationManager();
            Telemetry = new Trace();
            notificationManager = new NotificationManager();
        }

        // GET: Conversation
        public ActionResult Home()
        {            
            KindAds.Utils.Enums.Roles rol;
            rol = (User.IsInRole(KindAds.Utils.Enums.Roles.Publisher.ToString()) ? KindAds.Utils.Enums.Roles.Publisher : KindAds.Utils.Enums.Roles.Advertiser);
            int rolId = Convert.ToInt32(rol);
            ConversationViewModel viewModel = new ConversationViewModel();
            var email = System.Web.HttpContext.Current.User.Identity.Name;
            viewModel.conversationItems = viewModel.GetAllConversations(email, rolId);
            return View(viewModel);
        }


        public PartialViewResult GetChatMessages(string Id)
        {
           
            string algo = string.Empty;
            ConversationViewModel viewModel = new ConversationViewModel();
            viewModel.conversationItems = viewModel.GetConversationById(Id);

            return PartialView("_ChatConversation", viewModel);
        }

        public ActionResult CreateConversationFromProposal(string conversationJSON)
        {
            bool result = true;
            ConversationViewModel viewModel = new ConversationViewModel();
            KindAds.Utils.Enums.Roles rol;
            rol = (User.IsInRole(KindAds.Utils.Enums.Roles.Publisher.ToString()) ? KindAds.Utils.Enums.Roles.Publisher : KindAds.Utils.Enums.Roles.Advertiser);
            int rolId = Convert.ToInt32(rol);
            var email = System.Web.HttpContext.Current.User.Identity.Name;
            try {
                ConversationParametersViewModel conversationParameters = JsonConvert.DeserializeObject<ConversationParametersViewModel>(conversationJSON);
                result=Manager.CreateConversation(conversationParameters);
                viewModel.conversationItems = viewModel.GetAllConversations(email, rolId);
            }
            catch (Exception e) {
                var messageException = Telemetry.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Telemetry.Critical(messageException);
            }      
            return View("Home",viewModel);
        }

       
        public ActionResult GoToProposal([FromBody]CurrentConversation proposalParametersNavigation)
        {
            CurrentConversation currentConversation = proposalParametersNavigation;
            ConversationDocument conversation = Manager.GetConversationByConversationId(currentConversation.Id);
            string UrlString = Url.Action("Proposal", "Marketplace", new { idAudienceChannel = conversation.AudienceChannelId });           
            return Json(new { url = UrlString }, JsonRequestBehavior.AllowGet);
        }

      
        public ActionResult AddMessage([FromBody] CurrentConversation proposalParametersNavigation)
        {
            proposalParametersNavigation.MessageWithoutHtml = proposalParametersNavigation.Message;
            proposalParametersNavigation.Message = Manager.MakeRefIfContainLink(proposalParametersNavigation.Message);
            string serviceBusTopicName = "conversationtopicdev"; // obtenerlo del web.config
            string profileName = string.Empty;
            PublisherProfileDocument publisherProfile = new PublisherProfileDocument();
            AdvertiserProfileDocument advertiserProfile = new AdvertiserProfileDocument();

            List<string> usersIdInConversation = Manager.GetUsersIdInConversation(proposalParametersNavigation.Id);
            var email = System.Web.HttpContext.Current.User.Identity.Name;
            KindAds.Utils.Enums.Roles rol;
            rol = (User.IsInRole(KindAds.Utils.Enums.Roles.Publisher.ToString()) ? KindAds.Utils.Enums.Roles.Publisher : KindAds.Utils.Enums.Roles.Advertiser);
            int rolId = Convert.ToInt32(rol);
            if(rolId==1) {
                publisherProfile = Manager.GetPublisherProfile(email);
                profileName = publisherProfile.Name;
            }
            else {
                advertiserProfile = Manager.GetAdvertiserProfile(email);
                profileName = advertiserProfile.Title;
            }
           
            proposalParametersNavigation.ProfileName = profileName;
            ConversationMessageNotification message = new ConversationMessageNotification() {
                currentConversation = proposalParametersNavigation,
                users= usersIdInConversation
            };

            Manager.AddMessageToCosmos(message);
            notificationManager = new NotificationManager(serviceBusTopicName);
            notificationManager.ProcessConversationNotification(message);
            notificationManager.Close();

            return Json(new { Result = "Send message OK" }, JsonRequestBehavior.AllowGet);
        }
    }
}
