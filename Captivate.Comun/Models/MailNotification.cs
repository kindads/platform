using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Comun.Models
{
    public class MailNotification
    {
        public MailMessage EMail { set; get; }

        public Notification notificacion { set; get; }

        public MailNotification()
        {
            EMail = new MailMessage();
            notificacion = new Notification();
        }
    }
}
