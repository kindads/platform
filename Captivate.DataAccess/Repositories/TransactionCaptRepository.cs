using Captivate.Common.Interfaces;
using Captivate.Common.Models;
using Captivate.Common.Models.Entities;
using Captivate.DataAccess.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.DataAccess
{
    public class TransactionCaptRepository : DGenericRepository<TransactionsCaptEntity>, ITransactionCaptRepository
    {
        public TransactionCaptRepository(string connectionStringName = "KindAdsDefaultConnection") : base(connectionStringName)
        {
            LoadProfileMapper(typeof(TransactionCaptMapper));
        }
    }
}
