using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Azure
{
    public static class QueueManager
    {

        public static void InsertMessage(string Message, string QueueName)
        {
            CloudStorageAccount storageAccount = CreateConexion();
            CloudQueueClient cloudQueueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to the table.
            CloudQueue queue = cloudQueueClient.GetQueueReference(QueueName.ToLower());

            // Create the table if it doesn't exist.
            queue.CreateIfNotExists();

            // Create a message and add it to the queue.
            CloudQueueMessage message = new CloudQueueMessage(Message);
            queue.AddMessage(message);
        }

        public static string GetMessage(string QueueName)
        {
            string peticionData = string.Empty;

            CloudStorageAccount storageAccount = CreateConexion();
            CloudQueueClient cloudQueueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to the table.
            
            CloudQueue queue = cloudQueueClient.GetQueueReference(QueueName);

            // Create the table if it doesn't exist.
            queue.CreateIfNotExists();

            TimeSpan ttl = new TimeSpan(0, 60, 0);

            // Get the next message
            CloudQueueMessage retrievedMessage = queue.GetMessage(ttl);
            if (retrievedMessage != null)
            {
                peticionData = retrievedMessage.AsString;
                queue.DeleteMessage(retrievedMessage);
            }

            return peticionData;
        }

        public static CloudStorageAccount CreateConexion()
        {
            CloudStorageAccount storageAccount;
            string storageConnectionString = ConfigurationManager.AppSettings["azure-storage-connectionstring"];
            CloudStorageAccount.TryParse(storageConnectionString, out storageAccount);
            return storageAccount;
        }


    }
}
