using Captivate.Common.Models;
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

        public static CloudStorageAccount CreateConexion()
        {
            CloudStorageAccount storageAccount;
            string storageConnectionString = ConfigurationManager.ConnectionStrings["DefaultAzureConnection"].ConnectionString;
            CloudStorageAccount.TryParse(storageConnectionString, out storageAccount);
            return storageAccount;
        }
    }
}
