using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KindAds.Common.Utils.Partners.Mail.Aweber
{
  public interface IAdapter
  {

    String ConsumerKey { get; set; }
    String ConsumerSecret { get; set; }
    String AppId { get; set; }

    String CallbackUrl { get; set; }

    String OAuthTokenSecret { get; set; }
    String OAuthToken { get; set; }
    String OAuthVerifier { get; set; }

    String GetResponse(String url);

    OAuth.Request BuildRequest();


  }
}
