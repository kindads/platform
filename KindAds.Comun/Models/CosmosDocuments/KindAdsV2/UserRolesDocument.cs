using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models.CosmosDocuments.KindAdsV2
{
    public class UserRolesDocument : KindAdsV2Document
    {
        public string UserId { set; get; }

        public string RoleId { set; get; }

        public UserRolesDocument()
        {
            UserId = string.Format("<NULL>");
            RoleId = string.Format("<NULL>");
        }
    }
}
