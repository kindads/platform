using System;
using System.Linq;
using System.Web.Mvc;

namespace KindAds.Utils.Security
{
  [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
  public class AuthorizeRoles : AuthorizeAttribute
  {
    public AuthorizeRoles(params object[] roles)
     {
      if (roles.Any(r => r.GetType().BaseType != typeof(Enum)))
        throw new ArgumentException("roles");

      Roles = string.Join(",", roles.Select(r => Enum.GetName(r.GetType(), r)));
    }
  }
}
