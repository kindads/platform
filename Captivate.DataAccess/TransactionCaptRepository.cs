using Captivate.Comun.Interfaces;
using Captivate.Comun.Models;
using Captivate.Comun.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.DataAccess
{
    public class TransactionCaptRepository : AGenericRepository<KindadsContext, TransactionsCaptEntity>, ITransactionCaptRepository
    {
    }
}
