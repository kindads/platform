using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KindAds.Models.Core
{

  public class TableViewModel
  {
   public int PageSize { get; set; }
   public int TotalRecord { get; set; }
   public int NoOfPages { get; set; }
  }
}
