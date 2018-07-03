using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Common.Models
{
    public class SiteToken
    {
        public string Url { set; get; }

        public Guid SiteId { set; get; }

        public string Name { set; get; }
    }
}
