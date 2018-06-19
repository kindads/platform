using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Comun.Models
{
    public class ApplicationData
    {
        public string MailContent { set; get; }

        public string IdUser { set; get; }

        public ApplicationData()
        {
            MailContent = string.Empty;
            IdUser = string.Empty;
        }
    }
}
