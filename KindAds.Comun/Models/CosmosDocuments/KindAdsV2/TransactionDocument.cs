using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models.CosmosDocuments.KindAdsV2
{
    public class TransactionDocument : KindAdsV2Document
    {
        public string UserId { set; get; }

        public string AffectedDatabase { set; get; }

        public string AffectedCollection { set; get; }

        public string Operation { set; get; }

        public TransactionDocument()
        {
            UserId = string.Format("<NULL>");
            AffectedCollection = string.Format("<NULL>");
            AffectedDatabase = string.Format("<NULL>");
            Operation = string.Format("<NULL>");
        }
    }
}
