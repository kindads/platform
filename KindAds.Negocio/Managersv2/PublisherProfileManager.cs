using KindAds.Comun.Enums;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using KindAds.Comun.Models.ViewModel.KindAdsV2;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Negocio.Managersv2
{
    public class PublisherProfileManager : BaseManager
    {
        private readonly string catIndustryCollection;
        private readonly string catCountryCollection;
        private readonly string catBusinessExpertiseCollection;
        private readonly string publisherProfileCollection;
        private readonly string publisherPreferenceCollection;

        public PublisherProfileManager()
        {
            databaseName = ConfigurationManager.AppSettings["azure-cosmos-databasename"];
            catIndustryCollection = CosmosCollections.CAT_Industry.ToString();
            catCountryCollection = CosmosCollections.CAT_Country.ToString();
            catBusinessExpertiseCollection = CosmosCollections.CAT_BusinessExpertise.ToString();
            publisherProfileCollection = CosmosCollections.PublisherProfile.ToString();
            publisherPreferenceCollection = CosmosCollections.PublisherProfilePreference.ToString();
        }

        public List<IndustryDocument> GetCatIndustry()
        {
            return context.GetAllDocuments<IndustryDocument>(databaseName, catIndustryCollection);
        }
        public PublisherProfileViewModel FindProfileByUserId(string userId)
        {
            PublisherProfileDocument profile = context.GetAllDocuments<PublisherProfileDocument>(databaseName, CosmosCollections.PublisherProfile.ToString()).SingleOrDefault(u => u.UserId == userId);

            if (profile == null)
                return null;

            return new PublisherProfileViewModel
            {
                Country = Guid.Parse( profile.CountryBusinessInId),
                Description = profile.Description,
                ExperienceLevel = profile.ExperienceLevel,
                Name= profile.Name,
                profile = profile
            };
        }

        public PublisherProfileDocument FindProfileDocByUserId(string userId)
        {

            PublisherProfileDocument profile = new PublisherProfileDocument();

            collectionName = CosmosCollections.PublisherProfile.ToString();
            string query = $"SELECT * FROM {collectionName} WHERE {collectionName}.UserId='{userId}'";
            profile = context.ExecuteQuery<PublisherProfileDocument>(databaseName, collectionName, query).FirstOrDefault();
            

            if (profile == null)
                return null;
            return profile;
        }

        public void Update(PublisherProfileDocument profile, List<PublisherPreferenceDocument> preferences)
        {
            try
            {  
                context.UpsertDocument<PublisherProfileDocument>(databaseName,CosmosCollections.PublisherProfile.ToString(), profile);
                string query = $"SELECT * FROM {CosmosCollections.PublisherProfilePreference.ToString()} WHERE {CosmosCollections.PublisherProfilePreference.ToString()}.PublisherProfileId='{profile.Id}'";
                List<PublisherPreferenceDocument> currentPreferences =  context.ExecuteQuery<PublisherPreferenceDocument>(databaseName, CosmosCollections.PublisherProfilePreference.ToString(), query);
                context.DeleteDocumentsById(databaseName, CosmosCollections.PublisherProfilePreference.ToString(), currentPreferences.Select(c => c.Id).ToList());

                foreach (var preference in preferences)
                {
                    context.AddDocument<PublisherPreferenceDocument>(databaseName, CosmosCollections.PublisherProfilePreference.ToString(), preference);
                }
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
        }

        public List<PublisherPreferenceDocument> FindPreferencesByProfileId(string id)
        {
            return context.GetAllDocuments<PublisherPreferenceDocument>(databaseName, CosmosCollections.PublisherProfilePreference.ToString()).Where(u => u.PublisherProfileId== id).ToList();
        }

        public bool IsProfileCompleted(string userId)
        {
            return context.GetAllDocuments<PublisherProfileDocument>(databaseName, CosmosCollections.PublisherProfile.ToString()).Any(u => u.UserId == userId);
        }

        public List<CountryDocument> GetCatCountry()
        {
            return context.GetAllDocuments<CountryDocument>(databaseName, catCountryCollection).OrderBy(c => c.Name).ToList();
        }

        public List<BusinessExpertiseDocument> GetCatBusinessExpertise()
        {
            return context.GetAllDocuments<BusinessExpertiseDocument>(databaseName, catBusinessExpertiseCollection);
        }

        public bool AddProfile(PublisherProfileViewModel model)
        {
            bool result = false;
            try
            {
                context.AddDocument<PublisherProfileDocument>(databaseName, publisherProfileCollection, model.profile);

                foreach (var preference in model.preferences)
                {
                    context.AddDocument<PublisherPreferenceDocument>(databaseName, publisherPreferenceCollection, preference);
                }

                // set result to true
                result = true;
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return result;
        }

        public List<PublisherPreferenceDocument> GetPreferences(string stringify, string id)
        {
            List<PublisherPreferenceDocument> preferences = new List<PublisherPreferenceDocument>();
            preferences = JsonConvert.DeserializeObject<List<PublisherPreferenceDocument>>(stringify);
            cleanPreferences(preferences, id);
            return preferences;
        }

        private void cleanPreferences(List<PublisherPreferenceDocument> preferences, string id)
        {
            for (int i = 0; i <= (preferences.Count() - 1); i++)
            {
                preferences[i].IndustryId = preferences[i].IndustryId.Replace("flex-item-", "");
                preferences[i].PublisherProfileId = id;
            }
        }

    }
}
