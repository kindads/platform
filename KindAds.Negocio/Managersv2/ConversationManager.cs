using KindAds.Comun.Enums;
using KindAds.Comun.Models;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using KindAds.Comun.Models.ViewModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Negocio.Managersv2
{
    public class ConversationManager: BaseManager
    {

        public ConversationManager()
        {
            databaseName = ConfigurationManager.AppSettings["azure-cosmos-databasename"];
        }

        #region Get methods

        public void ValidateCurrentChunk(ref List<ChunkMessage> chunks)
        {
            DateTime time = DateTime.UtcNow;
            if (chunks.Count() == 0)
            {
                ChunkMessage chunk = new ChunkMessage
                {
                    Messages = new List<ConversationMessageDocument>(),
                    DateConversation = time,
                    HeadingDate = CalculateHeadingDate(time),
                    ElementToAttach = CalculateElementToAttach(time)
                };
                chunks.Add(chunk);
            }
        }


        public List<ChunkMessage> GetConversationMessagesPerChunk(List<ConversationMessageDocument> messages)
        {
            // siempre debe de traer al menos el chunck del dia actual
            List<ChunkMessage> chunks = new List<ChunkMessage>();

            try
            {
                List<List<ConversationMessageDocument>> messagesOrderByDate = GetMessagesOrdeByDistinctDay(messages);
                foreach(var messageByDate in messagesOrderByDate)
                {
                    if (messageByDate.Count > 0)
                    {
                        ChunkMessage chunk = new ChunkMessage
                        {
                            Messages = messageByDate,
                            DateConversation = DateTime.Parse(messageByDate[0].MessageTime),
                            HeadingDate = CalculateHeadingDate(DateTime.Parse(messageByDate[0].MessageTime)),
                            ElementToAttach = CalculateElementToAttach(DateTime.Parse(messageByDate[0].MessageTime))
                        };

                        chunks.Add(chunk);
                    }
                }
                ValidateCurrentChunk(ref chunks);
                chunks.OrderByDescending(x => x.DateConversation);
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return chunks;
        }

        public string CalculateElementToAttach(DateTime time)
        {
            string elementToAttach = string.Empty;
            string month = time.Month.ToString();
            string day = time.Day.ToString();
            string year = time.Year.ToString();

            if (time.Month < 10)
            {
                month = "0" + time.Month.ToString();
            }
            if(time.Day < 10)
            {
                day = "0" + time.Day.ToString();
            }
            elementToAttach = string.Format("date_{0}_{1}_{2}", year , month , day );
            return elementToAttach;
        }

        public string CalculateHeadingDate(DateTime time)
        {
            string headingDate = string.Empty;
            //todo, el formato debera ser {nombreMes} {dia}, {año}
            headingDate = string.Format(" {0} {1}, {2}", time.ToString("MMM", CultureInfo.InvariantCulture), time.Day.ToString(),time.Year.ToString());
            return headingDate;
        }

        public List<List<ConversationMessageDocument>> GetMessagesOrdeByDistinctDay(List<ConversationMessageDocument> messages)
        {
            List<List<ConversationMessageDocument>> orderBy = new List<List<ConversationMessageDocument>>();
            // obtenemos todos los elementos distinos tomando en cuenta el parametro 
            List<string> distinctdays = new List<string>();
            if(messages.Count>0)
            {
                // get all distinct days
                string distinctday = messages[0].ElementToAttach;
                distinctdays.Add(distinctday);
                foreach (var msm in messages)
                {
                    if(msm.ElementToAttach != distinctday)
                    {
                        distinctday = msm.ElementToAttach;
                        distinctdays.Add(distinctday);
                    }
                }

                // get list by distinct day
                foreach(var day in distinctdays)
                {
                    List<ConversationMessageDocument> messagePerDay = new List<ConversationMessageDocument>();
                    foreach (var msm in messages)
                    {
                        if (msm.ElementToAttach == day)
                        {
                            messagePerDay.Add(msm);
                        }
                    }
                    //todo ordenamiento.
                    messagePerDay.OrderByDescending(x => x.MessageTime);
                    orderBy.Add(messagePerDay);
                }
            }
            return orderBy;
        }

        public List<ConversationMessageDocument> GetConversationMessagesByConversationId(string conversationId)
        {
            List<ConversationMessageDocument> messages = new List<ConversationMessageDocument>();

            try
            {
                collectionName = CosmosCollections.ConversationMessage.ToString();
                string query = $"SELECT * FROM {collectionName} WHERE {collectionName}.ConversationId='{conversationId}'";
                messages = context.ExecuteQuery<ConversationMessageDocument>(databaseName, collectionName, query);
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
           
            return messages;
        }

        public string GetFormatedMessageTime(string messageTime)
        {
            string formattedTime = string.Empty;
            DateTime time = DateTime.Parse(messageTime);           
            string HourOfMessage = time.Hour.ToString();
            string MinuteOfMessage = time.Minute.ToString();
            formattedTime = HourOfMessage + ": " + MinuteOfMessage;
            return formattedTime;
        }

        public List<string> GetUsersIdInConversation(string conversationId)
        {
            List<string> userIds = new List<string>();
            try
            {
                List<string> profileIds = GetProfilesFromConversationPeople(conversationId);
                userIds = GetUserIdsWithProfileId(profileIds);
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }            
            return userIds;
        }

        public List<string> GetProfilesFromConversationPeople(string conversationId)
        {
            List<ConversationPeopleDocument> peopleInConversation = new List<ConversationPeopleDocument>();
            List<string> profileIds = new List<string>();
            try
            {
                collectionName = CosmosCollections.ConversationPeople.ToString();
                string query = $"SELECT {collectionName}.ProfileId FROM {collectionName} WHERE {collectionName}.ConversationId='{conversationId}'";
                peopleInConversation = context.ExecuteQuery<ConversationPeopleDocument>(databaseName, collectionName, query);
                foreach (var peopleIn in peopleInConversation)
                {
                    profileIds.Add(peopleIn.ProfileId);
                }
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }           
            return profileIds;
        }

        public AdvertiserProfileDocument GetAdvertiserProfileByProfileId(string profileId)
        {
            AdvertiserProfileDocument profile = null;
            try
            {
                collectionName = CosmosCollections.AdvertiserProfile.ToString();
                string query = $"SELECT * FROM {collectionName} WHERE {collectionName}.id='{profileId}'";
                profile = context.ExecuteQuery<AdvertiserProfileDocument>(databaseName, collectionName, query).FirstOrDefault();
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return profile;
        }

        public PublisherProfileDocument GetPublisherProfileByProfileId(string profileId)
        {
            PublisherProfileDocument profile = null;
            try
            {
                collectionName = CosmosCollections.PublisherProfile.ToString();
                string query = $"SELECT * FROM {collectionName} WHERE {collectionName}.id='{profileId}'";
                profile = context.ExecuteQuery<PublisherProfileDocument>(databaseName, collectionName, query).FirstOrDefault();
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return profile;
        }

        public List<string> GetUserIdsWithProfileId(List<string> profileIds)
        {
            List<string> userIds = new List<string>();
            try
            {
                foreach (var profileId in profileIds)
                {
                    AdvertiserProfileDocument adervertiserProfile = GetAdvertiserProfileByProfileId(profileId);
                    PublisherProfileDocument publisherProfile = GetPublisherProfileByProfileId(profileId);

                    if (adervertiserProfile != null)
                    {
                        userIds.Add(adervertiserProfile.UserId);
                    }
                    if (publisherProfile != null)
                    {
                        userIds.Add(publisherProfile.UserId);
                    }
                }
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }           
            return userIds;
        }

        public ConversationDocument GetConversationByConversationId(string conversationId)
        {
            ConversationDocument conversation = new ConversationDocument();
            try
            {
                collectionName = CosmosCollections.Conversation.ToString();
                string query = $"SELECT * FROM {collectionName} WHERE {collectionName}.id='{conversationId}'";
                conversation = context.ExecuteQuery<ConversationDocument>(databaseName, collectionName, query).FirstOrDefault();
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }           
            return conversation;
        }

        public AudienceChannelDocument GetAudienceChannelById(string audienceChannelId)
        {
            AudienceChannelDocument audienceChannel = null;
            try
            {
                collectionName = CosmosCollections.AudienceChannel.ToString();
                string query = $"SELECT * FROM {collectionName} WHERE {collectionName}.id='{audienceChannelId}'";
                audienceChannel = context.ExecuteQuery<AudienceChannelDocument>(databaseName, collectionName, query).FirstOrDefault();
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }           
            return audienceChannel;
        }

        public PublisherProfileDocument GetPublisherProfile(string email)
        {
            PublisherProfileDocument profile = null;
            try
            {
                string userId = GetUserId(email);
                collectionName = CosmosCollections.PublisherProfile.ToString();
                string query = $"SELECT * FROM {collectionName} WHERE {collectionName}.UserId='{userId}'";
                profile = context.ExecuteQuery<PublisherProfileDocument>(databaseName, collectionName, query).FirstOrDefault();
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }           
            return profile;
        }

        public AdvertiserProfileDocument GetAdvertiserProfile(string email)
        {
            AdvertiserProfileDocument profile = null;
            try
            {
                string userId = GetUserId(email);
                collectionName = CosmosCollections.AdvertiserProfile.ToString();
                string query = $"SELECT * FROM {collectionName} WHERE {collectionName}.UserId='{userId}'";
                profile = context.ExecuteQuery<AdvertiserProfileDocument>(databaseName, collectionName, query).FirstOrDefault();
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }           
            return profile;
        }

        public string GetAdvertiserImageByConversationId(string conversationId,string email)
        {
            AdvertiserProfileDocument publisherProfile = new AdvertiserProfileDocument();

            string profileImage = string.Empty;
            string query = string.Empty;
            List<string> users = GetUsersIdInConversation(conversationId);
            // todo
            collectionName = CosmosCollections.User.ToString();

            // user one
            query = $"SELECT * FROM {collectionName} WHERE {collectionName}.id='{users[0]}'";
            UserDocument userOne = context.ExecuteQuery<UserDocument>(databaseName, collectionName, query).FirstOrDefault();

            // user two
            query = $"SELECT * FROM {collectionName} WHERE {collectionName}.id='{users[1]}'";
            UserDocument userTwo = context.ExecuteQuery<UserDocument>(databaseName, collectionName, query).FirstOrDefault();

            if (userOne != null && userOne.Email != email)
            {
                publisherProfile = GetAdvertiserProfile(userOne.Email);
                profileImage = publisherProfile.IconUrl;
            }
            if (userTwo != null && userTwo.Email != email)
            {
                publisherProfile = GetAdvertiserProfile(userTwo.Email);
                profileImage = publisherProfile.IconUrl;
            }
            return profileImage;
        }

        public string GetPublisherImageByConversationId(string conversationId,string email)
        {
            PublisherProfileDocument publisherProfile = new PublisherProfileDocument();

            string profileImage = string.Empty;
            string query = string.Empty;
            List<string> users = GetUsersIdInConversation(conversationId);
            // todo
            collectionName = CosmosCollections.User.ToString();

            // user one
            query = $"SELECT * FROM {collectionName} WHERE {collectionName}.id='{users[0]}'";
            UserDocument userOne = context.ExecuteQuery<UserDocument>(databaseName, collectionName, query).FirstOrDefault();

            // user two
            query = $"SELECT * FROM {collectionName} WHERE {collectionName}.id='{users[1]}'";
            UserDocument userTwo = context.ExecuteQuery<UserDocument>(databaseName, collectionName, query).FirstOrDefault();

            if (userOne != null && userOne.Email != email)
            {
                publisherProfile = GetPublisherProfile(userOne.Email);
                profileImage = publisherProfile.IconUrl;
            }
            if (userTwo != null && userTwo.Email != email)
            {
                publisherProfile = GetPublisherProfile(userTwo.Email);
                profileImage = publisherProfile.IconUrl;
            }
            return profileImage;
        }

        public string GetAdvertiserProfileNameByConversationId(string conversationId,string email)
        {
            AdvertiserProfileDocument publisherProfile = new AdvertiserProfileDocument();

            string profileName = string.Empty;
            string query = string.Empty;
            List<string> users = GetUsersIdInConversation(conversationId);
            // todo
            collectionName = CosmosCollections.User.ToString();

            // user one
            query = $"SELECT * FROM {collectionName} WHERE {collectionName}.id='{users[0]}'";
            UserDocument userOne = context.ExecuteQuery<UserDocument>(databaseName, collectionName, query).FirstOrDefault();

            // user two
            query = $"SELECT * FROM {collectionName} WHERE {collectionName}.id='{users[1]}'";
            UserDocument userTwo = context.ExecuteQuery<UserDocument>(databaseName, collectionName, query).FirstOrDefault();

            if (userOne != null && userOne.Email != email)
            {
                publisherProfile = GetAdvertiserProfile(userOne.Email);
                profileName = publisherProfile.Title;
            }
            if (userTwo != null && userTwo.Email != email)
            {
                publisherProfile = GetAdvertiserProfile(userTwo.Email);
                profileName = publisherProfile.Title;
            }
            return profileName;
        }

        public string GetPublisherProfileNameByConversationId(string conversationId,string email)
        {
            PublisherProfileDocument publisherProfile = new PublisherProfileDocument();          

            string profileName = string.Empty;
            string query = string.Empty;
            List<string> users = GetUsersIdInConversation(conversationId);
            // todo
            collectionName = CosmosCollections.User.ToString();

            // user one
            query = $"SELECT * FROM {collectionName} WHERE {collectionName}.id='{users[0]}'";
            UserDocument userOne = context.ExecuteQuery<UserDocument>(databaseName, collectionName, query).FirstOrDefault();

            // user two
            query = $"SELECT * FROM {collectionName} WHERE {collectionName}.id='{users[1]}'";
            UserDocument userTwo = context.ExecuteQuery<UserDocument>(databaseName, collectionName, query).FirstOrDefault();

            if (userOne != null && userOne.Email != email)
            {
                publisherProfile = GetPublisherProfile(userOne.Email);
                profileName = publisherProfile.Name;
            }
            if(userTwo!=null && userTwo.Email != email)
            {
                publisherProfile = GetPublisherProfile(userTwo.Email);
                profileName = publisherProfile.Name;
            }
            return profileName;
        }

        public List<ConversationDocument> GetConversationsAsync(string email,int rolId)
        {
            collectionName = CosmosCollections.Conversation.ToString();
            List<ConversationDocument> conversations = new List<ConversationDocument>();
            List<string> conversationIds = new List<string>();

            try
            {
                string userId = GetUserId(email);
                string publisherProfileId = GetPublisherProfileId(userId);
                string advertiserProfileId = GetAdvertiserProfileId(userId);

                if (publisherProfileId != string.Empty && rolId==1)
                {
                    var audienceIds = GetAllAudienceByPublisherProfileIdAsync(publisherProfileId);
                    var audienceChannelIds =  GetAudienceChannelsIdsAsync(audienceIds);
                    conversations.AddRange(GetAllConversationAsync(audienceChannelIds));
                }
                if (advertiserProfileId != string.Empty && rolId==2)
                {       
                    conversationIds =  GetAllConversationIdFromProfileIdAsync(advertiserProfileId);
                    conversations.AddRange(GetAllConversationWithListIdsAsync(conversationIds));
                }                
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }    
            return conversations;
        }

        public List<ConversationDocument> GetAllConversationWithListIdsAsync(List<string> conversationIds)
        {
            List<ConversationDocument> conversations = new List<ConversationDocument>();
            try
            {
                int numOfConversations = conversationIds.Count();
                int currentConversationId = 0;
                string inRange = "(";

                foreach (var conversationId in conversationIds)
                {
                    if (currentConversationId == numOfConversations - 1)
                    {
                        inRange = inRange + "'" + conversationId + "'";
                    }
                    else
                    {
                        inRange = inRange + "'" + conversationId + "',";
                    }                    
                    currentConversationId++;
                }

                inRange = inRange + ")";

                collectionName = CosmosCollections.Conversation.ToString();
                string query = $"SELECT * FROM {collectionName} WHERE {collectionName}.id IN {inRange}";
                conversations = context.ExecuteQuery<ConversationDocument>(databaseName, collectionName, query);
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return conversations;
        }

        public List<string> GetAllConversationIdFromProfileIdAsync(string profileId)
        {
            List<ConversationPeopleDocument> allconversations = new List<ConversationPeopleDocument>();
            List<string> conversations = new List<string>();
            try
            {
                collectionName = CosmosCollections.ConversationPeople.ToString();
                string query = $"SELECT {collectionName}.ConversationId FROM {collectionName} WHERE {collectionName}.ProfileId='{profileId}'";
                allconversations = context.ExecuteQuery<ConversationPeopleDocument>(databaseName, collectionName, query);

                // then filter only return distinct ConversationId
                Dictionary<string, int> distinctConversation = new Dictionary<string, int>();
                ConcurrentDictionary<string, int> distinctConversationCurrent = new ConcurrentDictionary<string, int>();

                Parallel.ForEach(allconversations, (conversation) => {
                    int auxvalue = 0;
                    distinctConversationCurrent.TryGetValue(conversation.ConversationId, out auxvalue);
                    if (auxvalue == 0)
                    {
                        distinctConversationCurrent.AddOrUpdate(conversation.ConversationId, 1, (key, oldValue) => oldValue);
                    }
                });

                distinctConversation = distinctConversationCurrent.ToDictionary(entry => entry.Key, entry => entry.Value);
                conversations = distinctConversation.Keys.ToList();
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return conversations;
        }

        public string GetUserId(string email)
        {
            string userId = string.Empty;
            try
            {
                collectionName = CosmosCollections.User.ToString();
                string query = $"SELECT * FROM {collectionName} WHERE {collectionName}.Email='{email}'";
                var user = context.ExecuteQuery<UserDocument>(databaseName, collectionName, query).FirstOrDefault();
                userId = user.Id;
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }            
            return userId;
        }

        public string GetAdvertiserProfileId(string userId)
        {
            string advertiserProfileId = string.Empty;
            try
            {
                collectionName = CosmosCollections.AdvertiserProfile.ToString();
                string query = $"SELECT * FROM {collectionName} WHERE {collectionName}.UserId='{userId}'";
                var profile = context.ExecuteQuery<AdvertiserProfileDocument>(databaseName, collectionName, query).FirstOrDefault();

                if (profile != null)
                {
                    advertiserProfileId = profile.Id;
                }
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }            
            return advertiserProfileId;
        }

        public string GetPublisherProfileId(string userId)
        {
            string publisherProfileId = string.Empty;
            try
            {
                collectionName = CosmosCollections.PublisherProfile.ToString();
                string query = $"SELECT * FROM {collectionName} WHERE {collectionName}.UserId='{userId}'";
                var profile = context.ExecuteQuery<PublisherProfileDocument>(databaseName, collectionName, query).FirstOrDefault();

                if (profile != null)
                {
                    publisherProfileId = profile.Id;
                }
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }                                           
            return publisherProfileId;
        }

      

        public List<string> GetAllAudienceByPublisherProfileIdAsync(string publisherProfileId)
        {
            List<string> audienceIds = new List<string>();
            try
            {
                collectionName = CosmosCollections.Audience.ToString();
                string query = $"SELECT * FROM {collectionName} WHERE {collectionName}.PublisherId='{publisherProfileId}'";
                var audiences = context.ExecuteQuery<PublisherProfileDocument>(databaseName, collectionName, query);

                foreach (var audience in audiences)
                {
                    audienceIds.Add(audience.Id);
                }
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }            
            return audienceIds;
        }

        public List<string> GetAudienceChannelsIdsAsync(List<string> audienceIds)
        {
            List<string> audienceChannelIds = new List<string>();
            try
            {
                collectionName = CosmosCollections.AudienceChannel.ToString();
                string query = string.Empty;

                foreach (var audienceId in audienceIds)
                {
                    query = $"SELECT * FROM {collectionName} WHERE {collectionName}.AudienceId='{audienceId}'";
                    var audienceChannels = context.ExecuteQuery<AudienceChannelDocument>(databaseName, collectionName, query);
                    if (audienceChannels.Count > 0)
                    {
                        foreach (var audienceChannel in audienceChannels)
                        {
                            audienceChannelIds.Add(audienceChannel.Id);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }           
            return audienceChannelIds;
        }

        public List<ConversationDocument> GetAllConversationAsync(List<string> audienceChannelIds)
        {
            ConcurrentBag<ConversationDocument> conversationsParallel = new ConcurrentBag<ConversationDocument>();
            List<ConversationDocument> conversations = new List<ConversationDocument>();
            try
            {
                collectionName = CosmosCollections.Conversation.ToString();
                string query = string.Empty;

                Parallel.ForEach(audienceChannelIds, (audienceChannelId) =>
                 {
                     query = $"SELECT * FROM {collectionName} WHERE {collectionName}.AudienceChannelId='{audienceChannelId}'";
                     var nconversation = context.ExecuteQuery<ConversationDocument>(databaseName, collectionName, query);
                     if (nconversation != null)
                     {
                         foreach(var conversation in nconversation)
                         {
                             conversationsParallel.Add(conversation);
                         }                         
                     }
                 });
                
                conversations = conversationsParallel.ToList();
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }            
            return conversations;
        }
        #endregion

        #region create conversation
        public bool CreateConversation(ConversationParametersViewModel conversationParameters)
        {
            bool result = false;
            bool resultCreateConversationDocument = false;
            bool resultCreateAddConversationPeople = false;
            string conversationId = Guid.NewGuid().ToString();

            try
            {
                resultCreateConversationDocument = CreateConversationDocument(conversationId,conversationParameters.AudienceChannelId, conversationParameters.NameConversation);
                resultCreateAddConversationPeople = AddConversationPeopleDocument(conversationId, conversationParameters.PublisherId, conversationParameters.AdvertiserId);

                result = (resultCreateConversationDocument && resultCreateAddConversationPeople)==true?true:false;
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return result;
        }

        public bool CreateConversationDocument(string conversationId,string audienceChannelId,string name)
        {
            bool result = false;
            try
            {
                ConversationDocument conversation = new ConversationDocument
                {
                    Id = conversationId,
                    Name = name,
                    AudienceChannelId = audienceChannelId
                };

                collectionName = CosmosCollections.Conversation.ToString();
                result=context.AddDocument<ConversationDocument>(databaseName, collectionName, conversation);  
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return result;
        }

        public bool AddConversationPeopleDocument(string conversationId, string publisherId, string advertiserId)
        {
            bool result = false;
            bool addConversationPublisherId = false;
            bool addConversationAdvertiserId = false;

            try
            {
                addConversationPublisherId = CreateConversationPeopleDocument(conversationId, publisherId);
                addConversationAdvertiserId = CreateConversationPeopleDocument(conversationId, advertiserId);

                result = (addConversationPublisherId && addConversationAdvertiserId)==true?true:false;
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return result;
        }


        public bool CreateConversationPeopleDocument(string conversationId,string profileId)
        {
            bool result = false;
            try
            {
                ConversationPeopleDocument peopleDocument = new ConversationPeopleDocument
                {
                    ProfileId = profileId,
                    ConversationId=conversationId
                };               
                collectionName = CosmosCollections.ConversationPeople.ToString();
                result = context.AddDocument<ConversationPeopleDocument>(databaseName, collectionName, peopleDocument);
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return result;
        }

        public string MakeRefIfContainLink(string message)
        {
            StringBuilder s = new StringBuilder();
            List<string> words = message.Split(' ').ToList();

            foreach (var word in words)
            {
                bool containLink = (word.Contains("https://") || word.Contains("http://") || word.Contains("www."));
                if (containLink)
                {
                    if (word.Contains("www.") && !word.Contains("http://") && !word.Contains("https://"))
                    {
                        s.Append($@"<a href='http://{word}' target='_blank'>{word}</a> ");
                    }
                    else
                    {
                        s.Append($@"<a href='{word}' target='_blank'>{word}</a> ");
                    }
                }
                else
                {
                    s.Append($@"{word} ");
                }
            }
            return s.ToString();
        }

        public bool AddMessageToCosmos(ConversationMessageNotification conversationNotification)
        {
            bool result = false;
            try
            {
                ConversationMessageDocument message = new ConversationMessageDocument
                {
                    AttachedFile = false,
                    AttachedFileUrl = string.Empty,
                    ConversationId = conversationNotification.currentConversation.Id,
                    Message = conversationNotification.currentConversation.Message,
                    MessageWithoutHtml= conversationNotification.currentConversation.MessageWithoutHtml,
                    MessageTime = conversationNotification.currentConversation.MessageTime.ToString(),
                    SignedBy=conversationNotification.currentConversation.ProfileName
                };

                collectionName = CosmosCollections.ConversationMessage.ToString();
                result = context.AddDocument<ConversationMessageDocument>(databaseName, collectionName, message);
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return result;
        }

        #endregion
    }
}
