using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models.ViewModel
{
    public class MoneyAdsViewModel
    {
        public KindAds.Comun.Models.ViewModel.DefaultAds defaultAd { set; get; }

        public List<Comun.Models.DefaultAds> defaultAds { set; get; }

        public int PageSize { set; get; }

        public int TotalRecord { set; get; }

    }

    public class DefaultAds
    {
        [Display(Name = "Javascript id")]
        public string javascriptId { set; get; }

        [Display(Name ="Text")]
        [Required]
        public string text { set; get; }

        public string image { set; get; }

        [Display(Name = "Ads Types")]
        public string typeSelected { set; get; }

        [Display(Name = "Name ad")]
        public string name { set; get; }

        [Display(Name = "Site")]
        public string IdSite { set; get; }
    }
       
}
