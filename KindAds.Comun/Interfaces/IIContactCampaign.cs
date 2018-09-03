using KindAds.Common.Interfaces;
using KindAds.Common.Utils.Partners.Mail.Aweber.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Common.Interfaces
{
    public interface IIContactCampaign
    {
        Campaign campaign { set; get; }
    }

    //Proximo proveeedor (prueba de compatibilidad generica)
    public interface IEWeberCampaign
    {
        Campaign campaign { set; get; }

        IRequestSettings<IIContactRequest> config { set; get; }
    }

    public interface ICampaign
    {
        IRequestSettings<IRequest> config { set; get; }
    }
}
