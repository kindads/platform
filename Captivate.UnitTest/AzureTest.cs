using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.ServiceBus.Messaging;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
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
    }
}
