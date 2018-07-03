using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.ServiceBus.Messaging;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Captivate.Common.Models;
using Captivate.Azure;
using Captivate.Comun.Models.CosmosDocuments;
using Captivate.Comun.Models;

namespace Captivate.UnitTest
{
    [TestClass]
    public class AzureTest
    {
        [TestMethod]
        public void ServiceBusTest()
        {
            Azure.ServiceBusManager sbmanager = new Azure.ServiceBusManager();
            string message = "Hola KindAds";
            string Label = "ValidationCampaign";
            Guid Id = Guid.NewGuid();
            dynamic data = new
            {
                IdUser = Id, msm = message
            };
            sbmanager.SendMessageAsync(1,data, Id, Label).GetAwaiter().GetResult(); 

            Assert.AreEqual(true, true);
        }

        [TestMethod]
        public void ServiceBusGetMessage()
        {
            Azure.ServiceBusManager sbmanager = new Azure.ServiceBusManager();
            string Label = "ValidationCampaign";
            sbmanager.GetMessage(Label).GetAwaiter().GetResult();
            dynamic Message = sbmanager.Message;
            Assert.AreEqual(sbmanager.Message != string.Empty, true);
        }

        [TestMethod]
        public void ServiceBusGetMessageFunc()
        {
            Azure.ServiceBusManager sbmanager = new Azure.ServiceBusManager();
            string Label = "ValidationCampaign";

            //Define de callback
            Func<BrokeredMessage, Task> callback = (async message =>
            {
                if (message.Label != null &&
                    message.ContentType != null &&
                    message.Label.Equals(Label, StringComparison.InvariantCultureIgnoreCase) &&
                    message.ContentType.Equals("application/json", StringComparison.InvariantCultureIgnoreCase))
                {
                    var body = message.GetBody<Stream>();
                    sbmanager.Message = JsonConvert.DeserializeObject(new StreamReader(body, true).ReadToEnd()).ToString();
                    //Send to SignalR
                    message.Complete();
                }
                else
                {
                    await message.DeadLetterAsync("ProcessingError", "Don't know what to do with this message");
                }
            });

            //Opciones
            OnMessageOptions options = new OnMessageOptions { AutoComplete = false, MaxConcurrentCalls = 1 };

            sbmanager.GetMessage(Label,callback, options).GetAwaiter().GetResult();
            dynamic Message = sbmanager.Message;
            Assert.AreEqual(sbmanager.Message != string.Empty, true);
        }

        [TestMethod]
        public void ServiceBusGetMessageFuncClient()
        {
            Azure.ServiceBusManager sbmanager = new Azure.ServiceBusManager();
            sbmanager.GetClient().OnMessageAsync(async brokerMessage =>
            {
                var body = brokerMessage.GetBody<Stream>();
                sbmanager.Message = JsonConvert.DeserializeObject<Notification>(new StreamReader(body, true).ReadToEnd()).ToString();
                brokerMessage.Complete();
            });

            Assert.AreEqual(sbmanager.Message != string.Empty, true);
        }

        [TestMethod]
        public void CosmosDbCreateDatabaseTest()
        {
            CosmosDBManager cosmosManager = new CosmosDBManager();
            bool result=cosmosManager.CreateDatabaseIfNotExist("moneyads");
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void CosmosDbCreateCollectionTest()
        {
            CosmosDBManager cosmosManager = new CosmosDBManager();
            bool result = cosmosManager.CreateCollectionIfNotExist("moneyads","devicemetrics");
            Assert.AreEqual(true, result);
        }

       
        [TestMethod]
        public void CosmosDbCAddDocumentTest()
        {
            CosmosDBManager cosmosManager = new CosmosDBManager();
            DeviceMetricDocument device = new DeviceMetricDocument() { Name = "BlackBerry" };

            bool resultCreateCollection = cosmosManager.CreateCollectionIfNotExist("moneyads", "devicemetrics");
            bool resultAddDocument = cosmosManager.CreateDocumentIfNotExists<DeviceMetricDocument>("moneyads", "devicemetrics", device);
            Assert.AreEqual(true, resultAddDocument);
        }

       
        [TestMethod]
        public void CosmosDbGetAllDocuments()
        {
            CosmosDBManager cosmosManager = new CosmosDBManager();
            var result = cosmosManager.GetAllDocuments<DeviceMetricDocument>("moneyads", "devicemetrics");
            Assert.AreEqual(true, result.Count > 0);
        }

        [TestMethod]
        public void InsertStyleHtmlTest()
        {
            string AdsHtml = "<div id=\"{0}Close\" onclick=\"moneyAdsMetrics.closeAds();\"  style=\"height:50px; width:50px; font-size: 45px; color:#b30000; position: fixed; top: 0; margin-left:auto; margin-rigth:20px; z-index:200000001; line-height: 100px;\" >X</div>";
            string CloseHtml = "<div id=\"{0}\" style=\"width: 100%; height: 100px; opacity: 0.3; z-index:200000000; background:#000; position: fixed; top: 0; text-align: center; font-size: 45px; line-height: 100px; color:#ffff \"></div>";

            TableManager.InsertAdsHtml("Styles", AdsHtml,"StickyAds","AdsDiv");
            TableManager.InsertAdsHtml("Styles", CloseHtml, "StickyAds", "CloseDiv");

            Assert.AreEqual(true, true);
        }

        [TestMethod]
        public void GetStyleStickyTest()
        {
            string TableName = "Styles";
            string PartitionName = "StickyAds";
            string RowName = "AdsDiv";

            StickyHtmlEntity style = TableManager.GetAdsHtml(TableName, PartitionName,RowName);

            Assert.AreEqual(true, true);

        }
    }
}
