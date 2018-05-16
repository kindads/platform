using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace captivate_express_webapp.Controllers
{
  public class TestController : Controller
  {

 

    public ActionResult Index()
    {
      Session.Clear();
      List<SelectListItem> li = new List<SelectListItem>();
      li.Add(new SelectListItem { Text = "Select", Value = "0" });
      li.Add(new SelectListItem { Text = "India", Value = "1" });
      li.Add(new SelectListItem { Text = "Srilanka", Value = "2" });
      li.Add(new SelectListItem { Text = "China", Value = "3" });
      li.Add(new SelectListItem { Text = "Austrila", Value = "4" });
      li.Add(new SelectListItem { Text = "USA", Value = "5" });
      li.Add(new SelectListItem { Text = "UK", Value = "6" });

      ViewData["country"] = li;
      return View();
    }

    public PartialViewResult MostrarResultado()
    {
     
     
      string country = (String)Session["country"];
      string state = (String)Session["state"];
      string city = (String)Session["city"];

      string result = $"Country: {country} State: {state} City{city}";
      return PartialView("DatosPartial", result );
    }

    public JsonResult GetStates(string id)
    {
      Session["country"]   = id;
      List<SelectListItem> states = new List<SelectListItem>();
      switch (id)
      {
        case "1":
          states.Add(new SelectListItem { Text = "Select", Value = "0" });
          states.Add(new SelectListItem { Text = "ANDAMAN & NIKOBAR ISLANDS", Value = "1" });
          states.Add(new SelectListItem { Text = "ANDHRA PRADESH", Value = "2" });
          states.Add(new SelectListItem { Text = "ARUNACHAL PRADESH", Value = "3" });
          states.Add(new SelectListItem { Text = "ASSAM", Value = "4" });
          states.Add(new SelectListItem { Text = "BIHAR", Value = "5" });
          states.Add(new SelectListItem { Text = "CHANDIGARH", Value = "6" });
          states.Add(new SelectListItem { Text = "CHHATTISGARH", Value = "7" });
          states.Add(new SelectListItem { Text = "DADRA & NAGAR HAVELI", Value = "8" });
          states.Add(new SelectListItem { Text = "DAMAN & DIU", Value = "9" });
          states.Add(new SelectListItem { Text = "GOA", Value = "10" });
          states.Add(new SelectListItem { Text = "GUJARAT", Value = "11" });
          states.Add(new SelectListItem { Text = "HARYANA", Value = "12" });
          states.Add(new SelectListItem { Text = "HIMACHAL PRADESH", Value = "13" });
          states.Add(new SelectListItem { Text = "JAMMU & KASHMIR", Value = "14" });
          states.Add(new SelectListItem { Text = "JHARKHAND", Value = "15" });
          states.Add(new SelectListItem { Text = "KARNATAKA", Value = "16" });
          states.Add(new SelectListItem { Text = "KERALA", Value = "17" });
          states.Add(new SelectListItem { Text = "LAKSHADWEEP", Value = "18" });
          states.Add(new SelectListItem { Text = "MADHYA PRADESH", Value = "19" });
          states.Add(new SelectListItem { Text = "MAHARASHTRA", Value = "20" });
          states.Add(new SelectListItem { Text = "MANIPUR", Value = "21" });
          states.Add(new SelectListItem { Text = "MEGHALAYA", Value = "22" });
          states.Add(new SelectListItem { Text = "MIZORAM", Value = "23" });
          states.Add(new SelectListItem { Text = "NAGALAND", Value = "24" });
          states.Add(new SelectListItem { Text = "NCT OF DELHI", Value = "25" });
          states.Add(new SelectListItem { Text = "ORISSA", Value = "26" });
          states.Add(new SelectListItem { Text = "PUDUCHERRY", Value = "27" });
          states.Add(new SelectListItem { Text = "PUNJAB", Value = "28" });
          states.Add(new SelectListItem { Text = "RAJASTHAN", Value = "29" });
          states.Add(new SelectListItem { Text = "SIKKIM", Value = "30" });
          states.Add(new SelectListItem { Text = "TAMIL NADU", Value = "31" });
          states.Add(new SelectListItem { Text = "TRIPURA", Value = "32" });
          states.Add(new SelectListItem { Text = "UTTAR PRADESH", Value = "33" });
          states.Add(new SelectListItem { Text = "UTTARAKHAND", Value = "34" });
          states.Add(new SelectListItem { Text = "WEST BENGAL", Value = "35" });

          break;
        case "UK":

          break;
        case "India":

          break;
      }
      return Json(new SelectList(states, "Value", "Text"));
    }

    public JsonResult GetCity(string id)
    {
      Session["state"] = id;
      List<SelectListItem> City = new List<SelectListItem>();
      switch (id)
      {
        case "20":
          City.Add(new SelectListItem { Text = "Select", Value = "0" });
          City.Add(new SelectListItem { Text = "MUMBAI", Value = "1" });
          City.Add(new SelectListItem { Text = "PUNE", Value = "2" });
          City.Add(new SelectListItem { Text = "KOLHAPUR", Value = "3" });
          City.Add(new SelectListItem { Text = "RATNAGIRI", Value = "4" });
          City.Add(new SelectListItem { Text = "NAGPUR", Value = "5" });
          City.Add(new SelectListItem { Text = "JALGAON", Value = "6" });
          break;

      }

      return Json(new SelectList(City, "Value", "Text"));
    }

    [HttpPost]
    public ActionResult Index(FormCollection FC)
    {

      string country = FC["Country"].ToString();
      string state = FC["State"].ToString();
      string city = FC["city"].ToString();

      ViewBag.Resultado = $"Country: {country} State: {state} City{city}";

      #region Rebinding after posting

      List<SelectListItem> li = new List<SelectListItem>();
      li.Add(new SelectListItem { Text = "Select", Value = "0" });
      li.Add(new SelectListItem { Text = "India", Value = "1" });
      li.Add(new SelectListItem { Text = "Srilanka", Value = "2" });
      li.Add(new SelectListItem { Text = "China", Value = "3" });
      li.Add(new SelectListItem { Text = "Austrila", Value = "4" });
      li.Add(new SelectListItem { Text = "USA", Value = "5" });
      li.Add(new SelectListItem { Text = "UK", Value = "6" });

      ViewData["country"] = li;
      #endregion

      return View();
    }

    [HttpGet]
    public JsonResult DoTransfer()
    {
      Helpers.NethereumHelper.TransferTokenFrontTest("0xd42e518424856c5f0b9bb88b2b3f42597d54826a", "0x8ecd7814dc849e080ff24124be7dead24aae73ec", "1");
      //public async static Task<string> GetBalance(string _wallet, Boolean fixdecimal)
      return Json("{ok}");
    }

    [HttpGet]
    public JsonResult Balance()
    {
      System.Threading.Tasks.Task task = System.Threading.Tasks.Task.Run(async () => await (Helpers.NethereumHelper.GetBalance("0x1b55835b19bfa3557d271294e1c743ca7150f27a",true)));

      string sbalance = task.ToString();
      return Json("{ok}");
    }

    [HttpGet]
    public JsonResult VerifySiteTest()
    {
      Services.SiteService _service = new Services.SiteService();

      Boolean bresult = _service.VerifySite(Guid.Parse("C645ADC2-6EA1-4797-82C7-CF013FADB09D"));

      return Json("{ok}");
    }

    public ActionResult CreateVerificationFile()
    {

      Services.SiteService _service = new Services.SiteService();

      var bresult = _service.CreateVerificationFile(Guid.Parse("759a3e64-6369-4ed9-9f49-474254d637eb"));
      return bresult;
    }

    public JsonResult subscriberskey()
    {

      Boolean bresult = Helpers.SubscribersHelper.ValidateKey("bef7b928-0ca6-4569-89c6-434791256d9e","http://www.blockbliss.com");
      Boolean bresult2 = Helpers.SubscribersHelper.ValidateKey("bef7b928-0ca6-4569-89c6-434791256d9e", "https://www.blockbliss.com");
      Boolean bresult3 = Helpers.SubscribersHelper.ValidateKey("bef7b928-0ca6-4569-89c6-434791256d9e", "http://blockbliss.com");
      Boolean bresult4 = Helpers.SubscribersHelper.ValidateKey("bef7b928-0ca6-4569-89c6-434791256d9e", "https://blockbliss.com");

      return Json("{ok}");
    }

    [HttpGet]
    public JsonResult SendGift()
    {
      System.Threading.Tasks.Task task = System.Threading.Tasks.Task.Run(async () => await (Helpers.NethereumHelper.SendGiftTokens("0xa23d692aff231cfec286655121bada6f95b18677")));

      string sGift = task.ToString();
      return Json("{ok}");
    }

    public JsonResult SendMessageSubscribers()
    {
      Models.KindadsEntities _context = new Models.KindadsEntities();

      var idCampaign = Guid.Parse("4B39BCEC-B3F2-47DB-A0CF-9DBAC13B2454");
      Models.CAMPAIGN _campaign = (from d in _context.CAMPAIGNs1 where d.IdCampaign.Equals(idCampaign) select d).FirstOrDefault();

      string suuid = Helpers.SubscribersHelper.SendCampaignMessage(_campaign);

      _context.Dispose();

      return Json("{ok}", JsonRequestBehavior.AllowGet);
    }

  }
}
