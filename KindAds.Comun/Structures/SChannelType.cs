using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Structures
{
    public struct SChannelType
    {
        public static (string channelTypeId, string channelTypeName) PushNotification = ("69349c7c-48b2-4628-9f3f-22846b1bc6de", "Push Notification");
        public static (string channelTypeId, string channelTypeName) WebsiteAdSpace = ("88c34fff-a1ab-401e-8908-5a4929abf36a", "Website Ad Space");
        public static (string channelTypeId, string channelTypeName) Email = ("b4ee7512-5f12-40f1-8408-8f96bf43df6d", "Email");
    }
}
