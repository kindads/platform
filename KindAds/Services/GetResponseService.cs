using KindAds.Utils.GetResponse;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System;

namespace KindAds.Services
{
  public class GetResponseService
  {
    private static string baseUrl = "https://api.getresponse.com/v3/";

    CampaignService _campaignService;
    public GetResponseService()
    {
      _campaignService = new CampaignService();
    }

    public bool IsValid(string apiKey)
    {
      var result = MethodGet(apiKey, "accounts");
      return result.Code.Equals(HttpStatusCode.OK);
    }

    public List<Models.GenericElement> GetLists(string key)
    {
      List<Models.GenericElement> list = new List<Models.GenericElement>();
      Models.GenericElement element;
      var result = MethodGet(key, "campaigns");
      dynamic data = JsonConvert.DeserializeObject(result.Data);
      foreach (var items in data)
      {
        element = new Models.GenericElement()
        {
          Id = items["campaignId"],
          Name = items["name"]
        };
        list.Add(element);
      }
      return list;
    }

    public List<Models.GenericElement> GetFromFields(string key)
    {
      List<Models.GenericElement> list = new List<Models.GenericElement>();
      Models.GenericElement element;
      var result = MethodGet(key, "from-fields");
      dynamic data = JsonConvert.DeserializeObject(result.Data);
      foreach (var items in data)
      {
        if (Convert.ToBoolean(items["isActive"]))
        {
          element = new Models.GenericElement()
          {
            Id = items["fromFieldId"],
            Name = items["email"]
          };
          list.Add(element);
        }
      }
      return list;
    }

    [Obsolete]
    public string CreateCampaign(string idCampaign)
    {
      dynamic json = new ExpandoObject();
      dynamic fromField = new ExpandoObject();
      dynamic campaign = new ExpandoObject();
      dynamic content = new ExpandoObject();
      dynamic sendSettings = new ExpandoObject();
      dynamic selectedCampaigns = new List<string>();

      string apiKey = null;
      string idList = null;
      string idFromField = null;
      string subject = null;

      var _campaign = _campaignService.GetCampaignById(idCampaign);
      var _product = _campaign.PRODUCT;

      if (_product.PRODUCT_SETTINGS != null && _product.PRODUCT_SETTINGS.Any())
      {
        foreach (var item in _product.PRODUCT_SETTINGS)
        {
          apiKey = item.SettingName.Equals("getResponseApiToken") ? item.SettingValue : apiKey;
          idList = item.SettingName.Equals("getResponseList") ? item.SettingValue : idList;
          idFromField = item.SettingName.Equals("getResponseFromField") ? item.SettingValue : idFromField;
        }
      }

      if (_campaign.CAMPAIGN_SETTINGS != null && _campaign.CAMPAIGN_SETTINGS.Any())
      {
        foreach (var setting in _campaign.CAMPAIGN_SETTINGS)
        {
          subject = setting.SettingName.Equals("getResponseSubject") ? setting.SettingValue : subject;
        }
      }

      json.name = _campaign.Name;
      json.type = "broadcast";
      json.subject = subject;

      fromField.fromFieldId = idFromField;
      json.fromField = fromField;

      campaign.campaignId = idList;
      json.campaign = campaign;

      content.html = _campaign.AdText;
      content.plain = null;
      json.content = content;

      json.replyTo = fromField;

      selectedCampaigns.Add(idList);
      sendSettings.timeTravel = "false";
      sendSettings.perfectTiming = "true";
      sendSettings.selectedCampaigns = selectedCampaigns;
      sendSettings.selectedSegments = new List<string>();
      sendSettings.selectedSuppressions = new List<string>();
      sendSettings.excludedCampaigns = new List<string>();
      sendSettings.excludedSegments = new List<string>();
      sendSettings.selectedContacts = new List<string>();

      json.sendSettings = sendSettings;
      var result = MethodPost(apiKey, "newsletters", (IDictionary<string, object>)json);
      dynamic data = JsonConvert.DeserializeObject(result.Data);
      return (string)data.newsletterId;
    }

    public static ApiResult MethodGet(string apikey, string action)
    {
      string result = "";
      ApiResult apiResult = new ApiResult();
      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + action);
      request.Method = "GET";
      request.Headers.Add("X-Auth-Token", "api-key " + apikey);
      try
      {
        WebResponse webResponse = request.GetResponse();
        apiResult.Code = ((HttpWebResponse)webResponse).StatusCode;
        apiResult.Message = ((HttpWebResponse)webResponse).StatusDescription;
        using (Stream webStream = webResponse.GetResponseStream())
        {
          if (webStream != null)
          {
            using (StreamReader responseReader = new StreamReader(webStream))
            {
              result = responseReader.ReadToEnd();
              apiResult.Data = result;
            }
          }
        }
      }
      catch (WebException wex)
      {
        result = wex.Message;
      }
      return apiResult;
    }

    public static ApiResult MethodPost(string apikey, string action, IDictionary<string, object> parameters)
    {
      ApiResult apiResult = new ApiResult();

      var serialized = JsonConvert.SerializeObject(parameters);

      WebRequest request = WebRequest.Create(baseUrl + action);
      request.Method = "POST";
      request.Headers.Add("X-Auth-Token", "api-key " + apikey);
      byte[] byteArray = Encoding.UTF8.GetBytes(serialized);
      request.ContentType = "application/json";

      Stream dataStream = request.GetRequestStream();
      dataStream.Write(byteArray, 0, byteArray.Length);
      dataStream.Close();

      WebResponse response = request.GetResponse();
      apiResult.Code = ((HttpWebResponse)response).StatusCode;
      apiResult.Message = ((HttpWebResponse)response).StatusDescription;
      dataStream = response.GetResponseStream();

      StreamReader reader = new StreamReader(dataStream);
      string responseFromServer = reader.ReadToEnd();
      apiResult.Data = responseFromServer;

      reader.Close();
      dataStream.Close();
      response.Close();

      return apiResult;
    }
  }
}
