using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Comun.Models
{
    public class GoogleTagManagerValidation
    {
        public string Token { set; get; }

        public GoogleTagManagerValidation()
        {
            Token = string.Empty;
        }
    }
}
