using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Captivate.Common.Interfaces;
using Captivate.Business;
using Captivate.DataAccess;
using Captivate.Comun.Utils.Partners.Mail.ActiveCampaign;
using Newtonsoft.Json;
using Captivate.Comun.Models.Entities;

namespace Captivate.Negocio.Partners.Mail
{
    public class ActiveCampaignManager
    {
        public ITrace telemetria { set; get; }
        public CampaignRepository CampaignRepository { set; get; }
        public ProductRepository ProductRepository { set; get; }

        public ActiveCampaignManager()
        {
            KindadsContext context = new KindadsContext();
            telemetria = new Trace();
            CampaignRepository = new CampaignRepository { Context = context };
            ProductRepository = new ProductRepository { Context = context };
        }

        public string ValidateCampaign(string idCampaign)
        {
            string apiKey = null;
            string idList = null;
            string url = null;
            string subject = null;
            string idCampaignActiveCampaign = null;
            string fromName = null;
            string fromEmail = null;
            string idMessage = null;

            CampaignEntity campaign = CampaignRepository.FindBy(c => c.IdCampaign == new Guid(idCampaign)).FirstOrDefault();
            ProductEntity product = ProductRepository.FindBy(p => p.IdProduct == campaign.PRODUCT_IdProduct).FirstOrDefault();

            if (product.ProductSettingsEntitys != null && product.ProductSettingsEntitys.Any())
            {
                foreach (var item in product.ProductSettingsEntitys)
                {
                    apiKey = item.SettingName.Equals("activeCampaignApiToken") ? item.SettingValue : apiKey;
                    idList = item.SettingName.Equals("activeCampaignList") ? item.SettingValue : idList;
                    url = item.SettingName.Equals("activeCampaignUrl") ? item.SettingValue : url;
                }
            }

            if (campaign.CAMPAIGN_SETTINGS != null && campaign.CAMPAIGN_SETTINGS.Any())
            {
                foreach (var setting in campaign.CAMPAIGN_SETTINGS)
                {
                    subject = setting.SettingName.Equals("activeCampaignSubject") ? setting.SettingValue : subject;
                }
            }

            ActiveCampaignClient activeCampaignClient = new ActiveCampaignClient(apiKey, url);
            Dictionary<string, string> parameters = new Dictionary<string, string>() { };

            var result = activeCampaignClient.ApiAsync("account_view", parameters);
            dynamic data = JsonConvert.DeserializeObject(result.Data);
            fromEmail = data.email;
            fromName = data.fname;

            activeCampaignClient = new ActiveCampaignClient(apiKey, url);
            parameters = new Dictionary<string, string>
      {
        { "format", "mime" },
        { "subject", subject },
        { "fromemail", fromEmail },
        { "fromname", fromName },
        { "reply2", "" },
        { "priority", "5" },
        { "charset", "utf-8" },
        { "encoding", "quoted-printable" },
        { "htmlconstructor", "editor" },
        { "html", campaign.AdText },
        { "textconstructor", "editor" },
        { "p[" + idList + "]", idList }
      };
            result = activeCampaignClient.ApiAsync("message_add", parameters);
            data = JsonConvert.DeserializeObject(result.Data);
            idMessage = data.id;
            //Console.WriteLine(result.Code);//1 successfull

            activeCampaignClient = new ActiveCampaignClient(apiKey, url);
            parameters = new Dictionary<string, string>
      {
        { "type", "text" },
        { "segmentid", "0" },
        { "bounceid", "-1" },
        { "name", campaign.Name },
        { "sdate", DateTime.Now.ToString() },
        { "status", "1" },
        { "public", "1" },
        { "mailer_log_file", "4" },
        { "tracklinks", "all" },
        { "tracklinksanalytics", "" },
        { "trackreads", "0" },
        { "trackreplies", "0" },
        { "htmlunsub", "1" },
        { "textunsub", "1" },
        { "p[" + idList + "]", idList },
        { "m["+ idMessage +"]", "100" }
      };
            result = activeCampaignClient.ApiAsync("campaign_create", parameters);
            data = JsonConvert.DeserializeObject(result.Data);
            idCampaignActiveCampaign = data.id;
            //Console.WriteLine(data.id);
            //Console.WriteLine(result.Code);//1 successfull

            idCampaignActiveCampaign = Convert.ToBoolean(result.Code) ? data.id : null;
            return idCampaignActiveCampaign;
        }
    }


}
