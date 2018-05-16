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

namespace Captivate.Comun.Utils.Partners.Mail.Aweber.OAuth
{
    /// <summary>
    /// Provides the parameters required to pass to the AWeber API.
    /// https://labs.aweber.com/docs/authentication 
    /// </summary>
    public class Request
    {

        public String oauth_callback { get; set; }
        public String oauth_consumer_key { get; set; }
        public String oauth_consumer_secret { get; set; }
        public String oauth_nonce { get; set; }
        public String oauth_signature { get; set; }
        public String oauth_signature_method { get; set; }
        public String oauth_timestamp { get; set; }
        public String oauth_token { get; set; }
        public String oauth_token_secret { get; set; }
        public String oauth_verifier { get; set; }
        public String oauth_version { get; set; }
        public String Parameters { get; set; }

        public Request()
        {
            // Set defaults
            oauth_version = "1.0";
            oauth_signature_method = "HMAC-SHA1";
        }

        public void Build(SortedList<String, String> parameters, String url)
        {
            Build(parameters, url, "POST");
        }

        /// <summary>
        /// Will fill in and build all required values for a valid OAuth request
        /// </summary>
        public void Build(SortedList<String, String> parameters, String url, String httpMethod)
        {
            // Set the timestamp
            oauth_timestamp = Base.GenerateTimeStamp();

                       // Set the nonce (Random integer converted to ASCII string)
            oauth_nonce = Base.GenerateNonce();
            String normalizedUrl = String.Empty;
            String normalizedRequestParameters = String.Empty;

            // Generate the signature
            
            oauth_signature = Base.GenerateSignature(new Uri(url), oauth_consumer_key, oauth_consumer_secret, oauth_token, oauth_token_secret, httpMethod, oauth_timestamp, oauth_nonce, Base.SignatureTypes.HMACSHA1, parameters, out normalizedUrl, out normalizedRequestParameters);
            
            Parameters = normalizedRequestParameters;

            // Add signature
            Parameters += String.Format("&oauth_signature={0}", Base.UrlEncode(oauth_signature));



        }


    }
}
