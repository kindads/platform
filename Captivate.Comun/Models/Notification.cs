using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Comun.Models
{
    public class Notification
    {
        public string Title { set; get; }

        public string Message { set; get; }

        public Guid IdUser { set; get; }

        public string Label { set; get; }

        public string IdCampaignExternal { set; get; }

        public string MailContent { set; get; }

        public Notification()
        {
            Label = string.Empty;
            Title = string.Empty;
            Message = string.Empty;
            IdCampaignExternal = string.Empty;
            MailContent = string.Empty;
        }
    }
}
