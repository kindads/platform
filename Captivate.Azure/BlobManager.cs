using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Azure
{
    public static class BlobManager
    {
        public static CloudStorageAccount CreateConexion()
        {
            CloudStorageAccount storageAccount;
            string storageConnectionString = ConfigurationManager.ConnectionStrings["DefaultAzureConnection"].ConnectionString;
            CloudStorageAccount.TryParse(storageConnectionString, out storageAccount);
            return storageAccount;
        }


        public static string UploadFile(string fileName, Stream source)
        {
            CloudStorageAccount storageAccount = CreateConexion();
            CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

            // Create a container called 'quickstartblobs' and append a GUID value to it to make the name unique. 
            CloudBlobContainer container = cloudBlobClient.GetContainerReference("cvsin");
            container.CreateIfNotExists();

            //Block blob
            CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(fileName);

            // Upload file          
            //cloudBlockBlob.UploadFromFile(path);
            BlobRequestOptions options = new BlobRequestOptions();

            cloudBlockBlob.UploadFromStream(source);

            string uri = cloudBlockBlob.Uri.ToString();

            return uri;
        }

        public static string CreateHtmlTemplateAzure(string htmlText)
        {
            string storageConnectionString = ConfigurationManager.AppSettings["HtmlTemplateStorageConnection"];
            string filextension = ".html";
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            CloudBlobContainer container = blobClient.GetContainerReference("html");

            // Create the container if it doesn't already exist.
            container.CreateIfNotExists();

            string sguid = GetHashString(Guid.NewGuid().ToString());
            string sblobfile = sguid + filextension;


            CloudBlockBlob blockBlob = container.GetBlockBlobReference(sblobfile);
            blockBlob.Properties.ContentType = "text/html";

            blockBlob.UploadFromStream(new MemoryStream());
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    blockBlob.DownloadToStream(ms);
                    byte[] dataToWrite = Encoding.UTF8.GetBytes(htmlText);
                    ms.Write(dataToWrite, 0, dataToWrite.Length);
                    ms.Position = 0;
                    blockBlob.UploadFromStream(ms);
                }

                return blockBlob.Uri.AbsoluteUri;
            }
            catch (StorageException excep)
            {
                if (excep.RequestInformation.HttpStatusCode != 404)
                {
                    throw;
                }
            }

            return null;
        }

        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        public static byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = MD5.Create();  //or use SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string CreateUsrWalletBlobFile(string _passphrase, string connection)
        {
            //(returns the blobfilename to save it in user's entity

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connection);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("docskindads");
            container.CreateIfNotExists();

            string sblobfile = "bbcw" + Guid.NewGuid() + GetRandomNumber(100);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(sblobfile);

            //_passphrase = CryptoHelper.EncryptAES(_passphrase);
            blockBlob.UploadText(_passphrase);
            return sblobfile;
        }

        public static string GetRandomNumber(int maxnumber)
        {
            Random rnd = new Random();
            return rnd.Next(1, maxnumber).ToString();
        }
    }
}
