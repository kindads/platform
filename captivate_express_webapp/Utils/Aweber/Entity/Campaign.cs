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

namespace Aweber.Entity
{
    /// <summary>
    /// A single campaign for a given list.
    /// https://labs.aweber.com/docs/reference/1.0#campaign
    /// </summary>
    public class Campaign : Base
    {
        public Campaign(IAdapter adapter)
            : base(adapter)
        {

        }

        /// <summary>
        ///  Links in this message (link to link collection)
        /// </summary>
        public String links_collection_link { get { return _links_collection_link; } }
        private String _links_collection_link = String.Empty;

        /// <summary>
        /// Campaign messages sent (link to message collection)
        /// </summary>
        public String messages_collection_link { get { return _messages_collection_link; } }
        private String _messages_collection_link = String.Empty;

        /// <summary>
        /// Click tracking enabled? 
        /// </summary>
        public Boolean click_tracking_enabled { get { return _click_tracking_enabled; } }
        private Boolean _click_tracking_enabled = false;

        /// <summary>
        /// Type of message. (Plain, HTML, or both) 
        /// </summary>
        public String content_type { get { return _content_type; } }
        private String _content_type = String.Empty;

        /// <summary>
        /// Spam assassin score.
        /// </summary>
        public String spam_assassin_score { get { return _spam_assassin_score; } }
        private String _spam_assassin_score = String.Empty;

        /// <summary>
        /// Subject of message.
        /// </summary>
        public String subject { get { return _subject; } }
        private String _subject = String.Empty;

        /// <summary>
        /// Total clicks of this message
        /// </summary>
        public Int32 total_clicks { get { return _total_clicks; } }
        private Int32 _total_clicks = -1;

        /// <summary>
        /// total # of messages opened.
        /// </summary>
        public Int32 total_opens { get { return _total_opens; } }
        private Int32 _total_opens = -1;

        /// <summary>
        /// total # of messages sent.
        /// </summary>
        public Int32 total_sent { get { return _total_sent; } }
        private Int32 _total_sent = -1;

        /// <summary>
        /// total # of spam complaints received.
        /// </summary>
        public Int32 total_spam_complaints { get { return _total_spam_complaints; } }
        private Int32 _total_spam_complaints = -1;

        /// <summary>
        /// total # of undeliverable messages.
        /// </summary>
        public Int32 total_undelivered { get { return _total_undelivered; } }
        private Int32 _total_undelivered = -1;

        /// <summary>
        /// total # of unsubscribes from this campaign. 
        /// </summary>
        public Int32 total_unsubscribes { get { return _total_unsubscribes; } }
        private Int32 _total_unsubscribes = -1;

        /// <summary>
        /// Is the campaign in the campaign archive? 
        /// </summary>
        public Boolean is_archived { get { return _is_archived; } }
        private Boolean _is_archived = false;

        /// <summary>
        /// Date/Time campaign was sent.
        /// </summary>
        public DateTime? sent_at { get { return _sent_at; } }
        private DateTime? _sent_at = null;

        /// <summary>
        /// Twitter account where broadcast was tweeted (A link to the integration resource.)
        /// </summary>
        public String twitter_account_link { get { return _twitter_account_link; } }
        private String _twitter_account_link = String.Empty;

        /// <summary>
        /// Followup sequence number
        /// </summary>
        public String message_interval { get { return _message_interval; } }
        private String _message_interval = String.Empty;

        /// <summary>
        /// Campaign messages sent. (A link to a message collection) 
        /// </summary>
        public Int32 message_number { get { return _message_number; } }
        private Int32 _message_number = -1;
        
        /// <summary>
        /// Whether a follow-up or broadcast campaign
        /// </summary>
        public String type { get { return _type; } }
        private String _type = String.Empty;
    }
}
