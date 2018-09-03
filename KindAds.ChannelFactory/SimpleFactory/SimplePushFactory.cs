using KindAds.Comun.Enums;
using KindAds.Comun.Interfaces;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using KindAds.Negocio.Partnersv2.Push;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.ChannelFactory.SimpleFactory
{
    public static class SimplePushFactory
    {
        public static IPushChannel CreateChannel(ChannelType type)
        {
            switch (type)
            {
                case ChannelType.OneSignal:
                    {
                        return new OneSignalManagerv2();
                    }
                case ChannelType.PushEngage:
                    {
                        return new PushEngageManagerv2();
                    }
                case ChannelType.PushCrew:
                    {
                        return new PushCrewManagerv2();
                    }
                case ChannelType.Subscribers:
                    {
                        return new SubscribersManagerv2();
                    }
                default:
                    return null;
            }
        }

        public static IPushChannel CreateChannel(ChannelType type, IList<AudiencePropertieSetting> settings, string emailProductId)
        {
            switch (type)
            {
                case ChannelType.OneSignal:
                    {
                        return new OneSignalManagerv2(settings, emailProductId);
                    }
                case ChannelType.PushEngage:
                    {
                        return new PushEngageManagerv2(settings, emailProductId);
                    }
                case ChannelType.PushCrew:
                    {
                        return new PushCrewManagerv2(settings, emailProductId);
                    }
                case ChannelType.Subscribers:
                    {
                        return new SubscribersManagerv2(settings, emailProductId);
                    }
                default:
                    return null;
            }
        }

        public static IPushChannel CreateChannel(ChannelType type, string emailProductId)
        {
            switch (type)
            {
                case ChannelType.OneSignal:
                    {
                        return new OneSignalManagerv2( emailProductId);
                    }
                case ChannelType.PushEngage:
                    {
                        return new PushEngageManagerv2( emailProductId);
                    }
                case ChannelType.PushCrew:
                    {
                        return new PushCrewManagerv2( emailProductId);
                    }
                case ChannelType.Subscribers:
                    {
                        return new SubscribersManagerv2( emailProductId);
                    }
                default:
                    return null;
            }
        }
    }
}
