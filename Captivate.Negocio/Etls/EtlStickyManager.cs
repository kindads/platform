using Captivate.Azure;
using Captivate.Common.Interfaces;
using Captivate.Comun.Interfaces;
using Captivate.Comun.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Negocio.Etls
{
    public class EtlStickyClickManager : EtlGeneric<StickyClicInfo>
    {
        public EtlStickyClickManager() :
            base(ConfigurationManager.AppSettings["CosmosDatabaseName"],
                ConfigurationManager.AppSettings["CosmosStickyClicCollectionName"])
        { }
    }

    public class EtlStickyImpressionManager : EtlGeneric<StickyImpressionInfo>
    {
        public EtlStickyImpressionManager() :
            base(ConfigurationManager.AppSettings["CosmosDatabaseName"],
                ConfigurationManager.AppSettings["CosmosStickyImpressionCollectionName"])
        { }
    }
}
