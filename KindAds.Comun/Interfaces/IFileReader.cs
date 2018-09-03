using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Common.Interfaces
{
    public interface IFileReader
    {
        string GetContent(string path);
    }
}
