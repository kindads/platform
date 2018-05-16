using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Comun.Models.Entities
{
    [Table("CAT_CAMPAIGN_STATUS")]
    public class CatalogoCampaignStatusEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CatalogoCampaignStatusEntity()
        {
            this.CAMPAIGNs1 = new HashSet<CampaignEntity>();
        }

        [Key]
        public int IdStatus { get; set; }
        public string Descripcion { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CampaignEntity> CAMPAIGNs1 { get; set; }
    }
}
