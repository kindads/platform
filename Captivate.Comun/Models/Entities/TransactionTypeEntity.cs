using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Comun.Models.Entities
{
    [Table("TRANSACTION_TYPE")]
    public class TransactionTypeEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TransactionTypeEntity()
        {
            this.TRANSACTIONS_CAPT = new HashSet<TransactionsCaptEntity>();
            this.TRANSACTIONS_EXTERNAL = new HashSet<TransactionsExternalEntity>();
        }

        [Key]
        public short IdTransactionType { get; set; }
        public string Name { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TransactionsCaptEntity> TRANSACTIONS_CAPT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TransactionsExternalEntity> TRANSACTIONS_EXTERNAL { get; set; }
    }
}
