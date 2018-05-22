using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;
using System.Configuration;

[assembly: OwinStartup(typeof(captivate_express_webapp.Startup))]

namespace captivate_express_webapp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ServiceBusDefaultConnection"].ConnectionString;
            string TopicName = ConfigurationManager.AppSettings["TopicName"];
            GlobalHost.DependencyResolver.UseServiceBus(connectionString, TopicName);
            app.MapSignalR();

            ConfigureAuth(app);
        }
    }
}
