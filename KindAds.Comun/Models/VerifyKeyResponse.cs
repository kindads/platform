using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models
{
    public class VerifyKeyResponse
    {
        public int total_subscribers { get; set; }
        public string url { get; set; }

        public string[] errors { get; set; }
    }
}
