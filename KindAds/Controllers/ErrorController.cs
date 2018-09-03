using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KindAds.Controllers
{
  public class ErrorController : Controller
  {
    // GET: Error
    public ActionResult Index()
    {
      return View();
    }

    public ViewResult NotFound()
    {
      Response.StatusCode = 404;  //you may want to set this to 404
      return View("NotFound");
    }

    public ViewResult InternalServer()
    {
      Response.StatusCode = 500;  //you may want to set this to 500
      return View("InternalServer");
    }

  }
}
