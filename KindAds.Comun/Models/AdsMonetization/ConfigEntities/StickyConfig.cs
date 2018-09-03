using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models
{
    public class StickyConfig
    {
        public string Name { set; get; }

        public string Text { set; get; }

        public string JsId { set; get; }

        public string CloseHtml { set; get; }

        public string AdsHtml { set; get; }

        public StickyConfig()
        {
            Name = string.Empty;
            Text = string.Empty;
            JsId = Guid.NewGuid().ToString();
            CloseHtml = string.Empty;
            AdsHtml = string.Empty;
        }
    }
}
