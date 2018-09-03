using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Common.Models
{
    public class GoogleTagManagerValidation
    {
        public string Token { set; get; }
        public string Script { set; get; }

        public List<string> ScriptByRows { set; get; }

        public GoogleTagManagerValidation()
        {
            Token = string.Empty;
            Script = string.Empty;
            ScriptByRows = new List<string>();
        }
    }
}
