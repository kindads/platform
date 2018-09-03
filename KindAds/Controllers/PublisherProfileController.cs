using KindAds.AuthorizeAttributes;
using KindAds.Business.Managers;
using KindAds.Common.Utils;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using KindAds.Comun.Models.ViewModel.KindAdsV2;
using KindAds.Negocio.Managersv2;
using KindAds.Negocio.ViewModels.KindAdsV2;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace KindAds.Controllers {
    [Authorize(Roles = "Publisher")]
    public class PublisherProfileController : BaseController {
        private PublisherProfileManager publisherProfileManager;
        private SiteViewModelManager siteViewModelManager;
        private AudienceManager audienceManager;

        private readonly CosmosIdentityManager _cosmosIdentityManager;


        public PublisherProfileController()
        {
            publisherProfileManager = new PublisherProfileManager();
            siteViewModelManager = new SiteViewModelManager();
            audienceManager = new AudienceManager();
            _cosmosIdentityManager = new CosmosIdentityManager();
        }

        [PreventCreateProfile]
        // GET: Create Publisher Profile
        public ActionResult CreateProfile()
        {
            PublisherProfileViewModel viewModel = new PublisherProfileViewModel();
            return View(viewModel);
        }

        [ValidProfile]
        // GET: Publisher Profile
        public ActionResult Profile()
        {
            
            string userId = User.Identity.GetUserId();
            PublisherProfileViewModel profile = publisherProfileManager.FindProfileByUserId(userId);
            List<PublisherPreferenceDocument> preferences = publisherProfileManager.FindPreferencesByProfileId(profile.profile.Id);
            ViewBag.Preferences = preferences;
            ApplicationUser appUser = _cosmosIdentityManager.FindUserByUserId(userId);
            ViewBag.Name = appUser.Name;
            ViewBag.Email = appUser.Email;



            return View(profile);
        }

        [HttpPost]
        public JsonResult EditProfile(PublisherProfileViewModel profile)
        {
            try {
                string userId = User.Identity.GetUserId();

                SetProfile(profile);
                publisherProfileManager.Update(profile.profile, profile.preferences);


                return Json(new { success = true, message = "Profile updated sucessfull" });
                
            }
            catch(Exception ex) {
                return Json(new { error = "Error updating profile" });
            }
    
        }

        [HttpPost]
        [PreventCreateProfile]
        public ActionResult CreateProfile(PublisherProfileViewModel model)
        {
            bool result = false;
            SetProfile(model);
            result = publisherProfileManager.AddProfile(model);

            if (result) {
                return result ? Json(new { success = true, message = "Profile create sucessfull" }) : Json(new { error = "Error creating profile" });
            }

            model = new PublisherProfileViewModel();
            return View(model);
        }

        public JsonResult FillCatIndustry()
        {
            List<IndustryDocument> listCatIndustry = publisherProfileManager.GetCatIndustry();
            return Json(new SelectList(listCatIndustry, "Id", "Name"));
        }

        public JsonResult FillCatCountry()
        {
            List<CountryDocument> listCatCountry = publisherProfileManager.GetCatCountry();
            return Json(new SelectList(listCatCountry, "Id", "Name"));
        }

        public JsonResult FillCatBusinessExpertise()
        {
            List<BusinessExpertiseDocument> listCatBusinessExpertise = publisherProfileManager.GetCatBusinessExpertise();
            return Json(new SelectList(listCatBusinessExpertise, "Id", "Name"));
        }

        private List<FileUpload> GetFilesUpload()
        {
            FileUpload fileUpload = null;
            List<FileUpload> listFileUpload = new List<FileUpload>();
            for (int i = 0; i < Request.Files.Count; i++) {
                fileUpload = new FileUpload();
                var file = Request.Files[i];
                fileUpload.HtmlItem = Request.Files.AllKeys[i];
                if (file != null && !String.IsNullOrEmpty(file.FileName)) {
                    fileUpload.Filextension = Helpers.FileHelper.GetFileExtension(file.FileName);

                    if (file != null && file.ContentLength > 0) {
                        using (var binaryReader = new BinaryReader(file.InputStream)) {
                            fileUpload.FileData = binaryReader.ReadBytes(file.ContentLength);
                            listFileUpload.Add(fileUpload);
                        }
                    }
                }
            }
            return listFileUpload;
        }

        private void SetProfile(PublisherProfileViewModel viewModel)
        {
            viewModel.Description = viewModel.Description.Trim();
            // create Id
            var publisher = publisherProfileManager.FindProfileByUserId(User.Identity.GetUserId());
            Guid profileId;
            if (publisher == null)
                profileId = Guid.NewGuid();
            else {
                profileId = Guid.Parse(publisher.profile.Id);
                viewModel.profile.RegisterDate = publisher.profile.RegisterDate;
            }

            
            // get images and urls
            List<Models.Core.FileUpload> images = GetFiles();
            Dictionary<string, string> urls = UploadAndGetUrls(images);

            // check if icon is uploaded
            string value = string.Empty;
            urls.TryGetValue("fileupIcon", out value);
            if (!string.IsNullOrEmpty (value) ) {
                viewModel.profile.IconUrl = value;
            }
            else {
                if (publisher != null) {
                    viewModel.profile.IconUrl = publisher.profile.IconUrl;
                    
                }
                else {
                    viewModel.profile.IconUrl = string.Empty;
                }
                
            }

            // add preferences
            viewModel.preferences = publisherProfileManager.GetPreferences(viewModel.preferencesStringify, profileId.ToString());

            // update properties
            viewModel.profile.ExperienceLevel = viewModel.ExperienceLevel;
            viewModel.profile.Id = profileId.ToString();
            viewModel.profile.Name = viewModel.Name;
            viewModel.profile.Description = Negocio.Utilerias.StringUtilities.RemoveNewLineInString(viewModel.Description);
            viewModel.profile.CountryBusinessInId = viewModel.Country.ToString();
            viewModel.profile.UserId = User.Identity.GetUserId();
        }

        private List<Models.Core.FileUpload> GetFiles()
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

        private Dictionary<string, string> UploadAndGetUrls(List<Models.Core.FileUpload> files)
        {
            Dictionary<string, string> urls = new Dictionary<string, string>();
            string url = string.Empty;

            foreach (var file in files) {
                switch (file.HtmlItem) {
                    case "fileupImg":
                        url = GetImageAzure(file,480,480);
                        break;
                    case "fileupIcon":
                        url = GetImageAzure(file,160,160);
                        break;
                }                
                
                if (url != string.Empty) {
                    var htmlItem = file.HtmlItem;
                    urls.Add(htmlItem, url);
                }
            }
            return urls;
        }

        private string GetImageAzure(Models.Core.FileUpload fileUpload,int width, int height)
        {
            if (fileUpload.FileData != null && fileUpload.Filextension != null) {
                return Helpers.AzureStorageHelper.CreateBlobImageFile(fileUpload.FileData, fileUpload.Filextension, width, height);
            }
            else { return ""; }
        }

        public ActionResult VerifyWebSite(string audienceId)
        {
            //cargar el script y mostralo en la vista
            AudienceDocument audience = new AudienceDocument();
            VerifyAudienceViewModel model = new VerifyAudienceViewModel();
            model.gtm.Script = siteViewModelManager.CreateScriptGtm(new Guid(audienceId));
            model.gtm.ScriptByRows = siteViewModelManager.CreateScriptGtmByRows(new Guid(audienceId));

            if (audienceId != null) {
                AudienceDocument ad =  audienceManager.GetAudienceById(audienceId);
                ViewBag.site = ad.WebSiteUrl;
                TempData["tdIdAudience"] = audienceId;
                audience = ad;
                model.audience = audience;
            }

            return View(model);
        }

        public FileStreamResult DownloadFileSite(string IdAudience)
        {
            FileStreamResult result = siteViewModelManager.CreateVerificationFile(new Guid(IdAudience));
            return result;
        }

        public FileStreamResult DownloadScriptSite(string IdAudience)
        {
            FileStreamResult result = siteViewModelManager.CreateScriptFile(new Guid(IdAudience));
            return result;
        }

        public ActionResult VerifySiteTxtAndGTM(string typeValidation)
        {
            var Token = string.Empty;
            var result = siteViewModelManager.VerifySite(new Guid(TempData["tdIdAudience"].ToString()), Convert.ToInt16(typeValidation));
            return Json(new { success = result });
        }
    }
}
