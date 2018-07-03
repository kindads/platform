using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Common.Models.Entities
{

    public class CampaignChatEntity
    {

        public System.Guid IdCampaignMessage { get; set; }

        public System.Guid CAMPAIGN_IdCampaign { get; set; }

        public string AspNetUser_IdCreator { get; set; }
        public string CampaignChatMessage { get; set; }
        public System.DateTime RegisterDate { get; set; }
        public Nullable<int> CampaignChatStatus { get; set; }

        public virtual AspNetUserEntity AspNetUser { get; set; }
        public virtual CampaignEntity CAMPAIGN { get; set; }
    }
}
