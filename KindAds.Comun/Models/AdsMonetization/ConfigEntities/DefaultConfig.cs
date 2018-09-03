using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models
{
    public class DefaultConfig
    {
        public string Name { set; get; }

        public string Text { set; get; }

        public string Image { set; get; }

        public string JsId { set; get; }

        public string ClicBehavior { set; get; }

        public DefaultConfig()
        {
            Name = string.Empty;
            Text = string.Empty;
            Image = string.Empty;
            JsId = string.Empty;
            ClicBehavior = string.Empty;
        }
    }
}
