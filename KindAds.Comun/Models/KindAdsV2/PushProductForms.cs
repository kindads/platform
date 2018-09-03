using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models.KindAdsV2
{
    public class PushProductForms
    {
        public OneSignalForm oneSignalForm { set; get; }

        public PushCrewForm pushCrewForm { set; get; }

        public PushEngageForm pushEnagageForm { set; get; }

        public SubscribersForm subscribersForm { set; get; }

    }

    public class OneSignalForm
    {
        public string ApiToken { set; get; }
        //public string Name { set; get; }

        public string AppId { set; get; }

        public OneSignalForm()
        {
            ApiToken = string.Empty;
            //Name = string.Empty;
        }
    }

    public class PushCrewForm
    {
        public string ApiToken { set; get; }
        //public string Name { set; get; }
        public string Url { set; get; }

        public PushCrewForm()
        {
            ApiToken = string.Empty;
            //Name = string.Empty;
            Url = string.Empty;
        }
    }

    public class PushEngageForm
    {
        public string ApiToken { set; get; }
        //public string Name { set; get; }

        public PushEngageForm()
        {
            ApiToken = string.Empty;
            //Name = string.Empty;
        }
    }

    public class SubscribersForm
    {
        public string ApiToken { set; get; }
        //public string Name { set; get; }

        public string Url { set; get; }

        public SubscribersForm()
        {
            ApiToken = string.Empty;
            //Name = string.Empty;
            Url = string.Empty;
        }
    }
}
