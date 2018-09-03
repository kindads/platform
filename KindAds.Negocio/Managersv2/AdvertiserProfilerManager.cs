using KindAds.Comun.Enums;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using KindAds.Comun.Models.ViewModel.KindAdsV2;
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
    public class AdvertiserProfileManager : BaseManager
    {
        public string databaseName { set; get; }
        public string collectionName { set; get; }

        public AdvertiserProfileManager()
        {          
            databaseName = ConfigurationManager.AppSettings["azure-cosmos-databasename"];
        }


        public List<IndustryDocument> GetIndustries()
        {
            collectionName = CosmosCollections.CAT_Industry.ToString();
            List<IndustryDocument> industries = new List<IndustryDocument>();
            industries = context.GetAllDocuments<IndustryDocument>(databaseName, collectionName);
            return industries;
        }
        public bool IsProfileCompleted(string userId)
        {
            return context.GetAllDocuments<AdvertiserProfileDocument>(databaseName, CosmosCollections.AdvertiserProfile.ToString()).Any(u => u.UserId == userId);
        }
        public List<string> GetYears()
        {
            List<string> years = new List<string>();
            for(var year= 2018; year>=1900; year--)
            {
                years.Add(year.ToString());
            }
            return years;
        }

        public List<AdvertiserPreferenceDocument> FindPreferencesByProfileId(string id)
        {
            return context.GetAllDocuments<AdvertiserPreferenceDocument>(databaseName, CosmosCollections.AdvertiserProfilePreference.ToString()).Where(u => u.AdvertiserProfileId== id).ToList();
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

        public void Update(AdvertiserProfileDocument profile, List<AdvertiserPreferenceDocument> preferences)
        {
            try
            {
                context.UpsertDocument<AdvertiserProfileDocument>(databaseName, CosmosCollections.AdvertiserProfile.ToString(), profile);
                string query = $"SELECT * FROM {CosmosCollections.AdvertiserProfilePreference.ToString()} WHERE {CosmosCollections.AdvertiserProfilePreference.ToString()}.AdvertiserProfileId='{profile.Id}'";
                List<AdvertiserPreferenceDocument> currentPreferences = context.ExecuteQuery<AdvertiserPreferenceDocument>(databaseName, CosmosCollections.AdvertiserProfilePreference.ToString(), query);
                context.DeleteDocumentsById(databaseName, CosmosCollections.AdvertiserProfilePreference.ToString(), currentPreferences.Select(c => c.Id).ToList());

                foreach (var preference in preferences)
                {
                    context.AddDocument<AdvertiserPreferenceDocument>(databaseName, CosmosCollections.AdvertiserProfilePreference.ToString(), preference);
                }
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
        }

        public List<BusinessExpertiseDocument> GetBusinessExpertise()
        {
            collectionName = CosmosCollections.CAT_BusinessExpertise.ToString();
            List<BusinessExpertiseDocument> expertise = new List<BusinessExpertiseDocument>();
            expertise = context.GetAllDocuments<BusinessExpertiseDocument>(databaseName, collectionName);
            return expertise;
        }

        public List<CountryDocument> GetCountries()
        {
            collectionName = CosmosCollections.CAT_Country.ToString();
            List<CountryDocument> countries = new List<CountryDocument>();
            countries = context.GetAllDocuments<CountryDocument>(databaseName, collectionName).OrderBy(c => c.Name).ToList();
            return countries;
        }

        public List<AdvertiserPreferenceDocument> GetPreferences(string stringify,string id)
        {
            List<AdvertiserPreferenceDocument> preferences = new List<AdvertiserPreferenceDocument>();
            preferences=JsonConvert.DeserializeObject<List<AdvertiserPreferenceDocument>>(stringify);
            cleanPreferences(preferences, id);
            return preferences;
        }

        public bool AddProfile(AdvertiserProfileViewModel viewModel)
        {
            bool result = false;
            try
            {
                // add advertiser profile document
                collectionName = CosmosCollections.AdvertiserProfile.ToString();
                context.AddDocument<AdvertiserProfileDocument>(databaseName, collectionName,viewModel.profile);

                // add preference advertiser profile documents
                collectionName = CosmosCollections.AdvertiserProfilePreference.ToString();
                foreach (var preference in viewModel.preferences)
                {
                    context.AddDocument<AdvertiserPreferenceDocument>(databaseName, collectionName, preference);
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

        public AdvertiserProfileViewModel FindProfileByUserId(string userId)
        {
            AdvertiserProfileDocument profile = context.GetAllDocuments<AdvertiserProfileDocument>(databaseName, CosmosCollections.AdvertiserProfile.ToString()).SingleOrDefault(u => u.UserId == userId);

            if (profile == null)
                return null;

            return new AdvertiserProfileViewModel {
                profile = profile
            };
        }

        public AdvertiserProfileDocument FindProfileByProfileId(string advertiserProfileId)
        {
            string query = $"SELECT * FROM {CosmosCollections.AdvertiserProfile.ToString()} WHERE {CosmosCollections.AdvertiserProfile.ToString()}.id='{advertiserProfileId}'";
            AdvertiserProfileDocument advertiserProfile  = context.ExecuteQuery<AdvertiserProfileDocument>(databaseName, CosmosCollections.AdvertiserProfile.ToString(), query).SingleOrDefault();
            
            if (advertiserProfile == null)
                return null;

            return advertiserProfile;
        }

        private void cleanPreferences(List<AdvertiserPreferenceDocument> preferences,string id)
        {
            for(int i=0; i <= (preferences.Count()-1); i++)
            {
                preferences[i].IndustryId = preferences[i].IndustryId.Replace("flex-item-", "");
                preferences[i].AdvertiserProfileId = id;
            }
        }

    }
}
