using KindAds.Azure;
using KindAds.Common.Interfaces;
using KindAds.Comun.Interfaces;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KindAdsV2.Azure
{ 
    public class KindAdsV2DataAccess : ITelemetria, IKindAdsV2DataAccess
    {
        public string EndpointUrl { set; get; }
        public string PrimaryKey { set; get; }
        public DocumentClient client { set; get; }
        public ITrace telemetria { set; get; }


        public KindAdsV2DataAccess(string endPointUrl, string primaryKey)
        {
            EndpointUrl = endPointUrl;
            PrimaryKey = primaryKey;
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

        // main method to add document
        public bool AddDocument<T>(string databaseName, string collectionName, T document) where T : KindAdsV2Document
        {
            bool result = false;
            try
            {
                CreateCollectionIfNotExist(databaseName, collectionName);
                result = CreateDocumentIfNotExists<T>(databaseName, collectionName, document);
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
                   
                    Document response=this.client.ReadDocumentAsync(UriFactory.CreateDocumentUri(DatabaseName, CollectionName, Document.Id) ).GetAwaiter().GetResult();
                    Uri docUri = UriFactory.CreateDocumentUri(DatabaseName, CollectionName, Document.Id);
                    client.ReplaceDocumentAsync(docUri, Document).GetAwaiter().GetResult();
                    result = true;
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

        public List<T> GetAllDocuments<T>(string DatabaseName, string CollectionName) where T : KindAdsV2Document, new()
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


        public void UpdateFieldByCollectionId(string databaseName, string collectionName, string id,  string field, string value)
        {


            var project = (Document)client.CreateDocumentQuery<dynamic>(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName))
            .AsEnumerable()
            .First();

            project?.SetPropertyValue(field, value );
            var document = client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, project?.Id), project).Result.Resource;
        }

        public List<T> UpsertDocuments<T>(string DatabaseName, string CollectionName, List<T> documents) where T : KindAdsV2Document, new()
        {
            List<T> result = new List<T>();

            try
            {
                foreach(T doc in documents)
                {
                    Uri docUri = UriFactory.CreateDocumentUri(DatabaseName, CollectionName, doc.Id);
                    client.ReplaceDocumentAsync(docUri, doc).GetAwaiter().GetResult();
                }   
            }
            catch (Exception e)
            {
                string messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }

            return result;
        }

        public List<T> ExecuteQuery<T>(string DatabaseName, string CollectionName, string Query) where T : KindAdsV2Document, new()
        {
            List<T> result = new List<T>();

            try
            {
                FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 , EnableCrossPartitionQuery = true };
                IQueryable<T> executionQuery = client.CreateDocumentQuery<T>(
                   UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName),
                   Query,
                   queryOptions);

                foreach (T document in executionQuery)
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

        public void DeleteDocumentsById(string DatabaseName, string collectionName, List<string> idsToDelete)
        {
            List<Document> documentsToDelete = idsToDelete.Select(i => GetDocument(DatabaseName, collectionName, i)).ToList();
            foreach(var document in documentsToDelete)
            {
                client.DeleteDocumentAsync(document.SelfLink);
            }
            
        }

        private  Document GetDocument(string DatabaseName, string collectionName, string id)
        {
            return client.CreateDocumentQuery(UriFactory.CreateDocumentCollectionUri(DatabaseName, collectionName))
                .Where(d => d.Id == id)
                .AsEnumerable()
                .FirstOrDefault();
        }

        public List<T> UpsertDocument<T>(string DatabaseName, string CollectionName, T Document) where T : KindAdsV2Document, new()
        {
            List<T> result = new List<T>();

            try
            {               
                Uri docUri = UriFactory.CreateDocumentUri(DatabaseName, CollectionName, Document.Id);
                client.ReplaceDocumentAsync(docUri, Document).GetAwaiter().GetResult();

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
