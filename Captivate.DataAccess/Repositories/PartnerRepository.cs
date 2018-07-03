using Captivate.Common.Models;
using Captivate.Common.Models.Entities;
using Captivate.DataAccess.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.DataAccess
{
   
    public class PartnerRepository : DGenericRepository<PartnerEntity>
    {
        public PartnerRepository(string connectionStringName = "KindAdsDefaultConnection") : base(connectionStringName)
        {
            LoadProfileMapper(typeof(PartnerMapper));
        }

      
    }
}
