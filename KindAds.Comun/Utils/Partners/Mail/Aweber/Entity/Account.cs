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
using KindAds.Common.Utils.Partners.Mail.Aweber.OAuth;

namespace KindAds.Common.Utils.Partners.Mail.Aweber.Entity
{
    /// <summary>
    /// A single account you have been authorized to access. 
    /// https://labs.aweber.com/docs/reference/1.0#account
    /// </summary>
    public class Account : Base
    {
        public Account(IAdapter adapter)
            : base(adapter)
        {

        }

        /// <summary>
        /// Third party service integrations for this account
        /// </summary>
        public String integrations_collection_link { get { return _integrations_collection_link; } }
        private String _integrations_collection_link = String.Empty;

        /// <summary>
        /// Active lists owned by this account
        /// </summary>
        public String lists_collection_link { get { return _lists_collection_link; } }
        private String _lists_collection_link = String.Empty;

        /// <summary>
        /// A collection of lists owned by this account
        /// </summary>
        /// <returns></returns>
        public BaseCollection<Entity.List> lists()
        {
            String url = String.Format("{0}accounts/{1}/lists", Settings.apiBase, id);
            return Factory.BaseCollection<Entity.List>.Build(url, JSON.Read(api.GetResponse(url)), api);
        }

        public BaseCollection<Entity.List> lists(String next_link)
        {
            String url = next_link;
            return Factory.BaseCollection<Entity.List>.Build(url, JSON.Read(api.GetResponse(url)), api);
        }
    }
}
