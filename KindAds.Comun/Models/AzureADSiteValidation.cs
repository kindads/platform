using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Common.Models
{
    public class AzureADSiteValidation
    {
        [Display( Name ="Client App Id")]       
        public string ClientAppId { set; get; }

        [Display(Name = "Subscription Id")]
        public string SubscriptionId { set; get; }

        [Display(Name = "Tenant Id")]
        public string TenantId { set; get; }

        [Display(Name = "App Key")]
        public string AppKey { set; get; }

        public AzureADSiteValidation()
        {
            ClientAppId = string.Empty;
            SubscriptionId = string.Empty;
            TenantId = string.Empty;
            AppKey = string.Empty;
        }
    }
}
