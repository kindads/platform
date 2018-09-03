using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KindAds.Models.Core
{
  public class FileUpload
  {
    public byte[] FileData { get; set; }
    public string Filextension { get; set; }

    public string HtmlItem { set; get; }
  }
}
