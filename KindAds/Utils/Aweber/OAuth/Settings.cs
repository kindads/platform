/*
* AWeber API .NET SDK v1.0
* Providing the ability to connect a .NET application to the AWeber API.
* 
* Copyright (c) 2011 - Binkd
* Licensed under the GNU General Public License (GNU GPL v3.0)
* 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aweber.OAuth
{

  /// <summary>
  /// Settings required to authenticate with AWeber using OAuth v1.0
  /// Hardcoded due to the fact these settings are not user configurable
  /// </summary>
  internal class Settings
  {
    // Authorize App
    public const String authorizeApp = "https://auth.aweber.com/1.0/oauth/authorize_app/";

    // Request Token Url
    public const String requestToken = "https://auth.aweber.com/1.0/oauth/request_token";

    // Authorization Url
    public const String authorization = "https://auth.aweber.com/1.0/oauth/authorize";

    // Access Token Url
    public const String accessToken = "https://auth.aweber.com/1.0/oauth/access_token";

    // API Base
    public const String apiBase = "https://api.aweber.com/1.0/";

    //Broadcast Url
    public const String createBroadcast = "https://api.aweber.com/1.0/accounts/{0}/lists/{1}/broadcasts";

    //Broadcast
    public const String getBroadcast = "https://api.aweber.com/1.0/accounts/{0}/lists/{1}/broadcasts/{2}";

    //Schedule
    public const String scheduleBroadcast = "https://api.aweber.com/1.0/accounts/{0}/lists/{1}/broadcasts/{2}/schedule";
  }
}
