using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models.KindAdsV2
{
    public class EmailProductForms
    {
        public SendinBlueForm sendinBlueForm { set; get; }

        public AweberForm aweberForm { set; get; }

        public GetResponseForm getResponseForm { set; get; }

        public SendGridForm sendGridForm { set; get; }

        public MailChimpForm mailChimpForm { set; get; }

        public CampaignMonitorForm campaignMonitorForm { set; get; }

        public ActiveCampaignForm activeCampaignForm { set; get; }

        public MailJet mailJetForm { set; get; }

        public IContact icontactForm { set; get; }

        public EmailProductForms()
        {
            sendinBlueForm = new SendinBlueForm();
            aweberForm = new AweberForm();
            getResponseForm = new GetResponseForm();
            sendGridForm = new SendGridForm();
            mailChimpForm = new MailChimpForm();
            campaignMonitorForm = new CampaignMonitorForm();
            activeCampaignForm = new ActiveCampaignForm();
            mailJetForm = new MailJet();
            icontactForm = new IContact();
        }

    }

    public class SendinBlueForm
    {
        public string ApiToken { set; get; }

        //public string Name { set; get; }

        public string FolderId { set; get; }

        public string ListId { set; get; }

        public string Category { set; get; }

        public SendinBlueForm()
        {
            ApiToken = string.Empty;
            //Name = string.Empty;
            FolderId = string.Empty;
            ListId = string.Empty;
            Category = string.Empty;
        }
    }

    public class AweberForm
    {
        public string ApiToken { set; get; }
        public string ListId { set; get; }
        //public string Name { set; get; }

        public AweberForm()
        {
            ApiToken = string.Empty;
            ListId = string.Empty;
            //Name = string.Empty;
        }
    }

    public class GetResponseForm
    {
        public string ApiToken { set; get; }
        //public string Name { set; get; }
        public string ListId { set; get; }
        public string FromFieldId { set; get; }


        public GetResponseForm()
        {
            ApiToken = string.Empty;
            //Name = string.Empty;
            ListId = string.Empty;
            FromFieldId = string.Empty;
        }
    }

    public class SendGridForm
    {
        public string ApiToken { set; get; }
        public string ListId { set; get; }
        //public string Name { set; get; }


        public SendGridForm()
        {
            ApiToken = string.Empty;
            ListId = string.Empty;
            //Name = string.Empty;
        }
    }

    public class MailChimpForm
    {
        public string ApiToken { set; get; }
        //public string Name { set; get; }
        public string TemplateId { set; get; }
        public string ListId { set; get; }


        public MailChimpForm()
        {
            ApiToken = string.Empty;
            //Name = string.Empty;
            TemplateId = string.Empty;
            ListId = string.Empty;
        }
    }

    public class CampaignMonitorForm
    {
        public string ApiToken { set; get; }
        //public string Name { set; get; }

        public string ClientId { set; get; }
        public string ListId { set; get; }
        public string SegmentId { set; get; }

        public CampaignMonitorForm()
        {
            ApiToken = string.Empty;
            //Name = string.Empty;
            ClientId = string.Empty;
            ListId = string.Empty;
            SegmentId = string.Empty;
        }
    }

    public class ActiveCampaignForm
    {
        public string ApiToken { set; get; }
        public string Url { set; get; }
        //public string Name { set; get; }

        public ActiveCampaignForm()
        {
            ApiToken = string.Empty;
            Url = string.Empty;
            //Name = string.Empty;
        }
    }

    public class MailJet
    {
        public string ApiToken { set; get; }

        public string SecretKey { set; get; }

        //public string Name { set; get; }

        public string ListId { set; get; }

        public string Segment { set; get; }

        public MailJet()
        {
            ApiToken = string.Empty;
            SecretKey = string.Empty;
            //Name = string.Empty;

            ListId = string.Empty;
            Segment = string.Empty;
        }
    }

    public class IContact
    {
        public string ApiAppId { set; get; }

        public string Username { set; get; }

        public string AccountId { set; get; }

        public string ClientFolderId { set; get; }

        //public string Name { set; get; }

        public IContact()
        {
            ApiAppId = string.Empty;
            Username = string.Empty;
            AccountId = string.Empty;
            ClientFolderId = string.Empty;
            //Name = string.Empty;
        }
    }
}
