﻿using Captivate.Comun.Models;
using Captivate.Comun.Models.Entities;
using Captivate.DataAccess.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.DataAccess
{
    

    public class AzureSubscriptionRepository : DGenericRepository< AzureSupcriptionEntity>
    {

        public AzureSubscriptionRepository()
        {
            LoadProfileMapper(typeof(AzureSubscriptionMapper));
        }







    }
}