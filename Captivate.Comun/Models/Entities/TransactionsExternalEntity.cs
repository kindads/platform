using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Common.Models.Entities
{

    public class TransactionsExternalEntity
    {

        public System.Guid IdTransactionext { get; set; }
        public string HashFrom { get; set; }
        public string HashTo { get; set; }
        public double Amount { get; set; }
        public string RegisterDate { get; set; }
        public string HashTransaction { get; set; }
        public string Gas { get; set; }


        public short TRANSACTION_TYPE_IdTransactionType { get; set; }

        public virtual TransactionTypeEntity TRANSACTION_TYPE { get; set; }
    }
}
