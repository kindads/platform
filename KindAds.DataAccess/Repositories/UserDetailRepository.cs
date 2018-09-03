using KindAds.Common.Models;
using KindAds.Comun.Models.Entities;
using KindAds.DataAccess.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.DataAccess.Repositories
{
   

    public class UserDetailRepository : DGenericRepository<UserDetailEntity>
    {
        public UserDetailRepository(string connectionStringName = "KindAdsDefaultConnection") : base(connectionStringName)
        {
            LoadProfileMapper(typeof(UserDetailMapper));
        }

        public UserDetailEntity GetByUserById(string UserId)
        {
            return GetAll().Where(p => p.UserId == UserId).SingleOrDefault();
        }
    }
}
