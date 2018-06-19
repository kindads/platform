using Captivate.Business;
using Captivate.Common.Interfaces;
using Captivate.Comun.Models.Entities;
using Captivate.DataAccess;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Negocio.Partners.Push
{
    public class PushEngageManger
    {
        public ITrace telemetria { set; get; }
        public CampaignRepository CampaignRepository { set; get; }
        public ProductRepository ProductRepository { set; get; }
        private static readonly string api_url = "https://api.pushengage.com/apiv1/notifications";
        public PushEngageManger()
        {
            
            telemetria = new Trace();
            CampaignRepository = new CampaignRepository ();
            ProductRepository = new ProductRepository ();
        }

        public bool IsValid(string apiKey)
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    wc.Headers.Add("api_key", apiKey);
                    byte[] bret = wc.DownloadData(api_url + "?status=sent");
                    string HtmlResult = System.Text.Encoding.UTF8.GetString(bret);

                    if (HtmlResult.Length > 0)
                    {
                        dynamic dyn = JsonConvert.DeserializeObject(HtmlResult);
                        return dyn.success;
                    }
                }
            }
            catch (Exception ex)
            {
                var messageException = telemetria.MakeMessageException(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }

            return false;
        }

        public string ValidateCampaign(string idCampaign)
        {
            string idCampaignEngage = null;

            var _campaign = CampaignRepository.FindById(new Guid(idCampaign));

            try
            {
                if (_campaign != null)
                {
                    EngageMessage.Title = _campaign.Name;
                    EngageMessage.Message = _campaign.AdText;
                    EngageMessage.Url = GetCampaignSetting(_campaign.CAMPAIGN_SETTINGS, "pushNotifUrl");
                    EngageMessage.Image_url = GetCampaignSetting(_campaign.CAMPAIGN_SETTINGS, "pushNotifImage");
                    EngageMessage.Key = GetProductSetting(_campaign.PRODUCT.ProductSettingsEntitys, "pushApiToken");

                    string newMessage = "notification_title=" + EngageMessage.Title + "&";
                    newMessage += "notification_message=" + EngageMessage.Message + "&";
                    newMessage += "notification_url=" + EngageMessage.Url + "&";
                    newMessage += "image_url=" + EngageMessage.Image_url + "";

                    using (WebClient wc = new WebClient())
                    {
                        wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                        wc.Headers.Add("api_key", EngageMessage.Key);
                        byte[] bret = wc.UploadData(api_url, "POST", System.Text.Encoding.UTF8.GetBytes(newMessage));
                        string HtmlResult = Encoding.UTF8.GetString(bret);

                        if (HtmlResult.Length > 0)
                        {
                            dynamic dyn = JsonConvert.DeserializeObject(HtmlResult);
                            idCampaignEngage = dyn.notification_id;

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                var messageException = telemetria.MakeMessageException(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }

            return idCampaignEngage;
        }

        private class EngageMessage
        {
            internal static string Title;
            internal static string Message;
            internal static string Url;
            internal static string Image_url;
            internal static string Key;
        }

        private string GetCampaignSetting(ICollection<CampaignSettingsEntity> settings, string key)
        {
            return (from r in settings where r.SettingName.Equals(key) select r).FirstOrDefault().SettingValue;
        }

        private string GetProductSetting(ICollection<ProductSettingsEntity> settings, string key)
        {
            return (from r in settings where r.SettingName.Equals(key) select r).FirstOrDefault().SettingValue;
        }
    }
}
