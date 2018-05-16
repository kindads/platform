using Captivate.Comun.Interfaces;
using captivate_express_webapp.Models.Partner;
using captivate_express_webapp.Models.Partner.IContact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace captivate_express_webapp.Interfaces
{
  public interface IIContactService<TCampaign> : IMailingProvider<TCampaign>
  {
    IResponse CreateSends( IContactPostSendsRequest requestBody, IContactRequest requestFrm);
    IResponse CreateMessage( IContactPostMessageRequest requestBody, IContactRequest requestFrm);
    IResponse CreateMessageToHttpClient(IContactPostMessageRequest requestBody, IContactRequest requestFrm);
    IValidateProvider ValidateApiToken(IContactRequest request);
    IResponse CreateCampaign( IContactPostCampaignRequest requestBody, IContactRequest requestFrm);

  }
}
