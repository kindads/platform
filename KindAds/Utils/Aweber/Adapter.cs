using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Aweber
{
  /// <summary>
  /// Communicates with the Aweber API
  /// Specifically connection based
  /// </summary>
  public class Adapter : IAdapter
  {
    #region Properties

    public String ConsumerKey { get; set; }
    public String ConsumerSecret { get; set; }
    public String AppId { get; set; }
    public String CallbackUrl { get; set; }
    public String OAuthTokenSecret { get; set; }
    public String OAuthToken { get; set; }
    public String OAuthVerifier { get; set; }  
    #endregion

    /// <summary>
    /// Connects to the API, issues the command and retrieves the JSON response
    /// </summary>
    /// <param name="url">The base url to contact for the request</param>
    /// <returns>The JSON returned</returns>
    public String GetResponse(String url)
    {
      // Build Post Data
      OAuth.Request request = new OAuth.Request();

      // Set Values
      request.oauth_consumer_key = ConsumerKey;
      request.oauth_consumer_secret = ConsumerSecret;
      request.oauth_token = OAuthToken;
      request.oauth_token_secret = OAuthTokenSecret;
      request.oauth_verifier = OAuthVerifier;

      // Build custom parameters for this OAuth Request
      SortedList<String, String> parameters = new SortedList<string, string>();

      String par = String.Empty;

      if (url.Contains("?"))
      {
        par = url.Substring(url.LastIndexOf("?") + 1);

        String[] values = par.Split('&');

        foreach (String value in values)
        {
          String[] keyValue = value.Split('=');

          parameters.Add(keyValue[0], keyValue[1]);
        }

        url = url.Substring(0, url.LastIndexOf("?"));

        if (url.Contains("http://"))
        {
          url = url.Replace("http://", "https://");
        }
      }

      // Build request
      request.Build(parameters, url, "GET");

      WebClient client = new WebClient();

      client.Headers["Content-type"] = "application/x-www-form-urlencoded";

      String response = String.Empty;

      try
      {
        // Get response
        response = client.DownloadString(url + "?" + request.Parameters);

      }
      catch (WebException ex)
      {
        if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response != null)
        {
          var resp = (HttpWebResponse)ex.Response;
          if (resp.StatusCode == HttpStatusCode.NotFound)
          {
            // 404 error
            return String.Empty;
          }
          else
          {
            throw ex;
          }
        }
        else
        {
          throw ex;
        }
      }
      catch (Exception ex)
      {
        // Throw Error Codes back to client
        // Client responsibility to handle them
        throw ex;
      }

      return response;
    }

    public OAuth.Request BuildRequest()
    {
      // Build Post Data
      OAuth.Request request = new OAuth.Request();

      // Set Values
      request.oauth_consumer_key = ConsumerKey;
      request.oauth_consumer_secret = ConsumerSecret;
      request.oauth_token = OAuthToken;
      request.oauth_token_secret = OAuthTokenSecret;
      request.oauth_verifier = OAuthVerifier;

      return request;
    }

  }
}
