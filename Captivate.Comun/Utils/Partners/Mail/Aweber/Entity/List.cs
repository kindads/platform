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
using Captivate.Comun.Utils.Partners.Mail.Aweber.OAuth;

namespace Captivate.Comun.Utils.Partners.Mail.Aweber.Entity
{
    /// <summary>
    /// A single list for a given account.
    /// https://labs.aweber.com/docs/reference/1.0#list
    /// </summary>
    public class List : Base
    {

        public List(IAdapter adapter)
            : base(adapter)
        {

        }

        public BaseCollection<Entity.Campaign> campaigns()
        {
            String url = String.Format("{0}/campaigns", self_link);
            return Factory.BaseCollection<Entity.Campaign>.Build(url, JSON.Read(api.GetResponse(url)), api);
        }

        public BaseCollection<Entity.Campaign> campaigns(String next_link)
        {
            String url = next_link;
            return Factory.BaseCollection<Entity.Campaign>.Build(url, JSON.Read(api.GetResponse(url)), api);
        }

        public BaseCollection<Entity.Subscriber> subscribers()
        {
            String url = String.Format("{0}/subscribers", self_link);
            return Factory.BaseCollection<Entity.Subscriber>.Build(url, JSON.Read(api.GetResponse(url)), api);
        }

        public BaseCollection<Entity.Subscriber> subscribers(String next_link)
        {
            String url = next_link;
            return Factory.BaseCollection<Entity.Subscriber>.Build(url, JSON.Read(api.GetResponse(url)), api);
        }

        /// <summary>
        /// Campaigns owned by this list (A link to a campaign collection)
        /// </summary>
        public String campaigns_collection_link { get { return _campaigns_collection_link; } }
        private String _campaigns_collection_link = String.Empty;


        /// <summary>
        /// CustomFields used by this List (A link to a custom_field collection)
        /// </summary>
        public String custom_fields_collection_link { get { return _custom_fields_collection_link; } }
        private String _custom_fields_collection_link = String.Empty;

        /// <summary>
        /// Subscribers owned by list (A link to a subscriber collection)
        /// </summary>
        public String custom_subscribers_collection_link { get { return _subscribers_collection_link; } }
        private String _subscribers_collection_link = String.Empty;


        /// <summary>
        /// Name of the list
        /// </summary>
        public String name { get { return _name; } }
        private String _name = String.Empty;

        /// <summary>
        /// Number of currently subscribed Subscribers
        /// </summary>
        public Int32 total_subscribed_subscribers { get { return _total_subscribed_subscribers; } }
        private Int32 _total_subscribed_subscribers = -1;

        /// <summary>
        /// Number of Subscribers
        /// </summary>
        public Int32 total_subscribers { get { return _total_subscribers; } }
        private Int32 _total_subscribers = -1;

        /// <summary>
        /// Number of Subscribers that subscribed today
        /// </summary>
        public Int32 total_subscribers_subscribed_today { get { return _total_subscribers_subscribed_today; } }
        private Int32 _total_subscribers_subscribed_today = -1;

        /// <summary>
        /// Number of Subscribers that subscribed yesterday
        /// </summary>
        public Int32 total_subscribers_subscribed_yesterday { get { return _total_subscribers_subscribed_yesterday; } }
        private Int32 _total_subscribers_subscribed_yesterday = -1;

        /// <summary>
        /// Number of currently unconfirmed Subscribers
        /// </summary>
        public Int32 total_unconfirmed_subscribers { get { return _total_unconfirmed_subscribers; } }
        private Int32 _total_unconfirmed_subscribers = -1;

        /// <summary>
        /// Number of currently unsubscribed Subscribers
        /// </summary>
        public Int32 total_unsubscribed_subscribers { get { return _total_unsubscribed_subscribers; } }
        private Int32 _total_unsubscribed_subscribers = -1;

        /// <summary>
        /// Web form split tests configured for this list (A link to a web_form_split_test collection)
        /// </summary>
        public String web_form_split_tests_collection_link { get { return _web_form_split_tests_collection_link; } }
        private String _web_form_split_tests_collection_link = String.Empty;

        /// <summary>
        /// Web forms owned by this list (A link to a web_form collection)
        /// </summary>
        public String web_forms_collection_link { get { return _web_forms_collection_link; } }
        private String _web_forms_collection_link = String.Empty;

    }
}
