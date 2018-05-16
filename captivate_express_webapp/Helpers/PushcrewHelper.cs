using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Web;

namespace captivate_express_webapp.Helpers
{
  public class PushcrewHelper
  {

    public static bool validatePushCrewToken(string _apiKey, string _siteUrl)
    {
      if (_apiKey.Length > 0)
      {
        System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("https://pushcrew.com/api/v1/segments");
        request.Method = "GET";
        request.Headers.Add("Authorization", _apiKey);
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
                    Models.Partner.PushcrewModel.ValidKeyResponse _response = new Models.Partner.PushcrewModel.ValidKeyResponse();
                    _response = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Partner.PushcrewModel.ValidKeyResponse>(_apiresponse);
                    if (_response != null)
                    {
                      if (_response.status != null)
                      {
                        if ((_response.message != null && !_response.message.Contains("not authorized")) || _response.status == "success")
                        {
                          return true;
                        }
                        else
                        {
                          return false;
                        }
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

        if (_apikey != null)
        {
          Models.Partner.PushcrewModel.MessageRequest _message = new Models.Partner.PushcrewModel.MessageRequest();

          _message.title = _campaign.Name;
          _message.message = _campaign.AdText;
          _message.url = _campaign.AdURL;
          _message.image_url = _campaign.AdImage;

          string newMessage = "title=" + _message.title + "&";
          newMessage += "message=" + _message.message + "&";
          newMessage += "url=" + _message.url + "&";
          newMessage += "image_url=" + _message.image_url + "";

          //POST
          try
          {
            using (WebClient wc = new WebClient())
            {
              wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
              wc.Headers[HttpRequestHeader.Authorization] = _apikey.SettingValue.ToString();
              byte[] bret = wc.UploadData("https://pushcrew.com/api/v1/send/all/", "POST", System.Text.Encoding.UTF8.GetBytes(newMessage));
              string HtmlResult = System.Text.Encoding.UTF8.GetString(bret);
              string key = "";
              //Models.Partner.PushcrewModel.MessageResponse _response = new Models.Partner.PushcrewModel.MessageResponse();
              if (HtmlResult.Length > 0)
              {
                dynamic dyn = JsonConvert.DeserializeObject(HtmlResult);
                key = dyn.request_id;

                Models.CAMPAIGN _campaignUpdate = (from d in _context.CAMPAIGNs1 where d.IdCampaign.Equals(_campaign.IdCampaign) select d).FirstOrDefault();
                _campaignUpdate.IdCampaign3rdParty = key;
                _context.CAMPAIGNs1.Attach(_campaignUpdate);
                _context.Entry(_campaignUpdate).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
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
            return ex.Message;
          }
        }
      }
      return null;
    }

  }
}
