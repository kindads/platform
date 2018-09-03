using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models.CosmosDocuments.KindAdsV2
{
    public class AudienceChannelDocument : KindAdsV2Document
    {

        public string Name { set; get; }
        public string AudienceId { set; get; }
        public string ChannelId { set; get; }
        public bool Visibility { set; get; }
        public int AdvertiserExperience { set; get; }
        public int KindAdsScore { set; get; }
        public string TagLine { set; get; }
        public string ImageUrl { set; get; }
        public string Description { set; get; }
        public bool IsPremium { set; get; }
        public bool IsActive { set; get; }
        public string ProductId { set; get; }
        public bool IsDefaultImage { set; get; }
        public bool IsDefaultDescription { set; get; }


        public double Price { set; get; }
        public string Detail { get; set; }

        public DateTime LastProviderDataUpdated { get; set; }

        public string providerName { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string ChannelType { get; set; }
        public string Rating { get; set; }
        public string Score { get; set; }

        public string AudienceName { get; set; }
        public string ProductProviderId { get; set; }

        public AudienceChannelDocument()
        {
            Name = string.Empty;
            AudienceId = string.Empty;
            ChannelId = string.Empty;
            Visibility = true;
            AdvertiserExperience = 0;
            KindAdsScore = 0;
            TagLine = string.Empty;
            ImageUrl = string.Empty;
            Description = string.Empty;
            Price = 0;
            IsPremium = false;
            IsActive = false;
            ProductId = string.Empty;
            IsDefaultImage = false;
            IsDefaultDescription = false;
            Detail = string.Empty;
            Category = string.Empty;
            SubCategory = string.Empty;
            ChannelType = string.Empty;
            Rating = string.Empty;
            Score = string.Empty;
        }

    }
}
