using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Common.Models.Entities
{

    public class CategorySiteEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CategorySiteEntity()
        {
        }

        //[ForeignKey("CATEGORY")]
        public int CATEGORY_IdCategory { get; set; }

        //[ForeignKey("SITE")]
        public System.Guid SITEs_IdSite { get; set; }
    }
}
