using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KindAds.Azure
{
    public class ServiceBusManager
    {
        public string ServiceBusConnectionString = string.Empty;
        public string topicName = string.Empty;
        public TopicClient topicClient;
        public string subscriptionname = string.Empty;
        public Microsoft.ServiceBus.Messaging.SubscriptionClient subscriptionClient;
        public string QueueName { set; get; }
        public QueueClient queueClient { set; get; }

        public SubscriptionClient subClient { set; get; }

        public dynamic Message = string.Empty;

        public int Priority { set; get; }

        public ServiceBusManager()
        {
            topicName = ConfigurationManager.AppSettings["azure-servicebus-topicName"];
            ServiceBusConnectionString = ConfigurationManager.AppSettings["azure-servicebus-connectionstring"];
            topicClient = TopicClient.CreateFromConnectionString(ServiceBusConnectionString, topicName);
            subscriptionname = ConfigurationManager.AppSettings["azure-servicebus-subscriptionname"];
            Priority = 1;

        }

        public ServiceBusManager(string topicName)
        {
            this.topicName = topicName;
            ServiceBusConnectionString = ConfigurationManager.AppSettings["azure-servicebus-connectionstring"];
            topicClient = TopicClient.CreateFromConnectionString(ServiceBusConnectionString, topicName);
            subscriptionname = ConfigurationManager.AppSettings["azure-servicebus-subscriptionname"]; 
            Priority = 1;

            subClient = SubscriptionClient.CreateFromConnectionString(ServiceBusConnectionString, topicName, subscriptionname);
        }

        public void SendMessageAsyncWithoutPriority( dynamic data, string Label)
        {
            try
            {
                var topicClient = TopicClient.CreateFromConnectionString(ServiceBusConnectionString, topicName);
                string userIds = string.Empty;
                foreach (var id in data.users)
                {
                    userIds = userIds + id + "|";
                }
                if (userIds.Length > 0)
                {
                    userIds = userIds.Remove(userIds.Length - 1);
                }

                //Creamos el mensaje
                var message = new BrokeredMessage(new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data))))
                {
                    ContentType = "application/json",
                    Label = Label,
                    MessageId = Priority.ToString(),
                    TimeToLive = TimeSpan.FromMinutes(5),
                    SessionId = userIds
                };

                topicClient.SendAsync(message).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                //Todo
            }
        }

        public void SendMessageAsync(int Priority,dynamic data, Guid id, string Label)
        {           
            try
            {
                var topicClient = TopicClient.CreateFromConnectionString(ServiceBusConnectionString, topicName);


                //Creamos el mensaje
                var message = new BrokeredMessage(new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data))))
                {
                    ContentType = "application/json",
                    Label = Label,
                    MessageId = Priority.ToString(),
                    TimeToLive = TimeSpan.FromMinutes(5),
                    SessionId = id.ToString(),
                    CorrelationId = id.ToString(),
                    Properties =
                    {
                        { "Priority", Priority }
                    }
                };

                topicClient.SendAsync(message).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                //Todo
            }
        }

        public async Task GetMessage(string Label)
        {
            var cts = new CancellationTokenSource();
            var allReceives = Task.WhenAll(this.GetMessageAsync(cts.Token, Label));
            cts.Cancel();
            await allReceives;
        }

        public async Task GetMessageAsync(CancellationToken cancellationToken, string Label)
        {

            var receiverFactory = MessagingFactory.CreateFromConnectionString(ServiceBusConnectionString);
            subscriptionClient = receiverFactory.CreateSubscriptionClient(topicName, subscriptionname);

            var doneReceiving = new TaskCompletionSource<bool>();
            cancellationToken.Register(
                async () =>
                {
                    await subscriptionClient.CloseAsync();
                    await receiverFactory.CloseAsync();
                    doneReceiving.SetResult(true);
                });

          

             subscriptionClient.OnMessageAsync(
              async message =>
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
              },
              new OnMessageOptions { AutoComplete = false, MaxConcurrentCalls = 1 });

            await doneReceiving.Task;
        }

        public async Task GetMessage(string Label, Func<BrokeredMessage, Task> callback, OnMessageOptions options)
        {
            var cts = new CancellationTokenSource();
            var allReceives = Task.WhenAll(this.GetMessageAsync(cts.Token, Label, callback, options));
            cts.Cancel();
            await allReceives;
        }

        public async Task GetMessageAsync(CancellationToken cancellationToken, string Label, Func<BrokeredMessage, Task> callback, OnMessageOptions options)
        {

            var receiverFactory = MessagingFactory.CreateFromConnectionString(ServiceBusConnectionString);
            subscriptionClient = receiverFactory.CreateSubscriptionClient(topicName, subscriptionname);
            var doneReceiving = new TaskCompletionSource<bool>();
            cancellationToken.Register(
                async () =>
                {
                    await subscriptionClient.CloseAsync();
                    await receiverFactory.CloseAsync();
                    doneReceiving.SetResult(true);
                });

            subscriptionClient.OnMessageAsync(callback, options);
            await doneReceiving.Task;
        }

        public SubscriptionClient GetClient()
        {
            SubscriptionClient subClient = SubscriptionClient.CreateFromConnectionString(ServiceBusConnectionString, topicName, subscriptionname);
            return subClient;
        }


        public void Close()
        {
            topicClient.Close();
        }

        #region version con Queue Session
        public QueueClient GetQueueClient()
        {
            var receiverFactory = MessagingFactory.CreateFromConnectionString(ServiceBusConnectionString);
            var queueClient = receiverFactory.CreateQueueClient(QueueName, ReceiveMode.PeekLock);
            return queueClient;
        }

        public async Task GetQueueMessageAsync(string Label, Func<BrokeredMessage, Task> callback, OnMessageOptions options)
        {
            var cts = new CancellationTokenSource();
            var allReceives = Task.WhenAll(this.GetQueueMessageSession(cts.Token, Label, callback, options));
            cts.Cancel();
            await allReceives;
        }

        public async Task GetQueueMessageSession(CancellationToken cancellationToken, string Label, Func<BrokeredMessage, Task> callback, OnMessageOptions options)
        {
            var receiverFactory = MessagingFactory.CreateFromConnectionString(ServiceBusConnectionString);
            var client = receiverFactory.CreateQueueClient(QueueName, ReceiveMode.PeekLock);
            var messageSession = client.AcceptMessageSession();
            client.RegisterSessionHandler(
             typeof(SessionHandler),
             new SessionHandlerOptions
             {
                MessageWaitTimeout = TimeSpan.FromSeconds(5),
                MaxConcurrentSessions = 1,
                AutoComplete = false
             });
            messageSession.OnMessageAsync(callback, options);           
            var doneReceiving = new TaskCompletionSource<bool>();
            //client.OnMessageAsync(callback, options);
            await doneReceiving.Task;
        }


        public async Task SendMessageToSessionQueue(dynamic data, Guid id, string Label)
        {
            try
            {
                //Creamos el mensaje
                var message = new BrokeredMessage(new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data))))
                {
                    ContentType = "application/json",
                    Label = Label,
                    MessageId = id.ToString(),
                    TimeToLive = TimeSpan.FromMinutes(20),
                    SessionId = id.ToString(),
                    CorrelationId = id.ToString()                    
                };

               

                await queueClient.SendAsync(message);
            }
            catch (Exception e)
            {
                //Todo
            }
        }
        #endregion

    }

    public class SessionHandler : IMessageSessionAsyncHandler
    {
        public Task OnCloseSessionAsync(MessageSession session)
        {
            throw new NotImplementedException();
        }

        public async Task OnMessageAsync(MessageSession session, BrokeredMessage message)
        {
            if (message.Label != null &&
              message.ContentType != null &&
              message.Label.Equals("RecipeStep", StringComparison.InvariantCultureIgnoreCase) &&
              message.ContentType.Equals("application/json", StringComparison.InvariantCultureIgnoreCase))
            {
                var body = message.GetBody<Stream>();

                dynamic recipeStep = JsonConvert.DeserializeObject(new StreamReader(body, true).ReadToEnd());
                lock (Console.Out)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(
                        "\t\t\t\tMessage received:  \n\t\t\t\t\t\tSessionId = {0}, \n\t\t\t\t\t\tMessageId = {1}, \n\t\t\t\t\t\tSequenceNumber = {2}," +
                        "\n\t\t\t\t\t\tContent: [ step = {3}, title = {4} ]",
                        message.SessionId,
                        message.MessageId,
                        message.SequenceNumber,
                        recipeStep.step,
                        recipeStep.title);
                    Console.ResetColor();
                }
                await message.CompleteAsync();

                if (recipeStep.step == 5)
                {
                    // end of the session!
                    await session.CloseAsync();
                }
            }
            else
            {
                await message.DeadLetterAsync("BadMessage", "Unexpected message");
            }
        }

        public Task OnSessionLostAsync(Exception exception)
        {
            throw new NotImplementedException();
        }
    }
}
