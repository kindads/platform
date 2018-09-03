using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Interfaces
{
    public interface IChannel<T>
    {
        IList<AudiencePropertieSetting> settings { set; get; }
        T CreateChannel(IList<AudiencePropertieSetting> settings, string emailProductId);
        string databaseName { set; get; }
        string collectionName { set; get; }
        bool AreSettingsValid { set; get; }
    }
}
