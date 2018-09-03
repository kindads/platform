using KindAds.Azure;
using KindAds.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Business
{
    //public class Trace : EventSource, ITrace
    //{
    //    public class Keywords
    //    {
    //        public const EventKeywords Endup = (EventKeywords)1;
    //        public const EventKeywords Notify = (EventKeywords)2;
    //        public const EventKeywords Failure = (EventKeywords)4;
    //        public const EventKeywords Startup = (EventKeywords)8;
    //        public const EventKeywords Warning = (EventKeywords)16;
    //        public const EventKeywords Critical = (EventKeywords)32;
    //    }

    //    [Event(1, Message = "Application Failure", Level = EventLevel.Critical, Keywords = Keywords.Failure)]
    //    public void Failure(string message)
    //    {
    //        WriteEvent(1, message);
    //    }

    //    [Event(2, Message = "Starting up.", Keywords = Keywords.Startup, Level = EventLevel.Informational)]
    //    public void Startup()
    //    {
    //        this.WriteEvent(2, "Starting up.");
    //    }

    //    [Event(3, Message = "End up", Keywords = Keywords.Endup, Level = EventLevel.Informational)]
    //    public void Endup()
    //    {
    //        this.WriteEvent(3, "End up");
    //    }

    //    [Event(4, Message = "Notify", Keywords = Keywords.Notify, Level = EventLevel.Informational)]
    //    public void Notify(string message)
    //    {
    //        TableManager.InsertMessage("Notify", message);
    //        this.WriteEvent(4, message);
    //    }

    //    [Event(5, Message = "Warning", Keywords = Keywords.Warning, Level = EventLevel.Informational)]
    //    public void Warning(string message)
    //    {
    //        TableManager.InsertMessage("Warning", message);
    //        this.WriteEvent(5, message);
    //    }

    //    [Event(6, Message = "Critical", Keywords = Keywords.Critical, Level = EventLevel.Critical)]
    //    public void Critical(string message)
    //    {
    //        TableManager.InsertMessage("Critical", message);
    //        this.WriteEvent(6, message);
    //    }

    //    public string MakeMessageException(Exception e, string metodo)
    //    {
    //        StringBuilder messageException = new StringBuilder();
    //        messageException.Append("Metodo:" + metodo);
    //        if (e.TargetSite != null)
    //        {
    //            messageException.Append("From:");
    //            messageException.AppendLine(e.TargetSite.ToString());
    //        }
    //        if (e.Source != null)
    //        {
    //            messageException.Append("App or Object:");
    //            messageException.AppendLine(e.Source.ToString());
    //        }
    //        if (e.StackTrace != null)
    //        {
    //            messageException.Append("StackTrace:");
    //            messageException.AppendLine(e.StackTrace.ToString());
    //        }
    //        if (e.Message != null)
    //        {
    //            messageException.AppendLine("Mensaje:");
    //            messageException.AppendLine(e.Message.ToString());
    //        }
    //        return messageException.ToString();
    //    }
    //}
}
