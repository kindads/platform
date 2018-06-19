using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Captivate.Common.Interfaces;
using Captivate.Comun.Models.Entities;
using Captivate.Business;
using Captivate.DataAccess;
using createsend_dotnet;
using Captivate.Azure;
using System.Net;

namespace Captivate.Negocio.Partners.Mail
{
    public class CampaignMonitorManager
    {
        public ITrace telemetria { set; get; }
        public CampaignRepository CampaignRepository { set; get; }
        public ProductRepository ProductRepository { set; get; }

        private readonly ProductSettingsRepository productSettingsRepository;
        private readonly CampaignSettingsRepository campaignSettingsRepository;
        public CampaignMonitorManager()
        {
            telemetria = new Trace();
            CampaignRepository = new CampaignRepository();
            campaignSettingsRepository = new CampaignSettingsRepository();
            ProductRepository = new ProductRepository();
            productSettingsRepository = new ProductSettingsRepository();
        }

        public string ValidateCampaign(string idCampaign)
        {
            CampaignEntity campaign = CampaignRepository.FindById(new Guid(idCampaign));
            campaign.CAMPAIGN_SETTINGS = campaignSettingsRepository.GetCampaignSettingsByIdCampaign(campaign.IdCampaign);
            ProductEntity product = ProductRepository.FindById(campaign.PRODUCT_IdProduct);
            product.ProductSettingsEntitys = productSettingsRepository.GetProductSettingsByIdProduct(product.IdProduct);
            string idList = null;
            string apiKey = null;
            string clientID = null;
            string subject = "";
            string fromEmail = "";
            string fromName = "";
            string segment = "";


            if (product.ProductSettingsEntitys != null && product.ProductSettingsEntitys.Any())
            {
                foreach (var item in product.ProductSettingsEntitys)
                {
                    apiKey = item.SettingName.Equals("campaignMonitorApiToken") ? item.SettingValue : apiKey;
                    idList = item.SettingName.Equals("campaignMonitorList") ? item.SettingValue : idList;
                    clientID = item.SettingName.Equals("campaignMonitorClient") ? item.SettingValue : clientID;
                    segment = item.SettingName.Equals("campaignMonitorSegment") ? item.SettingValue : segment;
                }
            }

            if (campaign.CAMPAIGN_SETTINGS != null && campaign.CAMPAIGN_SETTINGS.Any())
            {
                foreach (var setting in campaign.CAMPAIGN_SETTINGS)
                {
                    subject = setting.SettingName.Equals("campaignMonitorSubject") ? setting.SettingValue : subject;
                    fromName = setting.SettingName.Equals("campaignMonitorFromName") ? setting.SettingValue : fromName;
                    fromEmail = setting.SettingName.Equals("campaignMonitorFromEmail") ? setting.SettingValue : fromEmail;
                }
            }

            try
            {
                AuthenticationDetails auth = new ApiKeyAuthenticationDetails(apiKey);
                List<string> listIDs = new List<string> { idList };
                List<string> segmentIDs = new List<string> { segment };

                TemplateContent templateContent = new TemplateContent();

                string campaignID = createsend_dotnet.Campaign.CreateFromTemplate(
                    auth,
                    clientID,
                    subject,
                    campaign.Name,
                    fromName + "-" + DateTime.Now.ToString("MMddyyyyhhmm"),
                    fromEmail,
                    fromEmail,
                    listIDs,
                    segmentIDs,
                    CreateTemplate(auth, clientID, campaign),
                    templateContent);

                createsend_dotnet.Campaign campaignMonitorCampaign = new createsend_dotnet.Campaign(auth, campaignID);
                campaignMonitorCampaign.Send(fromEmail);
                return campaignID;
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }

            return null;
        }

        private string CreateTemplate(AuthenticationDetails auth, string clientID, CampaignEntity campaign)
        {
            string template = string.Empty;
            try
            {
                var htmlText = WebUtility.HtmlDecode(GenerateTemplateCampaignMonitor(campaign));
                var templateID = BlobManager.CreateHtmlTemplateAzure(htmlText);
                template = Template.Create(auth, clientID, "TEMPLATE " + campaign.Name, templateID, "");
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return template;
        }

        private string GenerateTemplateCampaignMonitor(CampaignEntity campaign)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                sb.Append("<html><head><title>").Append(campaign.Name).Append("</title></head>");
                sb.Append("<body><p><singleline>").Append(campaign.Name).Append("</singleline></p>");
                sb.Append("<div><multiline>").Append(campaign.AdText).Append("</multiline></div>");
                sb.Append("<p><unsubscribe>Unsubscribe</unsubscribe></p></body></html>");
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return sb.ToString();
        }
    }
}
