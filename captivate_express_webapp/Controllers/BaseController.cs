using captivate_express_webapp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Resources;
using System.Collections;
using System.Globalization;

namespace captivate_express_webapp.Controllers
{
  public class BaseController : Controller
  {
    protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
    {
      string cultureName = RouteData.Values["culture"] as string;
      cultureName = Request.QueryString["culture"] != null ? Request.QueryString["culture"] : cultureName;


      // Attempt to read the culture cookie from Request
      if (cultureName == null)
      cultureName = Request.UserLanguages != null && Request.UserLanguages.Length > 0 ? Request.UserLanguages[0] : null; // obtain it from HTTP header AcceptLanguages

      // Validate culture name
      cultureName = CultureHelper.GetImplementedCulture(cultureName); // This is safe

      // Modify current thread's cultures            
      Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
      Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

      return base.BeginExecuteCore(callback, state);
    }

    public JsonResult GetLanguages()
    {
      List<LanguageResource> list = new List<LanguageResource>();
      LanguageResource item;
      //ResourceManager MyResourceClass = new ResourceManager(typeof(Captivate.Comun.LanguageResources.LanguagesResources /* Reference to your resources class -- may be named differently in your case */));

      ResourceSet resourceSet = Captivate.Comun.LanguageResources.LanguagesResources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
      foreach (DictionaryEntry entry in resourceSet)
      {
        item = new LanguageResource()
        {
          Name = entry.Key.ToString(),
          Language = entry.Value.ToString()
        };

        list.Add(item);
      }

      return Json(new SelectList(list, "Language", "Name"));
    }
   
    public class LanguageResource
    {
      public string Name { set; get; }
      public string Language { set; get; }
    }
  }
}
