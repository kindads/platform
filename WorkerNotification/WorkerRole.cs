using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Captivate.Negocio;
using Captivate.Azure;

namespace WorkerNotification
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        public NotificationManager notificationManager { set; get; }
        public Dictionary<string,int> UsersPriority { set; get; }

        public override void Run()
        {
            Trace.TraceInformation("WorkerNotification is running");
            string MailQueueName = "mail-notifications";
            string NotificationQueueName = "notifications";
            UsersPriority = new Dictionary<string, int>();

            try
            {
                while(true)
                {
                    string mailMessage = QueueManager.GetMessage(MailQueueName);
                    string notificationMessage = QueueManager.GetMessage(NotificationQueueName);

                    if (mailMessage != string.Empty)
                    {
                        notificationManager = new NotificationManager();
                        notificationManager.ProcessMailNotification(mailMessage, UsersPriority);
                        notificationManager.Close();

                        TimeSpan delay = TimeSpan.FromSeconds(1);
                        Task.Delay(delay);
                    }

                    if (notificationMessage != string.Empty)
                    {
                        notificationManager = new NotificationManager();
                        notificationManager.ProcessNotification(notificationMessage, UsersPriority);
                        notificationManager.Close();

                        TimeSpan delay = TimeSpan.FromSeconds(1);
                        Task.Delay(delay);
                    }
                    //this.RunAsync(this.cancellationTokenSource.Token).Wait();
                }
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("WorkerNotification has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("WorkerNotification is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("WorkerNotification has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");
                await Task.Delay(1000);
            }
        }
    }
}
