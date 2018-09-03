using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Common.Models.Entities
{ 

    public class TransactionsCaptEntity
    {

        public int IdTransaction { get; set; }
        public string HashFrom { get; set; }
        public string HashTo { get; set; }
        public string Amount { get; set; }
        public string BlockDate { get; set; }
        public string RegisterDate { get; set; }
        public string HashTransaction { get; set; }
        public string Gas { get; set; }

        public short TRANSACTION_TYPE_IdTransactionType { get; set; }

        public Guid CAMPAIGN_IdCampaign { set; get; }

        public virtual TransactionTypeEntity TRANSACTION_TYPE { get; set; }
        public virtual CampaignEntity CAMPAIGN { get; set; }
    }
}
