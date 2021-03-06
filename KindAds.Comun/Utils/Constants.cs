using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KindAds.Common.Utils
{
    public static class Constants
    {
        public const string PROVIDER_SUBSCRIBERS = "5bc235aa-792f-41a9-a9b0-a1a202517898";
        public const string PROVIDER_MAIL_CHIMP = "242bed16-e278-4ebf-9c71-d1aeca06a36e";
        public const string PROVIDER_PUSH_CREW = "8c993a27-9142-4ccf-9d6d-e0c1079da37d";
        public const string PROVIDER_CAMPAIGN_MONITOR = "83283ddf-e525-4ab5-bae7-d51253d09c1f";
        public const string PROVIDER_AWEBER = "358359ab-2921-49de-83ce-225e74bfb312";
        public const string PROVIDER_GETRESPONSE = "14aedd6f-ea33-4ded-b388-61543bfb520d";
        public const string PROVIDER_SEND_GRID = "0b4562b7-ddff-42e5-895d-b94606358285";
        public const string PROVIDER_ACTIVE_CAMPAIGN = "70dd43a5-80d7-4b7a-83dd-a2aed0d1a544";
        public const string PROVIDER_ICONTACT = "2d45c5e3-11c9-46f3-a931-45d397718001";
        public const string PROVIDER_SENDINBLUE = "88bee210-5bdb-44a6-8e58-1a5106b9e0cc";
        public const string PROVIDER_PUSH_ENGAGE = "ff8ddb95-ddbd-488b-bd75-01c3c04d1847";
        public const string PROVIDER_ONE_SIGNAL = "cbea6db4-0a09-40fe-831d-da001ac1351b";
        public const string PROVIDER_MAILJET = "757dd20e-b548-4aba-a618-bb577221dc4d";
    }

    public enum MailingProviders
    {
        Subscribers = 1,
        MailChimp,
        PushCrew,
        CampaignMonitor,
        Aweber,
        GetResponse,
        SendGrid,
        ActiveCampaign,
        IContact,
        SendinBlue
    }

    public enum IContactRequest
    {
        AddCampaign = 1,
        AddSends = 2,
        AddMessage = 3,

        GetCamapign = 4,
        GetSends = 5,
        GetMessage = 6,
        GetLists = 7, //https://app.sandbox.icontact.com/icp/a/{accountid}/c/{clientfolderid}/lists

        //https://app.sandbox.icontact.com/icp/a/[accountid]/users/
        GetUsers = 8,
        //https://app.sandbox.icontact.com/icp/a/{accountid}/c/
        GetFolders = 9,
    }
}
