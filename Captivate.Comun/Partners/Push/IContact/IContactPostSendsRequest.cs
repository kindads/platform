using Captivate.Common.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Script.Serialization;

namespace Captivate.Common.Partners.IContact
{
  public class IContactPostSendsRequest : IRequest
  {
    public int messageId { set; get; }  

    public string includeListIds { set; get; }
    public string scheduledTime { set; get; }

    [ScriptIgnore]
    public DateTime scheduled { set; get; }

    [ScriptIgnore]
    public string BaseUrl { set; get; }

    public IContactPostSendsRequest()
    {
      messageId = 0;
      includeListIds = string.Empty;
      scheduled = DateTime.Now;
      scheduledTime = scheduled.ToString();
    }
   
  }

  public class IContactPostSendsRequestWithoutScheduledTime: IRequest
  {
    public int messageId { set; get; }

    public string includeListIds { set; get; }

    [ScriptIgnore]
    public DateTime scheduled { set; get; }

    [ScriptIgnore]
    public string BaseUrl { set; get; }

    public IContactPostSendsRequestWithoutScheduledTime()
    {
      messageId = 0;
      includeListIds = string.Empty;
      scheduled = DateTime.Now;
    }
  }


  public class PropertyRenameAndIgnoreSerializerContractResolver : DefaultContractResolver
  {
    private readonly Dictionary<Type, HashSet<string>> _ignores;
    private readonly Dictionary<Type, Dictionary<string, string>> _renames;

    public PropertyRenameAndIgnoreSerializerContractResolver()
    {
      _ignores = new Dictionary<Type, HashSet<string>>();
      _renames = new Dictionary<Type, Dictionary<string, string>>();
    }

    public void IgnoreProperty(Type type, params string[] jsonPropertyNames)
    {
      if (!_ignores.ContainsKey(type))
        _ignores[type] = new HashSet<string>();

      foreach (var prop in jsonPropertyNames)
        _ignores[type].Add(prop);
    }

    public void RenameProperty(Type type, string propertyName, string newJsonPropertyName)
    {
      if (!_renames.ContainsKey(type))
        _renames[type] = new Dictionary<string, string>();

      _renames[type][propertyName] = newJsonPropertyName;
    }

    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
      var property = base.CreateProperty(member, memberSerialization);

      if (IsIgnored(property.DeclaringType, property.PropertyName))
        property.ShouldSerialize = i => false;

      if (IsRenamed(property.DeclaringType, property.PropertyName, out var newJsonPropertyName))
        property.PropertyName = newJsonPropertyName;

      return property;
    }

    private bool IsIgnored(Type type, string jsonPropertyName)
    {
      if (!_ignores.ContainsKey(type))
        return false;

      return _ignores[type].Contains(jsonPropertyName);
    }

    private bool IsRenamed(Type type, string jsonPropertyName, out string newJsonPropertyName)
    {
      Dictionary<string, string> renames;

      if (!_renames.TryGetValue(type, out renames) || !renames.TryGetValue(jsonPropertyName, out newJsonPropertyName))
      {
        newJsonPropertyName = null;
        return false;
      }

      return true;
    }
  }
}
