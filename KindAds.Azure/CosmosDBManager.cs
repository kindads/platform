using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System.Configuration;
using KindAds.Common.Interfaces;
using KindAds.Comun.Models.CosmosDocuments;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;

namespace KindAds.Azure
{
    public class CosmosDBManager : ITelemetria
    {
        public string EndpointUrl { set; get; }
        public string PrimaryKey { set; get; }
        public DocumentClient client { set; get; }
        public ITrace telemetria { set; get; }

        public CosmosDBManager()
        {
            EndpointUrl = ConfigurationManager.AppSettings["azure-cosmos-endpoint"];
            PrimaryKey = ConfigurationManager.AppSettings["azure-cosmos-primarykey"];
            client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);
            telemetria = new Trace();
        }

        public bool CreateDatabaseIfNotExist(string DatabaseName)
        {
            bool result = false;
            try
            {
                client.CreateDatabaseIfNotExistsAsync(new Database { Id = DatabaseName }).GetAwaiter().GetResult();
                result = true;
            }
            catch (Exception e)
            {
                string messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return result;
        }

        public bool CreateCollectionIfNotExist(string DatabaseName, string CollectionName)
        {
            bool result = false;
            try
            {
                bool resultCreateDatabaseIfNotExist = CreateDatabaseIfNotExist(DatabaseName);
                if (resultCreateDatabaseIfNotExist)
                {
                    client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(DatabaseName), new DocumentCollection { Id = CollectionName }).GetAwaiter().GetResult();
                    result = true;
                }
            }
            catch (Exception e)
            {
                string messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return result;
        }


        public bool CreateDocumentIfNotExists<T>(string DatabaseName, string CollectionName, T Document) where T : KindAdsV2Document
        {
            bool result = false;
            try
            {
                if (Document.Id != null && Document.Id != string.Empty)
                {
                    this.client.ReadDocumentAsync(UriFactory.CreateDocumentUri(DatabaseName, CollectionName, Document.Id)).GetAwaiter().GetResult();
                }
                else
                {
                    this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName), Document).GetAwaiter().GetResult();
                    result = true;
                }
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName), Document).GetAwaiter().GetResult();
                    result = true;
                }
                else
                {
                    string messageException = telemetria.MakeMessageException(de, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    telemetria.Critical(messageException);
                }
            }
            return result;
        }

        public List<T> GetAllDocuments<T>(string DatabaseName, string CollectionName) where T: KindAdsV2Document, new()
        {
            List<T> result = new List<T>();

            try
            {
                FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };
                IQueryable<T> allDocuments = this.client.CreateDocumentQuery<T>(
                        UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName), queryOptions);

                foreach (T document in allDocuments)
                {
                    result.Add(document);
                }
            }
            catch (Exception e)
            {
                string messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }          

            return result;
        }

    }
}
