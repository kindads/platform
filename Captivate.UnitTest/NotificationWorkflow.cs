using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Captivate.Negocio;
using Captivate.Comun.Models;
using Newtonsoft.Json;
using Captivate.Azure;
using System.IO;
using Microsoft.ServiceBus.Messaging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Captivate.UnitTest
{
    [TestClass]
    public class NotificationWorkflow
    {
        Dictionary<string, int> UsersPriority = new Dictionary<string, int>();

        [TestMethod]
        public void InsertFirstNotificationToServiceBusTes()
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
            notificationManager.ProcessMailNotification(message, UsersPriority);
            notificationManager.Close();

            Assert.AreEqual(true, true);
        }

        [TestMethod]
        public async Task GetFirstNotificationToServiceBusTes()
        {
            string Label = NotificationLabels.EMail;
            dynamic Message = string.Empty;
            ServiceBusManager serviceBusManager = new ServiceBusManager();
            Notification notification = new Notification();
            TimeSpan RenewTimeout = TimeSpan.FromSeconds(1);
            OnMessageOptions options = new OnMessageOptions { AutoComplete = false, MaxConcurrentCalls = 1, AutoRenewTimeout = RenewTimeout };

            var client = serviceBusManager.GetClient();

            var message = await client.ReceiveAsync(TimeSpan.Zero);
            if (message != null)
            {
                if (message.Label != null &&
                         message.ContentType != null &&
                         message.Label.Equals(Label, StringComparison.InvariantCultureIgnoreCase) &&
                         message.ContentType.Equals("application/json", StringComparison.InvariantCultureIgnoreCase))
                {
                    var body = message.GetBody<Stream>();
                    Message = JsonConvert.DeserializeObject(new StreamReader(body, true).ReadToEnd()).ToString();
                    message.Complete();
                }
                else
                {
                    await message.DeadLetterAsync("ProcessingError", "Don't know what to do with this message");
                }
            }

            //serviceBusManager.GetClient().OnMessageAsync(async brokerMessage =>
            //{
            //    try
            //    {
            //        var message = brokerMessage.GetBody<Stream>();                    
            //        serviceBusManager.Message = JsonConvert.DeserializeObject<Notification>(new StreamReader(message, true).ReadToEnd());
            //        notification = serviceBusManager.Message;
            //        await brokerMessage.CompleteAsync();
            //        serviceBusManager.Close();
            //    }
            //    catch (Exception e)
            //    {
            //        brokerMessage.Abandon();
            //    }
            //}, options);

            Assert.AreEqual(true, notification.Message.Contains("Mensaje 1"));
        }

        [TestMethod]
        public void InsertSecondNotificationToServiceBusTes()
        {
            //Preparar el mensaje
            MailNotification mailNotification = new MailNotification();
            MailMessage mailMessage = new MailMessage();
            Notification notification = new Notification();
            string text = "Mensaje 2";

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
            notificationManager.ProcessMailNotification(message, UsersPriority);
            notificationManager.Close();

            Assert.AreEqual(true, true);
        }

        [TestMethod]
        public void GetSecondNotificationToServiceBusTes()
        {
            ServiceBusManager serviceBusManager = new ServiceBusManager();
            Notification notification = new Notification();
            TimeSpan RenewTimeout = TimeSpan.FromSeconds(1);
            OnMessageOptions options = new OnMessageOptions { AutoComplete = false, MaxConcurrentCalls = 1, AutoRenewTimeout = RenewTimeout };

            serviceBusManager.GetClient().OnMessageAsync(async brokerMessage =>
            {
                try
                {
                    var message = brokerMessage.GetBody<Stream>();
                    serviceBusManager.Message = JsonConvert.DeserializeObject<Notification>(new StreamReader(message, true).ReadToEnd());
                    notification = serviceBusManager.Message;
                    await brokerMessage.CompleteAsync();
                    serviceBusManager.Close();
                }
                catch (Exception e)
                {
                    brokerMessage.Abandon();
                }
            }, options);

            Assert.AreEqual(true, notification.Message.Contains("Mensaje 2"));
        }
    }
}
