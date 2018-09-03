using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace KindAds.Helpers
{
  public class SubscribersHelper
  {
    public static Boolean ValidateKey(string _apikey, string _urlsite)
    {
      if (_apikey.Length > 0)
      {
        System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("http://app.subscribers.com/api/v1/site");
        request.Method = "GET";
        request.Headers.Add("X-API-Key", _apikey);

        try
        {
          System.Net.HttpWebResponse webResponse = (System.Net.HttpWebResponse)request.GetResponse();
          if (webResponse.StatusCode == System.Net.HttpStatusCode.OK)
          {
            using (System.IO.Stream webStream = webResponse.GetResponseStream())
            {
              if (webStream != null)
              {
                using (System.IO.StreamReader responseReader = new System.IO.StreamReader(webStream))
                {
                  var _apiresponse = responseReader.ReadToEnd();
                  if (_apiresponse != null)
                  {
                    Models.Partner.SubscribersModels.VerifyKeyResponse _response = new Models.Partner.SubscribersModels.VerifyKeyResponse();
                    _response = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Partner.SubscribersModels.VerifyKeyResponse>(_apiresponse);
                    if (_response != null)
                    {
                      string _authorizedsite = _response.url.Replace("http://www.", "").Replace("https://www.", "").Replace("http://", "").Replace("https://", "");
                      _urlsite = _urlsite.Replace("http://www.", "").Replace("https://www.", "").Replace("http://", "").Replace("https://", "");
                      if (_urlsite == _authorizedsite)
                      {
                        return true;
                      }
                    }
                  }
                }
              }
            }
          }
        }
        catch (Exception Ex)
        {
          return false;
        }
      }
      return false;
    }

    [Obsolete]
    public static string SendCampaignMessage(Models.CAMPAIGN _campaign)
    {

      if (_campaign != null)
      {
        Models.KindadsEntities _context = new Models.KindadsEntities();

        var _apikey = (from d in _context.PRODUCT_SETTINGS where d.SettingName.Equals("pushApiToken") where d.PRODUCT_IdProduct.Equals(_campaign.PRODUCT.IdProduct) select d).FirstOrDefault();
        var _url = (from r in _campaign.CAMPAIGN_SETTINGS where r.SettingName.Equals("pushNotifUrl") select r).FirstOrDefault();
        if (_apikey != null)
        {
          Models.Partner.SubscribersModels.MessageRequest _message = new Models.Partner.SubscribersModels.MessageRequest();

          _message.body = _campaign.AdText;
          _message.title = _campaign.Name;
          _message.landing_page_url = _url.SettingValue;
          _message.image_url = _campaign.AdImage;
          _message.utm = new Models.Partner.SubscribersModels.UTM() { campaign = _campaign.UTM_Campaign, medium = _campaign.UTM_Medium, source = _campaign.UTM_Source };
          _message.metadata = new Models.Partner.SubscribersModels.METADATA() { additionalProp1 = "", additionalProp2 = "", additionalProp3 = "" };

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

          Models.Partner.SubscribersModels.MessageResponse _response = new Models.Partner.SubscribersModels.MessageResponse();
          try
          {
            WebResponse webResponse = request.GetResponse();
            using (Stream webStream = webResponse.GetResponseStream())
            {
              if (webStream != null)
              {
                using (StreamReader responseReader = new StreamReader(webStream))
                {
                  _response = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Partner.SubscribersModels.MessageResponse>(responseReader.ReadToEnd());
                  if (_response != null)
                  {
                    if (_response.uuid.Length > 0)
                    {
                      Models.CAMPAIGN _campaignUpdate = (from d in _context.CAMPAIGNs1 where d.IdCampaign.Equals(_campaign.IdCampaign) select d).FirstOrDefault();
                      _campaignUpdate.IdCampaign3rdParty = _response.uuid;
                      _context.CAMPAIGNs1.Attach(_campaignUpdate);
                      _context.Entry(_campaignUpdate).State = System.Data.Entity.EntityState.Modified;
                      _context.SaveChanges();
                    }
                  }
                  return _response.uuid;
                }
              }
            }
          }
          catch (WebException eresp)
          {
            //Do nothing
          }
          finally
          {
            _context.Dispose();
          }
        }
        else
        {
          _context.Dispose();
        }
      }
      return null;
    }
  }
}
