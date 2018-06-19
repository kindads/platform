using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Comun.Models.Entities
{
 
    public class ProductSettingsEntity
    {
        [Key]
        public System.Guid IdProductSetting { get; set; }
        public string SettingName { get; set; }
        public string SettingValue { get; set; }

        [ForeignKey("PRODUCT")]
        public System.Guid PRODUCT_IdProduct { get; set; }

        public virtual ProductEntity PRODUCT { get; set; }
    }
}
