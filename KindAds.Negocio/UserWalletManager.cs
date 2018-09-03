using KindAds.Common.Models;
using KindAds.Comun.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Negocio
{
    public class UserWalletManager
    {
        public Notification GetNotification(string rawMessage)
        {
            Notification notification = JsonConvert.DeserializeObject<Notification>(rawMessage);
            return notification;
        }
    }
}
