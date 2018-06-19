using Captivate.Comun.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Comun.Models
{
    public class CreateSite
    {
        [Display(Name = "Website Name")]
        [Required(ErrorMessage = "Site Name is required")]
        public string Name { set; get; }

        [Display(Name = "Website Url")]
        [Required(ErrorMessage = "Site Url is required")]
        public string URL { set; get; }

        [Display(Name = "Categories")]
       
        public short CategoryTypeSelect { get; set; }
        public List<CategoryEntity> ListCategory { get; set; }
    }
}
