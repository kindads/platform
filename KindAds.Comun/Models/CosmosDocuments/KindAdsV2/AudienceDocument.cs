using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models.CosmosDocuments.KindAdsV2
{
    public class AudienceDocument : KindAdsV2Document
    {
        public string UrlProtocol { set; get; }
        public string Title { set; get; }
        public string Tagline { set; get; }

        [RegularExpression(@"[a-zA-Z]+.([a-zA-Z][a-zA-Z])+")]       
        public string WebSiteUrl { set; get; }
        public string Description { set; get; }
        public string CategoryId { set; get; }
        public string SubCategoryId { set; get; }
        public string YearFounded { set; get; }
        public string PeopleInYourBusiness { set; get; }
        public string CountryBusinessInId { set; get; }

        public string HowManyAdvertisers { set; get; }

        public bool Visibility { set; get; }

        public string ImageUrl { set; get; }

        public string IconUrl { set; get; }

        public bool Verified { get; set; }

        public string PublisherId { get; set; }

        public string VerificationString { get; set; }

        public bool IsPremium { set; get; }

        public bool IsActive { set; get; }

        public AudienceDocument()
        {
            Title = string.Empty;
            Tagline = string.Empty;
            WebSiteUrl = string.Empty;
            Description = string.Empty;
            CategoryId = string.Empty;
            YearFounded = string.Empty;
            CountryBusinessInId = string.Empty;
            Visibility = false;
            HowManyAdvertisers = string.Empty;
            ImageUrl = string.Empty;
            IconUrl = string.Empty;
            Verified = false;
            VerificationString = string.Empty;
            IsPremium = false;
            IsActive = false;
        }

    }
}
