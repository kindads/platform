using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Comun.Models.CosmosDocuments
{
    public class CosmosDbDocument
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string IdUser { set; get; }

        public string IdSite { set; get; }

        public string Metric { set; get; }

        public string PrintTime { set; get; }

        public string Year { set; get; }

        public string Month { set; get; }

        public string Day { set; get; }

        public string Hour { set; get; }

        public string Type { set; get; }

        public string Ip { set; get; }

        public CosmosDbDocument()
        {
            Id = string.Empty;
            IdUser = string.Empty;
            IdSite = string.Empty;

            PrintTime = DateTime.Now.ToString();
            DateTime Now = DateTime.Now;
            Year = Now.Year.ToString();
            Month = Now.Month.ToString();
            Day = Now.Day.ToString();
            Hour = Now.Hour.ToString();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
