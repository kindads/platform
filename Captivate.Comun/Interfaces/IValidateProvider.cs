using Captivate.Comun.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Comun.Interfaces
{
    public interface IValidateProvider
    {
        HttpStatusCode StatusCode { set; get; }

         string Message { set; get; }

         bool IsValid { set; get; }

         EnumMailProviders enumMailProvider { set; get; }

        string Validate(bool IsValid, HttpStatusCode Status);

        void MakeMessage();
        
    }
}
