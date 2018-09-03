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
using KindAds.Common.Utils.Partners.Mail.Aweber.OAuth;
using System.IO;

namespace KindAds.Common.Utils.Partners.Mail.Aweber.Entity
{
    /// <summary>
    /// A single subscriber to a given list.
    /// https://labs.aweber.com/docs/reference/1.0#subscriber
    /// </summary>
    public class Subscriber : Base
    {
        public Subscriber(IAdapter adapter)
            : base(adapter)
        {
        }
        
        /// <summary>
        /// Ad Tracking
        /// </summary>
        public String ad_tracking { get { return _ad_tracking; } set { _ad_tracking = value; OnPropertyChanged("ad_tracking"); } } //writeable
        private String _ad_tracking = String.Empty;

        /// <summary>
        /// Area Code (Based on ip_address)
        /// </summary>
        public String area_code { get { return _area_code; } }
        private String _area_code = String.Empty;

        /// <summary>
        /// City (Based on ip_address)
        /// </summary>
        public String city { get { return _city; } }
        private String _city = String.Empty;

        /// <summary>
        /// Country (Based on ip_address)
        /// </summary>
        public String country { get { return _country; } }
        private String _country = String.Empty;

        /// <summary>
        /// Custom subscriber data (writeable)
        /// This has custom subscriber data. The keys are defined at the list level and cannot be modified via this attribute. When updating these items all keys must be passed, with the desired values.
        /// </summary>
        public String custom_fields { get { return _custom_fields; } set { _custom_fields = value; OnPropertyChanged("custom_fields"); } }
        private String _custom_fields = String.Empty;

        /// <summary>
        /// DMA Code (Based on ip_address)
        /// </summary>
        public String dma_code { get { return _dma_code; } }
        private String _dma_code = String.Empty;

        /// <summary>
        /// Email Address
        /// </summary>
        public String email { get { return _email; } set { _email = value; OnPropertyChanged("email"); } } // writeable
        private String _email = String.Empty;

        /// <summary>
        /// ip address 
        /// </summary>
        public String ip_address { get { return _ip_address; } }
        private String _ip_address = String.Empty;

        /// <summary>
        /// Is the subscribers email confirmed? 
        /// </summary>
        public bool is_verified { get { return _is_verified; } }
        private bool _is_verified = false;

        /// <summary>
        /// Message number of the last followup sent (writeable)
        /// </summary>
        public int? last_followup_message_number_sent { get { return _last_followup_message_number_sent; } set { _last_followup_message_number_sent = value; OnPropertyChanged("last_followup_message_number_sent"); } }
        private int? _last_followup_message_number_sent = null;

        /// <summary>
        /// When the last followup message was sent
        /// </summary>
        public DateTime? last_followup_sent_at { get { return _last_followup_sent_at; } }
        private DateTime? _last_followup_sent_at = null;

        /// <summary>
        /// A link to the followup_campaign resource. 
        /// 
        /// Last Followup Message Sent
        ///
        /// This is not guaranteed to be the last message sent if last_followup_message_number_sent has been modified since the last message was sent or if the followup sequence has changed since the last message was sent.
        ///
        /// Use last_followup_message_number_sent to modify this value.
        /// </summary>
        public String last_followup_sent_link { get { return _last_followup_sent_link; } }
        private String _last_followup_sent_link = String.Empty;

        /// <summary>
        ///  Latitude (Based on ip_address) 
        /// </summary>
        public String latitude { get { return _latitude; } }
        private String _latitude = String.Empty;

        /// <summary>
        /// Longitude (Based on ip_address) 
        /// </summary>
        public String longitude { get { return _longitude; } }
        private String _longitude = String.Empty;

        /// <summary>
        /// misc notes (writeable)
        /// </summary>
        public String misc_notes { get { return _misc_notes; } set { _misc_notes = value; OnPropertyChanged("misc_notes"); } }
        private String _misc_notes = String.Empty;

        /// <summary>
        /// Subscriber Name (writeable)
        /// </summary>
        public String name { get { return _name; } set { _name = value; OnPropertyChanged("name"); } }
        private String _name = String.Empty;

        /// <summary>
        /// Postal Code (Based on ip_address)
        /// </summary>
        public String postal_code { get { return _postal_code; } }
        private String _postal_code = String.Empty;

        /// <summary>
        /// Region (Based on ip_address) 
        /// </summary>
        public String region { get { return _region; } }
        private String _region = String.Empty;

        /// <summary>
        /// Status of the subscriber (writeable)
        /// </summary>
        public String status { get { return _status; } set { _status = value; OnPropertyChanged("status"); } }
        private String _status = String.Empty;

        /// <summary>
        /// Time subscriber signed up.
        /// </summary>
        public DateTime subscribed_at { get { return _subscribed_at; } }
        private DateTime _subscribed_at = DateTime.MinValue;

        /// <summary>
        /// Subscription method 
        /// </summary>
        public String subscription_method { get { return _subscription_method; } }
        private String _subscription_method = String.Empty;

        /// <summary>
        /// Subscription url 
        /// </summary>
        public String subscription_url { get { return _subscription_url; } }
        private String _subscription_url = String.Empty;

        /// <summary>
        /// How subscriber unsubscribed 
        /// </summary>
        public String unsubscribe_method { get { return _unsubscribe_method; } }
        private String _unsubscribe_method = String.Empty;

        /// <summary>
        /// When subscriber unsubscribed 
        /// </summary>
        public DateTime? unsubscribed_at { get { return _unsubscribed_at; } }
        private DateTime? _unsubscribed_at = null;

        /// <summary>
        /// When the subscriber confirmed.
        /// </summary>
        public DateTime? verified_at { get { return _verified_at; } }
        private DateTime? _verified_at = null;

        /// <summary>
        /// Will save or create the subscriber
        /// depending on whether it is a new subscriber or one retrived
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
      
            /// Did not put this in Base as many entities do no posses the ability to save themselves.
            /// This could possibily be split into another base class if there are more entities added to this sdk that support saving. 

            bool success = false;

            // Build Post Data
            OAuth.Request request = api.BuildRequest();

            String url = self_link;

            // Build custom parameters for this OAuth Request
            SortedList<String, String> parameters = new SortedList<string, string>();

            // Build request
            request.Build(parameters, url, "PATCH");

            // custom built json, otherwise serializing the entity will result in all fields been serialized
            // and we are only doing a PATCH (updating only fields that have changed)
            String json = "{";

            bool first = true;

            // Build new subscriber entity only with modified entities
            foreach (String dirtyField in get_dirty())
            {

                String pre = ",";
                if (first)
                {
                    first = false;
                    pre = String.Empty;
                }

                json += String.Format("{2}\"{0}\":\"{1}\"", dirtyField, Convert.ToString(this.GetType().GetProperty(dirtyField).GetValue(this, null)), pre);

            }

            json += "}";

            WebClient client = new WebClient();


            try
            {
                // Make the PATCH request
                WebRequest webRequest = WebRequest.Create(url + "?" + request.Parameters);

                webRequest.ContentType = "application/json";

                webRequest.Method = "PATCH";

                Stream dataStream = webRequest.GetRequestStream();

                byte[] byteArray = System.Text.UTF8Encoding.UTF8.GetBytes(json);

                dataStream.Write(byteArray, 0, byteArray.Length);

                dataStream.Close();

                WebResponse response = webRequest.GetResponse();


                success = true;
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return success;
        }


        /// <summary>
        /// Will delete this subscriber from the list and account
        /// </summary>
        /// <returns></returns>
        public bool delete()
        {
            bool success = false;

            // Build Post Data
            OAuth.Request request = api.BuildRequest();

            String url = self_link;

            // Build custom parameters for this OAuth Request
            SortedList<String, String> parameters = new SortedList<string, string>();

            // Build request
            request.Build(parameters, url, "DELETE");
            
            try
            {
                WebRequest webRequest = WebRequest.Create(url + "?" + request.Parameters);

                webRequest.ContentType = "application/json";

                webRequest.Method = "DELETE";

                using (WebResponse response = webRequest.GetResponse())
                {

                }

                success = true;
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return success;
        }


    }
}
