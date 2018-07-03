using Captivate.Azure;
using Captivate.Common.Interfaces;
using Captivate.Comun.Interfaces;
using Captivate.Comun.Models.AdsMonetization;
using Captivate.Comun.Models.CosmosDocuments;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Negocio.Etls
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
