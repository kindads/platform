using KindAds.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Interfaces
{
    public interface IKindAdsV2DataAccess
    {
         string EndpointUrl { set; get; }
         string PrimaryKey { set; get; }
         ITrace telemetria { set; get; }
    }
}
