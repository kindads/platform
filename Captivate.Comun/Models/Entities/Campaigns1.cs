using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Common.Models.Entities
{

    public class Campaigns1
    {
      
        public System.Guid IdCampaign { get; set; }
        public string Name { get; set; }
        public System.DateTime RegisterDate { get; set; }
        public string AdText { get; set; }
        public string AdURL { get; set; }
        public string AdImage { get; set; }
        public string UTM_Source { get; set; }
        public string UTM_Medium { get; set; }
        public string UTM_Campaign { get; set; }
      
        public string IdCampaign3rdParty { get; set; }
        
        public Nullable<System.DateTime> StartDate { get; set; }
        
        public Nullable<System.DateTime> EndDate { get; set; }


        #region FK
        //[ForeignKey("AspNetUser")]
        public string AspNetUser_Id { set; get; }

        //[ForeignKey("PRODUCT")]
        public Guid PRODUCT_IdProduct { set; get; }

        //[ForeignKey("CAT_CAMPAIGN_STATUS")]
        public Nullable<int> CAT_CAMPAIGN_STATUS_IdStatus { get; set; }
        #endregion


        //public virtual AspNetUserEntity AspNetUser { get; set; }
        //public virtual ProductEntity PRODUCT { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<TransactionsCaptEntity> TRANSACTIONS_CAPT { get; set; }
        //public virtual CatalogoCampaignStatusEntity CAT_CAMPAIGN_STATUS { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<CampaignSettingsEntity> CAMPAIGN_SETTINGS { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<CampaignChatEntity> CAMPAIGN_CHAT { get; set; }
    }

  
}
