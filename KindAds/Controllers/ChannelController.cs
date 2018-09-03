using createsend_dotnet;
using KindAds.Azure;
using KindAds.ChannelFactory.AbstractFactory;
using KindAds.Common.Interfaces;
using KindAds.Common.Utils;
using KindAds.Comun.Enums;
using KindAds.Comun.Interfaces;
using KindAds.Comun.Models;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using KindAds.Comun.Models.KindAdsV2;
using KindAds.Comun.Structures;
using KindAds.Comun.Utils;
using KindAds.Negocio.Managersv2;
using KindAds.Negocio.Partnersv2.Mail;
using KindAds.Negocio.Partnersv2.Push;
using KindAds.Negocio.ViewModels.KindAdsV2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;


namespace KindAds.Controllers {
    public class ChannelController : BaseController {
        private AudienceManager audienceManager;
        private AudienceChannelManager _audienceChannelManager;

        private readonly ProposalManager _proposalManager;

        public ChannelController()
        {
            audienceManager = new AudienceManager();
            _audienceChannelManager = new AudienceChannelManager();
            _proposalManager = new ProposalManager();
            telemetria = new Trace();

        }

        // GET: Channel
        private List<QuestionAskToAudienceChannelDocument> listQuestions;

        #region CRUD channel
        [System.Web.Mvc.HttpGet]
        public ActionResult CreateChannel([FromBody] string audienceId)
        {
            audienceManager.audienceId = audienceId;
            AudienceDocument audience = audienceManager.GetAudienceById(audienceManager.audienceId);
            TempData["tdIdAudience"] = audienceManager.audienceId;
            ViewBag.ImageSrcPreview = audience.ImageUrl;
            ViewBag.descriptionPreview = audience.Description;
            ViewBag.site = audience.WebSiteUrl;
            AudienceChannelViewModel model = new AudienceChannelViewModel();
            if (!string.IsNullOrEmpty(audienceId)) {
                TempData["tdIdAudience"] = audienceId;
            }
            return View(model);
        }

        public ActionResult Channel(string audienceId, string audienceChannelId, string channelTypeId)
        {
            audienceManager.audienceId = audienceId;
            AudienceDocument audience = audienceManager.GetAudienceById(audienceManager.audienceId);
            TempData["tdIdAudience"] = audienceManager.audienceId;

            ViewBag.site = audience.WebSiteUrl;
            ViewBag.channelTypeId = channelTypeId;
            ViewBag.audienceName = audience.Title;
            ViewBag.audienceImage = audience.ImageUrl;
            ViewBag.siteUrl = audience.WebSiteUrl;

            ViewBag.NoProposals = this._proposalManager.GetNoProposalsByAudienceChannelId(audienceChannelId);

            if (!string.IsNullOrEmpty(audienceId)) {
                TempData["tdIdAudience"] = audienceId;
            }

            ViewBag.audienceChannelId = audienceChannelId;
            return View();
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult Channel([FromBody] AudienceChannelViewModel model)
        {
            try {
                model.channel.Description = Negocio.Utilerias.StringUtilities.RemoveNewLineInString(model.channel.Description);

                //List<QuestionAskToAudienceChannelDocument> newQuestions = (List<QuestionAskToAudienceChannelDocument>)TempData["vdListQuestions"];

                AudienceChannelDocument audienceChannelSaved = this._audienceChannelManager.GetAudienceChannelById(model.channel.Id);
                bool changeImage = model.channel.IsDefaultImage != audienceChannelSaved.IsDefaultImage;
                bool changeDescription = model.channel.IsDefaultDescription != audienceChannelSaved.IsDefaultDescription;

                string newImageUrl = string.Empty;
                string newDescription = string.Empty;
                AudienceDocument audience = null;
                if (changeImage) {
                    if (model.channel.IsDefaultImage) {
                        audience = this.audienceManager.GetAudienceById(audienceChannelSaved.AudienceId);
                        newImageUrl = audience.ImageUrl;
                    }
                    else {
                        //Si la imagen es seleccionada por el usuario
                        newImageUrl = BindFile(model);
                        if (string.IsNullOrEmpty( newImageUrl) ) {
                            newImageUrl = audienceChannelSaved.ImageUrl;
                        }
                    }
                }
                else {
                    if(model.channel.IsDefaultImage) {

                        newImageUrl = audienceChannelSaved.ImageUrl;
                    }
                    else {
                        //Si la imagen es seleccionada por el usuario
                        newImageUrl = BindFile(model);
                       
                        if (string.IsNullOrEmpty(newImageUrl)) {
                            if (string.IsNullOrEmpty(audienceChannelSaved.ImageUrl)) {
                                audience = this.audienceManager.GetAudienceById(audienceChannelSaved.AudienceId);
                                newImageUrl = audience.ImageUrl;
                            }
                            else {
                                newImageUrl = audienceChannelSaved.ImageUrl;
                            }
                           
                        }
                    }
                }

                if (changeDescription) {
                    if (model.channel.IsDefaultDescription) {
                        audience = audience == null ? this.audienceManager.GetAudienceById(audienceChannelSaved.AudienceId) : audience;
                        newDescription = audience.Description;
                    }
                }

                audienceChannelSaved.Price = model.channel.Price;
                audienceChannelSaved.Name = model.channel.Name;
                audienceChannelSaved.TagLine = model.channel.TagLine;
                audienceChannelSaved.ImageUrl = newImageUrl ;
                audienceChannelSaved.Description = changeDescription ? (model.channel.IsDefaultDescription ? newDescription : Negocio.Utilerias.StringUtilities.RemoveNewLineInString(model.channel.Description)) : audienceChannelSaved.Description;
                audienceChannelSaved.IsDefaultImage = model.channel.IsDefaultImage;
                audienceChannelSaved.IsDefaultDescription = model.channel.IsDefaultDescription;
                audienceChannelSaved.Visibility = model.channel.Visibility;
                model.channel = audienceChannelSaved;
                //model.listQuestions = newQuestions;
                this._audienceChannelManager.UpdateAudienceChannel(model);
                return Json(new { success = true, message = "Audience Channel edited successfully" });
            }
            catch {
                return Json(new { error = "Error creating Audience Channel" });
            }
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult CreateChannel([FromBody]AudienceChannelViewModel model)
        {
            string audienceId = TempData["tdIdAudience"].ToString();
            Guid productId = Guid.NewGuid();
            Guid audienceChannelId = Guid.NewGuid();
            Guid settingId = Guid.NewGuid();
            AudienceDocument audience = _audienceChannelManager.getAudienceById(audienceId);
            (string channelTypeId, string channelTypeName) channelType = ("", "");
            string detail = string.Empty;

            if (model.EmailProviderSelected != string.Empty && model.EmailProviderSelected != null) {
                AudienceChannelViewModel viewModel = new AudienceChannelViewModel();

                if (model.EmailProviderSelected == ChannelProvider.MailChimpId) {
                    detail = viewModel.GetSubscribersMailChimp(model.emailProductForm.mailChimpForm.ApiToken, model.emailProductForm.mailChimpForm.ListId);
                }
                else if (model.EmailProviderSelected == ChannelProvider.CampaignMonitorId) {
                    detail = viewModel.GetSubscribersCampaignMonitor(model.emailProductForm.campaignMonitorForm.ApiToken, model.emailProductForm.campaignMonitorForm.ListId);
                }
                else if (model.EmailProviderSelected == ChannelProvider.SendGridId) {
                    detail = viewModel.GetSubscribersSendGrid(model.emailProductForm.sendGridForm.ApiToken, model.emailProductForm.sendGridForm.ListId);
                }
                else if (model.EmailProviderSelected == ChannelProvider.AweberId) {
                    detail = viewModel.GetSubscribersAweber((ProviderAWeberApiResult)TempData["AweberApiTokenData"], model.emailProductForm.aweberForm.ListId);
                }

                channelType = KindAds.Comun.Structures.SChannelType.Email;
                model.channel.ChannelId = channelType.channelTypeId;
                _audienceChannelManager.AddEmailProduct(model, audienceChannelId.ToString(), settingId.ToString(), productId.ToString(), detail);
                model.channel.Id = audienceChannelId.ToString();
                model.channel.ProductId = productId.ToString();
                _audienceChannelManager.AddEmailProductSettings(audience.WebSiteUrl, model, productId.ToString());
            }

            if (model.PushProviderSelected != string.Empty && model.PushProviderSelected != null) {
                channelType = KindAds.Comun.Structures.SChannelType.PushNotification;
                model.channel.ChannelId = channelType.channelTypeId;
                _audienceChannelManager.AddPushProduct(model, audienceChannelId.ToString(), settingId.ToString(), productId.ToString(), string.Empty);
                model.channel.Id = audienceChannelId.ToString();
                model.channel.ProductId = productId.ToString();
                _audienceChannelManager.AddPushProductSettings(audience.WebSiteUrl, model, productId.ToString());
                detail = "";
            }
            if (model.WebSpaceProviderSelected != string.Empty) {

                channelType = KindAds.Comun.Structures.SChannelType.WebsiteAdSpace;
                //todo
            }

            string productProviderId = _audienceChannelManager.GetProductProviderId(model);
            // add AudienceChannelDocument and Questions
            var selectedCat = audienceManager.GetIndustrieById(audience.CategoryId);
            BindQuestionsImageDescriptionAudienceChannelModel(model);
            model.channel.AudienceId = audienceId;
            model.channel.Visibility = audience.Visibility ? model.channel.Visibility : audience.Visibility;
            model.channel.IsActive = audience.Verified;
            model.channel.Detail = detail;
            model.channel.LastProviderDataUpdated = DateUtils.GetMexicanDateTimeNow();
            model.channel.Category = selectedCat.Name;
            model.channel.SubCategory = selectedCat.SubIndustries.Where(s => s.Id == audience.SubCategoryId).SingleOrDefault().Name;
            model.channel.AudienceName = audience.Title;
            model.channel.ChannelType = channelType.channelTypeName;
            model.channel.providerName = SChannelProvider.Providers.SingleOrDefault(p => p.providerId == productProviderId).description;
            model.channel.ProductProviderId = productProviderId;
            model.channel.Rating = string.Empty;
            model.channel.Score = string.Empty;
            var operationResult = _audienceChannelManager.AddAudienceChannel(model);

            return operationResult ? Json(new { success = true, message = "Audience Channel created successfully" }) : Json(new { error = "Error creating Audience Channel" });
        }

        private void BindQuestionsImageDescriptionAudienceChannelModel(AudienceChannelViewModel model)
        {
            var audience = audienceManager.GetAudienceById(TempData["tdIdAudience"].ToString());
            model.channel.AudienceId = audience.Id;
            model.listQuestions = (List<QuestionAskToAudienceChannelDocument>)TempData["vdListQuestions"];
            model.channel.ImageUrl = model.channel.IsDefaultImage ? audience.ImageUrl : BindFile(model);
            model.channel.Description = model.channel.IsDefaultDescription ? audience.Description : Negocio.Utilerias.StringUtilities.RemoveNewLineInString(model.channel.Description);
        }

        private string BindFile(AudienceChannelViewModel model)
        {
            // create Id
            Guid profileId = Guid.NewGuid();

            // get images and urls
            List<Models.Core.FileUpload> images = GetFiles();
            Dictionary<string, string> urls = GetUrls(images, 480, 320);

            // check if Img is uploaded
            string value = string.Empty;
            urls.TryGetValue("fileupImg", out value);

            if (!string.IsNullOrEmpty(value)) {
                return value;
            }
            else {
                return string.Empty;
            }



            //if (!string.IsNullOrEmpty(value)) {
            //    return value;
            //}
            //else {
            //    if (advertiser != null) {
            //        viewModel.profile.PhotoUrl = advertiser.profile.PhotoUrl;
            //    }
            //    else {
            //        viewModel.profile.PhotoUrl = string.Empty;
            //    }
            //}
        }
        #endregion
    
        #region Questions
        public PartialViewResult AddQuestion(AudienceChannelViewModel model, string question)
        {
            listQuestions = (List<QuestionAskToAudienceChannelDocument>)TempData["vdListQuestions"];
            if (listQuestions == null) {
                listQuestions = new List<QuestionAskToAudienceChannelDocument>();
            }

            if (!string.IsNullOrEmpty(question) && !question.Equals("__QUESTION__")) {
                listQuestions.Add(new QuestionAskToAudienceChannelDocument(question));
            }
            TempData["vdListQuestions"] = listQuestions;

            model.listQuestions = listQuestions;
            model.ValidationQuestion = ValidateQuestions(listQuestions);
            return PartialView("_TableQuestionChannel", model);
        }



        public PartialViewResult RemoveQuestion(AudienceChannelViewModel model, string question)
        {
            listQuestions = (List<QuestionAskToAudienceChannelDocument>)TempData["vdListQuestions"];
            if (listQuestions == null) {
                listQuestions = new List<QuestionAskToAudienceChannelDocument>();
            }
            listQuestions.RemoveAll(l => l.Id == question);
            TempData["vdListQuestions"] = listQuestions;

            model.listQuestions = listQuestions;
            model.ValidationQuestion = ValidateQuestions(listQuestions);
            return PartialView("_TableQuestionChannel", model);
        }

        private bool ValidateQuestions(List<QuestionAskToAudienceChannelDocument> listQuestions)
        {
            return listQuestions != null && listQuestions.Any() && listQuestions.Count > 0;
        }

        public PartialViewResult SaveNewQuestion(string audienceChannelId, string question)
        {
            QuestionAskToAudienceChannelDocument newQuestion = new QuestionAskToAudienceChannelDocument {
                AudienceChannelId = audienceChannelId,
                Question = question
            };
            _audienceChannelManager.AddQuestion(newQuestion);
            AudienceChannelViewModel model = new AudienceChannelViewModel();
            model.listQuestions = _audienceChannelManager.GetQuestions(audienceChannelId);
            return PartialView("_TableEditQuestionChannel", model);
        }


        public PartialViewResult RemoveExistingQuestion(string audienceChannelId, string questionId)
        {
            _audienceChannelManager.DeleteQuestion(questionId);

            AudienceChannelViewModel model = new AudienceChannelViewModel();
            model.listQuestions = _audienceChannelManager.GetQuestions(audienceChannelId);
            return PartialView("_TableEditQuestionChannel", model);
        }
        #endregion

        #region Proposal Publisher
        [System.Web.Mvc.HttpPost]
        public ActionResult BeginContract(PublisherProposalDetailViewModel proposal)
        {
            PublisherProposalDetailViewModel prop;
            if (proposal.Accepted == true) {
                _audienceChannelManager.AcceptProposal(proposal.ProposalId, proposal.Price);
                return RedirectToAction("ReviewProposal", new { proposalId = proposal.ProposalId });
            }

            prop = _audienceChannelManager.FindPublisherProposalDetailVMById(proposal.ProposalId);
            if (proposal.Accepted == false) {
                prop.Accepted = false;
                prop.Price = proposal.Price;
                ViewBag.errorMessage = "DontAccept";
            }
            return View(prop);
        }

        public ActionResult ReviewProposal(string proposalId)
        {
            PublisherProposalDetailViewModel proposal = _audienceChannelManager.FindPublisherProposalDetailVMById(proposalId);

            return View(proposal);
        }

        public PartialViewResult ReviewChannelProposal(string audienceChannelId)
        {
            List<ProposalReviewListItemViewModel> proposals = _proposalManager.GetProposalsByAudienceChannelId(audienceChannelId);
            return PartialView("_ListReviewProposal", proposals);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult ReviewProposal(PublisherProposalDetailViewModel proposal)
        {
            if (proposal.Accepted == true) {
                PublisherProposalDetailViewModel prop = _audienceChannelManager.FindPublisherProposalDetailVMById(proposal.ProposalId);
                prop.Price = proposal.Price;
                ViewBag.kindAdsRewardPool = "";
                ViewBag.TotalPaidToYou = "";
                prop.Accepted = false;
                return View("BeginContract", prop);
            }
            else if (proposal.Accepted == false) {

                PublisherProposalDetailViewModel prop = _audienceChannelManager.RejectProposal(proposal);
                return View(prop);
            }
            else {
                return RedirectToAction("ReviewProposal", new { proposalId = proposal.ProposalId });
            }
        }


        #endregion

        public ActionResult PublisherContract()
        {
            return View();
        }

        public PartialViewResult GetProviderForm(string emailChannelProvider)
        {
            AudienceChannelViewModel viewModel = new AudienceChannelViewModel();
            string path = _audienceChannelManager.GetPartialPathFromPartnerId(emailChannelProvider);
            return PartialView(path, viewModel);
        }

        public PartialViewResult GetProviderDetail(string emailChannelProvider, string emailproductId)
        {
            AudienceChannelViewModel viewModel = new AudienceChannelViewModel();
            IEnumerable<EmailProductSettingDocument> settings = this._audienceChannelManager.GetProductSettingsByProductId(emailproductId);
            switch (emailChannelProvider) {
                case ChannelProvider.ActiveCampaignId:
                    viewModel.emailProductForm = new EmailProductForms { activeCampaignForm = new ActiveCampaignForm() };
                    break;
                case ChannelProvider.AweberId:
                    viewModel.emailProductForm = new EmailProductForms { aweberForm = new AweberForm() };
                    break;
                case ChannelProvider.CampaignMonitorId:
                    viewModel.emailProductForm = new EmailProductForms { campaignMonitorForm = new CampaignMonitorForm() };
                    viewModel.emailProductForm.campaignMonitorForm.ApiToken = settings.SingleOrDefault(s => s.Name == SettingsEmailProvidersEnum.campaignMonitorApiToken.ToString() && s.ProductId == emailproductId).Value;
                    viewModel.emailProductForm.campaignMonitorForm.ListId = settings.SingleOrDefault(s => s.Name == SettingsEmailProvidersEnum.campaignMonitorList.ToString() && s.ProductId == emailproductId).Value;
                    viewModel.emailProductForm.campaignMonitorForm.SegmentId = settings.SingleOrDefault(s => s.Name == SettingsEmailProvidersEnum.campaignMonitorSegment.ToString() && s.ProductId == emailproductId).Value;
                    viewModel.emailProductForm.campaignMonitorForm.ClientId = settings.SingleOrDefault(s => s.Name == SettingsEmailProvidersEnum.campaignMonitorClient.ToString() && s.ProductId == emailproductId).Value;
                    break;
                case ChannelProvider.GetResponseId:
                    viewModel.emailProductForm = new EmailProductForms { getResponseForm = new GetResponseForm() };
                    viewModel.emailProductForm.getResponseForm.ApiToken = settings.SingleOrDefault(s => s.Name == SettingsEmailProvidersEnum.getResponseApiToken.ToString() && s.ProductId == emailproductId).Value;
                    viewModel.emailProductForm.getResponseForm.FromFieldId = settings.SingleOrDefault(s => s.Name == SettingsEmailProvidersEnum.getResponseFromField.ToString() && s.ProductId == emailproductId).Value;
                    viewModel.emailProductForm.getResponseForm.ListId = settings.SingleOrDefault(s => s.Name == SettingsEmailProvidersEnum.getResponseList.ToString() && s.ProductId == emailproductId).Value;
                    break;
                case ChannelProvider.IContactId:

                    break;
                case ChannelProvider.MailChimpId:
                    viewModel.emailProductForm = new EmailProductForms { mailChimpForm = new MailChimpForm() };
                    viewModel.emailProductForm.mailChimpForm.ApiToken = settings.SingleOrDefault(s => s.Name == SettingsEmailProvidersEnum.mailChimpApiToken.ToString() && s.ProductId == emailproductId).Value;
                    viewModel.emailProductForm.mailChimpForm.ListId = settings.SingleOrDefault(s => s.Name == SettingsEmailProvidersEnum.mailChimpList.ToString() && s.ProductId == emailproductId).Value;
                    viewModel.emailProductForm.mailChimpForm.TemplateId = settings.SingleOrDefault(s => s.Name == SettingsEmailProvidersEnum.mailChimpTemplate.ToString() && s.ProductId == emailproductId).Value;

                    break;
                case ChannelProvider.MailJetId:
                    viewModel.emailProductForm = new EmailProductForms { mailJetForm = new MailJet() };
                    viewModel.emailProductForm.mailJetForm.ApiToken = settings.SingleOrDefault(s => s.Name == SettingsEmailProvidersEnum.mailJetApiKey.ToString() && s.ProductId == emailproductId).Value;
                    viewModel.emailProductForm.mailJetForm.ListId = settings.SingleOrDefault(s => s.Name == SettingsEmailProvidersEnum.mailJetList.ToString() && s.ProductId == emailproductId).Value;
                    viewModel.emailProductForm.mailJetForm.SecretKey = settings.SingleOrDefault(s => s.Name == SettingsEmailProvidersEnum.mailJetSecretKey.ToString() && s.ProductId == emailproductId).Value;
                    viewModel.emailProductForm.mailJetForm.Segment = settings.SingleOrDefault(s => s.Name == SettingsEmailProvidersEnum.mailJetSegment.ToString() && s.ProductId == emailproductId).Value;

                    break;
                case ChannelProvider.OneSignalId:

                    break;
                case ChannelProvider.PushCrewId:

                    break;
                case ChannelProvider.PushEngageId:

                    break;
                case ChannelProvider.SendGridId:
                    viewModel.emailProductForm.sendGridForm = new SendGridForm();
                    viewModel.emailProductForm.sendGridForm.ApiToken = settings.SingleOrDefault(s => s.Name == SettingsEmailProvidersEnum.sendGridApiToken.ToString() && s.ProductId == emailproductId).Value;
                    viewModel.emailProductForm.sendGridForm.ListId = settings.SingleOrDefault(s => s.Name == SettingsEmailProvidersEnum.sendGridList.ToString() && s.ProductId == emailproductId).Value;
                    break;
                case ChannelProvider.SendinBlueId:
                    viewModel.emailProductForm = new EmailProductForms { sendinBlueForm = new SendinBlueForm() };
                    viewModel.emailProductForm.sendinBlueForm.ApiToken = settings.SingleOrDefault(s => s.Name == SettingsEmailProvidersEnum.sendinBlueApiKey.ToString() && s.ProductId == emailproductId).Value;
                    viewModel.emailProductForm.sendinBlueForm.ListId = settings.SingleOrDefault(s => s.Name == SettingsEmailProvidersEnum.sendinBlueListId.ToString() && s.ProductId == emailproductId).Value;
                    viewModel.emailProductForm.sendinBlueForm.FolderId = settings.SingleOrDefault(s => s.Name == SettingsEmailProvidersEnum.sendinBlueFolder.ToString() && s.ProductId == emailproductId).Value;
                    viewModel.emailProductForm.sendinBlueForm.Category = settings.SingleOrDefault(s => s.Name == SettingsEmailProvidersEnum.sendinBlueCategory.ToString() && s.ProductId == emailproductId).Value;

                    break;
                case ChannelProvider.SubscribersId:

                    break;

            }

            EmailProductDocument product = _audienceChannelManager.FindEmailProduct(viewModel.channel.ProductId);
            ViewBag.lastUpdated = product == null ? "" : (product.LastDetailUpdated != null ? ((DateTime)product.LastDetailUpdated.Value).ToShortTimeString() : "");
            string path = _audienceChannelManager.GetPartialPathFromPartnerId(emailChannelProvider, true);
            return PartialView(path, viewModel);
        }

        public PartialViewResult GetChannelForm(string channelId, string audienceId)
        {
            AudienceChannelViewModel viewModel = new AudienceChannelViewModel();
            AudienceDocument audience = audienceManager.GetAudienceById(audienceId);
            ViewBag.IsPremium = audience.IsPremium;
            ViewBag.visibility = audience.Visibility;
            string path = _audienceChannelManager.GetChannelPathFromPartnerId(channelId);
            viewModel.channel.IsDefaultDescription = true;
            viewModel.channel.IsDefaultImage = true;
            return PartialView(path, viewModel);
        }

        public PartialViewResult GetAudienceChannelEditForm(string audienceChannelId)
        {
            AudienceChannelViewModel viewModel = this._audienceChannelManager.FindAudienceChannelById(audienceChannelId);
            AudienceChannelDocument audienceChannel = _audienceChannelManager.GetAudienceChannelById(audienceChannelId);
            AudienceDocument audience = audienceManager.GetAudienceById(audienceChannel.AudienceId);

            viewModel.listQuestions = audienceManager.GetQuestionsByAudienceChannelId(audienceChannelId);
            ViewBag.IsPremium = audience.IsPremium;
            ViewBag.visibility = audience.Visibility;
            ViewBag.ImageSrcPreview = audience.ImageUrl;
            ViewBag.descriptionPreview = audience.Description;
            string path = _audienceChannelManager.GetEditChannelPathFromPartnerId(audienceChannel.ChannelId);


            return PartialView(path, viewModel);
        }

        public JsonResult ValidateProvider(ValidateChannelRequest request)
        {
            string audienceId = TempData["tdIdAudience"].ToString();
            TempData["tdIdAudience"] = audienceId;
            bool result = false;
            IMailChannel newChannel;
            IPushChannel pushChannel;

            try {
                switch (request.ProviderId) {

                    // emails providers
                    case ChannelProvider.SendinBlueId: {
                            newChannel = ChannelAbstractFactory<IMailChannel>.CreateChannel(ChannelType.SendinBlue);
                            result = newChannel.ValidateApiToken(request.ApiToken);
                        }
                        break;
                    case ChannelProvider.SendGridId: {
                            newChannel = ChannelAbstractFactory<IMailChannel>.CreateChannel(ChannelType.SendGrid);
                            result = newChannel.ValidateApiToken(request.ApiToken);
                        }
                        break;
                    case ChannelProvider.MailChimpId: {
                            newChannel = ChannelAbstractFactory<IMailChannel>.CreateChannel(ChannelType.MailChimp);
                            result = newChannel.ValidateApiToken(request.ApiToken);
                        }
                        break;
                    case ChannelProvider.AweberId: {
                            var aweberChannel = new AWeberManagerv2();//ChannelAbstractFactory<IMailChannel>.CreateChannel(ChannelType.Aweber);
                            var aweberApiToken = aweberChannel.GetAweberApiToken(request.ApiToken);//newChannel.ValidateApiToken(request.ApiToken);
                            result = aweberApiToken.Success;
                            TempData["AweberApiTokenData"] = aweberApiToken;
                        }
                        break;
                    case ChannelProvider.GetResponseId: {
                            newChannel = ChannelAbstractFactory<IMailChannel>.CreateChannel(ChannelType.GetResponse);
                            result = newChannel.ValidateApiToken(request.ApiToken);
                        }
                        break;
                    case ChannelProvider.CampaignMonitorId: {
                            newChannel = ChannelAbstractFactory<IMailChannel>.CreateChannel(ChannelType.CampaignMonitor);
                            result = newChannel.ValidateApiToken(request.ApiToken);
                        }
                        break;
                    case ChannelProvider.ActiveCampaignId: {
                            newChannel = ChannelAbstractFactory<IMailChannel>.CreateChannel(ChannelType.ActiveCampaign);
                            result = newChannel.ValidateApiToken(request.ApiToken);
                        }
                        break;
                    case ChannelProvider.MailJetId: {
                            newChannel = ChannelAbstractFactory<IMailChannel>.CreateChannel(ChannelType.MailJet);
                            var managerV2 = newChannel as MailJetManagerv2;
                            managerV2.ApiSecret = request.SecretKey;
                            result = newChannel.ValidateApiToken(request.ApiToken);
                        }
                        break;
                    case ChannelProvider.IContactId: {
                            newChannel = ChannelAbstractFactory<IMailChannel>.CreateChannel(ChannelType.IContact);
                            result = newChannel.ValidateApiToken(request.ApiToken);
                        }
                        break;

                    // push providers
                    case ChannelProvider.OneSignalId: {
                            pushChannel = ChannelAbstractFactory<IPushChannel>.CreateChannel(ChannelType.OneSignal);
                            result = pushChannel.ValidateApiToken(request.ApiToken);
                        }
                        break;
                    case ChannelProvider.PushCrewId: {
                            pushChannel = ChannelAbstractFactory<IPushChannel>.CreateChannel(ChannelType.PushCrew);
                            var managerV2 = pushChannel as PushCrewManagerv2;
                            managerV2.Url = _audienceChannelManager.getAudienceUrlSite(audienceId);
                            result = pushChannel.ValidateApiToken(request.ApiToken);
                        }
                        break;
                    case ChannelProvider.PushEngageId: {
                            pushChannel = ChannelAbstractFactory<IPushChannel>.CreateChannel(ChannelType.PushEngage);
                            result = pushChannel.ValidateApiToken(request.ApiToken);
                        }
                        break;
                    case ChannelProvider.SubscribersId: {
                            pushChannel = ChannelAbstractFactory<IPushChannel>.CreateChannel(ChannelType.Subscribers);
                            var managerV2 = pushChannel as SubscribersManagerv2;
                            managerV2.Url = _audienceChannelManager.getAudienceUrlSite(audienceId);
                            result = pushChannel.ValidateApiToken(request.ApiToken);
                        }
                        break;
                }
            }
            catch (Exception e) {

                string exceptionMessage = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(exceptionMessage);
            }

            return Json(new { IsValid = result.ToString() });
        }

        public JsonResult GetListSendGrid(string ApiToken)
        {
            AudienceChannelViewModel viewModel = new AudienceChannelViewModel();
            var list = viewModel.GetListsSendGrid(ApiToken);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNumberSubsSendGrid(string ApiToken, string ListId)
        {
            AudienceChannelViewModel viewModel = new AudienceChannelViewModel();
            var subscribers = viewModel.GetSubscribersSendGrid(ApiToken, ListId);
            return Json(new { CountSubscribers = subscribers });
        }

        public JsonResult GetListFromMailChimp(string ApiToken)
        {
            AudienceChannelViewModel viewModel = new AudienceChannelViewModel();
            var Listas = viewModel.GetListFromMailChimp(ApiToken);
            return Json(Listas, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSubscribersFromList(string ApiToken, string listName)
        {
            AudienceChannelViewModel viewModel = new AudienceChannelViewModel();
            var subscribers = viewModel.GetSubscribersMailChimp(ApiToken, listName);
            return Json(new { subsCount = subscribers });
        }

        public JsonResult GetTemplatesFromMailChimp(string ApiToken)
        {
            AudienceChannelViewModel viewModel = new AudienceChannelViewModel();
            var Templates = viewModel.GetTemplatesFromMailChimp(ApiToken);
            return Json(Templates, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAppsFromOneSignal(string ApiToken)
        {
            AudienceChannelViewModel viewModel = new AudienceChannelViewModel();
            var apps = viewModel.GetAppsFromOneSignal(ApiToken);
            return Json(apps, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFoldersFromSendinBlue(string ApiToken)
        {
            AudienceChannelViewModel viewModel = new AudienceChannelViewModel();
            var folders = viewModel.GetFoldersFromSendinBlue(ApiToken);
            return Json(folders, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetListFromSendinBlue(string ApiToken, string FolderId)
        {
            AudienceChannelViewModel viewModel = new AudienceChannelViewModel();
            var listas = viewModel.GetListFromSendinBlue(ApiToken, FolderId);
            return Json(listas, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetListFromGetResponse(string ApiToken)
        {
            AudienceChannelViewModel viewModel = new AudienceChannelViewModel();
            var listas = viewModel.GetListFromGetResponse(ApiToken);
            return Json(listas, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFromFieldsFromGetResponse(string ApiToken)
        {
            AudienceChannelViewModel viewModel = new AudienceChannelViewModel();
            var fromFields = viewModel.GetFromFieldsFromGetResponse(ApiToken);
            return Json(fromFields, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetListFromMailJet(string ApiToken, string SecretKey)
        {
            AudienceChannelViewModel viewModel = new AudienceChannelViewModel();
            var lists = viewModel.GetListFromMailJet(ApiToken, SecretKey);
            return Json(lists, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSegmentsFromMailJet(string ApiToken, string SecretKey)
        {
            AudienceChannelViewModel viewModel = new AudienceChannelViewModel();
            var segments = viewModel.GetSegmentsFromMailJet(ApiToken, SecretKey);
            return Json(segments, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetClientCampaignMonitor(string ApiToken)
        {
            AuthenticationDetails auth = new ApiKeyAuthenticationDetails(ApiToken);
            General general = new General(auth);
            return Json(new SelectList(general.Clients(), "ClientID", "Name"));
        }

        public JsonResult GetListCampaignMonitor(string idClient, string ApiToken)
        {
            if (!String.IsNullOrEmpty(idClient)) {
                AuthenticationDetails auth = new ApiKeyAuthenticationDetails(ApiToken);
                Client client = new Client(auth, idClient);
                var list = client.Lists();
                return Json(new SelectList(list, "ListID", "Name"));
            }

            return Json(new SelectList(new List<BasicList>(), "ListID", "Name"));
        }

        public JsonResult GetSegmentCampaignMonitor(string idClient, string ApiToken)
        {
            if (!String.IsNullOrEmpty(idClient)) {
                //TempData["ClientCMSelecc"] = idClient;
                AuthenticationDetails auth = new ApiKeyAuthenticationDetails(ApiToken);
                Client client = new Client(auth, idClient);
                var list = client.Segments();
                return Json(new SelectList(list, "SegmentID", "Title"));
            }

            return Json(new SelectList(new List<BasicList>(), "ListID", "Name"));
        }

        public JsonResult GetNumberSubsCampaignMonitor(string ApiToken, string ListId)
        {
            int NoSubscribers = 0;
            if (!string.IsNullOrEmpty(ListId)) {
                AuthenticationDetails auth = new ApiKeyAuthenticationDetails(ApiToken);
                List list = new List(auth, ListId);
                NoSubscribers = list.Stats().TotalActiveSubscribers;
            }
            return Json(new { CountSubscribers = NoSubscribers });
        }

        public JsonResult GetNumberSubsAweber(string ListId)
        {
            var AweberApiToken = (ProviderAWeberApiResult)TempData["AweberApiTokenData"];
            int NoSubscribers = 0;
            if (!string.IsNullOrEmpty(ListId)) {
                AudienceChannelViewModel viewModel = new AudienceChannelViewModel();
                NoSubscribers = Convert.ToInt16( viewModel.GetSubscribersAweber(AweberApiToken, ListId));
            }
            TempData["AweberApiTokenData"] = AweberApiToken;
            return Json(new { CountSubscribers = NoSubscribers });
        }

        public ActionResult GetLinkAweberAuthentication()
        {
            return PartialView("~/Views/Channel/Email/_AweberAuthentication.cshtml", Utils.Configuration.AppSettings.provideraweberauthorizeappurl);
        }

        public JsonResult GetListAweber()
        {
            var AweberApiToken = (ProviderAWeberApiResult)TempData["AweberApiTokenData"];
            AudienceChannelViewModel viewModel = new AudienceChannelViewModel();
            var listas = viewModel.GetListAweber(AweberApiToken);
            TempData["AweberApiTokenData"] = AweberApiToken;
            return Json(listas, JsonRequestBehavior.AllowGet);
        }


    }

}
