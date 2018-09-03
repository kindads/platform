using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;
using System.Configuration;

[assembly: OwinStartup(typeof(KindAds.Startup))]

namespace KindAds
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            string connectionString = ConfigurationManager.AppSettings["azure-servicebus-connectionstring"];
            string topicName = ConfigurationManager.AppSettings["azure-servicebus-topicName"];
            GlobalHost.DependencyResolver.UseServiceBus(connectionString, topicName);
            app.MapSignalR();

            ConfigureAuth(app);
        }
    }
}
