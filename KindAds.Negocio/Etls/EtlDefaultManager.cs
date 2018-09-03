using KindAds.Azure;
using KindAds.Common.Interfaces;
using KindAds.Comun.Interfaces;
using KindAds.Comun.Models.AdsMonetization;
using KindAds.Comun.Models.CosmosDocuments;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Negocio.Etls
{
    public class EtlDefaulClickManager : EtlGeneric<DefaultClicInfo>
    {
        public EtlDefaulClickManager() :
            base(ConfigurationManager.AppSettings["CosmosDatabaseName"],
                ConfigurationManager.AppSettings["CosmosDefaultClicCollectionName"])
        { }
    }

    public class EtlDefaulImpressionManager : EtlGeneric<CosmosDbDocument>
    {
        public EtlDefaulImpressionManager() :
            base(ConfigurationManager.AppSettings["CosmosDatabaseName"],
                ConfigurationManager.AppSettings["CosmosDefaultImpressionCollectionName"])
        { }
    }
}
