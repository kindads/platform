using KindAds.ChannelFactory.SimpleFactory;
using KindAds.Comun.Enums;
using KindAds.Comun.Interfaces;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.ChannelFactory.AbstractFactory
{
    public static class ChannelAbstractFactory<T>
    {

        public static T CreateChannel(ChannelType type)
        {
            var simpleFactoryType = typeof(T);

            switch (simpleFactoryType.Name)
            {
                case "IMailChannel":
                    {
                        return (T)SimpleEmailFactory.CreateChannel(type);
                    }
                case "IPushChannel":
                    {
                        return (T)SimplePushFactory.CreateChannel(type);
                    }
                default:
                    {
                        return default(T);
                    }
            }
        }

        public static T CreateChannel(ChannelType type, IList<AudiencePropertieSetting> settings, string emailProductId)
        {
            var simpleFactoryType = typeof(T);

            switch (simpleFactoryType.Name)
            {
                case "IMailChannel":
                    {
                        return (T)SimpleEmailFactory.CreateChannel(type, settings, emailProductId);
                    }
                case "IPushChannel":
                    {
                        return (T)SimplePushFactory.CreateChannel(type, settings, emailProductId);
                    }
                default:
                    {
                        return default(T);
                    }
            }
        }

        public static T GetChannel(ChannelType type, string emailProductId)
        {
            var simpleFactoryType = typeof(T);

            switch (simpleFactoryType.Name)
            {
                case "IMailChannel":
                    {
                        return (T)SimpleEmailFactory.CreateChannel(type, emailProductId);
                    }
                case "IPushChannel":
                    {
                        return (T)SimplePushFactory.CreateChannel(type, emailProductId);
                    }
                default:
                    {
                        return default(T);
                    }
            }
        }
    }
}
