using Captivate.Azure;
using Captivate.Comun.Models;
using Captivate.Comun.Models.Entities;
using Captivate.DataAccess;
using Captivate.Negocio.Email;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Negocio
{
    public  class NotificationManager
    {
        public ServiceBusManager sbmanager { set; get; }
        public MailManager mailManager { set; get; }
        public KindadsContext context { set; get; }

        public NotificationManager()
        {
            sbmanager = new ServiceBusManager();
            mailManager = new MailManager();
            context = new KindadsContext();
        }

        public void EnqueueMailNotification(string CampaignName, string message, string IdUser)
        {
            AspNetUserRepository aspNetUserRepository = new AspNetUserRepository { Context = context };
            AspNetUserEntity userData = aspNetUserRepository.FindBy(u => u.Id == IdUser).FirstOrDefault();
            //Enviamos la notificacion

            MailNotification mailNotification = new MailNotification();
            // Fill object
            MailMessage email = new MailMessage();
            Notification notification = new Notification();

            email.Body = message;
            email.Destination = userData.Email;
            email.Subject = message;

            notification.Label = NotificationLabels.EMail;
            notification.IdUser = new Guid(IdUser);
            notification.Message = message;
            notification.Title = string.Format("Campaign notification {0}", DateTime.Now);

            //Add objects to MailNotification
            mailNotification.EMail = email;
            mailNotification.notificacion = notification;


            NotificationManager notificationManager = new NotificationManager();
            notificationManager.EnqueueMailNotification(mailNotification);
        }

        public  bool EnqueueMailNotification(MailNotification mailNotification)
        {
            bool result = false;
            string notification = JsonConvert.SerializeObject(mailNotification);
            string queueName = ConfigurationManager.AppSettings["mailNotificationQueue"];
            QueueManager.InsertMessage(notification, queueName);
            result = true;
            return result;
        }

        public bool EnqueueCampaignValidator(Notification campaignNotification)
        {
            //campaignqueue
            bool result = false;
            string notification = JsonConvert.SerializeObject(campaignNotification);
            string queueName = ConfigurationManager.AppSettings["campaignQueue"];
            QueueManager.InsertMessage(notification, queueName);
            result = true;
            return result;
        }

        public  bool EnqueueNotification(Notification notification)
        {
            bool result = false;
            string not = JsonConvert.SerializeObject(notification);
            string queueName = ConfigurationManager.AppSettings["notificationQueue"];
            QueueManager.InsertMessage(not, queueName);
            result = true;
            return result;
        }


        public Notification GetNotification(string rawMessage)
        {
            Notification notification = JsonConvert.DeserializeObject<Notification>(rawMessage);
            return notification;
        }

        public  void ProcessMailNotification(string message, Dictionary<string,int> UserPriority)
        {
            try
            {
                // Obtenemos la notificacion de la queue: notifications
                MailNotification pushNotification = JsonConvert.DeserializeObject<MailNotification>(message);
                mailManager.SendAsync(pushNotification.EMail);

                //Aumentamos la prioridad del usuario
                int Priority=AddUserIfNotExist( UserPriority,pushNotification.notificacion.IdUser);

                // Encolamos la notificacion al topic de service bus
                // para que tambien llegue en tiempo real               
                dynamic data = pushNotification.notificacion;
                sbmanager.SendMessageAsync(Priority,data, pushNotification.notificacion.IdUser, pushNotification.notificacion.Label);
            }
            catch (Exception e)
            {
            }           
        }

        public int AddUserIfNotExist( Dictionary<string,int> UserPriority,Guid IdUser)
        {
            int Priority=-1;
            UserPriority.TryGetValue(IdUser.ToString(), out Priority);

            if (Priority == 0)
            {               
                Priority = 1;
                UserPriority.Add(IdUser.ToString(), Priority);                
            }
            else
            {
                Priority++;
                if (Priority==1000)
                {
                    UserPriority[IdUser.ToString()] = Priority;
                    Priority = 1;
                }
                else
                {
                    UserPriority[IdUser.ToString()] = Priority;
                }                
            }
            return Priority;
        }

        public void ProcessMailSessionNotification(string message)
        {
            try
            {
                // Obtenemos la notificacion de la queue: notifications
                MailNotification pushNotification = JsonConvert.DeserializeObject<MailNotification>(message);
                mailManager.SendAsync(pushNotification.EMail);

                // Encolamos la notificacion al topic de service bus
                // para que tambien llegue en tiempo real               
                dynamic data = pushNotification.notificacion;
                sbmanager.SendMessageToSessionQueue(data, pushNotification.notificacion.IdUser, pushNotification.notificacion.Label).GetAwaiter().GetResult(); ;
            }
            catch (Exception e)
            {
            }
        }

        public  void  ProcessNotification(string message, Dictionary<string, int> UserPriority)
        {
            try
            {
                // Obtenemos la notificacion de la queue: notifications
                Notification pushNotification = JsonConvert.DeserializeObject<Notification>(message);

                //Aumentamos la prioridad del usuario
                int Priority = AddUserIfNotExist(UserPriority, pushNotification.IdUser);

                // Encolamos la notificacion al topic de service bus
                // para que tambien llegue en tiempo real
                dynamic data = pushNotification;
                sbmanager.SendMessageAsync(Priority, data, pushNotification.IdUser, pushNotification.Label);
            }
            catch (Exception e)
            {
            }          
        }

        public void Close()
        {
            sbmanager.Close();
        }

    }
}
