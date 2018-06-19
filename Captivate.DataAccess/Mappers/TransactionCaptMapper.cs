using Captivate.Comun.Models.Entities;
using DapperExtensions.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.DataAccess.Mappers
{
    public class TransactionCaptMapper : ClassMapper<TransactionsCaptEntity>
    {
        public TransactionCaptMapper()
        {
            //use a custom schema
            Schema("dbo");
            base.Table(@"TRANSACTIONS_CAPT");

            //Ignore this property entirely
            Map(x => x.IdTransaction).Key(KeyType.Identity);
            Map(x => x.TRANSACTION_TYPE).Ignore();
            Map(x => x.CAMPAIGN).Ignore();
            //optional, map all other columns
            AutoMap();
        }

    }

}
