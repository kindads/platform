using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace captivate_express_webapp.Models.Partner.IContact
{
  public class SpamCheck
  {
    public string rawScore { set; get; }

    public List<SpamDetail> spamDetails { set; get; }

  }
}
