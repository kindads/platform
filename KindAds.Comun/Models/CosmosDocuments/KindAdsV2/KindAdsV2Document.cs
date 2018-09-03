using KindAds.Comun.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models.CosmosDocuments.KindAdsV2
{
    public class KindAdsV2Document
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string RegisterDate { set; get; }

        public KindAdsV2Document()
        {
            RegisterDate = GetMexicanTime();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public DateTime GetMexicanDateTime()
        {
            return DateUtils.GetMexicanDateTimeNow();
        }

        public string GetMexicanTime()
        {
            return GetMexicanDateTime().ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
