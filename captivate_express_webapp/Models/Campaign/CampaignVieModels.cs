using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using captivate_express_webapp.Models.Core;

namespace captivate_express_webapp.Models.Campaign
{
  public class SortCampaignViewModel
  {
    public List<CAMPAIGN> ListCampaigns { set; get; }
  }

  public class TableCampaignViewModel : TableViewModel
  {
    public List<CAMPAIGN> ListCampaigns { set; get; } = new List<CAMPAIGN>();
  }

}
