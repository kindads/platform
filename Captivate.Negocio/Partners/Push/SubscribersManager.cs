using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Captivate.DataAccess;
using Captivate.Common.Models.Entities;
using Captivate.Common.Partners.Push;
using System.Net;
using System.IO;
using Captivate.Common.Interfaces;
using Captivate.Business;
using Captivate.Azure;

namespace Captivate.Business.Partners.Push
{
    public class SubscribersManager
    {
        public ITrace telemetria { set; get; }
        public CampaignRepository CampaignRepository { set; get; }
        public ProductRepository ProductRepository { set; get; }
        public SubscribersManager()
        {
            
            telemetria = new Trace();
            CampaignRepository = new CampaignRepository ();
            ProductRepository = new ProductRepository ();
        }
        public string ValidateCampaign(string idCampaign)
        {
            CampaignEntity _campaign = CampaignRepository.FindById(new Guid(idCampaign));
            ProductEntity product = ProductRepository.FindById(_campaign.PRODUCT_IdProduct);
            if (_campaign != null)
            {
                var _apikey = (from d in _campaign.PRODUCT.ProductSettingsEntitys where d.SettingName.Equals("pushApiToken") where d.PRODUCT_IdProduct.Equals(_campaign.PRODUCT.IdProduct) select d).FirstOrDefault();
                var _url = (from r in _campaign.CAMPAIGN_SETTINGS where r.SettingName.Equals("pushNotifUrl") select r).FirstOrDefault();
                if (_apikey != null)
                {
                    SubscribersModels.MessageRequest _message = new SubscribersModels.MessageRequest();

                    _message.body = _campaign.AdText;
                    _message.title = _campaign.Name;
                    _message.landing_page_url = _url.SettingValue;
                    _message.image_url = _campaign.AdImage;
                    _message.utm = new SubscribersModels.UTM() { campaign = _campaign.UTM_Campaign, medium = _campaign.UTM_Medium, source = _campaign.UTM_Source };
                    _message.metadata = new SubscribersModels.METADATA() { additionalProp1 = "", additionalProp2 = "", additionalProp3 = "" };

                    string sdata = Newtonsoft.Json.JsonConvert.SerializeObject(_message);

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://app.subscribers.com/api/v1/messages");
                    request.Method = "POST";
                    request.ContentType = "application/json";
                    request.Headers.Add("X-API-Key", _apikey.SettingValue.ToString());

                    //POST
                    request.ContentLength = sdata.Length;

                    using (Stream webStream = request.GetRequestStream())
                    using (StreamWriter requestWriter = new StreamWriter(webStream, System.Text.Encoding.ASCII))
                    {
                        requestWriter.Write(sdata);
                    }

                    SubscribersModels.MessageResponse _response = new SubscribersModels.MessageResponse();
                    try
                    {
                        WebResponse webResponse = request.GetResponse();
                        using (Stream webStream = webResponse.GetResponseStream())
                        {
                            if (webStream != null)
                            {
                                using (StreamReader responseReader = new StreamReader(webStream))
                                {
                                    _response = Newtonsoft.Json.JsonConvert.DeserializeObject<SubscribersModels.MessageResponse>(responseReader.ReadToEnd());
                                    return _response.uuid;
                                }
                            }
                        }
                    }
                    catch (WebException e)
                    {
                        var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                        telemetria.Critical(messageException);
                    }
                }
            }
            return null;
        }
    }
}
