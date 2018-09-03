using KindAds.Azure;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using KindAds.Negocio.Managersv2;
using KindAds.Negocio.ViewModels.KindAdsV2;
using KindAds.Utils.Enums;
using KindAds.Utils.Security;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KindAds.Controllers
{

    [Authorize(Roles = "Publisher")]
    public class AudienceController : BaseController {

        public AudienceManager manager { set; get; }
        private readonly PublisherProfileManager _publisherProfileManager;
        private readonly AudienceChannelManager _audienceManager;
        public string sessionKey { set; get; }

        public AudienceController()
        {
            _audienceManager = new AudienceChannelManager();
            _publisherProfileManager = new PublisherProfileManager();
            manager = new AudienceManager();
            sessionKey = "AudienceViewModel";
            
        }
        public ActionResult MyAudiences()
        {
            List<AudienceListItemViewModel> audiences = new List<AudienceListItemViewModel>();

            try {
                string userId = User.Identity.GetUserId();
                var publisher = manager.GetPublisherProfile(userId);
                manager = new AudienceManager();
                audiences = manager.GetAudiencesViewModelByPublisherProfileId(publisher.Id);
                
            }
            catch (Exception e) {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }

            return View(audiences);
        }

        // GET: Audience      
        public ActionResult CreateAudience()
        {
            AudienceViewModel viewModel = new AudienceViewModel();
            var publisherProfileDocument = _publisherProfileManager.FindProfileDocByUserId(User.Identity.GetUserId());
            viewModel.stage = "basic";
            string Id = User.Identity.GetUserId();
            ViewBag.ImageSrcPreview = publisherProfileDocument.IconUrl;
            ViewBag.descriptionPreview = publisherProfileDocument.Description;
            viewModel.audience.Visibility = true;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult CreateAudienceBasic(AudienceViewModel viewModel)
        {
            var publisherProfileDocument = _publisherProfileManager.FindProfileDocByUserId(User.Identity.GetUserId());
            
            if (manager.IsAudienceDuplicated(publisherProfileDocument.Id, viewModel.audience.WebSiteUrl)) {
                viewModel.stage = "basic";
                ViewBag.error = "SiteDuplicated";
                return View("CreateAudience", viewModel);
            }
                
            string id = string.Empty;
            
            string userId = User.Identity.GetUserId();
            var publisher = manager.GetPublisherProfile(userId);
            viewModel.audience.PublisherId = publisher.Id;
            viewModel.audience.Description = Negocio.Utilerias.StringUtilities.RemoveNewLineInString(viewModel.audience.Description);
            GetAndSetIconUrl(viewModel);
            var protocol = viewModel.audience.UrlProtocol == "1" ? "https://" : "http://";
            viewModel.audience.WebSiteUrl = string.Format("{0}{1}", protocol, viewModel.audience.WebSiteUrl.Replace("www.", "").Replace("http://","").Replace("https://", ""));
            id = manager.AddAudience(viewModel);
            

            // validation
            if (id != string.Empty) {
                viewModel.audience.Id = id;
                viewModel.stage = "preferences";
            }
            else {
                viewModel.stage = "error";
            }

            HoldViewModel(viewModel);
            viewModel.audience.Visibility = true;
            return View("CreateAudience", viewModel);
        }

        public ActionResult EditAudience(string audienceId)
        {
            AudienceViewModel viewModel = new AudienceViewModel();
            List<AudiencePreferenceDocument> audiencePreferences = new List<AudiencePreferenceDocument>();
            ViewBag.audiencePreferences = manager.GetPreferencesByAudienceId(audienceId);
            //manager.EnqueueAudienceChange(new AudienceChangeNotification {
            //    idAudience = audienceId,
            //    ChangeType = TypeAudienceChange.,
                
            //});
            var publisherProfileDocument = _publisherProfileManager.FindProfileDocByUserId(User.Identity.GetUserId());
            viewModel.audience = manager.GetAudienceById(audienceId);
            viewModel.audience.WebSiteUrl = viewModel.audience.WebSiteUrl.Replace("https://", "").Replace("http://", "");
            string Id = User.Identity.GetUserId();
            ViewBag.ImageSrcPreview = publisherProfileDocument.IconUrl;
            ViewBag.descriptionPreview = publisherProfileDocument.Description;
            return View(viewModel);
            
        }

        [HttpPost]
        public ActionResult EditAudience( AudienceViewModel viewModel)
        {
            string userId = User.Identity.GetUserId();

            try {
                var protocol = viewModel.audience.UrlProtocol == "1" ? "https://" : "http://";
                viewModel.audience.WebSiteUrl = string.Format("{0}{1}", protocol, viewModel.audience.WebSiteUrl.Replace("www.", "").Replace("https://", "").Replace("http://", ""));

                //Subimos imagen de icono en caso de que sea una nueva y la seteamos al VM
                bool newIconImage = true;
                newIconImage = string.IsNullOrEmpty(viewModel.audience.IconUrl);
                List<Models.Core.FileUpload> images = null;
                if (newIconImage) {
                    images = GetAndSetIconUrl(viewModel);
                }

                //Submis imagen en de que sea nueva y la seteamos al VM
                bool newImgImage = true;
                newImgImage = string.IsNullOrEmpty(viewModel.audience.ImageUrl);
                if (newImgImage) {
                    GetAndSetImageUrl(viewModel, images);
                }

                viewModel.audience.Description = Negocio.Utilerias.StringUtilities.RemoveNewLineInString(viewModel.audience.Description);

                //Seteamos campos basicos
                AudienceDocument audienceCandidate = manager.GetAudienceById(viewModel.audience.Id);
                string imgUrl = newImgImage ? viewModel.audience.ImageUrl : audienceCandidate.ImageUrl;
                bool changeImage = imgUrl != audienceCandidate.ImageUrl;
                bool changeDescription = audienceCandidate.Description != viewModel.audience.Description;
                bool changeVisibility = (audienceCandidate.Visibility != viewModel.audience.Visibility) && !viewModel.audience.Visibility;


                audienceCandidate.Title = viewModel.audience.Title;
                audienceCandidate.Tagline = viewModel.audience.Tagline;
                audienceCandidate.UrlProtocol = viewModel.audience.UrlProtocol;
                audienceCandidate.Description = viewModel.audience.Description;
                audienceCandidate.WebSiteUrl = viewModel.audience.WebSiteUrl;
                audienceCandidate.IconUrl = newIconImage ? viewModel.audience.IconUrl : audienceCandidate.IconUrl;
                audienceCandidate.ImageUrl = imgUrl;
                audienceCandidate.HowManyAdvertisers = viewModel.audience.HowManyAdvertisers;
                audienceCandidate.PeopleInYourBusiness = viewModel.audience.PeopleInYourBusiness;
                audienceCandidate.CountryBusinessInId = viewModel.audience.CountryBusinessInId;
                audienceCandidate.YearFounded = viewModel.audience.YearFounded;
                audienceCandidate.Visibility = viewModel.audience.Visibility;
                audienceCandidate.CategoryId = viewModel.audience.CategoryId;
                audienceCandidate.SubCategoryId = viewModel.audience.SubCategoryId;
                //Persistimos audience candidate and preferences
                AudienceDocument audience = manager.UpdateAudience(audienceCandidate, viewModel.preferencesStringify);
                //viewModel.audience = audience;

                if (changeDescription) {
                    manager.EnqueueAudienceChange(new AudienceChangeNotification {
                        ChangeType = TypeAudienceChange.ChangeDescription,
                        idAudience = viewModel.audience.Id,
                        Description = audienceCandidate.Description
                    });
                }

                if (changeImage) {
                    manager.EnqueueAudienceChange(new AudienceChangeNotification {
                        ChangeType = TypeAudienceChange.ChangeImage,
                        idAudience = viewModel.audience.Id,
                        ImageUrl = imgUrl
                    });
                }

                if (changeVisibility) {
                    manager.EnqueueAudienceChange(new AudienceChangeNotification {
                        ChangeType = TypeAudienceChange.DisableVisibility,
                        idAudience = viewModel.audience.Id
                    });
                }

                return Json(new { success = true, message = "Audience edited successfully", audienceId = viewModel.audience.Id });
            }
            catch(Exception ex) {
                return Json(new { error = "Error editing Audience"  });
            }
          
        }

        private void GetAndSetImageUrl(AudienceViewModel viewModel, List<Models.Core.FileUpload> images = null)
        {
            // create Id
            Guid profileId = Guid.NewGuid();

            // get images and urls
            if (images == null) {
                images = GetFilesFromCurrentRequest();
            }

            Dictionary<string, string> urls = GetUrls(images);

            // check if Img is uploaded
            string value = string.Empty;
            urls.TryGetValue("fileupImg", out value);

            if (value != string.Empty) {
                viewModel.audience.ImageUrl = value;
            }
        }


        private List<Models.Core.FileUpload> GetAndSetIconUrl(AudienceViewModel viewModel, List<Models.Core.FileUpload> images = null)
        {
            // create Id
            Guid profileId = Guid.NewGuid();

            // get images and urls
            if (images == null) {
                images = GetFilesFromCurrentRequest();
            }
            
            Dictionary<string, string> urls = GetUrls(images);

            // check if Img is uploaded
            string value = string.Empty;
            urls.TryGetValue("fileupIcon", out value);

            if (value != string.Empty) {
                viewModel.audience.IconUrl = value;
            }

            return images;
        }

        private List<Models.Core.FileUpload> GetFilesFromCurrentRequest()
        {
            List<Models.Core.FileUpload> files = new List<Models.Core.FileUpload>();

            for (int i = 0; i < Request.Files.Count; i++) {
                Models.Core.FileUpload fileUpload = null;
                fileUpload = new Models.Core.FileUpload();
                var file = Request.Files[i];
                
                fileUpload.HtmlItem = Request.Files.AllKeys[i];
                if (file != null && !String.IsNullOrEmpty(file.FileName)) {
                    fileUpload.Filextension = Helpers.FileHelper.GetFileExtension(file.FileName);
                    
                    if (file != null && file.ContentLength > 0) {
                        using (var binaryReader = new BinaryReader(file.InputStream)) {
                            fileUpload.FileData = binaryReader.ReadBytes(file.ContentLength);
                            files.Add(fileUpload);
                        }
                    }
                }
            }
            return files;
        }

        private Dictionary<string, string> GetUrls(List<Models.Core.FileUpload> files)
        {
            Dictionary<string, string> urls = new Dictionary<string, string>();

            foreach (var file in files) {
                string url = GetImageAzure(file);
                if (url != string.Empty) {
                    var htmlItem = file.HtmlItem;
                    urls.Add(htmlItem, url);
                }
            }
            return urls;
        }

        private string GetImageAzure(Models.Core.FileUpload fileUpload)
        {
            if (fileUpload.FileData != null && fileUpload.Filextension != null) {
                return Helpers.AzureStorageHelper.CreateBlobImageFile(fileUpload.FileData, fileUpload.Filextension);
            }
            else { return ""; }
        }

        [HttpPost]
        public ActionResult CreateAudienceImage(AudienceViewModel viewModel)
        {
            bool operationResult = false;
            AudienceViewModel viewModelStored = GetViewModelFromSession();
            GetAndSetImageUrl(viewModelStored);
            viewModelStored.audience.IsActive = true;
            manager.AddIconUrl(viewModelStored);
            TempData["audienceId"] = viewModelStored.audience.Id;

            //return RedirectToAction("VerifyWebSite", "PublisherProfile", new { audienceId = viewModelStored.audience.Id });
            return operationResult ? Json(new { success = true, message = "Audience created successfully", audienceId = viewModelStored.audience.Id }) : Json(new { error = "Error creating Audience" });
        }


        [HttpPost]
        public ActionResult CreateAudiencePreference(AudienceViewModel viewModel)
        {
            // todo
            AudienceViewModel viewModelStored = GetViewModelFromSession();

            // update
            viewModelStored.audience.HowManyAdvertisers = viewModel.audience.HowManyAdvertisers;
            viewModelStored.audience.Visibility = viewModel.audience.Visibility;
            viewModelStored.preferencesStringify = viewModel.preferencesStringify;

            // add preferences
            manager.AddPreferences(viewModelStored,viewModelStored.audience.Id);

            // set stage
            viewModelStored.stage = "images";

            HoldViewModel(viewModelStored);
            return View("CreateAudience", viewModelStored);
        }

        public ActionResult GoBack()
        {
            AudienceViewModel viewModel = GetViewModelFromSession();

            // select preview stage
            if (viewModel.stage== "preferences") {
                viewModel.stage = "basic";
            }
            if (viewModel.stage == "images") {
                viewModel.stage = "preferences";
            }

            HoldViewModel(viewModel);
            return View("CreateAudience", viewModel);
        }

        public ActionResult Audience(string audienceIdaudienceId)
        {
            return View();
        }

        private AudienceViewModel GetViewModelFromSession()
        {
            AudienceViewModel viewModel = (AudienceViewModel)Session[sessionKey];
            return viewModel;
        }

        private void HoldViewModel(AudienceViewModel viewModel)
        {
            if (viewModel.audience == null) {
                viewModel.audience = new AudienceDocument();
            }
            if (viewModel.manager == null) {
                viewModel.manager = new AudienceManager();
            }
            if (viewModel.telemetria == null) {
                viewModel.telemetria = new Trace();
            }
            Session[sessionKey] = viewModel;
        }

      

    }

}
