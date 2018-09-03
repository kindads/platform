using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models.CosmosDocuments.KindAdsV2
{
    public class UserDocument : KindAdsV2Document, IUser
    {
        public string UserName { get; set; }
        public string Name { set; get; }

        public string Email { set; get; }

        public bool EmailConfirmed { set; get; }

        public string PasswordHash { set; get; }
        //
        // Summary:
        //     A random value that should change whenever a users credentials have changed (password
        //     changed, login removed)
        public virtual string SecurityStamp { get; set; }

        //
        // Summary:
        //     PhoneNumber for the user
        public virtual string PhoneNumber { get; set; }
        //
        // Summary:
        //     True if the phone number is confirmed, default is false
        public virtual bool PhoneNumberConfirmed { get; set; }
        //
        // Summary:
        //     Is two factor enabled for the user
        public virtual bool TwoFactorEnabled { get; set; }
        //
        // Summary:
        //     DateTime in UTC when lockout ends, any time in the past is considered not locked
        //     out.
        public virtual DateTime? LockoutEndDateUtc { get; set; }
        //
        // Summary:
        //     Is lockout enabled for this user
        public virtual bool LockoutEnabled { get; set; }
        public int AccessFailAccount { set; get; }

        public string TokenAddress { set; get; }

        public string WalletAddress { set; get; }
        public string Hometown { get; set; }

        public List<RoleDocument> Roles { get; set; }

        public int PrimaryRolId { get; set; }
        public int SecondaryRolId { get; set; }
        public int LastRolId { get; set; }

        public bool IsPremium { get; set; }
        public bool IsMetamask { get; set; }


        public UserDocument()
        {
            Roles = new List<RoleDocument>();
            Name = string.Format("<NULL>");
            Email = string.Format("<NULL>");
            EmailConfirmed = false;
            PasswordHash = string.Format("<NULL>");
            PhoneNumber = string.Format("<NULL>");
            AccessFailAccount = 0;
            TokenAddress = string.Format("<NULL>");
            WalletAddress = string.Format("<NULL>");
        }
    }

    public class ApplicationUser : UserDocument
    {

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}
