using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Comun.Models.Entities
{
  

    public class AzureSupcriptionEntity
    {
        [Key]
        public Guid IdSite { set; get; }
        public string ClientAppId { set; get; }     
        public string SubscriptionId { set; get; }   
        public string TenantId { set; get; }      
        public string AppKey { set; get; }

        public AzureSupcriptionEntity()
        {
            ClientAppId = string.Empty;
            SubscriptionId = string.Empty;
            TenantId = string.Empty;
            AppKey = string.Empty;
        }
    }
}
