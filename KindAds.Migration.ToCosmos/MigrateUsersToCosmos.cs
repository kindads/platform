using KindAds.Business.Managers;
using KindAds.Common.Models.Entities;
using KindAds.Comun.Enums;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using KindAds.Negocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using KindAds.Comun.Interfaces;

namespace KindAds.Migration.ToCosmos
{
    public class MigrateUsersToCosmosETL : IEtlManager
    {
        private readonly CosmosIdentityManager _cosmosIdentityManager;
        private readonly List<RoleDocument> roles;

        public MigrateUsersToCosmosETL()
        {
            _cosmosIdentityManager = new CosmosIdentityManager();
            roles = _cosmosIdentityManager.GetRoles();
        }

        public (List<AspNetUser> sqlUsers, List<ApplicationUser> cosmosUsers) Extract()
        {
            List<AspNetUser> sqlUsers;
            List<ApplicationUser> cosmosUsers = _cosmosIdentityManager.GetUsers().ToList();

            using (var ctx = new Entities())
            {
                sqlUsers = ctx.AspNetUsers.Include(u=>u.AspNetRoles).Where(uf => uf.AspNetRoles.Count > 0).ToList();
            }

            return (sqlUsers, cosmosUsers);
        }

        /// <summary>
        /// Retorna usuarios de cosmos que se deben agregar la BD
        /// </summary>
        /// <param name="sqlUsers"></param>
        /// <param name="cosmosUsers"></param>
        /// <returns></returns>
        public List<ApplicationUser> Transform(List<AspNetUser> sqlUsers, List<ApplicationUser> cosmosUsers)
        {
            List<AspNetUser> newUsersInSql = sqlUsers.Where(u => !cosmosUsers.Any(cu => cu.UserName.ToLower() == u.UserName.ToLower())).ToList();
            List<ApplicationUser> applicationUsers = newUsersInSql.Select(u => SetRol(u.AspNetRoles.ToList(), MapUser(u))).ToList();
            return applicationUsers;
        }

        private ApplicationUser MapUser (AspNetUser aspnetUser)
        {
            return new ApplicationUser()
            {
                AccessFailAccount = 0,
                Email = aspnetUser.Email,
                EmailConfirmed = true,//aspnetUser.EmailConfirmed,
                Hometown = string.Empty,
                IsMetamask = false,
                LockoutEnabled = true,
                LockoutEndDateUtc = null,
                PasswordHash = aspnetUser.PasswordHash,
                PhoneNumber = aspnetUser.PhoneNumber,
                PhoneNumberConfirmed = aspnetUser.PhoneNumberConfirmed,
                SecurityStamp = aspnetUser.SecurityStamp,
                UserName = aspnetUser.UserName,
                TokenAddress = string.Empty,
                TwoFactorEnabled = aspnetUser.TwoFactorEnabled,
                WalletAddress = string.Empty,
                IsPremium = false,
                Roles = roles,
                Id = Guid.NewGuid().ToString()
            };
        }
 
        private ApplicationUser SetRol(List<AspNetRole> oldRoles, ApplicationUser user)
        {
            List<RoleDocument> defaultRoles = _cosmosIdentityManager.GetRoles();
            AspNetRole primaryRole = oldRoles.FirstOrDefault();
            switch(primaryRole.Name)
            {
                case "Publisher":
                    user.PrimaryRolId = user.LastRolId = Convert.ToInt32(defaultRoles.FirstOrDefault(r => r.Name == "Publisher").Id);
                    user.SecondaryRolId = Convert.ToInt32(defaultRoles.FirstOrDefault(r => r.Name == "Advertiser").Id);
                    break;
                case "Advertiser":
                    user.PrimaryRolId = user.LastRolId = Convert.ToInt32(defaultRoles.FirstOrDefault(r => r.Name == "Advertiser").Id);
                    user.SecondaryRolId = Convert.ToInt32(defaultRoles.FirstOrDefault(r => r.Name == "Publisher").Id);
                    break;
            }
            
            
            user.Roles = defaultRoles;
            return user;
        }

        public List<ApplicationUser> Load(List<ApplicationUser> cosmosUsers)
        {
             cosmosUsers.ForEach(u => _cosmosIdentityManager.CreateUser(u));
            return cosmosUsers;
        }

        public bool Execute()
        {
            (List<AspNetUser> sqlUsers, List<ApplicationUser> cosmosUsers) = Extract();
            List<ApplicationUser> newCosmosUsers = Transform(sqlUsers, cosmosUsers);
            Load(newCosmosUsers);
            return true;
        }
    }
}
