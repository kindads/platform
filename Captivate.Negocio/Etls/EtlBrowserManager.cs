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
    public class EtlBrowserManager : EtlGeneric<BrowserInfo>
    {
        public EtlBrowserManager() :
            base(ConfigurationManager.AppSettings["CosmosDatabaseName"],
                ConfigurationManager.AppSettings["CosmosBrowserCollectionName"])
        { }
    }
}
