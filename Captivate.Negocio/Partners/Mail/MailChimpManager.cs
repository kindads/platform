﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailChimp.Campaigns;
using MailChimp;
using Captivate.Common.Interfaces;
using Captivate.Business;
using Captivate.DataAccess;
using Captivate.Common.Models.Entities;
using Captivate.Azure;

namespace Captivate.Business.Partners.Mail
{
    public class MailChimpManager
    {
        public ITrace telemetria { set; get; }
        public CampaignRepository CampaignRepository { set; get; }
        public ProductRepository ProductRepository { set; get; }
        public CategoryRepository CategoryRepository { set; get; }
        public MailChimpManager()
        {
            
            telemetria = new Trace();
            CampaignRepository = new CampaignRepository();
            ProductRepository = new ProductRepository();
            CategoryRepository = new CategoryRepository();
        }

        public string ValidateCampaign(string idCampaign)
        {
            string idList = null;
            string apiKey = null;
            int idTemplate = 0;
            string subject = "";
            string fromEmail = "";
            string fromName = "";

            CampaignEntity campaign = CampaignRepository.FindById(new Guid(idCampaign));
            ProductEntity product = ProductRepository.FindById(campaign.PRODUCT_IdProduct);

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

        public bool addUserToList(string apiKey, string idList, string email)
        {
            MailChimp.Net.Models.Member member = new MailChimp.Net.Models.Member()
            {
                EmailAddress = email,
                Status = MailChimp.Net.Models.Status.Subscribed
            };

            MailChimp.Net.Interfaces.IMailChimpManager mailChimpManager = new MailChimp.Net.MailChimpManager(apiKey);
            var result =  Task.Run(() =>mailChimpManager.Members.AddOrUpdateAsync(idList, member)).Result;
            
            return result.Id != null;


        }
    }
}
