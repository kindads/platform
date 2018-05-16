using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SendGrid;

namespace captivate_express_webapp.Services
{
  public class SendGridService
  {
    private static string apiHost = "https://api.sendgrid.com/";
    private CampaignService _campaignService;
    public SendGridService()
    {
      _campaignService = new CampaignService();
    }

    public bool IsValid(string apiKey)
    {
      var headers = new Dictionary<string, string> { { "X-Mock", "200" } };
      var sg = new SendGridClient(apiKey, apiHost, headers);
      var response = sg.RequestAsync(method: SendGridClient.Method.GET, urlPath: "contactdb/lists").GetAwaiter().GetResult();
      return response.StatusCode.Equals(HttpStatusCode.OK);
    }

    public List<Models.GenericElement> GetLists(string apiKey)
    {
      List<Models.GenericElement> list = new List<Models.GenericElement>();
      Models.GenericElement genericElement;
      try
      {
        var headers = new Dictionary<string, string> { { "X-Mock", "200" } };
        var sg = new SendGridClient(apiKey, apiHost, headers);
        var response = sg.RequestAsync(method: SendGridClient.Method.GET, urlPath: "contactdb/lists").GetAwaiter().GetResult();
        var deserializeBody = response.DeserializeResponseBody(response.Body);
        foreach (var item in deserializeBody)
        {
          var arrayJson = (JArray)JsonConvert.DeserializeObject(Convert.ToString(item.Value));
          foreach (var element in arrayJson.Children())
          {
            var itemProperties = element.Children<JProperty>();
            var id = itemProperties.FirstOrDefault(x => x.Name == "id");
            var name = itemProperties.FirstOrDefault(x => x.Name == "name");
            genericElement = new Models.GenericElement()
            {
              Id = id.Value.ToString(),
              Name = name.Value.ToString()
            };
            list.Add(genericElement);
          }
        }
      }
      catch (Exception) { }

      return list;
    }

    public List<Models.GenericElement> GetSenders(string apiKey)
    {
      List<Models.GenericElement> list = new List<Models.GenericElement>();
      Models.GenericElement genericElement;
      try
      {
        var headers = new Dictionary<string, string> { { "X-Mock", "200" } };
        var sg = new SendGridClient(apiKey, apiHost, headers);
        var response = sg.RequestAsync(method: SendGridClient.Method.GET, urlPath: "senders").GetAwaiter().GetResult();
        var deserializeBody = response.Body.ReadAsStringAsync().GetAwaiter().GetResult();

        var arrayJson = (JArray)JsonConvert.DeserializeObject(Convert.ToString(deserializeBody));
        foreach (var element in arrayJson.Children())
        {
          var itemProperties = element.Children<JProperty>();
          var id = itemProperties.FirstOrDefault(x => x.Name == "id");
          var nickname = itemProperties.FirstOrDefault(x => x.Name == "nickname");
          var verified = itemProperties.FirstOrDefault(x => x.Name == "verified");
          JObject verifiedObject = JObject.Parse(verified.Value.ToString());
          string status = (string)verifiedObject["status"];
          if (Convert.ToBoolean(status))
          {
            genericElement = new Models.GenericElement()
            {
              Id = id.Value.ToString(),
              Name = nickname.Value.ToString()
            };
            list.Add(genericElement);
          }
        }
      }
      catch (Exception) { }

      return list;

    }

    public List<Models.GenericElement> GetUnsubscribeGroups(string apiKey)
    {
      List<Models.GenericElement> list = new List<Models.GenericElement>();
      Models.GenericElement genericElement;
      try
      {
        var headers = new Dictionary<string, string> { { "X-Mock", "200" } };
        var sg = new SendGridClient(apiKey, apiHost, headers);
        var queryParams = @"{}";
        var response = sg.RequestAsync(method: SendGridClient.Method.GET, urlPath: "asm/groups", queryParams: queryParams).GetAwaiter().GetResult();
        var deserializeBody = response.Body.ReadAsStringAsync().GetAwaiter().GetResult();

        var arrayJson = (JArray)JsonConvert.DeserializeObject(Convert.ToString(deserializeBody));
        foreach (var element in arrayJson.Children())
        {
          var itemProperties = element.Children<JProperty>();
          var id = itemProperties.FirstOrDefault(x => x.Name == "id");
          var name = itemProperties.FirstOrDefault(x => x.Name == "name");
          genericElement = new Models.GenericElement()
          {
            Id = id.Value.ToString(),
            Name = name.Value.ToString()
          };
          list.Add(genericElement);
        }
      }
      catch (Exception) { }

      return list;
    }

    [Obsolete]
    public string CreateCampaignSendGrid(string idCampaign)
    {
      string apiKey = null;
      string idList = null;
      string idSender = null;
      string idUnsubscriberGroup = null;
      string subject = null;
      string idCampaignSendGrid = null;

      var campaign = _campaignService.GetCampaignById(idCampaign);
      var product = _campaignService.GetProductByIdProduct(campaign.PRODUCT.IdProduct);

      if (product.PRODUCT_SETTINGS != null && product.PRODUCT_SETTINGS.Any())
      {
        foreach (var item in product.PRODUCT_SETTINGS)
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

      if (response.StatusCode.Equals(HttpStatusCode.Created)){
        idCampaignSendGrid = Convert.ToString(deserializeBody["id"]);
        SendCampaign(apiKey, idCampaignSendGrid);
      }

      return idCampaignSendGrid;

    }

    [Obsolete]
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
