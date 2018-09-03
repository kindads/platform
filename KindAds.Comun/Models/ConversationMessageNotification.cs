using KindAds.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models
{
    public class ConversationMessageNotification
    {
        public CurrentConversation currentConversation { set; get; }

        public string label { set; get; }

        public List<string> users { set; get; }

        public ConversationMessageNotification()
        {
             currentConversation = new CurrentConversation();
             label = NotificationLabels.ConversationMessage;
             users = new List<string>();
        }

    }
}
