using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace captivate_express_webapp.Helpers
{
  public class AzureStorageHelper
  {
    public static string CreateBlobFile(byte[] sdata, string filextension)
    {
      CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Utils.Configuration.AppSettings.AzureStorageConnection);

      // Create the blob client.
      CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

      // Retrieve a reference to a container.
      CloudBlobContainer container = blobClient.GetContainerReference("pubimageskindads");

      // Create the container if it doesn't already exist.
      container.CreateIfNotExists();

      byte[] resized_image = Helpers.FileHelper.ResizeImageSite(sdata);
      string sguid = Guid.NewGuid().ToString();
      string sblobfile = sguid + filextension;
     

      CloudBlockBlob blockBlob = container.GetBlockBlobReference(sblobfile);
      blockBlob.UploadFromByteArray(resized_image, 0, resized_image.Length);

      ////Saves the full size image
      //string sblobfile_fullsize = sguid + "_fullsize" + filextension;
      //CloudBlockBlob blockBlob_fullsize = container.GetBlockBlobReference(sblobfile_fullsize);
      //blockBlob_fullsize.UploadFromByteArray(sdata, 0, sdata.Length);

      return blockBlob.Uri.AbsoluteUri;

     

    }

    public static string GetBlobBase64(string sblobname)
    {
      string sreturn64 = "";

      if (sblobname != null)
      {
        if (sblobname.Length > 12)
        {
          // Retrieve storage account from connection string.
          CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Utils.Configuration.AppSettings.AzureStorageConnection);

          // Create the blob client.
          CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
          // Retrieve reference to a previously created container.
          CloudBlobContainer container = blobClient.GetContainerReference("docskindads");

          // Retrieve reference to a blob named "myblob.txt"
          CloudBlockBlob blockBlob2 = container.GetBlockBlobReference(sblobname);

          string sencriptada = "";
          using (var memoryStream = new MemoryStream())
          {
            blockBlob2.DownloadToStream(memoryStream);
            sencriptada = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
          }

          sreturn64 = CryptoHelper.DecryptAES(sencriptada);
          //sreturn64 = sencriptada;
        }
        else
        {
          //Get dummy image
          using (Image image = Image.FromFile(System.Web.HttpContext.Current.Server.MapPath("~/Img/Product/Photo.png")))
          {
            using (MemoryStream m = new MemoryStream())
            {
              image.Save(m, image.RawFormat);
              byte[] imageBytes = m.ToArray();
              sreturn64 = Convert.ToBase64String(imageBytes);
            }
          }
        }
      }
      
      return sreturn64;
    }


    public static string GetWalletPassphrase(string sblobname)
    {
      string sreturn = "";

      if (sblobname != null)
      {
        if (sblobname.Length > 12)
        {
          CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Utils.Configuration.AppSettings.AzureStorageConnection);
          CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
          CloudBlobContainer container = blobClient.GetContainerReference("docskindads");
          CloudBlockBlob blockBlob2 = container.GetBlockBlobReference(sblobname);

          string sencrypted = "";
          using (var memoryStream = new MemoryStream())
          {
            blockBlob2.DownloadToStream(memoryStream);
            sencrypted = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
          }

          sreturn = CryptoHelper.DecryptAES(sencrypted);
          //sreturn64 = sencriptada;
        }
      }

      return sreturn;
    }

    public static string CreateUsrWalletBlobFile(string _passphrase)
    {
      //(returns the blobfilename to save it in user's entity

      CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Utils.Configuration.AppSettings.AzureStorageConnection);

      CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
      CloudBlobContainer container = blobClient.GetContainerReference("docskindads");
      container.CreateIfNotExists();

      string sblobfile = "bbcw" + Guid.NewGuid() + GetRandomNumber(100);
      CloudBlockBlob blockBlob = container.GetBlockBlobReference(sblobfile);

      _passphrase = CryptoHelper.EncryptAES(_passphrase);
      blockBlob.UploadText(_passphrase);
      return sblobfile;
    }

    public static string GetRandomNumber(int maxnumber)
    {
      Random rnd = new Random();
      return rnd.Next(1, maxnumber).ToString(); 
    }

    [Obsolete]
    public static string CreateHtmlTemplateAzure(string htmlText)
    {
      string filextension = ".html";
      CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Utils.Configuration.AppSettings.HtmlTemplateStorageConnection);

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

    public static byte[] GetHash(string inputString)
    {
      HashAlgorithm algorithm = MD5.Create();  //or use SHA256.Create();
      return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
    }

    public static string GetHashString(string inputString)
    {
      StringBuilder sb = new StringBuilder();
      foreach (byte b in GetHash(inputString))
        sb.Append(b.ToString("X2"));

      return sb.ToString();
    }
  }
}
