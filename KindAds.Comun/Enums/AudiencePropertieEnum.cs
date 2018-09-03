using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Enums
{
    // renombrar a AudiencePropertieType
    public enum ChannelType
    {
        IContact,
        SendGrid,
        SendinBlue,
        ActiveCampaign,
        Aweber,
        CampaignMonitor,
        GetResponse,
        MailChimp,
        MailJet,

        OneSignal,
        PushEngage,
        PushCrew,
        Subscribers
    }

    public static class ChannelProvider
    {
        public const string IContactId = "b963c480-ba92-4305-a59a-80d2c6e43cd5";
        public const string SendGridId = "00f9f759-c3f0-4a96-b47c-44438f785756";
        public const string SendinBlueId = "8c0d6cc7-230c-4a2b-a379-51aa1fd321b6";
        public const string ActiveCampaignId = "48ef64fa-163f-43b7-9e4c-e1b19e72b785";
        public const string AweberId = "74679a58-e03f-470e-abc8-953c6b2ff79a";
        public const string CampaignMonitorId = "f30ae168-93b5-407d-b9a7-6d167555184b";
        public const string GetResponseId= "1f35b9b2-3cde-4893-87c7-74f2a6ceb29a";
        public const string MailChimpId = "260bb8bd-b343-48fe-a171-eb8a45ca6aac";
        public const string MailJetId = "fa7a4049-23f2-4485-b404-3c099b8740be";

        public const string OneSignalId = "e49a5ebf-0a07-4de3-b62f-f5a4627e5d0f";
        public const string PushEngageId = "3b90bfc4-911b-4980-8e8c-b89b9af51bee";
        public const string PushCrewId = "cb1e0ff2-8249-4a50-8dcb-381f85f8d4a1";
        public const string SubscribersId = "852da5fe-e8fc-46cb-8cbb-b4eecb320a6d";
    }
}
