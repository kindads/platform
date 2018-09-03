using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using KindAds.Comun.Models.KindAdsV2;
using KindAds.Negocio.Managersv2;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Negocio.ViewModels.KindAdsV2
{
    public class ConversationViewModel
    {      
        public ConversationManager manager { set; get; }
        public List<ConversationDocument> conversations { set; get; }

        public string searchConversation { set; get; }

        public string currentConversation { set; get; }

        public ConversationForm conversationForm { set; get; }

        public List<ConversationItemViewModel> conversationItems { set; get; }

        public ConversationViewModel()
        {
            conversations = new List<ConversationDocument>();
            manager = new ConversationManager();
            currentConversation = string.Empty;
            searchConversation = string.Empty;
            conversationItems = new List<ConversationItemViewModel>();
        }
       
        public List<ConversationItemViewModel> GetConversationById(string conversationId)
        {
            List<ConversationItemViewModel> conversationItems = new List<ConversationItemViewModel>();
            var conversation = manager.GetConversationByConversationId(conversationId);
            List<ConversationMessageDocument> messages = manager.GetConversationMessagesByConversationId(conversation.Id);

            ConversationItemViewModel viewModel = new ConversationItemViewModel()
            {
                ProfileName = string.Empty,
                ProfileTagLine = string.Empty,
                IconUrl = string.Empty,
                Messages = messages,
                LastMessage = null,
                ConversationId = conversation.Id,
                Chunks = manager.GetConversationMessagesPerChunk(messages)
            };

            conversationItems.Add(viewModel);
            return conversationItems;
        }

        public List<ConversationItemViewModel> GetAllConversations(string email,int rolId)
        {
            ConcurrentBag<ConversationItemViewModel> conversationItemsParallel = new ConcurrentBag<ConversationItemViewModel>();
            List<ConversationItemViewModel> conversationItems = new List<ConversationItemViewModel>();
            List<ConversationDocument> conversations = new List<ConversationDocument>();
            conversations = manager.GetConversationsAsync(email, rolId);

            string profileName = string.Empty;    // from profile
            string profileTagLine = string.Empty; // from audience when is publisher
            string iconUrl = string.Empty;        // from profile

           
            PublisherProfileDocument publisherProfile= manager.GetPublisherProfile(email);
            AdvertiserProfileDocument advertiserProfile = manager.GetAdvertiserProfile(email);

            if(publisherProfile != null && rolId==1)
            {               
                profileName = publisherProfile.Name;
                iconUrl = publisherProfile.IconUrl;

                Parallel.ForEach(conversations, (conversation) => {

                    List<ConversationMessageDocument> messages = manager.GetConversationMessagesByConversationId(conversation.Id);
                    ConversationMessageDocument lastMessage = new ConversationMessageDocument();
                    if (messages.Count() > 0)
                    {
                        lastMessage = (from message in messages orderby message.RegisterDate descending select message).FirstOrDefault();
                    }
                    AudienceChannelDocument audienceChannel = manager.GetAudienceChannelById(conversation.AudienceChannelId);
                    profileTagLine = audienceChannel.TagLine;

                    ConversationItemViewModel viewModel = new ConversationItemViewModel()
                    {
                        ProfileName = manager.GetAdvertiserProfileNameByConversationId(conversation.Id, email),
                        ProfileTagLine = profileTagLine,
                        IconUrl = manager.GetAdvertiserImageByConversationId(conversation.Id, email),
                        Messages = messages,
                        LastMessage = lastMessage,
                        ConversationId = conversation.Id,
                        Chunks = manager.GetConversationMessagesPerChunk(messages)
                    };
                    conversationItemsParallel.Add(viewModel);
                });
            }
            if(advertiserProfile!=null && rolId==2)
            {
                profileName = advertiserProfile.Title;
                iconUrl = advertiserProfile.IconUrl;
                profileTagLine = advertiserProfile.Tagline;

                Parallel.ForEach(conversations, (conversation) =>
                {
                    List<ConversationMessageDocument> messages = manager.GetConversationMessagesByConversationId(conversation.Id);
                    ConversationMessageDocument lastMessage = new ConversationMessageDocument()
                    {
                        Message = string.Empty
                    };

                    if (messages.Count() > 0)
                    {
                        lastMessage = (from message in messages orderby message.RegisterDate descending select message).FirstOrDefault();
                    }
                    ConversationItemViewModel viewModel = new ConversationItemViewModel()
                    {
                        ProfileName = manager.GetPublisherProfileNameByConversationId(conversation.Id, email),
                        ProfileTagLine = profileTagLine,
                        IconUrl = manager.GetPublisherImageByConversationId(conversation.Id, email),
                        Messages = messages,
                        LastMessage = lastMessage,
                        ConversationId = conversation.Id,
                        Chunks = manager.GetConversationMessagesPerChunk(messages)
                    };
                    conversationItemsParallel.Add(viewModel);
                });
            }
            conversationItems = conversationItemsParallel.ToList();
            conversationItems.OrderByDescending(x => x.LastMessage.MessageTime);
            return conversationItems;
        }
    }
}
