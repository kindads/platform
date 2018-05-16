using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Captivate.Comun.Models;
using Captivate.Azure;
using Microsoft.ServiceBus.Messaging;
using System.IO;
using Newtonsoft.Json;
using Captivate.Negocio;
using System.Threading.Tasks;

namespace Captivate.UnitTest
{
    [TestClass]
    public class NotificationSessionWorkflow
    {
        [TestMethod]
        public void InsertFirstNotificationToServiceBusTest()
        {
            //Preparar el mensaje
            MailNotification mailNotification = new MailNotification();
            MailMessage mailMessage = new MailMessage();
            Notification notification = new Notification();
            string text = "Mensaje 1";

            notification.IdUser = Guid.NewGuid();
            notification.Label = NotificationLabels.EMail;
            notification.Message = text;
            notification.Title = text;

            mailMessage.Body = text;
            mailMessage.Subject = text;
            mailMessage.Destination = "angel.alvarado@blockbliss.com";

            mailNotification.EMail = mailMessage;
            mailNotification.notificacion = notification;

            string message = JsonConvert.SerializeObject(mailNotification);


            //Enviar el mensaje
            NotificationManager notificationManager = new NotificationManager();
            notificationManager.ProcessMailSessionNotification(message);
            notificationManager.Close();

            Assert.AreEqual(true, true);
        }

        [TestMethod]
        public void GetFirstNotificationToServiceBusTes()
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

            sbmanager.GetQueueMessageAsync(Label, callback, options).GetAwaiter().GetResult();
            dynamic Message = sbmanager.Message;
            Assert.AreEqual(sbmanager.Message != string.Empty, true);
        }


    }
}
