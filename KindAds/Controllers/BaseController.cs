using KindAds.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Resources;
using System.Collections;
using System.Globalization;
using KindAds.Services;
using System.IO;
using System.Threading.Tasks;
using KindAds.Common.Interfaces;
using KindAds.Azure;

namespace KindAds.Controllers {


    public class BaseController : Controller, ITelemetria {
        AccessService accessService;

        public ITrace telemetria { set; get; }

        public BaseController()
        {
            accessService = new AccessService();
            telemetria = new Trace();
        }

        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            try {
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
            catch (Exception e) {
                //
            }
            return null;
        }

        public async Task<JsonResult> GetLanguages()
        {
            List<LanguageResource> list = new List<LanguageResource>();
            LanguageResource item;
            //ResourceManager MyResourceClass = new ResourceManager(typeof(KindAds.Comun.LanguageResources.LanguagesResources /* Reference to your resources class -- may be named differently in your case */));

            ResourceSet resourceSet = KindAds.Comun.LanguageResources.LanguagesResources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            foreach (DictionaryEntry entry in resourceSet) {
                item = new LanguageResource() {
                    Name = entry.Key.ToString(),
                    Language = entry.Value.ToString()
                };

                list.Add(item);
            }

            return Json(new SelectList(list, "Language", "Name"));
        }

        public JsonResult ShowMigrateWallet()
        {
            try {
                if (User.Identity.IsAuthenticated) {                    
                    var userId = Microsoft.AspNet.Identity.IdentityExtensions.GetUserId(User.Identity);
                    var userDetail = accessService.GetUserDetailByIdUser(new Guid(userId).ToString());
                    bool _isMigrate = userDetail != null ? userDetail.IsMetamask : false;
                    return Json(new { success = true, isMigrate = _isMigrate });
                }
                return Json(new { success = true, isMigrate = true });
            }
            catch (Exception) {
                return Json(new { success = false, isMigrate = false });
            }
        }

        public JsonResult MigrateWalletEthereum(string address)
        {
            try {
                if (User.Identity.IsAuthenticated) {
                    var userId = Microsoft.AspNet.Identity.IdentityExtensions.GetUserId(User.Identity);

                    bool _result = accessService.UpdateUserWallet(userId, address);
                    return Json(new { success = true, result = true });
                }
                return Json(new { success = true, result = false });
            }
            catch (Exception) {
                return Json(new { success = false, result = false });
            }
        }

        public class LanguageResource {
            public string Name { set; get; }
            public string Language { set; get; }
        }

        public List<Models.Core.FileUpload> GetFiles()
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

        public Dictionary<string, string> GetUrls(List<Models.Core.FileUpload> files, int width, int height)
        {
            Dictionary<string, string> urls = new Dictionary<string, string>();

            foreach (var file in files) {
                string url = GetImageAzure(file, width, height);
                if (url != string.Empty) {
                    var htmlItem = file.HtmlItem;
                    urls.Add(htmlItem, url);
                }
            }
            return urls;
        }

        private string GetImageAzure(Models.Core.FileUpload fileUpload, int width, int height)
        {
            if (fileUpload.FileData != null && fileUpload.Filextension != null) {
                return Helpers.AzureStorageHelper.CreateBlobImageFile(fileUpload.FileData, fileUpload.Filextension,width,height);
            }
            else { return ""; }
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;

            string ErrorId = Guid.NewGuid().ToString();
            string messageException = string.Format("ErrorId:{0} ,Details:{1}", ErrorId,
                telemetria.MakeMessageException(filterContext.Exception, System.Reflection.MethodBase.GetCurrentMethod().Name)
                );
            telemetria.Critical(messageException);

            // OR
            TempDataDictionary errors = new TempDataDictionary();
            errors.Add("ErrorId", ErrorId);

            filterContext.Result = new ViewResult {
                ViewName = "~/Views/Error/InternalServer.cshtml",
                TempData = errors
            };
        }

    }
}
