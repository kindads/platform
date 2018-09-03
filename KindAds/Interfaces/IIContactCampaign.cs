using captivate_express_webapp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace captivate_express_webapp.Interfaces
{
  public interface IIContactCampaign 
  { 
    CAMPAIGN campaign { set; get; }
  }

  //Proximo proveeedor (prueba de compatibilidad generica)
  public interface IEWeberCampaign
  {
    PRODUCT   campaign { set; get; }

    IRequestSettings<IIContactRequest> config { set; get; }
  }

  public interface ICampaign
  {
    IRequestSettings<IRequest> config { set; get; }
  }
}
