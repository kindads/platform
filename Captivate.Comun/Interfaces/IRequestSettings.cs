using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Common.Interfaces
{
  public interface IRequestSettings<T> where T : IRequest
  {
    string method { set; get; }
    string accept { set; get; }
    string contentType { set; get; }
    string authorization { set; get; }
    string data { set; get; }

    Dictionary<string,string> headers { set; get; }

    T mailingProvider { set; get; }
  }
}
