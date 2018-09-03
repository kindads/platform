using KindAds.Azure;
using KindAds.Common.Interfaces;
using KindAds.Comun.Enums;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using KindAds.Negocio.ViewModels.KindAdsV2;
using KindAdsV2.Azure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Negocio.Managersv2
{ 

    public class AudienceManager : BaseManager
    {

        public ServiceBusManager sbmanager { set; get; }
        public string audienceId { set; get; }

        public AudienceManager()
        {
            databaseName = ConfigurationManager.AppSettings["azure-cosmos-databasename"];
            audienceId = string.Empty;
        }

        public void ProcessAudienceChangeNotification(string message, Dictionary<string, int> userPriority)
        {
            AudienceChangeNotification changeNotification = JsonConvert.DeserializeObject<AudienceChangeNotification>(message);
            

            switch (changeNotification.ChangeType)
            {
                case TypeAudienceChange.ActiveChannels:
                    string queryAudienceChannel = $"SELECT * FROM {CosmosCollections.AudienceChannel.ToString()} WHERE {CosmosCollections.AudienceChannel.ToString()}.AudienceId='{changeNotification.idAudience}'";
                    List<AudienceChannelDocument> documents = context.ExecuteQuery <AudienceChannelDocument>(databaseName, CosmosCollections.AudienceChannel.ToString(), queryAudienceChannel);
                    foreach (AudienceChannelDocument doc in documents)
                    {
                        doc.IsActive = true;
                        context.UpsertDocument<AudienceChannelDocument>(databaseName, CosmosCollections.AudienceChannel.ToString(), doc);
                    }
                    break;
                case TypeAudienceChange.ChangeDescription:
                    string queryAudienceChannelDesc = $"SELECT * FROM {CosmosCollections.AudienceChannel.ToString()} WHERE {CosmosCollections.AudienceChannel.ToString()}.AudienceId='{changeNotification.idAudience}' and {CosmosCollections.AudienceChannel.ToString()}.IsDefaultDescription=true";
                    List<AudienceChannelDocument> documentsForCDescription = context.ExecuteQuery<AudienceChannelDocument>(databaseName, CosmosCollections.AudienceChannel.ToString(), queryAudienceChannelDesc);
                    foreach (AudienceChannelDocument doc in documentsForCDescription)
                    {
                        doc.Description = changeNotification.Description;
                        context.UpsertDocument<AudienceChannelDocument>(databaseName, CosmosCollections.AudienceChannel.ToString(), doc);
                    }
                    break;
                case TypeAudienceChange.ChangeImage:
                    string queryAudienceChannelImg = $"SELECT * FROM {CosmosCollections.AudienceChannel.ToString()} WHERE {CosmosCollections.AudienceChannel.ToString()}.AudienceId='{changeNotification.idAudience}' and {CosmosCollections.AudienceChannel.ToString()}.IsDefaultImage=true";
                    List<AudienceChannelDocument> documentsForImage = context.ExecuteQuery<AudienceChannelDocument>(databaseName, CosmosCollections.AudienceChannel.ToString(), queryAudienceChannelImg);
                    foreach (AudienceChannelDocument doc in documentsForImage)
                    {
                        doc.ImageUrl = changeNotification.ImageUrl;
                        context.UpsertDocument<AudienceChannelDocument>(databaseName, CosmosCollections.AudienceChannel.ToString(), doc);
                    }
                    break;
                case TypeAudienceChange.DisableVisibility:
                    string queryAudienceChannelVisibility = $"SELECT * FROM {CosmosCollections.AudienceChannel.ToString()} WHERE {CosmosCollections.AudienceChannel.ToString()}.AudienceId='{changeNotification.idAudience}' and {CosmosCollections.AudienceChannel.ToString()}.Visibility='true'";
                    List<AudienceChannelDocument> documentsForVisibility = context.ExecuteQuery<AudienceChannelDocument>(databaseName, CosmosCollections.AudienceChannel.ToString(), queryAudienceChannelVisibility);
                    foreach (AudienceChannelDocument doc in documentsForVisibility)
                    {
                        doc.Visibility = false;
                        context.UpsertDocument<AudienceChannelDocument>(databaseName, CosmosCollections.AudienceChannel.ToString(), doc);
                    }
                    break;
            }
        }

        //public List<AudienceListItemViewModel> GetAudiences()
        //{ 
        //    List<AudienceListItemViewModel> audiences = context.GetAllDocuments<AudienceListItemViewModel>(databaseName, CosmosCollections.Audience.ToString());
        //    return audiences;
        //}


        public List<IndustryDocument> GetIndustries()
        {
            collectionName = CosmosCollections.CAT_Industry.ToString();
            List<IndustryDocument> industries = new List<IndustryDocument>();
            industries = context.GetAllDocuments<IndustryDocument>(databaseName, collectionName);
            return industries;
        }

        public List<string> GetYears()
        {
            List<string> years = new List<string>();
            for (var year = 2018; year >= 1900; year--)
            {
                years.Add(year.ToString());
            }
            return years;
        }

        public string GetSubIndustrieById(string categoryId)
        {
            throw new NotImplementedException();
        }

        public IndustryDocument GetIndustrieById(string categoryId)
        {
            string query = $"SELECT * FROM {CosmosCollections.CAT_Industry.ToString()} WHERE {CosmosCollections.CAT_Industry.ToString()}.id='{categoryId}'";
            List<IndustryDocument> industries =  context.ExecuteQuery<IndustryDocument>(databaseName, CosmosCollections.CAT_Industry.ToString(), query);
            return industries.SingleOrDefault();
        }



        public List<string> GetManyPeople()
        {
            List<string> manyPeople = new List<string>();
            manyPeople.Add("1-5");
            manyPeople.Add("6-10");
            manyPeople.Add("11-20");
            manyPeople.Add("20-50");
            manyPeople.Add("50-100");
            manyPeople.Add("100-500");
            manyPeople.Add("500 plus");
            return manyPeople;
        }


        public List<BusinessExpertiseDocument> GetBusinessExpertise()
        {
            collectionName = CosmosCollections.CAT_BusinessExpertise.ToString();
            List<BusinessExpertiseDocument> expertise = new List<BusinessExpertiseDocument>();
            expertise = context.GetAllDocuments<BusinessExpertiseDocument>(databaseName, collectionName);
            return expertise;
        }

        public bool IsAudienceDuplicated(string publisherProfileId, string webSiteUrl)
        {
            webSiteUrl = webSiteUrl.ToLower().Replace("https://", "").Replace("http://", "").Replace("www", "");
            return  context.GetAllDocuments<AudienceDocument>(databaseName, CosmosCollections.Audience.ToString())
                .Any(a => a.PublisherId == publisherProfileId && a.IsActive == true && a.WebSiteUrl.ToLower().Replace("https://","").Replace("http://", "").Replace("www", "") == webSiteUrl);
        }

        public List<CountryDocument> GetCountries()
        {
            collectionName = CosmosCollections.CAT_Country.ToString();
            List<CountryDocument> countries = new List<CountryDocument>();
            countries = context.GetAllDocuments<CountryDocument>(databaseName, collectionName).OrderBy(c => c.Name).ToList();
            return countries;
        }

        public List<AudiencePreferenceDocument> GetPreferences(string stringify, string id)
        {
            List<AudiencePreferenceDocument> preferences = new List<AudiencePreferenceDocument>();
            preferences = JsonConvert.DeserializeObject<List<AudiencePreferenceDocument>>(stringify);
            cleanPreferences(preferences, id);
            return preferences;
        }


        public void AddIconUrl(AudienceViewModel viewModel)
        {
            try
            {
                collectionName = CosmosCollections.Audience.ToString();
                context.AddDocument<AudienceDocument>(databaseName, collectionName, viewModel.audience);
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }           
        }

        public void AddPreferences(AudienceViewModel viewModel,string Id)
        {
            try
            {
                //todo
                collectionName = CosmosCollections.AudiencePreference.ToString();
                List<AudiencePreferenceDocument> preferences = GetPreferences(viewModel.preferencesStringify, Id);
                viewModel.preferences = preferences;
                foreach (var preference in preferences)
                {
                    context.AddDocument<AudiencePreferenceDocument>(databaseName, collectionName, preference);
                }

                // updte audience
                collectionName = CosmosCollections.Audience.ToString();
                context.AddDocument<AudienceDocument>(databaseName, collectionName, viewModel.audience);

            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
        }

        public List<AudiencePreferenceDocument> GetPreferencesByAudienceId(string audienceId)
        {
            string query = $"SELECT * FROM {CosmosCollections.AudiencePreference.ToString()} WHERE {CosmosCollections.AudiencePreference.ToString()}.AudienceId='{audienceId}'";
            return context.ExecuteQuery<AudiencePreferenceDocument>(databaseName, CosmosCollections.AudiencePreference.ToString(), query);
        }

        public AudienceDocument UpdateAudience(AudienceDocument audience, string preferencesStringify)
        {
            
            List<AudiencePreferenceDocument> existingPreferences = GetPreferencesByAudienceId(audience.Id);

            context.DeleteDocumentsById(databaseName, CosmosCollections.AudiencePreference.ToString(), existingPreferences.Select(p => p.Id).ToList());
            if (!string.IsNullOrEmpty(preferencesStringify))
            {
                List<AudiencePreferenceDocument> preferences = GetPreferences(preferencesStringify, audience.Id);
                foreach (var preference in preferences)
                {
                    preference.Id = Guid.NewGuid().ToString();

                    context.AddDocument<AudiencePreferenceDocument>(databaseName, CosmosCollections.AudiencePreference.ToString(), preference);
                }
            }
            

            context.UpsertDocument<AudienceDocument>(this.databaseName, CosmosCollections.Audience.ToString(), audience);
            
            return GetAudienceById(audience.Id);
        }

        public List<QuestionAskToAudienceChannelDocument> GetQuestionsByAudienceChannelId(string audienceChannelId)
        {
            string query = $"SELECT * FROM {CosmosCollections.QuestionAskToAudienceChannel.ToString()} WHERE {CosmosCollections.QuestionAskToAudienceChannel.ToString()}.AudienceChannelId='{audienceChannelId}'";
            List<QuestionAskToAudienceChannelDocument>  questions =  context.ExecuteQuery<QuestionAskToAudienceChannelDocument>(databaseName, CosmosCollections.QuestionAskToAudienceChannel.ToString(), query).ToList();
            return questions;
        }

        public string AddAudience(AudienceViewModel viewModel)
        {
            string id = string.Empty;
            Guid audienceId = Guid.NewGuid();

            try
            {
                //todo
                viewModel.audience.VerificationString = Guid.NewGuid().ToString();
                collectionName = CosmosCollections.Audience.ToString();
                if (viewModel.audience.Id == null)
                {
                    viewModel.audience.Id = audienceId.ToString();
                    context.AddDocument<AudienceDocument>(databaseName, collectionName, viewModel.audience);
                    id = audienceId.ToString();
                }
                else
                {
                    context.AddDocument<AudienceDocument>(databaseName, collectionName, viewModel.audience);
                    id = audienceId.ToString();
                }                
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return id;
        }

        private void cleanPreferences(List<AudiencePreferenceDocument> preferences, string id)
        {
            for (int i = 0; i <= (preferences.Count() - 1); i++)
            {
                preferences[i].IndustryId = preferences[i].IndustryId.Replace("flex-item-", "");
                preferences[i].AudienceId = id;
            }
        }

        public AudienceDocument GetAudienceById(string idAudience)
        {
            collectionName = CosmosCollections.Audience.ToString();
            string query = $"SELECT * FROM {collectionName} WHERE {collectionName}.id='{idAudience}'";
            var audience = context.ExecuteQuery<AudienceDocument>(databaseName, collectionName, query).FirstOrDefault();
            return audience;
        }

        public bool UpdateAudience(AudienceDocument audienceDocument)
        {
            return context.AddDocument(databaseName, CosmosCollections.Audience.ToString(), audienceDocument);
        }

        public List<AudienceDocument> GetAudiencesByPublisherProfileId(string publisherProfileId)
        {
            List<AudienceDocument> audiences = new List<AudienceDocument>();
            collectionName = CosmosCollections.Audience.ToString();
            string query = $"SELECT * FROM {collectionName} WHERE {collectionName}.PublisherId='{publisherProfileId}' AND {collectionName}.IsActive= true ";
            audiences = context.ExecuteQuery<AudienceDocument>(databaseName, collectionName, query);
            return audiences;
        }

        public List<AudienceListItemViewModel> GetAudiencesViewModelByPublisherProfileId(string publisherProfileId)
        {
            List<AudienceListItemViewModel> audiencesViewModel = new List<AudienceListItemViewModel>();
            if(publisherProfileId!=null && publisherProfileId!=string.Empty)
            {
                var audiences = GetAudiencesByPublisherProfileId(publisherProfileId);
                foreach (var audience in audiences)
                {
                    AudienceListItemViewModel viewModel = new AudienceListItemViewModel();
                    viewModel.audience = audience;
                    audiencesViewModel.Add(viewModel);
                }
            }
            return audiencesViewModel;
        }

        public PublisherProfileDocument GetPublisherProfile(string userId)
        {
            collectionName = CosmosCollections.PublisherProfile.ToString();
            string query = $"SELECT * FROM {collectionName} WHERE {collectionName}.UserId='{userId}'";
            var profile = context.ExecuteQuery<PublisherProfileDocument>(databaseName, collectionName, query).FirstOrDefault();
            return profile;
        }


        public bool EnqueueAudienceChange(AudienceChangeNotification audience)
        {
            if (string.IsNullOrEmpty(audience.idAudience))
                return false;

            bool result = false;
            string notification = JsonConvert.SerializeObject(audience );
            string queueName = ConfigurationManager.AppSettings["azure-queue-audience-changenotification"];
            QueueManager.InsertMessage(notification, queueName);
            result = true;
            return result;
        }

        public void Close()
        {
            if (sbmanager != null)
                sbmanager.Close();
        }

    }

    public class AudienceChangeNotification
    {
        public TypeAudienceChange ChangeType { get; set; }

        public string idAudience { get; set; }

        public string ImageUrl { get; set; }

        public string Description { get; set; }
    }

    public enum TypeAudienceChange
    {
        DisableVisibility = 1,
        ChangeDescription =2,
        ChangeImage = 3,
        ActiveChannels = 4
    }

 

}
