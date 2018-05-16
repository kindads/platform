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
using System.Net;

namespace Captivate.Comun.Utils.Partners.Mail.Aweber.Entity
{
    public class BaseCollection<T>
    {

        #region Adapter

        private IAdapter _api = null;
        public IAdapter api { get { return _api; } } 

        public BaseCollection(IAdapter adapter)
        {
            _api = adapter;

            // Initialize integers
            total_size = -1;
            start = -1;

        }

        #endregion

        /// <summary>
        /// A link to this collection
        /// </summary>
        public String self_link { get; set; }

        /// <summary>
        /// Total number of entries in the collection
        /// </summary>
        public int total_size { get; set; }

        /// <summary>
        /// Starting index of the collection
        /// </summary>
        public int start { get; set; }

        /// <summary>
        /// Collection of entries
        /// </summary>
        public List<T> entries { get; set; }

        /// <summary>
        /// A link to a campaign collection.
        /// </summary>
        public String next_collection_link { get; set; }

        /// <summary>
        /// A link to a campaign collection. 
        /// </summary>
        public String prev_collection_link { get; set; }

        /// <summary>
        /// The link to the WADL description of this resource.
        /// </summary>
        public String resource_type_link { get; set; }


        public T create(SortedList<String, Object> param)
        {

            Type typeParameterType = typeof(T);

            T entity = (T)Activator.CreateInstance(typeParameterType, api);
                       
            // Build Post Data
            OAuth.Request request = new OAuth.Request();

            // Set Values
            request.oauth_consumer_key = api.ConsumerKey;
            request.oauth_consumer_secret = api.ConsumerSecret;
            request.oauth_token = api.OAuthToken;
            request.oauth_token_secret = api.OAuthTokenSecret;

            // Build custom parameters for this OAuth Request
            SortedList<String, String> parameters = new SortedList<string, string>();

            // Add parameters
            parameters.Add("ws.op", "create");

            foreach (String key in param.Keys)
            {
                parameters.Add(key, param[key].ToString());
            }

            request.Build(parameters, self_link, "POST");

            WebClient client = new WebClient();

            client.Headers["Content-type"] = "application/x-www-form-urlencoded";

            String response = String.Empty;

            try
            {
                // Get response
                response = client.UploadString(self_link, request.Parameters);

            }
            catch (WebException ex)
            {
                // Throw Error Codes back to client
                // Client responsibility to handle them
                throw ex;
            }


            String newUrl = client.ResponseHeaders["Location"];

            (entity as Entity.Base).load_from_url(newUrl);

            return entity;
        }



    }
}
