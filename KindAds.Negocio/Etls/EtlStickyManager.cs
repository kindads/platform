using KindAds.Azure;
using KindAds.Common.Interfaces;
using KindAds.Comun.Interfaces;
using KindAds.Comun.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Negocio.Etls
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
