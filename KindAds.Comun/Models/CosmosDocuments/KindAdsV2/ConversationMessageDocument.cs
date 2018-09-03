using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models.CosmosDocuments.KindAdsV2
{
    public class ConversationMessageDocument : KindAdsV2Document
    {
        public string ConversationId { set; get; }

        public string Message { set; get; }

        public string MessageWithoutHtml { set; get; }

        public bool Visibility { set; get; }

        public bool AttachedFile { set; get; }

        public string AttachedFileUrl { set; get; }

        public string MessageTime { set; get; }

        public string YearOfMessage { set; get; }

        public string MonthOfMessage { set; get; }

        public string DayOfMessage { set; get; }

        public string HourOfMessage { set; get; }

        public string MinuteOfMessage { set; get; }

        public string ElementToAttach { set; get; }

        public string SignedBy { set; get; } // profile name
        public string MessageHour { set; get; }


        public ConversationMessageDocument()
        {
            ConversationId = string.Format("<NULL>");
            Message = string.Format("");
            MessageWithoutHtml = string.Empty;
            Visibility = false;
            AttachedFile = false;
            AttachedFileUrl = string.Format("<NULL>");
            SignedBy = string.Empty;
            GetMessageTime();
        }

        public void GetMessageTime()
        {
            // calculates messages
            DateTime now = DateTime.UtcNow;
            MessageTime = now.ToString();
            YearOfMessage = now.Year.ToString();
            MonthOfMessage = now.Month.ToString();
            if (now.Month < 10)
            {
                MonthOfMessage = "0" + MonthOfMessage;
            }
            DayOfMessage = now.Day.ToString();
            if(now.Day < 10)
            {
                DayOfMessage = "0" + DayOfMessage;
            }
            HourOfMessage = now.Hour.ToString();
            if(now.Hour < 10)
            {
                HourOfMessage = "0" + now.Hour.ToString();
            }
            MinuteOfMessage = now.Minute.ToString();
            if(now.Minute < 10)
            {
                MinuteOfMessage = "0" + now.Minute.ToString();
            }

            // calculate ElementToAttach
            ElementToAttach = "date_" + YearOfMessage + "_" + MonthOfMessage + "_" + DayOfMessage;
            MessageHour = string.Format("{0} : {1}",HourOfMessage,MinuteOfMessage);
        }
    }
}
