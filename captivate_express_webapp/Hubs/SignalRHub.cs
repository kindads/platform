using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Captivate.Azure;
using Microsoft.ServiceBus.Messaging;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Captivate.Comun.Models;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Reactive;
using System.Collections.ObjectModel;
using System.Collections.Concurrent;

namespace NotificationSignalR
{

  public class PoolItem
  {
    public Captivate.Comun.Models.Notification not { set; get; }
    public long SequenceNumber { set; get; }
  }

  [HubName("notificationHub")]
  public class NotificationHub : Hub
  {
    public ServiceBusManager serviceBusManager { set; get; }
    public ConcurrentBag<PoolItem> notificationPool { set; get; }

    public Captivate.Comun.Models.Notification notification { set; get; }

    public int startCommunication { set; get; }

    public NotificationHub()
    {
      notificationPool = new ConcurrentBag<PoolItem>();
      startCommunication = DateTime.Now.Minute;
    }
   
    public void StartHandler(string IdUser)
    {
      serviceBusManager = new ServiceBusManager();

      try
      {
        TimeSpan RenewTimeout = TimeSpan.FromSeconds(1);
        OnMessageOptions options = new OnMessageOptions { AutoComplete = false, MaxConcurrentCalls = 1 , AutoRenewTimeout = RenewTimeout };

        
        serviceBusManager.GetClient().OnMessageAsync(async brokerMessage =>
        {
          try
          {            
            if (brokerMessage.SessionId==(IdUser))
            {
              var message = brokerMessage.GetBody<Stream>();
              serviceBusManager.Message = JsonConvert.DeserializeObject<Captivate.Comun.Models.Notification>(new StreamReader(message, true).ReadToEnd());
              notification = serviceBusManager.Message;
              await brokerMessage.CompleteAsync();
              serviceBusManager.Close();

              //Send to client
              SendNotification(notification.Title, notification.Message);
            }           
          }
          catch (Exception e)
          {
            brokerMessage.Abandon();
          }
        }, options);
      }
      catch(Exception e)
      {
        //Todo
      }
    }

    public void StartHandlerRx(string IdUser)
    {
      serviceBusManager = new ServiceBusManager();

      try
      {
        TimeSpan RenewTimeout = TimeSpan.FromSeconds(1);
        OnMessageOptions options = new OnMessageOptions { AutoComplete = false, MaxConcurrentCalls = 1, AutoRenewTimeout = RenewTimeout };

        // Create the Observable
        var collection=new ObservableCollection<string>();
        IObservable<BrokeredMessage> notifications = Observable.Create<BrokeredMessage>(
                observer =>
                {
                  serviceBusManager.GetClient().OnMessage(observer.OnNext, options);
                  return Disposable.Empty;
                }).Publish().RefCount();



        // Filtro para EMailNotification
        IDisposable emailNotification = notifications.Where(
              brokerMessage =>
             {
               return brokerMessage.SessionId == (IdUser) &&
               brokerMessage.Label == NotificationLabels.EMail;
             }).Subscribe(
             x => //OnNext
                {
                  try
                  {
                    if (x.SessionId == (IdUser))
                    {
                      var message = x.GetBody<Stream>();
                      serviceBusManager.Message = JsonConvert.DeserializeObject<Captivate.Comun.Models.Notification>(new StreamReader(message, true).ReadToEnd());
                      notification = serviceBusManager.Message;
                      

                      x.CompleteAsync();
                      serviceBusManager.Close();

                      //Send to client
                      SendNotification(notification.Title, notification.Message);
                    }
                  }
                  catch (Exception e)
                  {
                    x.Abandon();
                  }
                },
             x => Console.WriteLine(x.Message), //OnError
             () => Console.WriteLine("Complete")); //OnComplete

        // Filtro para CampaignValidation
        IDisposable campaignValidation = notifications.Where(
              brokerMessage =>
              {
                return brokerMessage.SessionId == (IdUser) &&
                brokerMessage.Label == NotificationLabels.CampaignValidation;
              }).Subscribe(
             x => //OnNext
             {
               try
               {
                 if (x.SessionId == (IdUser))
                 {
                   var message = x.GetBody<Stream>();
                   serviceBusManager.Message = JsonConvert.DeserializeObject<Captivate.Comun.Models.Notification>(new StreamReader(message, true).ReadToEnd());
                   notification = serviceBusManager.Message;


                   x.CompleteAsync();
                   serviceBusManager.Close();

                   //Send to client
                   SendNotification(notification.Title, notification.Message);
                 }
               }
               catch (Exception e)
               {
                 x.Abandon();
               }
             },
             x => Console.WriteLine(x.Message), //OnError
             () => Console.WriteLine("Complete")); //OnComplete


      }
      catch (Exception e)
      {
        //Todo
      }
    }

    public void SendNotification(string title,string message)
    {
      var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
      context.Clients.All.showNotification(title, message);
    }      
    

   
  }
}
