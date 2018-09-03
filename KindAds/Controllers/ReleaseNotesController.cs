using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace KindAds.Controllers
{
    public class ReleaseNotesController : BaseController
    {
    // GET: Releases
        public ActionResult Sprint8()
        {
          return View();
        }
        public ActionResult Sprint9()
        {
        return View();
        }
    }
}
