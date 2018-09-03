using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models.Entities
{
    public class MoneyAdsSettingsEntity
    {
        public int Id { set; get; }
        public string JavascriptId { set; get; }

        public string AdsText { set; get; }

        public string UrlImage { set; get; } 

        public string Name { set; get; }
    }
}
