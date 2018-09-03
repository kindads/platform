using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using KindAds.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using KindAds.DataAccess;
using KindAds.Common.Models;
using Twilio;
using Twilio.Types;
using Twilio.Rest.Api.V2010.Account;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using KindAds.Business.Managers;
using System.Web.Mvc;
using System.Web;

namespace KindAds
{
  public class EmailService : IIdentityMessageService
  {
    public Task SendAsync(IdentityMessage message)
    {
      return configSendGridasync(message);
    }

    private Task configSendGridasync(IdentityMessage message)
    {
      var apiKey = Utils.Configuration.AppSettings.Mailproviderapikeysendgrid;
      var client = new SendGridClient(apiKey);
      var from = new EmailAddress("no-response@kindads.io", "Kind Ads");
      var subject = message.Subject;
      var to = new EmailAddress(message.Destination, "");
      var htmlContent = message.Body;
      var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlContent);
      return client.SendEmailAsync(msg);
      
    }
  }

  public class SmsService : IIdentityMessageService
  {
    public Task SendAsync(IdentityMessage message)
    {
      //TwilioManager smsManager = new TwilioManager();
      //TwilioKeys Keys = smsManager.GetKeys();

      //string accountSid = Keys.SMSAccountIdentification;
      //string authToken = Keys.SMSAccountPassword;

      //TwilioClient.Init(accountSid, authToken);

      //var to = new PhoneNumber(message.Destination);
      //var SMSMessage = MessageResource.Create(
      //    to,
      //    from: new PhoneNumber(Keys.SMSAccountFrom),
      //    body: message.Body );


      return Task.FromResult(0);
    }
  }



    public class UserCosmosStore<TUser> : IUserTwoFactorStore<TUser, string>, IUserLockoutStore<TUser, string>, IUserStore<TUser>, IUserPasswordStore<TUser>, IUserEmailStore<TUser>, IUserRoleStore<TUser> where TUser : ApplicationUser {
        private readonly CosmosIdentityManager _cosmosIdentityManage = new CosmosIdentityManager();

        TUser user
        {
            set {
                if (HttpContext.Current != null ) {
                    HttpContext.Current.Session["user"] = value;
                }
                
            }
            get {
                
                return (HttpContext.Current == null? null: (HttpContext.Current.Session["user"] ==null)?null:(TUser)HttpContext.Current.Session["user"]);
            }
        }

        public UserCosmosStore()
        {
            _cosmosIdentityManage = new CosmosIdentityManager();
        }

        public Task AddToRoleAsync(TUser user, string roleName)
        {
            RoleDocument rol = _cosmosIdentityManage.GetRoles().Where(r => r.Name == roleName).Single();
            user.Roles.Add(rol);
            return Task.CompletedTask;
        }

        public  Task CreateAsync(TUser user)
        {
            _cosmosIdentityManage.CreateUser(user);
            return Task.FromResult<bool>(true);
        }

        public Task DeleteAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {

        }

        public Task<TUser> FindByEmailAsync(string email)
        {
            TUser us = null;
            if (user == null) {
                us = (TUser)_cosmosIdentityManage.FindUserByEmail(email);
                user = us;
                if (user == null) {
                    return Task.FromResult<TUser>(us);
                }
                return Task.FromResult<TUser>(user);
            }
            else if (user.Email != email) {
                user = (TUser)_cosmosIdentityManage.FindUserByEmail(email);
         
            }
            return Task.FromResult<TUser>(user);
        }

        public Task<TUser> FindByIdAsync(string userId)
        {
            TUser us = null;
            if (user == null) {
                us = (TUser)_cosmosIdentityManage.FindUserByUserId(userId);
                user = us;
                if (user == null) {
                    return Task.FromResult<TUser>(us);
                }
                return Task.FromResult<TUser>(user);
            }
            else if (user.Id != userId) {
                user = (TUser)_cosmosIdentityManage.FindUserByUserId(userId);
  
            }
            return Task.FromResult<TUser>(user);
        }

        public Task<TUser> FindByNameAsync(string userName)
        {
            TUser us = null;
            if( user==null ) {
                us = (TUser)_cosmosIdentityManage.FindUserByUserName(userName);
                user = us;
                if (user == null) {
                    return Task.FromResult < TUser >( us);
                }
                return Task.FromResult < TUser > (user);
             
            }
            else if (user.UserName != userName) {
                user = (TUser)_cosmosIdentityManage.FindUserByUserName(userName);
   
            }
            return Task.FromResult<TUser>(user);
        }

        public Task<int> GetAccessFailedCountAsync(TUser user)
        {
            return Task.FromResult(user.AccessFailAccount);
        }

        public Task<string> GetEmailAsync(TUser user)
        {
            return Task.FromResult<string>(user.Email.ToString());

        }

        public Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            return Task.FromResult<bool>(user.EmailConfirmed);
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            return Task.FromResult<bool>(user.LockoutEnabled);
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            if (user == null) {

                throw new ArgumentNullException("user");

            }

            return Task.FromResult(

                user.LockoutEndDateUtc.HasValue ?

                    new DateTimeOffset(DateTime.SpecifyKind(user.LockoutEndDateUtc.Value, DateTimeKind.Utc)) :

                    new DateTimeOffset());

        }

        public Task<string> GetPasswordHashAsync(TUser user)
        {
            return Task.FromResult<string>(user.PasswordHash);
        }

        public Task<IList<string>> GetRolesAsync(TUser user)
        {
            return Task.FromResult<IList<string>>(user.Roles.Where(rr => rr.Id == user.LastRolId.ToString()).Select(r => r.Name).ToList());
        }

        public Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {

            return Task.FromResult<bool>(user.TwoFactorEnabled);

        }

        public Task<bool> HasPasswordAsync(TUser user)
        {
            return Task.FromResult<bool>(user.PasswordHash != string.Empty);
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            var newCount = user.AccessFailAccount++;
   
            return Task.FromResult<int>(newCount);
        }

        public Task<bool> IsInRoleAsync(TUser user, string roleName)
        {
            return Task.FromResult(user.Roles.Any(r => r.Name == roleName));
        }

        public Task RemoveFromRoleAsync(TUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task ResetAccessFailedCountAsync(TUser user)
        {
            user.AccessFailAccount = 0;

            return Task.FromResult(0);
        }

        public Task SetEmailAsync(TUser user, string email)
        {
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            user.EmailConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            user.LockoutEnabled = enabled;
            return Task.CompletedTask;
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            user.LockoutEndDateUtc = lockoutEnd.UtcDateTime;
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            user.TwoFactorEnabled = enabled;
            return Task.FromResult(0);
        }

        public Task UpdateAsync(TUser user)
        {
            _cosmosIdentityManage.UpSertApplicationUser(user);
            this.user = null;
            return Task.CompletedTask;
        }
        //
        // Summary:
        //     Default constuctor which uses a new instance of a default EntityyDbContext

    }


    public class ApplicationUserManager : Microsoft.AspNet.Identity.UserManager<ApplicationUser> {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }
        
        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options,
            IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserCosmosStore<ApplicationUser>());
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager) {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator {
                //RequiredLength = 6,
                //RequireNonLetterOrDigit = true,
                //RequireDigit = true,
                //RequireLowercase = true,
                //RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser> {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser> {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null) {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }

            
            return manager;
        }

      

    }

 

    // Configure the application sign-in manager which is used in this application.  
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
  {
    public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager) :
        base(userManager, authenticationManager)
    { }

    public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
    {
      return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
    }


        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
    {
      return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
    }
  }

  
}
