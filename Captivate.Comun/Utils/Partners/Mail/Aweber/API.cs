/*
* AWeber API .NET SDK v1.0
* Providing the ability to connect a .NET application to the AWeber API.
* 
* Copyright (c) 2011 - Binkd
* Licensed under the GNU General Public License (GNU GPL v3.0)
* 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using Captivate.Comun.Utils.Partners.Mail.Aweber.OAuth;
using System.IO;

namespace Captivate.Comun.Utils.Partners.Mail.Aweber
{

    /// <summary>
    /// Provides the public facing API instance. Provides methods for .NET applications to use
    /// to communicate with the AWeber API.
    /// </summary>
    public class API
    {

        #region Properties

        public IAdapter adapter = new Adapter();


        public String OAuthTokenSecret { get { return adapter.OAuthTokenSecret; } set { adapter.OAuthTokenSecret = value; } }
        public String OAuthToken { get { return adapter.OAuthToken; } set { adapter.OAuthToken = value; } }
        public String OAuthVerifier { get { return adapter.OAuthVerifier; } set { adapter.OAuthVerifier = value; } }

        public String CallbackUrl { get { return adapter.CallbackUrl; } set { adapter.CallbackUrl = value; } }

        #endregion

       

        /// <summary>
        /// 
        /// </summary>
        /// <param name="consumerKey">The consumer key of your AWeber application</param>
        /// <param name="consumerSecret">The consumer secret of your AWeber application</param>
        public API(String consumerKey, String consumerSecret)
        {
            adapter.ConsumerKey = consumerKey;
            adapter.ConsumerSecret = consumerSecret;

            // If Web Based
            //if (System.Web.HttpContext.Current != null)
            //{
                // Set callback Url to default of this page (can be changed by manually if required)
                adapter.CallbackUrl = "";
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appId">The application Id of your AWeber application</param>
        public API(String appId)
        {
            adapter.AppId = appId;

            // If Web Based
            //if (HttpContext.Current != null)
            //{
                // Set callback Url to default of this page (can be changed by manually if required)
                adapter.CallbackUrl = "";
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appId">The application Id of your AWeber application</param>
        public API(String appId, IAdapter newAdapter)
        {
            adapter = newAdapter;

            adapter.AppId = appId;

            // If Web Based
            //if (HttpContext.Current != null)
            //{
                // Set callback Url to default of this page (can be changed by manually if required)
                adapter.CallbackUrl = "";
            //}
        }

        ///
        /// Calls during authentication require a POST request
        ///    

        #region Authentication

        /// <summary>
        /// Will issue the appropriate data to start OAuth 1.0 authorization
        /// </summary>
        /// <param name="callbackUrl">The callback url</param>
        public void get_request_token(String callbackUrl)
        {
            // Build Post Data
            OAuth.Request request = new OAuth.Request();

            // Set Values
            request.oauth_callback = callbackUrl;
            request.oauth_consumer_key = adapter.ConsumerKey;
            request.oauth_consumer_secret = adapter.ConsumerSecret;

            // Build custom parameters for this OAuth Request
            SortedList<String, String> parameters = new SortedList<string, string>();

            parameters.Add("oauth_callback", callbackUrl);

            // Build remainder of request
            request.Build(parameters, Settings.requestToken);

            // Send request to Aweber
            WebClient client = new WebClient();

            client.Headers["Content-type"] = "application/x-www-form-urlencoded";

            String response = String.Empty;

            try
            {
                // Get response
                response = client.UploadString(OAuth.Settings.requestToken, request.Parameters);

            }
            catch (WebException ex)
            {
                // Example on how to get the response text from the exception if needed

                //HttpWebResponse httpResponse = (HttpWebResponse)ex.Response;

                //using (Stream data = ex.Response.GetResponseStream())
                //{
                //    String text = new StreamReader(data).ReadToEnd();

                //}

                // Throw Error Codes back to client
                // Client responsibily to handle them
                throw ex;
            }

            // Parse Request Token
            SortedList<String, String> responseValues = new SortedList<string, string>();

            String[] keyValuePair = response.Split('&');

            foreach (String pair in keyValuePair)
            {

                String[] split = pair.Split('=');
                responseValues.Add(split[0], split[1]);

            }

            adapter.OAuthToken = responseValues["oauth_token"];
            adapter.OAuthTokenSecret = responseValues["oauth_token_secret"];

        }

        /// <summary>
        /// Will issue the appropriate data to start OAuth 1.0 authorization
        /// </summary>
        public void get_request_token()
        {
            get_request_token(adapter.CallbackUrl);
        }

        /// <summary>
        /// Redirect user to the authorization page on AWeber
        /// </summary>
        public void authorize()
        {
            if (!String.IsNullOrEmpty(adapter.ConsumerKey) && !String.IsNullOrEmpty(adapter.ConsumerSecret))
            {
                // Verify based on ConsumerKey and ConsumerSecret
                //HttpContext.Current.Response.Redirect(String.Format("{0}?oauth_token={1}", OAuth.Settings.authorization, HttpUtility.UrlEncode(adapter.OAuthToken)));
            }
            else
            {
                // Verify based on AppId
                //HttpContext.Current.Response.Redirect(String.Format("{0}{1}", OAuth.Settings.authorizeApp, adapter.AppId));
            }
        }

        /// <summary>
        /// Get Access token from AWeber (final step in OAuth) so we have a permanent token to use when performing API calls
        /// </summary>
        public String get_access_token()
        {
            // Build Post Data
            OAuth.Request request = new OAuth.Request();

            // Set Values
            request.oauth_verifier = adapter.OAuthVerifier;
            request.oauth_consumer_key = adapter.ConsumerKey;
            request.oauth_consumer_secret = adapter.ConsumerSecret;
            request.oauth_token = adapter.OAuthToken;
            request.oauth_token_secret = adapter.OAuthTokenSecret;

            // Build custom parameters for this OAuth Request
            SortedList<String, String> parameters = new SortedList<string, string>();

            parameters.Add("oauth_verifier", request.oauth_verifier);

            // Build request
            request.Build(parameters, Settings.accessToken);

            WebClient client = new WebClient();

            client.Headers["Content-type"] = "application/x-www-form-urlencoded";

            String response = String.Empty;

            try
            {
                // Get response
                response = client.UploadString(OAuth.Settings.accessToken, request.Parameters);

            }
            catch (WebException ex)
            {
                // Throw Error Codes back to client
                // Client responsibily to handle them
                throw ex;
            }

            // Save Access Token (Replace existing access tokens as they have now expired)
            SortedList<String, String> responseValues = new SortedList<string, string>();

            String[] keyValuePair = response.Split('&');

            foreach (String pair in keyValuePair)
            {

                String[] split = pair.Split('=');
                responseValues.Add(split[0], split[1]);

            }

            adapter.OAuthToken = responseValues["oauth_token"];
            adapter.OAuthTokenSecret = responseValues["oauth_token_secret"];

            return adapter.OAuthToken;
        }

        /// <summary>
        /// Input the token received from the Aweber website when using the verify by AppId method.
        /// This is unique to verification on OAuth using the AppId. If you use the ConsumerKey and ConsumerSecret this 
        /// method is not used or needed
        /// </summary>
        /// <param name="token">The token received from the Aweber website</param>
        public void process_appId_token(String token)
        {
            // Token from Aweber received in format of 

            // application key, application secret, request token, token secret, and oauth_verifier

            String[] tokenValues = token.Split('|');

            // Assign values in token to the appropriate internal values
            adapter.ConsumerKey = tokenValues[0];
            adapter.ConsumerSecret = tokenValues[1];
            adapter.OAuthToken = tokenValues[2];
            adapter.OAuthTokenSecret = tokenValues[3];
            adapter.OAuthVerifier = tokenValues[4];
        }

        #endregion




        #region Account

        /// <summary>
        /// Will retrieve all accounts attached to the token stored, should only ever be 1.
        /// </summary>
        /// <returns></returns>
        public Entity.Account getAccount()
        {
        
            // Build request
            String url = String.Format("{0}accounts", Settings.apiBase);
    
            return Factory.BaseCollection<Entity.Account>.Build(url, JSON.Read(adapter.GetResponse(url)), adapter).entries.First();

        }

        #endregion

      

    }
}
