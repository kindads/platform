using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Common.Interfaces
{
    public interface ITrace
    {
        void Failure(string message);

        void Startup();

        void Endup();

        void Notify(string message);

        void Warning(string message);

        void Critical(string message);

        string MakeMessageException(Exception e, string metodo);

    }
}
