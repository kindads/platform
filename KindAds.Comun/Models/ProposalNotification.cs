using KindAds.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models
{
    public class ProposalNotification
    {
        public string registerDate { set; get; }

        public string message { set; get; }

        public string label { set; get; }

        public string idUser { set; get; }

        public ProposalNotification()
        {
            registerDate = DateTime.Now.ToString();
            message = string.Empty;
            label = NotificationLabels.Proposal;
            idUser = string.Empty;
        }
    }
}
