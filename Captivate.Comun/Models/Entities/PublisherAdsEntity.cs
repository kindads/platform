using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Comun.Models.Entities
{
    public class PublisherAdsEntity
    {
        public int Id { set; get; }

        public Guid IdUser { set; get; }

        public Guid IdSite { set; get; }

        public int MoneyAdsId { set; get; }

        public bool IsActive { set; get; }

        public bool IsAlive { set; get; }
    }
}
