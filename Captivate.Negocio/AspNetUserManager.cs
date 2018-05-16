using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Captivate.Business;
using Captivate.Common.Interfaces;
using Captivate.Comun.Models.Entities;
using Captivate.DataAccess;

namespace Captivate.Negocio
{
    public class AspNetUserManager : ITelemetria
    {
        public ITrace telemetria { set; get; }
        public AspNetUserRepository repository { set; get; }

        public AspNetUserManager()
        {

        }
    }
}
