using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;

namespace KindAds.Utils.Configuration
{
  public class AppSettings
  {
    public static string Mailproviderapikeysendgrid
    {
      get
      {
        return Setting<string>("provider-apikey-sendgrid");
      }
    }

    public static string azurestorageconnectionstring
    {
      get
      {
        return Setting<string>("azure-storage-connectionstring");
      }
    }
    public static string BlockchainURL
    {
      get
      {
        return Setting<string>("BlockchainURL");
      }
    }

    public static string azurestoragehtmltemplateconnectionstring
    {
      get
      {
        return Setting<string>("azure-storage-htmltemplate-connectionstring");
      }
    }

    public static string ContractAddress
    {
      get
      {
        return Setting<string>("ContractAddress");
      }
    }

    public static string MainAddress
    {
      get
      {
        return Setting<string>("MainAddress");
      }
    }
    public static string MainAddressPassphrase
    {
      get
      {
        return Setting<string>("MainAddressPassphrase");
      }
    }

    public static string StorageConnection
    {
      get
      {
        return Setting<string>("StorageConnection");
      }
    }


    public static string EtherScanUrl
    {
      get
      {
        return Setting<string>("EtherScanUrl");
      }
    }


    private static T Setting<T>(string name)
    {
      string value = ConfigurationManager.AppSettings[name];

      if (value == null)
      {
        throw new Exception(String.Format("Could not find setting '{0}',", name));
      }

      return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
    }

    public static string provideraweberauthorizeappurl
    {
      get
      {
        return Setting<string>("provider-aweberauthorizeappurl");
      }
    }
  }
}
