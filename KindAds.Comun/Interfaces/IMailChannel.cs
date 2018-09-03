using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Interfaces
{
    public interface IMailChannel : IChannel<IMailChannel>
    {
        bool SettingsAreValid(string emailProductId);
        bool SettingsAreValid();
        bool Save(string emailProductId);
        void LoadSettings(string emailProductId);
        string Send(string emailProductId);

        bool ValidateApiToken(string ApiToken);
    }
}
