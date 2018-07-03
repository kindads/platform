using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace captivate_express_webapp.Controllers
{
    public class ReleaseNotesController : BaseController
    {
    // GET: Releases
    [Route("ReleaseNotes_Sprint8")]
    [AllowAnonymous]
    public ActionResult Kind_Sprint8()
    {
      return View();
    }
    [Route("ReleaseNotes_Sprint9")]
      [AllowAnonymous]
      public ActionResult Kind_Sprint9()
      {
        return View();
      }
    }
}
