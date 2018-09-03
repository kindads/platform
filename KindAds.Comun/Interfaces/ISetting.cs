using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Interfaces
{
    public interface ISetting
    {
        string Name { set; get; }

        string Value { set; get; }

        string ProductId { set; get; }

        string Id { set; get; }
    }
}
