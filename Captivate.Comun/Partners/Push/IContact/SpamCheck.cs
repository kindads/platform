using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Captivate.Negocio.Partners.IContact
{
  public class SpamCheck
  {
    public string rawScore { set; get; }

    public List<SpamDetail> spamDetails { set; get; }

  }
}
