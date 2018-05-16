using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace captivate_express_webapp.Utils
{
  static class TypeSwitch
  {
    public class CaseInfo
    {
      public bool IsDefault { get; set; }
      public Type Target { get; set; }
      public Action<object> Action { get; set; }
    }

    public static void Do(object source, params CaseInfo[] cases)
    {
      var type = source.GetType();
      foreach (var entry in cases)
      {
        if (entry.IsDefault || entry.Target.IsAssignableFrom(type))
        {
          entry.Action(source);
          break;
        }
      }
    }

    public static CaseInfo Case<T>(Action action)
    {
      return new CaseInfo()
      {
        Action = x => action(),
        Target = typeof(T)
      };
    }

    public static CaseInfo Case<T>(Action<T> action)
    {
      return new CaseInfo()
      {
        Action = (x) => action((T)x),
        Target = typeof(T)
      };
    }

    public static CaseInfo Default(Action action)
    {
      return new CaseInfo()
      {
        Action = x => action(),
        IsDefault = true
      };
    }
  }
}
