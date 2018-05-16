using captivate_express_webapp.Models;
using captivate_express_webapp.Utils.ActiveCampaign;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace captivate_express_webapp.Services
{
  public class ActiveCampaignService
  {
    private Services.CampaignService _campaignService;

    public ActiveCampaignService()
    {
      _campaignService = new CampaignService();
    }

    public bool IsValid(string apiKey, string url)
    {
      ActiveCampaignClient activeCampaignClient = new ActiveCampaignClient(apiKey, url);
      Dictionary<string, string> parameters = new Dictionary<string, string>() { };
      var result = activeCampaignClient.ApiAsync("account_view", parameters);
      return Convert.ToBoolean(result.Code);
    }

    public List<GenericElement> GetLists(string apiKey, string url)
    {
      GenericElement element;
      List<GenericElement> list = new List<GenericElement>();
      ActiveCampaignClient activeCampaignClient = new ActiveCampaignClient(apiKey, url);
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      var result = activeCampaignClient.ApiAsync("list_paginator", parameters);
      dynamic stuff = JsonConvert.DeserializeObject(result.Data);
      dynamic rows = stuff.rows;
      foreach (var item in rows)
      {
        element = new GenericElement()
        {
          Id = item.id,
          Name = item.name
        };
        list.Add(element);
      }
      return list;
    }

    [Obsolete]
    public string CreateCampaignActiveCampaign(string idCampaign)
    {
      string apiKey = null;
      string idList = null;
      string url = null;
      string subject = null;
      string idCampaignActiveCampaign = null;
      string fromName = null;
      string fromEmail = null;
      string idMessage = null;

      var campaign = _campaignService.GetCampaignById(idCampaign);
      var product = _campaignService.GetProductByIdProduct(campaign.PRODUCT.IdProduct);

      if (product.PRODUCT_SETTINGS != null && product.PRODUCT_SETTINGS.Any())
      {
        foreach (var item in product.PRODUCT_SETTINGS)
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
