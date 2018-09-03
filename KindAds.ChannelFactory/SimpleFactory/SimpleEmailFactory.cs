using KindAds.Comun.Enums;
using KindAds.Comun.Interfaces;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using KindAds.Negocio.Partnersv2.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.ChannelFactory.SimpleFactory
{
    public static class SimpleEmailFactory
    {
        public static IMailChannel CreateChannel(ChannelType type)
        {
            switch (type)
            {
                case ChannelType.IContact:
                    {
                        return new IContactManagerv2();
                    }
                case ChannelType.SendGrid:
                    {
                        return new SendGridManagerv2();
                    }
                case ChannelType.SendinBlue:
                    {
                        return new SendinBlueManagerv2();
                    }
                case ChannelType.ActiveCampaign:
                    {
                        return new ActiveCampaignManagerv2();
                    }
                case ChannelType.Aweber:
                    {
                        return new AWeberManagerv2();
                    }
                case ChannelType.CampaignMonitor:
                    {
                        return new CampaignMonitorManagerv2();
                    }
                case ChannelType.GetResponse:
                    {
                        return new GetResponseManagerv2();
                    }
                case ChannelType.MailChimp:
                    {
                        return new MailChimpManagerv2();
                    }
                case ChannelType.MailJet:
                    {
                        return new MailJetManagerv2();
                    }
                default:
                    return null;
            }
        }

        public static IMailChannel CreateChannel(ChannelType type, IList<AudiencePropertieSetting> settings, string emailProductId)
        {
            switch (type)
            {
                case ChannelType.IContact:
                    {
                        return new IContactManagerv2(settings, emailProductId);
                    }
                case ChannelType.SendGrid:
                    {
                        return new SendGridManagerv2(settings, emailProductId);
                    }
                case ChannelType.SendinBlue:
                    {
                        return new SendinBlueManagerv2(settings, emailProductId);
                    }
                case ChannelType.ActiveCampaign:
                    {
                        return new ActiveCampaignManagerv2(settings, emailProductId);
                    }
                case ChannelType.Aweber:
                    {
                        return new AWeberManagerv2(settings, emailProductId);
                    }
                case ChannelType.CampaignMonitor:
                    {
                        return new CampaignMonitorManagerv2(settings, emailProductId);
                    }
                case ChannelType.GetResponse:
                    {
                        return new GetResponseManagerv2(settings, emailProductId);
                    }
                case ChannelType.MailChimp:
                    {
                        return new MailChimpManagerv2(settings, emailProductId);
                    }
                case ChannelType.MailJet:
                    {
                        return new MailJetManagerv2(settings, emailProductId);
                    }
                default:
                    return null;
            }
        }

        public static IMailChannel CreateChannel(ChannelType type, string emailProductId)
        {
            switch (type)
            {
                case ChannelType.IContact:
                    {
                        return new IContactManagerv2( emailProductId);
                    }
                case ChannelType.SendGrid:
                    {
                        return new SendGridManagerv2( emailProductId);
                    }
                case ChannelType.SendinBlue:
                    {
                        return new SendinBlueManagerv2( emailProductId);
                    }
                case ChannelType.ActiveCampaign:
                    {
                        return new ActiveCampaignManagerv2(emailProductId);
                    }
                case ChannelType.Aweber:
                    {
                        return new AWeberManagerv2( emailProductId);
                    }
                case ChannelType.CampaignMonitor:
                    {
                        return new CampaignMonitorManagerv2( emailProductId);
                    }
                case ChannelType.GetResponse:
                    {
                        return new GetResponseManagerv2( emailProductId);
                    }
                case ChannelType.MailChimp:
                    {
                        return new MailChimpManagerv2( emailProductId);
                    }
                case ChannelType.MailJet:
                    {
                        return new MailJetManagerv2( emailProductId);
                    }
                default:
                    return null;
            }
        }
    }
}
