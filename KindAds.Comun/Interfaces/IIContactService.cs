using KindAds.Common.Interfaces;
using KindAds.Common.Partners.IContact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Common.Interfaces
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
