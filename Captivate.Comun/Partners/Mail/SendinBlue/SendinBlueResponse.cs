using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Common.Partners.Mail.SendinBlue
{
    public class SendinBlueResponse
    {
        public string code { set; get; }
        public string message { set; get; }
        public List<string> data { set; get; }

    }
}
