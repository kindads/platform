using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Captivate.Common.Interfaces;
using Captivate.Business;
using Captivate.DataAccess;

namespace Captivate.Negocio.Partners.Push
{
    public class PushCrewManager
    {
        public ITrace telemetria { set; get; }
        public CampaignRepository CampaignRepository { set; get; }
        public ProductRepository ProductRepository { set; get; }
        public PushCrewManager()
        {
            KindadsContext context = new KindadsContext();
            telemetria = new Trace();
            CampaignRepository = new CampaignRepository { Context = context };
            ProductRepository = new ProductRepository { Context = context };
        }
        public string ValidateCampaign(string idCampaign)
        {
            var _campaign = CampaignRepository.GetById(new Guid(idCampaign));
            var product = ProductRepository.FindById(new Guid(idCampaign));
            if (_campaign != null)
            {
                var _apikey = (from d in _campaign.PRODUCT.ProductSettingsEntitys where d.SettingName.Equals("pushApiToken") where d.PRODUCT_IdProduct.Equals(_campaign.PRODUCT.IdProduct) select d).FirstOrDefault();

                if (_apikey != null)
                {
                    PushcrewModel.MessageRequest _message = new PushcrewModel.MessageRequest();

                    _message.title = _campaign.Name;
                    _message.message = _campaign.AdText;
                    _message.url = _campaign.AdURL;
                    _message.image_url = _campaign.AdImage;

                    string newMessage = "title=" + _message.title + "&";
                    newMessage += "message=" + _message.message + "&";
                    newMessage += "url=" + _message.url + "&";
                    newMessage += "image_url=" + _message.image_url + "";

                    try
                    {
                        using (WebClient wc = new WebClient())
                        {
                            wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                            wc.Headers[HttpRequestHeader.Authorization] = _apikey.SettingValue.ToString();
                            byte[] bret = wc.UploadData("https://pushcrew.com/api/v1/send/all/", "POST", System.Text.Encoding.UTF8.GetBytes(newMessage));
                            string HtmlResult = System.Text.Encoding.UTF8.GetString(bret);
                            string key = "";

                            if (HtmlResult.Length > 0)
                            {
                                dynamic dyn = JsonConvert.DeserializeObject(HtmlResult);
                                key = dyn.request_id;
                                return key;
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var messageException = telemetria.MakeMessageException(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                        telemetria.Critical(messageException);
                    }
                }
            }
            return null;
        }
    }
}
