﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Comun.Interfaces
{
    public interface IEmailProviders
    {
        string ValidateCampaign(string IdCampaig,string IdUser);
    }
}