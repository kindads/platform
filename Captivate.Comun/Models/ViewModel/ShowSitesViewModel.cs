using Captivate.Common.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Common.Models.ViewModel
{
    public class ShowSitesViewModel
    {
        public List<SiteEntity> ListSitesPending { get; set; }
        public List<SiteEntity> ListSitesVerify { get; set; }

        public int PageSize { get; set; }
        public int TotalRecord { get; set; }
        public int NoOfPages { get; set; }
    }
}
