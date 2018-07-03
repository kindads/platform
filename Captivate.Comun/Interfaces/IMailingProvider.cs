using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Common.Interfaces
{
  public interface IMailingProvider<TCampaign> 
  {

    TCampaign campaign { set; get; }

   
   
  }

}
