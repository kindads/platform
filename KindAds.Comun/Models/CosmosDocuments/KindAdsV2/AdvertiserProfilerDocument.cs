using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models.CosmosDocuments.KindAdsV2
{
    public class AdvertiserProfileDocument : KindAdsV2Document
    {
        public string UserId { set; get; }

        public string Title { set; get; }

        public string Tagline { set; get; }

        public string  WebSiteUrl { set; get; }

        public int MounthFounded { set; get; }

        public int YearFounded { set; get; }

        public string PeopleInYourBusiness { set; get; }

        public string CategoryId { set; get; }

        public string SubCategoryId { set; get; }

        public string CountryBusinessInId { set; get; }

        public string ExperienceId { set; get; }


        public string AdvertiserNeeds { set; get; }

        public string DocumentInsightUrl { set; get; }

        public string PhotoUrl { get; set; }

        public string IconUrl { set; get; }

        public string OtherRelevantInfo { set; get; }

        public AdvertiserProfileDocument()
        {
            UserId = string.Format("<NULL>");
            Title = string.Empty; // string.Format("e.g. Your Company Name");
            Tagline = string.Empty;// string.Format("e.g. New SaaS product in hypergrowth");
            WebSiteUrl = string.Empty;// string.Format("e.g. www.yoursite.com");
            MounthFounded = 0;
            YearFounded = 0;
            PeopleInYourBusiness = "<NULL>";
            CountryBusinessInId = string.Format("<NULL>");
            ExperienceId = string.Format("<NULL>");
            AdvertiserNeeds = string.Empty; // string.Format("Write out your description here....");
            DocumentInsightUrl = string.Format("<NULL>");
            PhotoUrl = string.Format("<NULL>");
            IconUrl = string.Format("<NULL>");
            OtherRelevantInfo = string.Format("<NULL>");
        }
    }
}
