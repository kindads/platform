using KindAds.Common.Interfaces;
using KindAds.Common.Models;
using KindAds.Common.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DapperExtensions;
using System.Data;
using KindAds.DataAccess.Mappers;

namespace KindAds.DataAccess
{
    [Obsolete]
    public class AspNetUserRepository : DGenericRepository<AspNetUserEntity>, IAspNetUserRepository
    {
        public AspNetUserRepository(string connectionStringName = "KindAdsDefaultConnection") : base(connectionStringName)
        {
            LoadProfileMapper(typeof(AspNetUserMapper));
        }

        public AspNetUserEntity GetByEmail(string email)
        {
            using (var cnn = DBConnection)
            {
                cnn.Open();
                return cnn.Query<AspNetUserEntity>("sp_AspNetUsers_FindByEmail", new { email = email },commandType: CommandType.StoredProcedure).SingleOrDefault();
            }
            
        }

        public AspNetUserEntity GetById(Guid id)
        {
            return FindById(id);
        }


    }
}
