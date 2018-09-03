using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using KindAds.Business.Managers;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using KindAds.Models;

namespace KindAds.Services
{
  public class AccessService
  {
        private readonly CosmosIdentityManager _cosmosIdentityManage = new CosmosIdentityManager();
        private KindadsEntities _context;
    public AccessService()
    {
      _context = new KindadsEntities();
    }

    public List<AspNetRole> GetAllRoles()
    {
      return (from r in _context.AspNetRoles select r).ToList();
    }

        public string GetUserTokenAddress(string _walletAddress)
        {
            //The token address is the blob name in azure storage
            ApplicationUser _aspNetUser = _cosmosIdentityManage.FindUserByWalletAddress(_walletAddress);
            return _aspNetUser.TokenAddress;
        }
        public string GetUserWalletAddress(string _idusr)
        {
            ApplicationUser _aspNetUser = _cosmosIdentityManage.FindUserByUserId(_idusr);
            return _aspNetUser.WalletAddress;
        }


        public bool ContainRoleUser(string idUser, int idRole)
        {
            ApplicationUser user = _cosmosIdentityManage.FindUserByUserId(idUser);
            return (from r in user.Roles where r.Id.Equals(Convert.ToString(idRole)) select r).Count() > 0;
        }
        public AspNetUser GetUserById(string idUser)
    {
      return (from u in _context.AspNetUsers where u.Id.Equals(idUser) select u).FirstOrDefault();
    }

    public bool InsertUserDetail(UserDetail userDetail)
    {
      userDetail.RegistrationDate = DateTime.Now;
      _context.UserDetails.Add(userDetail);
      return _context.SaveChanges() > 0;
    }

    public UserDetail GetUserDetailByIdUser(string idUser)
    {
      return (from r in _context.UserDetails where r.UserId.Equals(idUser) select r).FirstOrDefault();
    }

    public bool ModifyUserDetail(UserDetail userDetail)
    {
      try
      {
        _context.Entry(userDetail).State = EntityState.Modified;
        return true;
      }
      catch (Exception)
      {
        return false;
      }

    }

        public bool UpdateUserWallet(string idUser, string wallet)
        {
            try {
                var user = _cosmosIdentityManage.FindUserByUserId(idUser);
                if (user.WalletAddress == string.Empty) //Si no tiene wallet
                {
                    user.WalletAddress = wallet;
                    user.IsMetamask = true;
                    _cosmosIdentityManage.UpSertApplicationUser(user);
                }
                return true;
            }
            catch (Exception) {
                return false;
            }
        }
    }


}
