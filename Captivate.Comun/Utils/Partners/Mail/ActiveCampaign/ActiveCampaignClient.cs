using System;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Captivate.Comun.Utils.Partners.Mail.ActiveCampaign
{
  public class ActiveCampaignClient
  {
    private readonly string _apiKey;
    private readonly string _baseUrl;

    public ActiveCampaignClient(string apiKey, string baseUrl)
    {
      if (string.IsNullOrEmpty(apiKey))
        throw new ArgumentNullException(nameof(apiKey));

      if (string.IsNullOrEmpty(baseUrl))
        throw new ArgumentNullException(nameof(baseUrl));

      _apiKey = apiKey;
      _baseUrl = baseUrl;
    }

    private string CreateBaseUrl(string apiAction)
    {
      return $"{_baseUrl}/admin/api.php?api_action={apiAction}&api_key={_apiKey}&api_output=json";
    }

    public ApiResult ApiAsync(string apiAction, Dictionary<string, string> parameters)
    {
      //var payload = PreparePayload(parameters);
      var uri = CreateBaseUrl(apiAction);

      using (HttpClient httpClient = new HttpClient())
      {
        using (var postContent = new FormUrlEncodedContent(parameters))
        {
          using (HttpResponseMessage response = httpClient.PostAsync(uri, postContent).GetAwaiter().GetResult())
          {
            response.EnsureSuccessStatusCode(); //throw if httpcode is an error
            using (HttpContent content = response.Content)
            {
              string rawData = content.ReadAsStringAsync().GetAwaiter().GetResult();
              var result = JsonConvert.DeserializeObject<ApiResult>(rawData);
              result.Data = rawData;
              return result;
            }
          }
        }
      }
    }
  }
}
