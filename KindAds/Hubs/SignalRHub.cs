using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using KindAds.Azure;
using Microsoft.ServiceBus.Messaging;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using KindAds.Common.Models;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Reactive;
using System.Collections.ObjectModel;
using System.Collections.Concurrent;
using KindAds.Comun.Models;
using KindAds.Common.Interfaces;
using System.Text;
using KindAds.Negocio.Managersv2;

namespace NotificationSignalR {

    public class PoolItem {
        public KindAds.Common.Models.Notification not { set; get; }
        public long SequenceNumber { set; get; }
    }

    [HubName("notificationHub")]
    public class NotificationHub : Hub {
        ITrace telemetry { set; get; }
        public ServiceBusManager serviceBusManager { set; get; }
        public ConcurrentBag<PoolItem> notificationPool { set; get; }

        public KindAds.Common.Models.Notification notification { set; get; }

        public ConversationMessageNotification conversationNotification { set; get; }

        public int startCommunication { set; get; }

        public NotificationHub()
        {
            notificationPool = new ConcurrentBag<PoolItem>();
            startCommunication = DateTime.Now.Minute;
            telemetry = new Trace();
        }

      
        public void StartHandler(string IdUser)
        {
            serviceBusManager = new ServiceBusManager();

            try {
                TimeSpan RenewTimeout = TimeSpan.FromSeconds(1);
                OnMessageOptions options = new OnMessageOptions { AutoComplete = false, MaxConcurrentCalls = 1, AutoRenewTimeout = RenewTimeout };


                serviceBusManager.GetClient().OnMessageAsync(async brokerMessage => {
                    try {
                        if (brokerMessage.SessionId == (IdUser)) {
                            var message = brokerMessage.GetBody<Stream>();
                            serviceBusManager.Message = JsonConvert.DeserializeObject<KindAds.Common.Models.Notification>(new StreamReader(message, true).ReadToEnd());
                            notification = serviceBusManager.Message;
                            await brokerMessage.CompleteAsync();
                            serviceBusManager.Close();

                            //Send to client
                            SendNotification(notification.Title, notification.Message);
                        }
                    }
                    catch (Exception e) {
                        brokerMessage.Abandon();
                    }
                }, options);
            }
            catch (Exception e) {
                var messageException = telemetry.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetry.Critical(messageException);
            }
        }

        public void StartHandlerRx(string IdUser)
        {
            serviceBusManager = new ServiceBusManager();

            try {
                TimeSpan RenewTimeout = TimeSpan.FromSeconds(1);
                OnMessageOptions options = new OnMessageOptions { AutoComplete = false, MaxConcurrentCalls = 1, AutoRenewTimeout = RenewTimeout };

                // Create the Observable
                var collection = new ObservableCollection<string>();
                IObservable<BrokeredMessage> notifications = Observable.Create<BrokeredMessage>(
                        observer => {
                            serviceBusManager.GetClient().OnMessage(observer.OnNext, options);
                            return Disposable.Empty;
                        }).Publish().RefCount();



                // Filtro para EMailNotification
                IDisposable emailNotification = notifications.Where(
                      brokerMessage => {
                          return brokerMessage.SessionId == (IdUser) &&
                  brokerMessage.Label == NotificationLabels.EMail;
                      }).Subscribe(
                     x => //OnNext
                        {
                            try {
                                if (x.SessionId == (IdUser)) {
                                    var message = x.GetBody<Stream>();
                                    serviceBusManager.Message = JsonConvert.DeserializeObject<KindAds.Common.Models.Notification>(new StreamReader(message, true).ReadToEnd());
                                    notification = serviceBusManager.Message;


                                    x.CompleteAsync();
                                    serviceBusManager.Close();

                                    //Send to client
                                    SendNotification(notification.Title, notification.Message);
                                }
                            }
                            catch (Exception e) {
                                x.Abandon();
                            }
                        },
                     x => Console.WriteLine(x.Message), //OnError
                     () => Console.WriteLine("Complete")); //OnComplete

                // Filtro para CampaignValidation
                IDisposable campaignValidation = notifications.Where(
                      brokerMessage => {
                          return brokerMessage.SessionId == (IdUser) &&
                  brokerMessage.Label == NotificationLabels.CampaignValidation;
                      }).Subscribe(
                     x => //OnNext
                     {
                         try {
                             if (x.SessionId == (IdUser)) {
                                 var message = x.GetBody<Stream>();
                                 serviceBusManager.Message = JsonConvert.DeserializeObject<KindAds.Common.Models.Notification>(new StreamReader(message, true).ReadToEnd());
                                 notification = serviceBusManager.Message;


                                 x.CompleteAsync();
                                 serviceBusManager.Close();

                                 //Send to client
                                 SendNotification(notification.Title, notification.Message);
                             }
                         }
                         catch (Exception e) {
                             x.Abandon();
                         }
                     },
                     x => Console.WriteLine(x.Message), //OnError
                     () => Console.WriteLine("Complete")); //OnComplete

            }
            catch (Exception e) {
                var messageException = telemetry.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetry.Critical(messageException);
            }
        }



        public void StartHandlerProposal(string IdUser)
        {
            serviceBusManager = new ServiceBusManager("proposaltopicdev");

            try {
                TimeSpan RenewTimeout = TimeSpan.FromSeconds(1);
                OnMessageOptions options = new OnMessageOptions { AutoComplete = false, MaxConcurrentCalls = 1, AutoRenewTimeout = RenewTimeout };

                // Create the Observable
                var collection = new ObservableCollection<string>();
                IObservable<BrokeredMessage> notifications = Observable.Create<BrokeredMessage>(
                        observer => {
                            serviceBusManager.GetClient().OnMessage(observer.OnNext, new OnMessageOptions());
                            return Disposable.Empty;
                        }).Publish().RefCount();

                // Filtro para CampaignValidation
                IDisposable porposalMessage = notifications.Where(
                      brokerMessage => {
                          return brokerMessage.SessionId == (IdUser) &&
                          brokerMessage.Label == NotificationLabels.Proposal;
                      }).Subscribe(
                     x => //OnNext
                     {
                         try {
                             if (x.SessionId == (IdUser)) {
                                 var message = x.GetBody<Stream>();
                                 serviceBusManager.Message = JsonConvert.DeserializeObject<ProposalNotification>(new StreamReader(message, true).ReadToEnd());
                                 var notification = serviceBusManager.Message;


                                 x.CompleteAsync();
                                 serviceBusManager.Close();

                                 //Send to client
                                 SendNotification(notification.message, notification.message);
                             }
                         }
                         catch (Exception e) {
                             x.Abandon();
                         }
                     },
                     x => Console.WriteLine(x.Message), //OnError
                     () => Console.WriteLine("Complete")); //OnComplete


            }
            catch (Exception e) {
                var messageException = telemetry.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetry.Critical(messageException);
            }
        }

        public void StartHandlerConversation(string IdUser)
        {
            string serviceBusTopicName = "conversationtopicdev"; //Obtenerlo del web.config
            serviceBusManager = new ServiceBusManager(serviceBusTopicName);

            try {
                TimeSpan RenewTimeout = TimeSpan.FromSeconds(1);
                OnMessageOptions options = new OnMessageOptions { AutoComplete = false, MaxConcurrentCalls = 1, AutoRenewTimeout = RenewTimeout };

                // Create the Observable
                var collection = new ObservableCollection<string>();
                IObservable<BrokeredMessage> notifications = Observable.Create<BrokeredMessage>(
                        observer => {
                            serviceBusManager.GetClient().OnMessage(observer.OnNext, options);
                            return Disposable.Empty;
                        }).Publish().RefCount();

                // Filtro para CampaignValidation
                IDisposable porposalMessage = notifications.Where(
                      brokerMessage => {                         
                          return (brokerMessage.Label == NotificationLabels.ConversationMessage);
                      }).Subscribe(
                     x => //OnNext
                     {
                         //split sessionIds
                         var authorizeUsers = x.SessionId.Split('|');
                         try {
                             if (authorizeUsers[0] == IdUser || authorizeUsers[1] == IdUser)
                             {
                                 var message = x.GetBody<Stream>();
                                 serviceBusManager.Message = JsonConvert.DeserializeObject<ConversationMessageNotification>(new StreamReader(message, true).ReadToEnd());
                                 conversationNotification = serviceBusManager.Message;


                                 x.CompleteAsync();
                                 serviceBusManager.Close();

                                 //Send to client
                                 SendChatMessage(conversationNotification);
                             }
                         }
                         catch (Exception e) {
                             x.Abandon();
                         }
                     },
                     x => Console.WriteLine(x.Message), //OnError
                     () => Console.WriteLine("Complete")); //OnComplete


            }
            catch (Exception e) {
                var messageException = telemetry.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetry.Critical(messageException);
            }
        }

        public void SendNotification(string title, string message)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            context.Clients.All.showNotification(title, message);
        }


        public void SendChatMessage(ConversationMessageNotification chatMessage)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();            
            string messageHtml = CreateHtmlMessageToAttachToChat(chatMessage);
            //string elementToAttach = "date_2018_08_25";//todo
            string elementToAttach = GetElementToAttach(chatMessage.currentConversation.MessageTime);
            context.Clients.All.showChatMessage(messageHtml, elementToAttach);
        }

        public string GetElementToAttach(DateTime time)
        {
            ConversationManager manager = new ConversationManager();
            string elementToAttach = manager.CalculateElementToAttach(time);
            return elementToAttach;
        }

        public string CreateHtmlMessageToAttachToChat(ConversationMessageNotification chatMessage)
        {
            string htmlToAttach = string.Empty;
            //todo

            StringBuilder sb = new StringBuilder();
            sb.Append($@"
                      <div class='chat'>
                          <span class='user'>{chatMessage.currentConversation.ProfileName}</span>
                          <span class='txt'>
                                {chatMessage.currentConversation.Message}
                          </span>
                          <span class='time'>{GetFormatedMessageTime(chatMessage.currentConversation.MessageTime.ToString())}</span>
                      </div>
                      ");           
            return sb.ToString();
        }

        public string GetFormatedMessageTime(string messageTime)
        {
            string formattedTime = string.Empty;            
            ConversationManager manager = new ConversationManager();
            formattedTime = manager.GetFormatedMessageTime(messageTime);
            return formattedTime;
        }
    }
}
