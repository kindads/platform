using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Utils
{
    public class DateUtils
    {
        public static DateTime GetMexicanDateTimeNow()
        {
            TimeZoneInfo setTimeZoneInfo;
            DateTime currentDateTime;
            setTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)");
            currentDateTime = TimeZoneInfo.ConvertTime(DateTime.Now, setTimeZoneInfo);



            return currentDateTime;
        }

        public static string GetTextDaysAgo(DateTime pastDate)
        {
            DateTime now= GetMexicanDateTimeNow();
            double noDaysDifference = (now - pastDate).TotalDays;
            return noDaysDifference > 1 ? pastDate.ToShortDateString() : (noDaysDifference == 0 ? "Today" : "Yesterday");
        }
    }
}
