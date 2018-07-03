using Captivate.Common.Models;
using Captivate.Comun.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Azure
{
    public static class TableManager
    {
        public static void InsertMessage(string tableName, string message)
        {
            try
            {
                CloudStorageAccount storageAccount = CreateConexion();
                CloudTableClient cloudTableClient = storageAccount.CreateCloudTableClient();

                // Retrieve a reference to the table.
                CloudTable table = cloudTableClient.GetTableReference(tableName);

                // Create the table if it doesn't exist.
                table.CreateIfNotExists();

                // Create a new customer entity.
                ErrorEntity error = new ErrorEntity(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
                error.Excepcion = message;

                // Create the TableOperation object that inserts the customer entity.
                TableOperation insertOperation = TableOperation.Insert(error);

                // Execute the insert operation.
                table.Execute(insertOperation);
            }
            catch (Exception e)
            {
                //Todo
            }            
        }

        public static void InsertAdsHtml(string tableName, string html, string PartitionName,string styleName)
        {
            try
            {
                CloudStorageAccount storageAccount = CreateConexion();
                CloudTableClient cloudTableClient = storageAccount.CreateCloudTableClient();

                // Retrieve a reference to the table.
                CloudTable table = cloudTableClient.GetTableReference(tableName);

                // Create the table if it doesn't exist.
                table.CreateIfNotExists();

                // Create a new customer entity.
                StickyHtmlEntity Style = new StickyHtmlEntity(PartitionName, styleName);
                Style.Html = html;

                // Create the TableOperation object that inserts the customer entity.
                TableOperation insertOperation = TableOperation.Insert(Style);

                // Execute the insert operation.
                table.Execute(insertOperation);
            }
            catch (Exception e)
            {
                //Todo
            }
        }


        public static DefaultHtmlEntity GetClicBehavior(string TableName, string PartitionName, string RowName)
        {
            DefaultHtmlEntity Style = new DefaultHtmlEntity();

            CloudStorageAccount storageAccount = CreateConexion();

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference(TableName);

            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<StickyHtmlEntity>(PartitionName, RowName);

            // Execute the retrieve operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);

            // Print the phone number of the result.
            if (retrievedResult.Result != null)
            {
                Style = (DefaultHtmlEntity)retrievedResult.Result;
            }
            return Style;
        }

        public static StickyHtmlEntity  GetAdsHtml(string TableName, string PartitionName,string RowName)
        {
            StickyHtmlEntity Style = new StickyHtmlEntity();

            CloudStorageAccount storageAccount = CreateConexion();

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference(TableName);

            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<StickyHtmlEntity>(PartitionName, RowName);

            // Execute the retrieve operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);

            // Print the phone number of the result.
            if (retrievedResult.Result != null)
            {
                Style = (StickyHtmlEntity)retrievedResult.Result;
            }
            return Style;
        }

        public static CloudStorageAccount CreateConexion()
        {
            CloudStorageAccount storageAccount;
            string storageConnectionString = ConfigurationManager.ConnectionStrings["DefaultAzureConnection"].ConnectionString;
            CloudStorageAccount.TryParse(storageConnectionString, out storageAccount);
            return storageAccount;
        }
    }
}
