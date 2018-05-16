using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;

namespace captivate_express_webapp.Utils.Configuration
{
  public class AppSettings
  {
    public static string MailApiKeySendGrid
    {
      get
      {
        return Setting<string>("ApiKeySendGrid");
      }
    }

    public static string AzureStorageConnection
    {
      get
      {
        return Setting<string>("AzureStorageConnection");
      }
    }
    public static string BlockchainURL
    {
      get
      {
        return Setting<string>("BlockchainURL");
      }
    }

    public static string HtmlTemplateStorageConnection
    {
      get
      {
        return Setting<string>("HtmlTemplateStorageConnection");
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

    public static string AWeberAuthorizeAppUrl
    {
      get
      {
        return Setting<string>("AWeberAuthorizeAppUrl");
      }
    }
  }
}
