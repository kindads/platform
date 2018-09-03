using KindAds.Azure;
using KindAds.Common.Models;
using KindAds.Common.Models.Entities;
using KindAds.DataAccess;
using KindAds.Business.Email;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KindAds.Comun.Models;
using KindAds.Common.Interfaces;

namespace KindAds.Business
{
    public  class NotificationManager
    {
        ITrace telemetry { set; get; }
        public ServiceBusManager sbmanager { set; get; }
        public MailManager mailManager { set; get; }
        //private readonly AspNetUserRepository aspNetUserRepository;

        public NotificationManager()
        {
            sbmanager = new ServiceBusManager();
            mailManager = new MailManager();
            telemetry = new Trace();
        }

        public NotificationManager(string serviceBusQueueName)
        {
            sbmanager = new ServiceBusManager(serviceBusQueueName);
            mailManager = new MailManager();
        }

        public bool EnqueueNewAccessUser(Notification accessData)
        {
            //azure-queue-campaign
            bool result = false;
            string notification = JsonConvert.SerializeObject(accessData);
            string queueName = ConfigurationManager.AppSettings["azure-queue-accessdata"];
            QueueManager.InsertMessage(notification, queueName);
            result = true;
            return result;
        }

       

        public void EnqueueMailNotification(string CampaignName, string message, string IdUser)
        {
            
            //AspNetUserEntity userData = aspNetUserRepository.FindById(IdUser);
            ////Enviamos la notificacion

            //MailNotification mailNotification = new MailNotification();
            //// Fill object
            //MailMessage email = new MailMessage();
            //Notification notification = new Notification();

            //email.Body = message;
            //email.Destination = userData.Email;
            //email.Subject = message;

            //notification.Label = NotificationLabels.EMail;
            //notification.IdUser = new Guid(IdUser);
            //notification.Message = message;
            //notification.Title = string.Format("Campaign notification {0}", DateTime.Now);

            ////Add objects to MailNotification
            //mailNotification.EMail = email;
            //mailNotification.notificacion = notification;


            //NotificationManager notificationManager = new NotificationManager();
            //notificationManager.EnqueueMailNotification(mailNotification);
        }

        public bool  EnqueueChatNotification(ChatNotification chatNotification)
        {
            bool result = false;
            string notification = JsonConvert.SerializeObject(chatNotification);
            string queueName = ConfigurationManager.AppSettings["azure-queue-chatnotification"];
            QueueManager.InsertMessage(notification, queueName);
            result = true;
            return result;
        }

        public  bool EnqueueMailNotification(MailNotification mailNotification)
        {
            bool result = false;
            string notification = JsonConvert.SerializeObject(mailNotification);
            string queueName = ConfigurationManager.AppSettings["azure-queue-mailnotification"];
            QueueManager.InsertMessage(notification, queueName);
            result = true;
            return result;
        }

        public bool EnqueueCampaignValidator(Notification campaignNotification)
        {
            //azure-queue-campaign
            bool result = false;
            string notification = JsonConvert.SerializeObject(campaignNotification);
            string queueName = ConfigurationManager.AppSettings["azure-queue-campaign"];
            QueueManager.InsertMessage(notification, queueName);
            result = true;
            return result;
        }

        public  bool EnqueueNotification(Notification notification)
        {
            bool result = false;
            string not = JsonConvert.SerializeObject(notification);
            string queueName = ConfigurationManager.AppSettings["azure-queue-notification"];
            QueueManager.InsertMessage(not, queueName);
            result = true;
            return result;
        }


        public Notification GetNotification(string rawMessage)
        {
            Notification notification = JsonConvert.DeserializeObject<Notification>(rawMessage);
            return notification;
        }

        public void ProcessChatNotification(string message, Dictionary<string, int> userPriority)
        {
            try
            {
                // Obtenemos la notificacion de la queue: notifications
                ChatNotification chatNotification = JsonConvert.DeserializeObject<ChatNotification>(message);
               

                //Aumentamos la prioridad del usuario
                int Priority = AddUserIfNotExist(userPriority, new Guid(chatNotification.idUser));

                // Encolamos la notificacion al topic de service bus
                // para que tambien llegue en tiempo real               
                dynamic data = chatNotification;
                sbmanager.SendMessageAsync(Priority, data, new Guid(chatNotification.idUser), chatNotification.label);
            }
            catch (Exception e)
            {
                var messageException = telemetry.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetry.Critical(messageException);
            }
        }

        public void ProcessMailNotification(string message, Dictionary<string,int> UserPriority)
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
                var messageException = telemetry.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetry.Critical(messageException);
            }           
        }


        public void ProcessConversationNotification(ConversationMessageNotification message)
        {
            try
            {
                dynamic data = message;
                sbmanager.SendMessageAsyncWithoutPriority(data, message.label);
            }
            catch (Exception e)
            {
                var messageException = telemetry.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetry.Critical(messageException);
            }
        }

        public void ProcessProposalNotification(string message, Dictionary<string, int> userPriority)
        {
            try
            {
                // Obtenemos la notificacion de la queue: notifications
                ProposalNotification proposalNotification = JsonConvert.DeserializeObject<ProposalNotification>(message);


                //Aumentamos la prioridad del usuario
                int Priority = AddUserIfNotExist(userPriority, new Guid(proposalNotification.idUser));

                // Encolamos la notificacion al topic de service bus
                // para que tambien llegue en tiempo real               
                dynamic data = proposalNotification;
                sbmanager.SendMessageAsync(Priority, data, new Guid(proposalNotification.idUser), proposalNotification.label);
            }
            catch (Exception e)
            {
                var messageException = telemetry.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetry.Critical(messageException);
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
                var messageException = telemetry.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetry.Critical(messageException);
            }
        }

        public  void ProcessNotification(string message, Dictionary<string, int> UserPriority)
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
                var messageException = telemetry.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetry.Critical(messageException);
            }          
        }

        public void Close()
        {
            sbmanager.Close();
        }

    }
}
