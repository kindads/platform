using KindAds.Common.Interfaces;
using KindAds.Common.Models;
using KindAds.Common.Models.Entities;
using KindAds.DataAccess.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.DataAccess
{
    public class TransactionCaptRepository : DGenericRepository<TransactionsCaptEntity>, ITransactionCaptRepository
    {
        public TransactionCaptRepository(string connectionStringName = "KindAdsDefaultConnection") : base(connectionStringName)
        {
            LoadProfileMapper(typeof(TransactionCaptMapper));
        }
    }
}
