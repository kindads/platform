using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Captivate.Negocio.Helper
{
  public class DateTimeHelper
  {
    public static string GetTimestamp(DateTime dateTime)
    {
      // format: YYYY-MM-DDTHH:MM::SS-04:00
      string sreturn = "";
      string syear = dateTime.Year.ToString();
      if (syear.Length < 4)
      {
        syear = (dateTime.Year + 2000).ToString();
      }
      string smonth = dateTime.Month.ToString();
      if (smonth.Length < 2)
      {
        smonth = "0" + smonth;
      }
      string sday = dateTime.Day.ToString();
      if (sday.Length < 2)
      {
        sday = "0" + sday;
      }
      string shour = dateTime.Hour.ToString();
      if (shour.Length < 2)
      {
        shour = "0" + shour;
      }
      string sminute = dateTime.Minute.ToString();
      if (sminute.Length < 2)
      {
        sminute = "0" + sminute;
      }
      string ssec = dateTime.Second.ToString();
      if (ssec.Length < 2)
      {
        ssec = "0" + ssec;
      }
      string smilisec = dateTime.Millisecond.ToString();

      sreturn = syear +"-"+ smonth +"-"+ sday +"T"+ shour +":"+ sminute +":"+ ssec + "-04:00";

      return sreturn;
    }

    public static string GetTimestamp()
    {
      string sreturn = "";
      string syear = DateTime.Today.Year.ToString();
      if (syear.Length < 4)
      {
        syear = (DateTime.Today.Year + 2000).ToString();
      }
      string smonth = DateTime.Today.Month.ToString();
      if (smonth.Length < 2)
      {
        smonth = "0" + smonth;
      }
      string sday = DateTime.Today.Day.ToString();
      if (sday.Length < 2)
      {
        sday = "0" + sday;
      }
      string shour = DateTime.Now.Hour.ToString();
      if (shour.Length < 2)
      {
        shour = "0" + shour;
      }
      string sminute = DateTime.Now.Minute.ToString();
      if (sminute.Length < 2)
      {
        sminute = "0" + sminute;
      }
      string ssec = DateTime.Now.Second.ToString();
      if (ssec.Length < 2)
      {
        ssec = "0" + ssec;
      }
      string smilisec = DateTime.Now.Millisecond.ToString();

      sreturn = syear + smonth + sday + shour + sminute + ssec + smilisec;

      return sreturn;
    }

    public static string CurrentDateTimeString()
    {
      string sreturn = "";
      string syear = DateTime.Today.Year.ToString();
      if (syear.Length < 4)
      {
        syear = (DateTime.Today.Year + 2000).ToString();
      }
      string smonth = DateTime.Today.Month.ToString();
      if (smonth.Length < 2)
      {
        smonth = "0" + smonth;
      }
      string sday = DateTime.Today.Day.ToString();
      if (sday.Length < 2)
      {
        sday = "0" + sday;
      }
      string shour = DateTime.Now.Hour.ToString();
      if (shour.Length < 2)
      {
        shour = "0" + shour;
      }
      string sminute = DateTime.Now.Minute.ToString();
      if (sminute.Length < 2)
      {
        sminute = "0" + sminute;
      }
      string ssec = DateTime.Now.Second.ToString();
      if (ssec.Length < 2)
      {
        ssec = "0" + ssec;
      }
      string smilisec = DateTime.Now.Millisecond.ToString();

      sreturn = syear + "-" + smonth + "-" + sday + " " + shour + ":" + sminute +":" + ssec;

      return sreturn;
    }
    public static string GetCurrentDateString()
    {
      string sreturn = "";
      DateTime ldt_today = DateTime.Today;
      string syear = ldt_today.Year.ToString();
      if (syear.Length < 4)
      {
        syear = (ldt_today.Year + 2000).ToString();
      }
      string smonth = ldt_today.Month.ToString();
      if (smonth.Length < 2)
      {
        smonth = "0" + smonth;
      }
      string sday = ldt_today.Day.ToString();
      if (sday.Length < 2)
      {
        sday = "0" + sday;
      }

      sreturn = syear + "-" + smonth + "-" + sday + " 00:00";
      return sreturn;
    }

    public static string GetCurrentDateString(int AddHours, int AddDays)
    {
      string sreturn = "";
      DateTime ldt_today = DateTime.Today;

      if (AddHours > 0)
      {
        ldt_today = DateTime.Today.AddHours(AddHours);
      }

      if (AddDays > 0)
      {
        ldt_today = DateTime.Today.AddDays(AddDays);
      }

      string syear = ldt_today.Year.ToString();
      if (syear.Length < 4)
      {
        syear = (ldt_today.Year + 2000).ToString();
      }
      string smonth = ldt_today.Month.ToString();
      if (smonth.Length < 2)
      {
        smonth = "0" + smonth;
      }
      string sday = ldt_today.Day.ToString();
      if (sday.Length < 2)
      {
        sday = "0" + sday;
      }

      sreturn = syear + "-" + smonth + "-" + sday + " 00:00";

      return sreturn;
    }

  }
}
