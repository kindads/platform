using KindAds.Comun.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Structures
{
    public struct SChannelProvider
    {
        public static (string providerId, string description) ActiveCampaign = (ChannelProvider.ActiveCampaignId, "Active Campaign");
        public static (string providerId, string description) Aweber = (ChannelProvider.AweberId, "AWeber");
        public static (string providerId, string description) CampaignMonitor = (ChannelProvider.CampaignMonitorId, "Campaign Monitor");
        public static (string providerId, string description) GetResponse = (ChannelProvider.GetResponseId, "GetResponse");
        public static (string providerId, string description) IContact = (ChannelProvider.IContactId, "IContact");
        public static (string providerId, string description) MailChimp = (ChannelProvider.MailChimpId, "MailChimp");
        public static (string providerId, string description) MailJet = (ChannelProvider.MailJetId, "Mail Jet");
        public static (string providerId, string description) OneSignal = (ChannelProvider.OneSignalId, "One Signal");
        public static (string providerId, string description) PushCrew = (ChannelProvider.PushCrewId, "PushCrew");
        public static (string providerId, string description) PushEngage = (ChannelProvider.PushEngageId, "PushEngage");
        public static (string providerId, string description) SendGrid = (ChannelProvider.SendGridId, "SendGrid");
        public static (string providerId, string description) SendinBlue = (ChannelProvider.SendinBlueId, "SendinBlue");
        public static (string providerId, string description) Subscribers = (ChannelProvider.SubscribersId, "Subscribers");
        
        public static List<(string providerId, string description)> Providers = new List<(string providerId, string description)>(new(string providerId, string description)[]
        {
            SChannelProvider.ActiveCampaign
            ,SChannelProvider.Aweber
            ,SChannelProvider.CampaignMonitor
            ,SChannelProvider.GetResponse
            ,SChannelProvider.IContact
            ,SChannelProvider.MailChimp
            ,SChannelProvider.MailJet
            ,SChannelProvider.OneSignal
            ,SChannelProvider.PushCrew
            ,SChannelProvider.PushEngage
            ,SChannelProvider.SendGrid
            ,SChannelProvider.SendinBlue
            ,SChannelProvider.Subscribers

        });
    }
    
}
