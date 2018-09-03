using KindAds.Comun.Enums;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;

using KindAds.Negocio.Managersv2;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace KindAds.Business.Managers {
    public class CosmosIdentityManager : BaseManager {

        //private ApplicationUser applicationUser { set; get; }

        private Lazy<ApplicationUser> applicationUser { set; get; }

        public CosmosIdentityManager():base()
        {
            databaseName = ConfigurationManager.AppSettings["azure-cosmos-databasename"];
            
        }

        public void CreateUser(ApplicationUser user)
        {
            telemetria.Notify("ACCESS DATA CosmosIdentityManager: CreateUser");
            context.AddDocument(databaseName, CosmosCollections.User.ToString(), user);
        }

        public List<ApplicationUser> GetUsers()
        {
            telemetria.Notify("ACCESS DATA CosmosIdentityManager: GetUsers");
            return context.GetAllDocuments < ApplicationUser>(databaseName, CosmosCollections.User.ToString());
        }

        public ApplicationUser FindUserByEmail(string email)
        {
            telemetria.Notify("ACCESS DATA CosmosIdentityManager: FindUserByEmail");
            if(applicationUser==null)
            {
                collectionName = CosmosCollections.User.ToString();
                string query = $"SELECT * FROM {collectionName} WHERE {collectionName}.Email='{email}'";
                var user = context.ExecuteQuery<UserDocument>(databaseName, collectionName, query).FirstOrDefault();
                applicationUser = MapToApplicationUser(user);
            }     
            return applicationUser.Value;
        }


        public Lazy<ApplicationUser> MapToApplicationUser(UserDocument user)
        {
            Lazy<ApplicationUser> applicationUser = new Lazy<ApplicationUser>();

            if(user!=null)
            {
                applicationUser.Value.AccessFailAccount = user.AccessFailAccount;
                applicationUser.Value.Email = user.Email;
                applicationUser.Value.EmailConfirmed = user.EmailConfirmed;
                applicationUser.Value.Hometown = user.Hometown;
                applicationUser.Value.Id = user.Id;
                applicationUser.Value.IsMetamask = user.IsMetamask;
                applicationUser.Value.IsPremium = user.IsPremium;
                applicationUser.Value.LastRolId = user.LastRolId;
                applicationUser.Value.LockoutEnabled = user.LockoutEnabled;
                applicationUser.Value.LockoutEndDateUtc = user.LockoutEndDateUtc;
                applicationUser.Value.Name = user.Name;
                applicationUser.Value.PasswordHash = user.PasswordHash;
                applicationUser.Value.PhoneNumber = user.PhoneNumber;
                applicationUser.Value.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
                applicationUser.Value.PrimaryRolId = user.PrimaryRolId;
                applicationUser.Value.RegisterDate = user.RegisterDate;
                applicationUser.Value.Roles = user.Roles;
                applicationUser.Value.SecondaryRolId = user.SecondaryRolId;
                applicationUser.Value.SecurityStamp = user.SecurityStamp;
                applicationUser.Value.TokenAddress = user.TokenAddress;
                applicationUser.Value.TwoFactorEnabled = user.TwoFactorEnabled;
                applicationUser.Value.UserName = user.UserName;
                applicationUser.Value.WalletAddress = user.WalletAddress;
            }            

            return applicationUser;
        }

        public ApplicationUser FindUserByUserName(string userName)
        {
            telemetria.Notify("ACCESS DATA CosmosIdentityManager: FindUserByUserName");
            if (userName != string.Empty && applicationUser==null)
            {
                collectionName = CosmosCollections.User.ToString();
                string query = $"SELECT * FROM {collectionName} WHERE {collectionName}.UserName='{userName}'";
                var user = context.ExecuteQuery<UserDocument>(databaseName, collectionName, query).FirstOrDefault();
                applicationUser = MapToApplicationUser(user);
            }
            return applicationUser.Value;
        }

        public ApplicationUser FindUserByUserId(string userId)
        {
            telemetria.Notify("ACCESS DATA CosmosIdentityManager: FindUserByUserId");
            if (userId != string.Empty && applicationUser == null)
            {
                collectionName = CosmosCollections.User.ToString();
                string query = $"SELECT * FROM {collectionName} WHERE {collectionName}.id='{userId}'";
                var user = context.ExecuteQuery<UserDocument>(databaseName, collectionName, query).FirstOrDefault();
                applicationUser = MapToApplicationUser(user);
            }           
            return applicationUser.Value;
        }

        public ApplicationUser VerifyUserById(string userId)
        {
            telemetria.Notify("ACCESS DATA CosmosIdentityManager: VerifyUserById");
            if (userId != string.Empty && applicationUser == null)
            {
                collectionName = CosmosCollections.User.ToString();
                string query = $"SELECT * FROM {collectionName} WHERE {collectionName}.id='{userId}'";
                var user = context.ExecuteQuery<UserDocument>(databaseName, collectionName, query).FirstOrDefault();
                user.EmailConfirmed = true;
                context.UpsertDocument<UserDocument>(databaseName, collectionName, user);

                applicationUser = MapToApplicationUser(user);
            }
            return applicationUser.Value;
        }

        public ApplicationUser FindUserByWalletAddress(string walletAddress)
        {
            telemetria.Notify("ACCESS DATA CosmosIdentityManager: FindUserByWalletAddress");
            if (walletAddress != string.Empty && applicationUser==null)
            {
                collectionName = CosmosCollections.User.ToString();
                string query = $"SELECT * FROM {collectionName} WHERE {collectionName}.WalletAddress='{walletAddress}'";
                var user = context.ExecuteQuery<UserDocument>(databaseName, collectionName, query).FirstOrDefault();
                applicationUser = MapToApplicationUser(user);
            }
          
            return applicationUser.Value;
        }


        public List<RoleDocument> GetRoles()
        {
   
            var listRoles = new List<RoleDocument>();
            listRoles.Add(new RoleDocument
                {
                Id="2",
                    Name=RoleEnum.Advertiser.ToString()
            });

            listRoles.Add(new RoleDocument
            {
                Id = "1",
                Name = RoleEnum.Publisher.ToString()
            });

            return listRoles;

            //return context.GetAllDocuments<RoleDocument>(databaseName, CosmosCollections.Role.ToString());
        }

        public void  UpSertApplicationUser(ApplicationUser applicationUser)
        {
            telemetria.Notify("ACCESS DATA CosmosIdentityManager: UpSertApplicationUser");
            context.UpsertDocument(databaseName, CosmosCollections.User.ToString(), applicationUser);
            //context.AddDocument<ApplicationUser>(databaseName, CosmosCollections.User.ToString(), applicationUser);
        }

        public RoleDocument GetRoleDocument(string RoleId)
        {
            telemetria.Notify("ACCESS DATA CosmosIdentityManager: GetRoleDocument");
            RoleDocument role = new RoleDocument();
            collectionName = CosmosCollections.Role.ToString();
            string query = $"SELECT * FROM {collectionName} WHERE {collectionName}.id='{RoleId}'";
            role = context.ExecuteQuery<RoleDocument>(databaseName, collectionName, query).FirstOrDefault();
            return role;
        }

        public void  SetSecondaryRoles(string userId)
        {
            telemetria.Notify("ACCESS DATA CosmosIdentityManager: SetSecondaryRoles");
            ApplicationUser user = new ApplicationUser();

            if (applicationUser == null)
            {
                user = FindUserByUserId(userId);
            }
            else
            {
                user = applicationUser.Value;
            }

            try
            {
                //var userRole = user.Roles.First(r => r.Id == user.LastRolId.ToString());
                RoleDocument userRole = GetRoleDocument(user.LastRolId.ToString());

                if (user.Roles.Count == 0)
                {
                    var roleEnum = (RoleEnum)Enum.Parse(typeof(RoleEnum), userRole.Name, true);
                    RoleDocument rolCandidate = null;
                    RoleDocument primaryRol = null;
                    switch (roleEnum)
                    {
                        case RoleEnum.Advertiser:
                            rolCandidate = GetRoles().SingleOrDefault(r => r.Name == RoleEnum.Publisher.ToString());
                            primaryRol= GetRoles().SingleOrDefault(r => r.Name == RoleEnum.Advertiser.ToString());
                            break;
                        case RoleEnum.Publisher:
                            rolCandidate = GetRoles().SingleOrDefault(r => r.Name == RoleEnum.Advertiser.ToString());
                            primaryRol= GetRoles().SingleOrDefault(r => r.Name == RoleEnum.Publisher.ToString());
                            break;

                    }
                    if (rolCandidate != null)
                    {
                        user.PrimaryRolId = Convert.ToInt32(userRole.Id);
                        user.SecondaryRolId = Convert.ToInt32(rolCandidate.Id);

                        user.Roles.Add(primaryRol);
                        user.Roles.Add(rolCandidate);

                        UpSertApplicationUser(user);
                    }

                }
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
        }

    }
}
