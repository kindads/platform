using KindAds.Comun.Enums;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Negocio.Managersv2
{
    public class CatalogManager : BaseManager
    {
        public CatalogManager()
        {
            databaseName = ConfigurationManager.AppSettings["azure-cosmos-databasename"];
        }

        public List<CountryDocument> GetCountries()
        {   
            List<CountryDocument> countries = new List<CountryDocument>();
            countries = context.GetAllDocuments<CountryDocument>(databaseName, CosmosCollections.CAT_Country.ToString()).OrderBy(c => c.Name).ToList();
            return countries;
        }

        public CountryDocument FindCountryById(string id)
        {

            List<CountryDocument> countries = new List<CountryDocument>();
            string query = $"SELECT * FROM {CosmosCollections.CAT_Country.ToString()} WHERE {CosmosCollections.CAT_Country.ToString()}.id='{id}'";
            return context.ExecuteQuery<CountryDocument>(databaseName, CosmosCollections.CAT_Country.ToString(), query).SingleOrDefault();
        }

    }
}
