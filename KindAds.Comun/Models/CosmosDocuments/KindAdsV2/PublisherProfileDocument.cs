using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models.CosmosDocuments.KindAdsV2
{
    public class PublisherProfileDocument : KindAdsV2Document
    {
        public string UserId { set; get; }

        public string Name { set; get; }

        public string Description { set; get; }

        public string CountryBusinessInId { set; get; }

        public Guid ExperienceLevel { get; set; }

        public string IconUrl { set; get; }

        //public string PhotoUrl { set; get; }

        public PublisherProfileDocument()
        {
            UserId = string.Format("<NULL>");
            Name = string.Format("<NULL>");
            Description = string.Format("<NULL>");
            CountryBusinessInId = string.Format("<NULL>");
           
            IconUrl = string.Format("<NULL>");
            //PhotoUrl = string.Format("<NULL>");
        }
    }
}
