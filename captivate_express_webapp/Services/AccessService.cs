using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using captivate_express_webapp.Models;

namespace captivate_express_webapp.Services
{
  public class AccessService
  {
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
      AspNetUser _aspNetUser = (AspNetUser)((from d in _context.AspNetUsers where d.WalletAddress == _walletAddress select d).FirstOrDefault());
      return _aspNetUser.TokenAddress;
    }
    public string GetUserWalletAddress(string _idusr)
    {
      AspNetUser _aspNetUser = (AspNetUser)((from d in _context.AspNetUsers where d.Id.Equals(_idusr) select d).FirstOrDefault());
      return _aspNetUser.WalletAddress;
    }

    public bool ContainRoleUser(string idUser, int idRole)
    {
      AspNetUser user = (from r in _context.AspNetUsers where r.Id.Equals(idUser) select r).FirstOrDefault();
      return (from r in user.AspNetRoles where r.Id.Equals(Convert.ToString(idRole)) select r).Count() > 0;
    }
    public AspNetUser GetUserById(string idUser)
    {
      return (from u in _context.AspNetUsers where u.Id.Equals(idUser) select u).FirstOrDefault();
    }

    public bool InsertUserDetail(UserDetail userDetail)
    {
      userDetail.RegistrationDate = DateTime.Now;
      _context.UserDetails.Add(userDetail);
      return  _context.SaveChanges() > 0;
    }

    public UserDetail GetUserDetailByIdUser(string idUser)
    {
      return (from r in _context.UserDetails where r.UserId.Equals(idUser) select r).FirstOrDefault();
    }
  }


}
