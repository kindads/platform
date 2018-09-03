using KindAds.Comun.Enums;
using KindAds.Comun.Interfaces;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using KindAds.Comun.Models.KindAdsV2;
using KindAds.Negocio.ViewModels.KindAdsV2;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KindAds.Comun.Enums;

namespace KindAds.Negocio.Managersv2
{
    public class AudienceChannelManager : BaseManager
    {
        private readonly string questionCollectionName;
        private readonly AdvertiserProfileManager _advertiserProfileManager;
       
        private readonly CatalogManager _catalogManager;

        public AudienceChannelManager()
        {
            databaseName = ConfigurationManager.AppSettings["azure-cosmos-databasename"];
            collectionName = CosmosCollections.AudienceChannel.ToString();
            questionCollectionName = CosmosCollections.QuestionAskToAudienceChannel.ToString();
            _advertiserProfileManager = new AdvertiserProfileManager();
       
            _catalogManager = new CatalogManager();
        }

        public List<AudienceChannelDocument> GetAudienceChannelsByAudience(string AudienceId)
        {
            collectionName = CosmosCollections.AudienceChannel.ToString();
            string query = $"SELECT * FROM {collectionName} WHERE {collectionName}.AudienceId='{AudienceId}'";
            List<AudienceChannelDocument> channels = context.ExecuteQuery<AudienceChannelDocument>(databaseName, collectionName, query);
            return channels;
        }


        public List<AudienceChannelDocument> GetAudienceChannelsLitsVMByAudience(string AudienceId)
        {
            collectionName = CosmosCollections.AudienceChannel.ToString();
            string query = $"SELECT * FROM {collectionName} WHERE {collectionName}.AudienceId='{AudienceId}'";
            List<AudienceChannelDocument> channels = context.ExecuteQuery<AudienceChannelDocument>(databaseName, collectionName, query);
            return channels;
        }

        public List<EmailProductProviderDocument> GetEmailProviders()
        {
            collectionName = CosmosCollections.CAT_EmailProductProvider.ToString();
            List<EmailProductProviderDocument> emailProviders = new List<EmailProductProviderDocument>();
            emailProviders = context.GetAllDocuments<EmailProductProviderDocument>(databaseName, collectionName);
            return emailProviders;
        }

        public List<PushProductProviderDocument> GetPushProviders()
        {
            collectionName = CosmosCollections.CAT_PushProductProvider.ToString();
            List<PushProductProviderDocument> pushProviders = new List<PushProductProviderDocument>();
            pushProviders = context.GetAllDocuments<PushProductProviderDocument>(databaseName, collectionName);
            return pushProviders;
        }

        public void UpdateAudienceChannel(AudienceChannelViewModel channel)
        {
            string query = $"SELECT * FROM {CosmosCollections.AudienceChannel.ToString()} WHERE {CosmosCollections.AudienceChannel.ToString()}.id='{channel.channel.Id}'";
            context.ExecuteQuery<AudienceChannelDocument>(this.databaseName, CosmosCollections.AudienceChannel.ToString(), query);
            context.UpsertDocument<AudienceChannelDocument>(this.databaseName,CosmosCollections.AudienceChannel.ToString(),channel.channel);


            if (channel.listQuestions != null && channel.listQuestions.Any())
            {
                foreach (var question in channel.listQuestions)
                {
                    question.AudienceChannelId = channel.channel.Id;
                    context.AddDocument<QuestionAskToAudienceChannelDocument>(databaseName, CosmosCollections.QuestionAskToAudienceChannel.ToString(), question);
                }
            }
        }

        public bool AddAudienceChannel(AudienceChannelViewModel model)
        {
            bool result = false;
            collectionName = CosmosCollections.AudienceChannel.ToString();

            try
            {
                model.channel.Id = Guid.NewGuid().ToString();
                context.AddDocument<AudienceChannelDocument>(databaseName, collectionName, model.channel);

                if (model.listQuestions != null && model.listQuestions.Any())
                {
                    foreach (var question in model.listQuestions)
                    {
                        question.AudienceChannelId = model.channel.Id;
                        context.AddDocument<QuestionAskToAudienceChannelDocument>(databaseName, questionCollectionName, question);
                    }
                }

                result = true;
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return result;
        }


        #region Obtencion de settings

        public IList<AudiencePropertieSetting> GetProductListSettings(string audienceUrlSite, AudienceChannelViewModel viewModel, string productId)
        {
            IList<AudiencePropertieSetting> settings = new List<AudiencePropertieSetting>();
            string Value = string.Empty;

            if (viewModel.EmailProviderSelected != null && viewModel.EmailProviderSelected != string.Empty)
            {
                switch (viewModel.EmailProviderSelected)
                {
                    case ChannelProvider.SendinBlueId:
                        {
                            Value = viewModel.emailProductForm.sendinBlueForm.ApiToken;
                            settings.Add(new AudiencePropertieSetting { Name = "sendinBlueApiKey", Value = Value, ProductId = productId });

                            Value = viewModel.emailProductForm.sendinBlueForm.FolderId;
                            settings.Add(new AudiencePropertieSetting { Name = "sendinBlueFolder", Value = Value, ProductId = productId });

                            Value = viewModel.emailProductForm.sendinBlueForm.ListId;
                            settings.Add(new AudiencePropertieSetting { Name = "sendinBlueListId", Value = Value, ProductId = productId });

                            Value = viewModel.emailProductForm.sendinBlueForm.Category;
                            settings.Add(new AudiencePropertieSetting { Name = "sendinBlueCategory", Value = Value, ProductId = productId });
                        }
                        break;
                    case ChannelProvider.SendGridId:
                        {
                            Value = viewModel.emailProductForm.sendGridForm.ApiToken;
                            settings.Add(new AudiencePropertieSetting { Name = "sendGridApiToken", Value = Value, ProductId = productId });

                            Value = viewModel.emailProductForm.sendGridForm.ListId;
                            settings.Add(new AudiencePropertieSetting { Name = "sendGridList", Value = Value, ProductId = productId });
                           
                        }
                        break;
                    case ChannelProvider.MailChimpId:
                        {
                            Value = viewModel.emailProductForm.mailChimpForm.ApiToken;
                            settings.Add(new AudiencePropertieSetting { Name = "mailChimpApiToken", Value = Value, ProductId = productId });

                            Value = viewModel.emailProductForm.mailChimpForm.TemplateId;
                            settings.Add(new AudiencePropertieSetting { Name = "mailChimpTemplate", Value = Value, ProductId = productId });

                            Value = viewModel.emailProductForm.mailChimpForm.ListId;
                            settings.Add(new AudiencePropertieSetting { Name = "mailChimpList", Value = Value, ProductId = productId });
                        }
                        break;
                    case ChannelProvider.MailJetId:
                        {
                            Value = viewModel.emailProductForm.mailJetForm.ApiToken;
                            settings.Add(new AudiencePropertieSetting { Name = "mailJetApiKey", Value = Value, ProductId = productId });

                            Value = viewModel.emailProductForm.mailJetForm.SecretKey;
                            settings.Add(new AudiencePropertieSetting { Name = "mailJetSecretKey", Value = Value, ProductId = productId });

                            Value = viewModel.emailProductForm.mailJetForm.ListId;
                            settings.Add(new AudiencePropertieSetting { Name = "mailJetList", Value = Value, ProductId = productId });

                            Value = viewModel.emailProductForm.mailJetForm.Segment;
                            settings.Add(new AudiencePropertieSetting { Name = "mailJetSegment", Value = Value, ProductId = productId });
                        }
                        break;
                    // push notifications
                    case ChannelProvider.OneSignalId:
                        {
                            //todo
                            Value = viewModel.pushProductForm.oneSignalForm.ApiToken;
                            settings.Add(new AudiencePropertieSetting { Name = "oneSignalApiKey", Value = Value, ProductId = productId });

                            Value = viewModel.pushProductForm.oneSignalForm.AppId;
                            settings.Add(new AudiencePropertieSetting { Name = "oneSignalAppId", Value = Value, ProductId = productId });
                        }
                        break;
                    case ChannelProvider.PushCrewId:
                        {
                            Value = viewModel.pushProductForm.pushCrewForm.ApiToken;
                            settings.Add(new AudiencePropertieSetting { Name = "pushApiToken", Value = Value, ProductId = productId });

                            Value = audienceUrlSite;
                            settings.Add(new AudiencePropertieSetting { Name = "pushNotifUrl", Value = Value, ProductId = productId });
                        }
                        break;
                    case ChannelProvider.PushEngageId:
                        {
                            Value = viewModel.pushProductForm.pushEnagageForm.ApiToken;
                            settings.Add(new AudiencePropertieSetting { Name = "pushApiToken", Value = Value, ProductId = productId });

                        }
                        break;
                    case ChannelProvider.SubscribersId:
                        {
                            Value = viewModel.pushProductForm.subscribersForm.ApiToken;
                            settings.Add(new AudiencePropertieSetting { Name = "pushApiToken", Value = Value, ProductId = productId });

                            Value = audienceUrlSite;
                            settings.Add(new AudiencePropertieSetting { Name = "pushNotifUrl", Value = Value, ProductId = productId });

                        }
                        break;
                    case ChannelProvider.GetResponseId:
                        {
                            Value = viewModel.emailProductForm.getResponseForm.ApiToken;
                            settings.Add(new AudiencePropertieSetting { Name = "getResponseApiToken", Value = Value, ProductId = productId });

                            Value = viewModel.emailProductForm.getResponseForm.ListId;
                            settings.Add(new AudiencePropertieSetting { Name = "getResponseList", Value = Value, ProductId = productId });

                            Value = viewModel.emailProductForm.getResponseForm.FromFieldId;
                            settings.Add(new AudiencePropertieSetting { Name = "getResponseFromField", Value = Value, ProductId = productId });
                        }
                        break;

                    case ChannelProvider.CampaignMonitorId:
                        {
                            Value = viewModel.emailProductForm.campaignMonitorForm.ApiToken;
                            settings.Add(new AudiencePropertieSetting { Name = "campaignMonitorApiToken", Value = Value, ProductId = productId });

                            Value = viewModel.emailProductForm.campaignMonitorForm.ListId;
                            settings.Add(new AudiencePropertieSetting { Name = "campaignMonitorList", Value = Value, ProductId = productId });

                            Value = viewModel.emailProductForm.campaignMonitorForm.ClientId;
                            settings.Add(new AudiencePropertieSetting { Name = "campaignMonitorClient", Value = Value, ProductId = productId });

                            Value = viewModel.emailProductForm.campaignMonitorForm.SegmentId;
                            settings.Add(new AudiencePropertieSetting { Name = "campaignMonitorSegment", Value = Value, ProductId = productId });
                        }
                        break;
                }
            }
            if (viewModel.PushProviderSelected != null && viewModel.PushProviderSelected != string.Empty)
            {

            }
            if (viewModel.WebSpaceProviderSelected != null && viewModel.WebSpaceProviderSelected != string.Empty)
            {

            }
            return settings;
        }

        public void DeleteQuestion(string questionId)
        {
            List<string> questionsIds = new List<string>();
            questionsIds.Add(questionId);
            context.DeleteDocumentsById(databaseName, CosmosCollections.QuestionAskToAudienceChannel.ToString(), questionsIds);
        }

        public void AddQuestion(QuestionAskToAudienceChannelDocument newQuestion)
        {
            context.AddDocument<QuestionAskToAudienceChannelDocument>(databaseName, CosmosCollections.QuestionAskToAudienceChannel.ToString(), newQuestion);
        }

        public ProposalDocument AcceptProposal(string proposalId, double price)
        {
            ProposalDocument vproposal = FindProposalById(proposalId);
            vproposal.AcceptedByPublisher = true;
            vproposal.Price = price;
            context.UpsertDocument<ProposalDocument>(databaseName, CosmosCollections.Proposal.ToString(), vproposal);
            return vproposal;
        }

        public AudienceChannelViewModel FindAudienceChannelById(string audienceChannelId)
        {
            string query = $"SELECT * FROM {CosmosCollections.AudienceChannel.ToString()} WHERE {CosmosCollections.AudienceChannel.ToString()}.id='{audienceChannelId}'";
            AudienceChannelDocument acd = context.ExecuteQuery<AudienceChannelDocument>(this.databaseName, CosmosCollections.AudienceChannel.ToString(), query).SingleOrDefault();
            
            return new AudienceChannelViewModel { channel = acd };
        }

        public ProposalDocument FindProposalById(string proposalId)
        {
            string query = $"SELECT * FROM {CosmosCollections.Proposal.ToString()} WHERE {CosmosCollections.Proposal.ToString()}.id='{proposalId}'";
            ProposalDocument proposal = context.ExecuteQuery<ProposalDocument>(databaseName, CosmosCollections.Proposal.ToString(), query).SingleOrDefault();
            return proposal;
        }

        public PublisherProposalDetailViewModel FindPublisherProposalDetailVMById(string proposalId)
        {
            ProposalDocument proposal = FindProposalById(proposalId);
            AdvertiserProfileDocument advertiser =  _advertiserProfileManager.FindProfileByProfileId(proposal.AdvertiserProfileId);
            AudienceChannelDocument audienceChannel = GetAudienceChannelById(proposal.AudienceChannelId);
            string audienceUrl = getAudienceUrlSite(audienceChannel.AudienceId);
            CountryDocument country =  _catalogManager.FindCountryById(advertiser.CountryBusinessInId);
            return new PublisherProposalDetailViewModel
            {
                Accepted = proposal.AcceptedByPublisher,
                AdvertiserImage = advertiser.IconUrl,
                AdvertiserLocation = country.Name,
                AdvertiserName = advertiser.Title,
                ProposalId = proposal.Id,
                QuestionsAndAnswers = proposal.Questions,
                RejectDetail = proposal.RejectDetail,
                WebSite = audienceUrl,
                MemberSinceYear= advertiser.RegisterDate,
                Price =proposal.Price
            };
        }

        public AudienceChannelDocument GetAudienceChannelById(string audienceChannelId)
        {
            string query = $"SELECT * FROM {CosmosCollections.AudienceChannel.ToString()} WHERE {CosmosCollections.AudienceChannel.ToString()}.id='{audienceChannelId}'";
            return context.ExecuteQuery<AudienceChannelDocument>(this.databaseName, CosmosCollections.AudienceChannel.ToString(), query).SingleOrDefault();
        }

        public PublisherProposalDetailViewModel RejectProposal(PublisherProposalDetailViewModel proposal)
        {
            ProposalDocument proposalInBD = FindProposalById(proposal.ProposalId);
            proposalInBD.RegisterDate = proposalInBD.GetMexicanTime();
            proposalInBD.RejectDetail = proposal.RejectDetail;
            proposalInBD.AcceptedByPublisher = false;
            context.UpsertDocument<ProposalDocument>(databaseName, CosmosCollections.Proposal.ToString(), proposalInBD);
            return FindPublisherProposalDetailVMById(proposal.ProposalId);
        }

        //public string GetProductName(AudienceChannelViewModel viewModel)
        //{
        //    string Name = string.Empty;

        //    if (viewModel.EmailProviderSelected != null && viewModel.EmailProviderSelected != string.Empty)
        //    {
        //        switch (viewModel.EmailProviderSelected)
        //        {
        //            case ChannelProvider.SendinBlueId:
        //                {
        //                    Name = viewModel.emailProductForm.sendinBlueForm.Name;
        //                }
        //                break;
        //            case ChannelProvider.SendGridId:
        //                {
        //                    Name = viewModel.emailProductForm.sendGridForm.Name;
        //                }
        //                break;
        //            case ChannelProvider.MailChimpId:
        //                {
        //                    Name = viewModel.emailProductForm.mailChimpForm.Name;
        //                }
        //                break;
        //            case ChannelProvider.AweberId:
        //                {
        //                    Name = viewModel.emailProductForm.aweberForm.Name;
        //                }
        //                break;
        //            case ChannelProvider.GetResponseId:
        //                {
        //                    Name = viewModel.emailProductForm.getResponseForm.Name;
        //                }
        //                break;
        //            case ChannelProvider.CampaignMonitorId:
        //                {
        //                    Name = viewModel.emailProductForm.campaignMonitorForm.Name;
        //                }
        //                break;
        //            case ChannelProvider.ActiveCampaignId:
        //                {
        //                    Name = viewModel.emailProductForm.activeCampaignForm.Name;
        //                }
        //                break;
        //            case ChannelProvider.MailJetId:
        //                {
        //                    Name = viewModel.emailProductForm.mailJetForm.Name;
        //                }
        //                break;
        //            case ChannelProvider.IContactId:
        //                {
        //                    Name = viewModel.emailProductForm.icontactForm.Name;
        //                }
        //                break;
        //        }
        //    }
        //    if (viewModel.PushProviderSelected != null && viewModel.PushProviderSelected != string.Empty)
        //    {
        //        switch (viewModel.PushProviderSelected)
        //        {
        //            case ChannelProvider.OneSignalId:
        //                {
        //                    Name = viewModel.pushProductForm.oneSignalForm.Name;
        //                }
        //                break;
        //            case ChannelProvider.PushCrewId:
        //                {
        //                    Name = viewModel.pushProductForm.pushCrewForm.Name;
        //                }
        //                break;
        //            case ChannelProvider.PushEngageId:
        //                {
        //                    Name = viewModel.pushProductForm.pushEnagageForm.Name;
        //                }
        //                break;
        //            case ChannelProvider.SubscribersId:
        //                {
        //                    Name = viewModel.pushProductForm.subscribersForm.Name;
        //                }
        //                break;
        //        }
        //    }
        //    if (viewModel.WebSpaceProviderSelected != null && viewModel.WebSpaceProviderSelected != string.Empty)
        //    {

        //    }
        //    return Name;
        //}


        public string GetProductProviderId(AudienceChannelViewModel viewModel)
        {
            string ProviderId = string.Empty;
            ProviderId = viewModel.EmailProviderSelected;
            return ProviderId;
        }

        public bool AddPushProduct(AudienceChannelViewModel viewModel, string audienceChannelId, string settingId, string productId, string detail)
        {
            bool result = false;
            PushProductDocument product = new PushProductDocument();
            //product.Name = GetProductName(viewModel);
            product.PushProductProviderId = GetProductProviderId(viewModel);
            product.AudiencePropertieId = audienceChannelId;
            product.SettingId = settingId;
            product.Id = productId;
            product.Detail = detail;
            product.LastDetailUpdated = product.GetMexicanDateTime();
            collectionName = CosmosCollections.PushProduct.ToString();
            context.AddDocument<PushProductDocument>(databaseName, collectionName, product);

            return result;
        }

        public bool AddEmailProduct(AudienceChannelViewModel viewModel, string audienceChannelId, string settingId, string productId, string detail)
        {
            bool result = false;
            EmailProductDocument product = new EmailProductDocument();
            //product.Name = GetProductName(viewModel);
            product.EmailProductProviderId = GetProductProviderId(viewModel);
            product.AudiencePropertieId = audienceChannelId;
            product.SettingId = settingId;
            product.Id = productId;
            product.Detail = detail;
            product.LastDetailUpdated = product.GetMexicanDateTime();
            
            collectionName = CosmosCollections.EmailProduct.ToString();
            context.AddDocument<EmailProductDocument>(databaseName, collectionName, product);

            return result;
        }


        public bool AddPushProductSettings(string audienceUrlSite, AudienceChannelViewModel viewModel, string productId)
        {
            bool result = false;
            var settings = GetProductListSettings(audienceUrlSite, viewModel, productId);
            collectionName = CosmosCollections.PushProductSetting.ToString();

            foreach (var setting in settings)
            {
                var aux = new PushProductSettingDocument() { Name = setting.Name, Value = setting.Value, ProductId = productId };
                context.AddDocument<PushProductSettingDocument>(databaseName, collectionName, aux);
            }
            return result;
        }

        

        public bool AddEmailProductSettings(string audienceUrlSite, AudienceChannelViewModel viewModel, string productId)
        {
            bool result = false;
            var settings = GetProductListSettings(audienceUrlSite, viewModel, productId);
            collectionName = CosmosCollections.EmailProductSetting.ToString();

            foreach (var setting in settings)
            {
                var aux = new EmailProductSettingDocument() { Name = setting.Name, Value = setting.Value, ProductId = productId };
                context.AddDocument<EmailProductSettingDocument>(databaseName, collectionName, aux);
            }
            return result;
        }

        public IEnumerable<EmailProductSettingDocument> GetProductSettingsByProductId(string productId)
        {
            string query = $"SELECT * FROM {CosmosCollections.EmailProductSetting.ToString()} WHERE {CosmosCollections.EmailProductSetting.ToString()}.ProductId='{productId}'";
            IEnumerable<EmailProductSettingDocument> result =  context.ExecuteQuery<EmailProductSettingDocument>(databaseName, CosmosCollections.EmailProductSetting.ToString(), query);
            return result;
        }

        public EmailProductDocument FindEmailProduct(string emailProductId)
        {
            EmailProductDocument emailProduct = new EmailProductDocument();
            string collectionName = CosmosCollections.EmailProduct.ToString();
            string query = $"SELECT * FROM {collectionName} WHERE {collectionName}.id='{emailProductId}'";
            emailProduct = context.ExecuteQuery<EmailProductDocument>(databaseName, collectionName, query).FirstOrDefault();
            return emailProduct;
        }

        #endregion

        public string getAudienceUrlSite(string audienceId)
        {
            string Url = string.Empty;
            collectionName = CosmosCollections.Audience.ToString();
            string query = $"SELECT * FROM {collectionName} WHERE {collectionName}.id='{audienceId}'";
            AudienceDocument audience = context.ExecuteQuery<AudienceDocument>(databaseName, collectionName, query).FirstOrDefault();
            Url = audience.WebSiteUrl;
            return Url;
        }

        public AudienceDocument getAudienceById(string audienceId)
        {
            string Url = string.Empty;
            collectionName = CosmosCollections.Audience.ToString();
            string query = $"SELECT * FROM {collectionName} WHERE {collectionName}.id='{audienceId}'";
            AudienceDocument audience = context.ExecuteQuery<AudienceDocument>(databaseName, collectionName, query).FirstOrDefault();
            return audience;
        }

        public string GetChannelPathFromPartnerId(string channelId)
        {
            string path = string.Empty;
            switch (channelId)
            {
                case "b4ee7512-5f12-40f1-8408-8f96bf43df6d":
                    {
                        path = "~/Views/Channel/_EmailChannel.cshtml";
                    }
                    break;
                case "69349c7c-48b2-4628-9f3f-22846b1bc6de":
                    {
                        path = "~/Views/Channel/_PushChannel.cshtml";
                    }
                    break;
                case "88c34fff-a1ab-401e-8908-5a4929abf36a":
                    {
                        path = "~/Views/Channel/_WebsiteAd.cshtml";
                    }
                    break;

            }
            return path;
        }

        public string GetEditChannelPathFromPartnerId(string channelId)
        {
            string path = string.Empty;
            switch (channelId)
            {
                case "b4ee7512-5f12-40f1-8408-8f96bf43df6d":
                    {
                        path = "~/Views/Channel/Edit/_EmailEditChannel.cshtml";
                    }
                    break;
                case "69349c7c-48b2-4628-9f3f-22846b1bc6de":
                    {
                        path = "~/Views/Channel/Edit/_PushEditChannel.cshtml";
                    }
                    break;
                case "88c34fff-a1ab-401e-8908-5a4929abf36a":
                    {
                        path = "~/Views/Channel/Edit/_WebsiteAdEditChannel.cshtml";
                    }
                    break;

            }
            return path;
        }
        public string GetPartialPathFromPartnerId(string Id, bool detail = false)
        {
            string path = string.Empty;
            string provider = string.Empty;
            string sufix = detail? "Detail":string.Empty;
            switch (Id)
            {
                case ChannelProvider.SendinBlueId:
                    {
                        provider = ChannelType.SendinBlue.ToString();

                        provider = string.Format("_{0}.cshtml", provider);
                        path = string.Format("~/Views/Channel/Email{0}/{1}", sufix, provider);
                    }
                    break;
                case ChannelProvider.AweberId:
                    {
                        provider = ChannelType.Aweber.ToString();

                        provider = string.Format("_{0}.cshtml", provider);
                        path = string.Format("~/Views/Channel/Email{0}/{1}", sufix, provider);
                    }
                    break;
                case ChannelProvider.IContactId:
                    {
                        provider = ChannelType.IContact.ToString();

                        provider = string.Format("_{0}.cshtml", provider);
                        path = string.Format("~/Views/Channel/Email{0}/{1}", sufix, provider);
                    }
                    break;
                case ChannelProvider.GetResponseId:
                    {
                        provider = ChannelType.GetResponse.ToString();

                        provider = string.Format("_{0}.cshtml", provider);
                        path = string.Format("~/Views/Channel/Email{0}/{1}", sufix, provider);
                    }
                    break;
                case ChannelProvider.ActiveCampaignId:
                    {
                        provider = ChannelType.ActiveCampaign.ToString();

                        provider = string.Format("_{0}.cshtml", provider);
                        path = string.Format("~/Views/Channel/Email{0}/{1}", sufix, provider);
                    }
                    break;
                case ChannelProvider.SendGridId:
                    {
                        provider = ChannelType.SendGrid.ToString();

                        provider = string.Format("_{0}.cshtml", provider);
                        path = string.Format("~/Views/Channel/Email{0}/{1}", sufix, provider);
                    }
                    break;
                case ChannelProvider.MailJetId:
                    {
                        provider = ChannelType.MailJet.ToString();

                        provider = string.Format("_{0}.cshtml", provider);
                        path = string.Format("~/Views/Channel/Email{0}/{1}", sufix, provider);
                    }
                    break;
                case ChannelProvider.MailChimpId:
                    {
                        provider = ChannelType.MailChimp.ToString();

                        provider = string.Format("_{0}.cshtml", provider);
                        path = string.Format("~/Views/Channel/Email{0}/{1}", sufix, provider);
                    }
                    break;
                case ChannelProvider.CampaignMonitorId:
                    {
                        provider = ChannelType.CampaignMonitor.ToString();

                        provider = string.Format("_{0}.cshtml", provider);
                        path = string.Format("~/Views/Channel/Email{0}/{1}", sufix, provider);
                    }
                    break;

                // push 
                case ChannelProvider.PushEngageId:
                    {
                        provider = ChannelType.PushEngage.ToString();

                        provider = string.Format("_{0}.cshtml", provider);
                        path = string.Format("~/Views/Channel/PushNotification{0}/{1}", sufix, provider);
                    }
                    break;
                case ChannelProvider.SubscribersId:
                    {
                        provider = ChannelType.Subscribers.ToString();

                        provider = string.Format("_{0}.cshtml", provider);
                        path = string.Format("~/Views/Channel/PushNotification{0}/{1}", sufix, provider);
                    }
                    break;
                case ChannelProvider.OneSignalId:
                    {
                        provider = ChannelType.OneSignal.ToString();

                        provider = string.Format("_{0}.cshtml", provider);
                        path = string.Format("~/Views/Channel/PushNotification{0}/{1}", sufix, provider);
                    }
                    break;
                case ChannelProvider.PushCrewId:
                    {
                        provider = ChannelType.PushCrew.ToString();

                        provider = string.Format("_{0}.cshtml", provider);
                        path = string.Format("~/Views/Channel/PushNotification{0}/{1}", sufix, provider);
                    }
                    break;
            }

            return path;
        }

        public List<ChannelDocument> GetChannelDocuments()
        {
            return context.GetAllDocuments<ChannelDocument>(databaseName, CosmosCollections.CAT_Channel.ToString());
        }

        public List<AudienceChannelDocument> GetAudienceChannelDocuments()
        {
            return context.GetAllDocuments<AudienceChannelDocument>(databaseName, CosmosCollections.AudienceChannel.ToString());
        }

        public List<AudienceChannelDocument> GetAudienceChannelForAdvertiser()
        {
            collectionName = CosmosCollections.AudienceChannel.ToString();
            string query = $"SELECT * FROM {collectionName} WHERE {collectionName}.IsActive = true AND {collectionName}.Visibility = true";
            List<AudienceChannelDocument> list = context.ExecuteQuery<AudienceChannelDocument>(databaseName, collectionName, query).ToList();
            return list;
        }

        public List<AudienceChannelDocument> GetAudienceChannelForPublisher(string idPublisher)
        {
            string query = string.Empty;
            var audienceCollectionName = CosmosCollections.Audience.ToString();
            var publisherCollectionName = CosmosCollections.PublisherProfile.ToString();
            collectionName = CosmosCollections.AudienceChannel.ToString();
            List<AudienceChannelDocument> audienceChannelList = new List<AudienceChannelDocument>();

            query = $"SELECT * FROM {publisherCollectionName} WHERE {publisherCollectionName}.UserId = '{idPublisher}'";
            PublisherProfileDocument publisherProfileDocument = context.ExecuteQuery<PublisherProfileDocument>(databaseName, publisherCollectionName, query).FirstOrDefault();

            if (publisherProfileDocument != null && publisherProfileDocument.Id != null)
            {
                query = $"SELECT * FROM {audienceCollectionName} WHERE {audienceCollectionName}.PublisherId = '{publisherProfileDocument.Id}'";
                List<AudienceDocument> audienceList = context.ExecuteQuery<AudienceDocument>(databaseName, audienceCollectionName, query).ToList();
                if (audienceList != null && audienceList.Any())
                {
                    query = $"SELECT * FROM {collectionName} WHERE {collectionName}.AudienceId IN ({GetListFormat(audienceList)})";
                    audienceChannelList = context.ExecuteQuery<AudienceChannelDocument>(databaseName, collectionName, query).ToList();

                }
            }

            return audienceChannelList;

        }

        private string GetListFormat(List<AudienceDocument> audienceList)
        {
            string audienceString = string.Empty;
            bool IsFirst = true;
            foreach (var item in audienceList)
            {
                string itemFormat = "'" + item.Id + "'";
                if (IsFirst)
                {
                    audienceString = itemFormat;
                    IsFirst = false;
                }
                else
                {
                    audienceString = audienceString + "," + itemFormat;
                }
            }

            return audienceString;
        }

        public string GetProviderImageCss(string idProvider)
        {
            string result = string.Empty;
            switch (idProvider)
            {
                case ChannelProvider.IContactId:
                    result = "icontact";
                    break;
                case ChannelProvider.SendGridId:
                    result = "sendgrid";
                    break;
                case ChannelProvider.SendinBlueId:
                    result = "sendinblue";
                    break;
                case ChannelProvider.ActiveCampaignId:
                    result = "activecampaign";
                    break;
                case ChannelProvider.AweberId:
                    result = "aweber";
                    break;
                case ChannelProvider.CampaignMonitorId:
                    result = "campaignmonitor";
                    break;
                case ChannelProvider.GetResponseId:
                    result = "getreponse";
                    break;
                case ChannelProvider.MailChimpId:
                    result = "mailchimp";
                    break;
                case ChannelProvider.MailJetId:
                    result = "mailjet";
                    break;
                case ChannelProvider.OneSignalId:
                    result = "onesignal";
                    break;
                case ChannelProvider.PushEngageId:
                    result = "pushengage";
                    break;
                case ChannelProvider.PushCrewId:
                    result = "pushcrew";
                    break;
                case ChannelProvider.SubscribersId:
                    result = "subscribers";
                    break;
            }

            return result;
        }

        public List<QuestionAskToAudienceChannelDocument> GetQuestions(string audienceChannelId)
        {
            collectionName = CosmosCollections.QuestionAskToAudienceChannel.ToString();
            string query = $"SELECT * FROM {collectionName} WHERE {collectionName}.AudienceChannelId = '{audienceChannelId}'";
            List<QuestionAskToAudienceChannelDocument> list = context.ExecuteQuery<QuestionAskToAudienceChannelDocument>(databaseName, collectionName, query).ToList();
            return list;
        }
    }
}
