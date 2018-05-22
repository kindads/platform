using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailChimp.Campaigns;
using MailChimp;
using Captivate.Common.Interfaces;
using Captivate.Business;
using Captivate.DataAccess;
using Captivate.Comun.Models.Entities;

namespace Captivate.Negocio.Partners.Mail
{
    public class MailChimpManager
    {
        public ITrace telemetria { set; get; }
        public CampaignRepository CampaignRepository { set; get; }
        public ProductRepository ProductRepository { set; get; }
        public CategoryRepository CategoryRepository { set; get; }
        public MailChimpManager()
        {
            KindadsContext context = new KindadsContext();
            telemetria = new Trace();
            CampaignRepository = new CampaignRepository { Context = context };
            ProductRepository = new ProductRepository { Context = context };
            CategoryRepository = new CategoryRepository { Context = context };
        }

        public string ValidateCampaign(string idCampaign)
        {
            string idList = null;
            string apiKey = null;
            int idTemplate = 0;
            string subject = "";
            string fromEmail = "";
            string fromName = "";

            CampaignEntity campaign = CampaignRepository.FindBy(c => c.IdCampaign == new Guid(idCampaign)).FirstOrDefault();
            ProductEntity product = ProductRepository.FindBy(p => p.IdProduct == campaign.PRODUCT_IdProduct).FirstOrDefault();

            try
            {
                if (product.ProductSettingsEntitys != null && product.ProductSettingsEntitys.Any())
                {
                    foreach (var item in product.ProductSettingsEntitys)
                    {
                        apiKey = item.SettingName.Equals("mailChimpApiToken") ? item.SettingValue : apiKey;
                        idList = item.SettingName.Equals("mailChimpList") ? item.SettingValue : idList;
                        idTemplate = item.SettingName.Equals("mailChimpTemplate") ? Convert.ToInt32(item.SettingValue) : idTemplate;
                    }
                }

                if (campaign.CAMPAIGN_SETTINGS != null && campaign.CAMPAIGN_SETTINGS.Any())
                {
                    foreach (var setting in campaign.CAMPAIGN_SETTINGS)
                    {
                        subject = setting.SettingName.Equals("mailChimpSubject") ? setting.SettingValue : subject;
                        fromName = setting.SettingName.Equals("mailChimpFromName") ? setting.SettingValue : fromName;
                        fromEmail = setting.SettingName.Equals("mailChimpFromEmail") ? setting.SettingValue : fromEmail;
                    }
                }

                CampaignCreateOptions cco = new CampaignCreateOptions()
                {
                    ListId = idList,
                    Title = campaign.Name,
                    Subject = subject,
                    FromEmail = fromEmail,
                    FromName = fromName
                };

                Dictionary<string, string> sections = new Dictionary<string, string>();
                var categorys = CategoryRepository.GetByIdSite(campaign.PRODUCT.SITE.IdSite);
                if (categorys != null && categorys.Any())
                {
                    foreach (var item in categorys)
                    {
                        sections.Add(item.IdCategory.ToString(), item.Description);
                    }
                }

                CampaignCreateContent ccc = new CampaignCreateContent()
                {
                    Sections = sections,
                    HTML = campaign.AdText
                };

                IMailChimpManager mailChimpManager = new MailChimp.MailChimpManager(apiKey);
                var campaignMailChimp = mailChimpManager.CreateCampaign("regular", cco, ccc, null, null);

                if (campaign != null && !String.IsNullOrEmpty(campaignMailChimp.Id))
                {
                    var resultSendCampaign = mailChimpManager.SendCampaign(campaignMailChimp.Id);
                    return resultSendCampaign.Complete ? campaignMailChimp.Id : null;
                }
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return null;
        }
    }
}
