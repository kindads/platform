using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Captivate.Common.Interfaces;
using Captivate.Business;
using Captivate.DataAccess;
using SendGrid;
using Newtonsoft.Json;
using System.Net;
using Captivate.Comun.Models.Entities;

namespace Captivate.Negocio.Partners.Mail
{
    public class SendGridManager
    {
        public ITrace telemetria { set; get; }
        public CampaignRepository CampaignRepository { set; get; }
        public ProductRepository ProductRepository { set; get; }
        private static string apiHost = "https://api.sendgrid.com/";

        public SendGridManager()
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
            string idSender = null;
            string idUnsubscriberGroup = null;
            string subject = null;
            string idCampaignSendGrid = null;

            CampaignEntity campaign = CampaignRepository.FindBy(c => c.IdCampaign == new Guid(idCampaign)).FirstOrDefault();
            ProductEntity product = ProductRepository.FindBy(p => p.IdProduct == campaign.PRODUCT_IdProduct).FirstOrDefault();

            if (product.ProductSettingsEntitys != null && product.ProductSettingsEntitys.Any())
            {
                foreach (var item in product.ProductSettingsEntitys)
                {
                    apiKey = item.SettingName.Equals("sendGridApiToken") ? item.SettingValue : apiKey;
                    idList = item.SettingName.Equals("sendGridList") ? item.SettingValue : idList;
                    idSender = item.SettingName.Equals("sendGridSender") ? item.SettingValue : idSender;
                    idUnsubscriberGroup = item.SettingName.Equals("sendGridUnsubscribeGroup") ? item.SettingValue : idUnsubscriberGroup;
                }
            }

            if (campaign.CAMPAIGN_SETTINGS != null && campaign.CAMPAIGN_SETTINGS.Any())
            {
                foreach (var setting in campaign.CAMPAIGN_SETTINGS)
                {
                    subject = setting.SettingName.Equals("sendGridSubject") ? setting.SettingValue : subject;
                }
            }

            var headers = new Dictionary<string, string> { { "X-Mock", "201" } };
            var sg = new SendGridClient(apiKey, apiHost, headers);
            JsonCampaign jsonCampaign = new JsonCampaign()
            {
                categories = new List<string>(),
                custom_unsubscribe_url = "",
                html_content = "<html><head><title></title></head><body>" + campaign.AdText + "[unsubscribe]</body></html>",
                list_ids = new List<int>() { Convert.ToInt32(idList) },
                plain_content = "",
                segment_ids = new List<int>(),
                sender_id = Convert.ToInt32(idSender),
                subject = subject,
                suppression_group_id = Convert.ToInt32(idUnsubscriberGroup),
                title = campaign.Name
            };
            var data = JsonConvert.SerializeObject(jsonCampaign);
            var response = sg.RequestAsync(method: SendGridClient.Method.POST, urlPath: "campaigns", requestBody: data).GetAwaiter().GetResult();
            var deserializeBody = response.DeserializeResponseBody(response.Body);

            if (response.StatusCode.Equals(HttpStatusCode.Created))
            {
                idCampaignSendGrid = Convert.ToString(deserializeBody["id"]);
                SendCampaign(apiKey, idCampaignSendGrid);
            }

            return idCampaignSendGrid;

        }

        private HttpStatusCode SendCampaign(string apiKey, string idCampaignSendGrid)
        {
            var headers = new Dictionary<string, string> { { "X-Mock", "200" } };
            var sg = new SendGridClient(apiKey, apiHost, headers);
            var response = sg.RequestAsync(method: SendGridClient.Method.POST, urlPath: "campaigns/" + idCampaignSendGrid + "/schedules/now").GetAwaiter().GetResult();
            var deserializeBody = response.DeserializeResponseBody(response.Body);
            return response.StatusCode;
        }

    }

    public class JsonCampaign
    {
        public List<string> categories { get; set; }
        public string custom_unsubscribe_url { get; set; }
        public string html_content { get; set; }
        public List<int> list_ids { get; set; }
        public string plain_content { get; set; }
        public List<int> segment_ids { get; set; }
        public int sender_id { get; set; }
        public string subject { get; set; }
        public string title { get; set; }
        public int suppression_group_id { get; set; }
    }

}
