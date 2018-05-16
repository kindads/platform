using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Captivate.Comun.Models;
using Captivate.Negocio;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Captivate.UnitTest
{
    [TestClass]
    public class PopulateServiceBus
    {

        Dictionary<string, int> UsersPriority = new Dictionary<string, int>();

        [TestMethod]
        public void PopulateServiceBusWithNotifications()
        {
            int numberOfMessages = 100;
            Guid IdUser = Guid.NewGuid();

            for(int i=1;i<=numberOfMessages;i++)
            {
                string text = string.Format("Message {0}", i.ToString());
                InsertFirstNotificationToServiceBusTes(text, IdUser,i);
            }

            Assert.AreEqual(true, true);
        }
       
        public void InsertFirstNotificationToServiceBusTes(string text,Guid IdUser, int Priority)
        {

            //Preparar el mensaje
            MailNotification mailNotification = new MailNotification();
            MailMessage mailMessage = new MailMessage();
            Notification notification = new Notification();
          

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

    }
}
