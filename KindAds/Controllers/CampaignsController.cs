using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using KindAds.Negocio.ViewModels.KindAdsV2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KindAds.Controllers
{
    public class CampaignsController : BaseController
    {

        private List<PublisherPreferenceQuestionDocument> listQuestions;
        // GET: Campaigns
        public ActionResult MyCampaigns()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CreateCampaign()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateCampaign(CampaignViewModel model)
        {
            return View();
        }

        public PartialViewResult AddQuestion(CampaignViewModel model, string question)
        {
            listQuestions = (List<PublisherPreferenceQuestionDocument>)TempData["vdListQuestions"];
            if (listQuestions == null) {
                listQuestions = new List<PublisherPreferenceQuestionDocument>();
            }

            if (!string.IsNullOrEmpty(question) && !question.Equals("__QUESTION__")) {
                listQuestions.Add(new PublisherPreferenceQuestionDocument(question));
            }
            TempData["vdListQuestions"] = listQuestions;

            model.listQuestions = listQuestions;
            return PartialView("_TableQuestionCampaign", model);
        }

        public PartialViewResult RemoveQuestion(CampaignViewModel model, string question)
        {
            listQuestions = (List<PublisherPreferenceQuestionDocument>)TempData["vdListQuestions"];
            if (listQuestions == null) {
                listQuestions = new List<PublisherPreferenceQuestionDocument>();
            }
            listQuestions.RemoveAll(l => l.Id == question);
            TempData["vdListQuestions"] = listQuestions;

            model.listQuestions = listQuestions;
            return PartialView("_TableQuestionCampaign", model);
        }
    }
}
