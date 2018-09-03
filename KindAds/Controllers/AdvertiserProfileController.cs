using KindAds.AuthorizeAttributes;
using KindAds.Business.Managers;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using KindAds.Comun.Models.ViewModel.KindAdsV2;
using KindAds.Negocio.Managersv2;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KindAds.Controllers
{
    [Authorize(Roles="Advertiser")]
    public class AdvertiserProfileController : BaseController
    {
        public AdvertiserProfileManager manager { set; get; }
        private readonly CosmosIdentityManager _cosmosIdentityManager;
        
        
        public AdvertiserProfileController()
        {
            manager = new AdvertiserProfileManager();
            _cosmosIdentityManager = new CosmosIdentityManager();
        }

        // GET: Create Advertiser Profile
        [PreventCreateProfile]
        public ActionResult CreateProfile()
        {          
            AdvertiserProfileViewModel vm = new AdvertiserProfileViewModel();
            return View(vm);
        }

        [ValidProfile]
        // GET: Advertiser Profile
        public new ActionResult Profile()
        {
            string userId = User.Identity.GetUserId();
            AdvertiserProfileViewModel profile = manager.FindProfileByUserId(userId);
            List<AdvertiserPreferenceDocument> preferences = manager.FindPreferencesByProfileId(profile.profile.Id);
            ViewBag.Preferences = preferences;
            ApplicationUser appUser = _cosmosIdentityManager.FindUserByUserId(userId);
            ViewBag.Name = appUser.Name;
            ViewBag.Email = appUser.Email;

            return View(profile);
        }

        [HttpPost]
        public JsonResult EditProfile(AdvertiserProfileViewModel model)
        {
            try {
                string userId = User.Identity.GetUserId();
                SetProfile(model);
                manager.Update(model.profile, model.preferences);

                return Json(new { success = true, message = "Profile updated sucessfull" });

            }
            catch (Exception ex) {
                return Json(new { error = "Error updating profile" });
            }

        }

        [HttpPost]
        [PreventCreateProfile]
        public ActionResult CreateProfile(AdvertiserProfileViewModel viewModel)
        {
            bool result = false;
            SetProfile(viewModel);
            result=manager.AddProfile(viewModel);

            if (result) {
                return result ? Json(new { success = true, message = "Profile create sucessfull" }) : Json(new { error = "Error creating profile" });
            }

            return View(viewModel);
        }


        private void SetProfile(AdvertiserProfileViewModel viewModel)
        {
            var advertiser = manager.FindProfileByUserId(User.Identity.GetUserId());
            // create Id
            Guid profileId;
            if (advertiser == null)
                profileId = Guid.NewGuid();
            else {
                profileId = Guid.Parse(advertiser.profile.Id);
                viewModel.profile.RegisterDate = advertiser.profile.RegisterDate;
            }
                

            // get images and urls
            List<Models.Core.FileUpload> images = GetFiles();
            Dictionary<string, string> urls = GetUrls(images);

            //// check if Img is uploaded
            string value = string.Empty;
            urls.TryGetValue("fileupImg", out value);

            if (!string.IsNullOrEmpty(value )) {
                viewModel.profile.PhotoUrl = value;
            }
            else {
                if (advertiser != null) {
                    viewModel.profile.PhotoUrl = advertiser.profile.PhotoUrl;
                }
                else {
                    viewModel.profile.PhotoUrl = string.Empty;
                }
            }

            // check if icon is uploaded
             value = string.Empty;
            urls.TryGetValue("fileupIcon", out value);
            if (!string.IsNullOrEmpty(value)) {
                viewModel.profile.IconUrl = value;
            }
            else {
                if (advertiser != null) {
                    viewModel.profile.IconUrl = advertiser.profile.IconUrl;
                }
                else {
                    viewModel.profile.IconUrl = string.Empty;
                }
            }

            // add preferences
            viewModel.preferences = manager.GetPreferences(viewModel.preferencesStringify, profileId.ToString());
            viewModel.profile.AdvertiserNeeds = Negocio.Utilerias.StringUtilities.RemoveNewLineInString(viewModel.profile.AdvertiserNeeds);
            // update Id
            viewModel.profile.Id = profileId.ToString();
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

        private Dictionary<string,string> GetUrls(List<Models.Core.FileUpload> files)
        {
            Dictionary<string,string> urls = new Dictionary<string,string>();

            foreach(var file in files) {
                string url = GetImageAzure(file,160,160);
                if(url!=string.Empty) {
                    var htmlItem = file.HtmlItem;
                    urls.Add(htmlItem, url);
                }                
            }
            return urls;
        }


        private string GetImageAzure(Models.Core.FileUpload fileUpload, int width, int height)
        {
            if (fileUpload.FileData != null && fileUpload.Filextension != null) {
                return Helpers.AzureStorageHelper.CreateBlobImageFile(fileUpload.FileData, fileUpload.Filextension, width, height);
            }
            else { return ""; }
        }


    }
}
