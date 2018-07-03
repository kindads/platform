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
    public class EtlMobileManager : EtlGeneric<MobileInfo>
    {
        public EtlMobileManager() :
            base(ConfigurationManager.AppSettings["CosmosDatabaseName"],
                ConfigurationManager.AppSettings["CosmosMobileCollectionName"])
        { }
    }
}
