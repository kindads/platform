using KindAds.Common.Models;
using KindAds.Common.Models.Entities;
using KindAds.DataAccess.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.DataAccess
{
    

    public class AzureSubscriptionRepository : DGenericRepository< AzureSupcriptionEntity>
    {

        public AzureSubscriptionRepository()
        {
            LoadProfileMapper(typeof(AzureSubscriptionMapper));
        }







    }
}
