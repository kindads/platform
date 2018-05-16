using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Comun.Models.Entities
{
    [Table("CATEGORYSITE")]
    public class CategorySiteEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CategorySiteEntity()
        {
        }
        [Key, Column(Order = 0)]
        //[ForeignKey("CATEGORY")]
        public int CATEGORY_IdCategory { get; set; }
        [Key, Column(Order = 1)]
        //[ForeignKey("SITE")]
        public System.Guid SITEs_IdSite { get; set; }
    }
}
