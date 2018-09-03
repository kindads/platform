using KindAds.Azure;
using KindAds.Common.Models;
using KindAds.Common.Models.Entities;
using KindAds.Common.Models.ViewModel;
using KindAds.Common.Utils;
using KindAds.DataAccess;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace KindAds.Business.ViewModels
{
    public  class SiteViewModelManager
    {
        public SiteViewModel viewModel { set; get; }
        

      

        public SiteViewModelManager()
        {
            viewModel = new SiteViewModel();
            
            InitializationObject();
        }

        public void CleanSessionProperties()
        {
            viewModel.CategorySelecc = string.Empty;
            viewModel.ProductTypeSelecc = string.Empty;
            viewModel.CategorySelecc = string.Empty;
            viewModel.TagSelecc = string.Empty;
            viewModel.PartnerSelecc = string.Empty;
            viewModel.SiteSelecc = string.Empty;
            viewModel.IdSite = string.Empty;
        }

        public SelectList Categories()
        {
            CategoryRepository repository = new CategoryRepository() ;
            List<CategoryEntity> categories = repository.GetAll().ToList();
            return new SelectList(categories, "IdCategory", "Description");
        }

        public SelectList Protocols()
        {
            List<string> listProtocol = new List<string>();
            listProtocol.Add("https://");
            listProtocol.Add("http://");
            return  new SelectList(listProtocol);
        }

        public void InitializationObject()
        {
            CleanSessionProperties();
        }

        public bool AddADSettings(SiteViewModel model, Guid IdSite)
        {
            bool result = false;
            //todo
            // Guardar los settings en la tabla
            return result;
        }

        public bool AddToken(Guid IdSite)
        {
            bool result = false;
            SiteRepository siteRepository = new SiteRepository() ;
            SiteEntity site = siteRepository.FindById(IdSite);
            site.Token = CreateToken(site);
            
            result = true;
            return result;
        }

        public string CreateToken(SiteEntity site)
        {
            string token = string.Empty;
            //Todo
            SiteToken siteToken = new SiteToken
            {
                Name = site.Name,
                Url = site.URL,
                SiteId = site.IdSite
            };

            string siteTokenRow = JsonConvert.SerializeObject(siteToken);
            token = Security.GetSha256(siteTokenRow);
            return token;
        }


        public bool CreateSite(SiteViewModel viewModel,string userId,out string message, out string IdSite)
        {
            bool result = false;
            IdSite = string.Empty;
            message = string.Empty;

            //Todo
            var model = viewModel;
            string protocolSelecc = viewModel.protocoloSelected;
            model.CategoryTypeSelect = viewModel.categorySelected;
            model.URL = String.IsNullOrEmpty(model.URL) ? "-" : (String.IsNullOrEmpty(protocolSelecc) ? "https://" : protocolSelecc) + model.URL;

            //Create site
            CategoryRepository categoryRepository = new CategoryRepository() ;
            var allCategory = categoryRepository.GetAll();
            CategoryEntity _category = allCategory.FirstOrDefault(x => x.IdCategory == model.CategoryTypeSelect);

            AspNetUserRepository aspNetUserRepository = new AspNetUserRepository() ;
            AspNetUserEntity _aspnetuser = aspNetUserRepository.GetAll().FirstOrDefault(x => x.Id == userId);

            SiteEntity _site = new SiteEntity
            {
                Name = model.Name,
                URL = model.URL,
                IdSite = Guid.NewGuid(),

                AspNetUsers_Id = userId,
                Verified = false,
                VerificationString = Guid.NewGuid().ToString(),
                RegistrationDate = DateTime.Now,
                IsActive = true
            };

            SiteRepository siteRepository = new SiteRepository() ;
            

            try
            {
                siteRepository.Add(_site);
                CategorySiteEntity categorySiteEntity = new CategorySiteEntity()
                {
                    CATEGORY_IdCategory = _category.IdCategory,
                    SITEs_IdSite = _site.IdSite
                };

                CategorySiteRepository categorySiteRepository = new CategorySiteRepository() ;
                categorySiteRepository.Add(categorySiteEntity);
                

                IdSite= _site.IdSite.ToString();
                message = "Site created successfully";
                result = true;
            }
            catch (Exception e)
            {
                message = "Error creating site";
            }
            return result;
        }

        //VerifyAzureSite
        public bool VerifyAzureSite(Guid IdSite, AzureADSiteValidation properties, int Type)
        {
            bool result = false;
            result = SiteWithADVerified(IdSite, properties);
            return result;
        }

        public bool VerifySite(Guid IdSite,int Type)
        {
            bool result = false;
            switch (Type)
            {               
                case (int)EnumTypeSiteValidation.Gtm:
                    {
                        result = SiteWithTokenVerified(IdSite);
                    }
                    break;
                case (int)EnumTypeSiteValidation.Txt:
                    {
                        result = SiteWithTxtVerified(IdSite);
                    }
                    break;
            }
            return result;
        }

        public string GetGoogleTagManagerToken(string IdSite)
        {
            string Token = string.Empty;
            SiteRepository repository = new SiteRepository() ;
            SiteEntity site = repository.FindById(new Guid(IdSite));

            if (site.Token == null)
            {
                Token = CreateToken(site);
                site.Token = Token;

            }
            else
            {
                Token = site.Token;
            }

            return Token;
        }
        public bool SiteWithTokenVerified(Guid IdSite)
        {
            bool result = false;
            SiteEntity site = GetSiteById(IdSite.ToString());
            result = (bool)site.Verified;
            return result;
        }

        public SiteEntity GetSiteById(string IdSite)
        {
            SiteRepository repository = new SiteRepository() ;
            SiteEntity site = repository.FindById(new Guid(IdSite));
            return site;
        }

        public ShowSitesViewModel GetSitesPendingByIdUser(string idUser, int page = 1, string sort = "URL", string sortdir = "ASC")
        {
            int pageSize = 4;
            ShowSitesViewModel m = new ShowSitesViewModel();
            SiteRepository siteRepository = new SiteRepository() ;
            IEnumerable<SiteEntity> sitesAvailable = siteRepository.GetAll().Where(x => x.IsActive.Value).ToList();
            m.PageSize = pageSize;
            m.TotalRecord = (from r in sitesAvailable where r.AspNetUsers_Id.Equals(idUser) && r.Verified == false select r).Count();
            m.NoOfPages = (m.TotalRecord / m.PageSize) + ((m.TotalRecord % m.PageSize) > 0 ? 1 : 0);
            var ListSitesPending= (from site in sitesAvailable
                                   orderby (sort + " "+ sortdir)
                                   where site.AspNetUsers_Id.Equals(idUser) &&
                                   site.Verified==false
                                   select site).Skip((page - 1) * m.PageSize).Take(m.PageSize).ToList();
            m.ListSitesPending = ListSitesPending;
            return m;
        }

        public SiteEntity UpdateSingleSite(SiteEntity site)
        {
            SiteRepository siteRepository = new SiteRepository() ;
            siteRepository.Edit(site);
            
            return site;
        }

        public bool DeleteSite(Guid IdSite)
        {
            Boolean result = true;
            SiteEntity site = GetSiteById(IdSite.ToString());
            if (site.PRODUCTs.Any()) 
            {
                result = false;
            }
            else
            {
                site.IsActive = false;
                UpdateSingleSite(site);
            }
            return result;
        }
        public ShowSitesViewModel GetSitesVerifyByIdUser(string idUser, int page = 1, string sort = "URL", string sortdir = "ASC")
        {
            int pageSize = 4;
            ShowSitesViewModel m = new ShowSitesViewModel();
            SiteRepository siteRepository = new SiteRepository ();
            List<SiteEntity> sitesAvailable = siteRepository.GetAll().Where(x => x.IsActive.Value).ToList();
            m.PageSize = pageSize;
            m.TotalRecord = (from r in sitesAvailable where r.AspNetUsers_Id.Equals(idUser) && r.Verified == true select r).Count();
            m.NoOfPages = (m.TotalRecord / m.PageSize) + ((m.TotalRecord % m.PageSize) > 0 ? 1 : 0);
            var ListSitesVerify = (from site in sitesAvailable
                                    orderby (sort + " " + sortdir)
                                    where site.AspNetUsers_Id.Equals(idUser) &&
                                    site.Verified == true
                                    select site).Skip((page - 1) * m.PageSize).Take(m.PageSize).ToList();
            m.ListSitesVerify = ListSitesVerify;
            return m;
        }

        public string CreateScriptGtm(Guid IdSite)
        {
            StringBuilder scriptM = new StringBuilder();
            //todo
            string token = GetGoogleTagManagerToken(IdSite.ToString());

            //generate token
            scriptM.AppendLine();

#if DEV 
            scriptM.AppendLine("<script src=\"https://kindadsscripts.blob.core.windows.net/site-validation-dev/KindAdsSites.js\"></script>");
#elif QA
            scriptM.AppendLine("<script src=\"https://kindadsscripts.blob.core.windows.net/site-validation-qa/KindAdsSites.js\"></script>");
#elif STAGING
            scriptM.AppendLine("<script src=\"https://kindadsscripts.blob.core.windows.net/site-validation-staging/KindAdsSites.js\"></script>");
#else
            scriptM.AppendLine("<script src=\"https://kindadsscripts.blob.core.windows.net/site-validation-prod/KindAdsSites.js\"></script>");
#endif

            scriptM.AppendLine("<script type=\"text / javascript\">");
#if DEV 
            scriptM.AppendLine("var url = 'https://captivateapi-dev.azurewebsites.net/api/site';");
#elif QA
            scriptM.AppendLine("var url = 'https://captivateapi-qa.azurewebsites.net/api/site';");
#elif STAGING
            scriptM.AppendLine("var url = 'https://captivateapi-staging.azurewebsites.net/api/site';");
#else
            scriptM.AppendLine("var url = 'https://captivateapi.azurewebsites.net/api/site';");
#endif
            scriptM.AppendLine(string.Format(" var apiToken = '{0}';", token));
            scriptM.AppendLine(string.Format(" var idSite = '{0}';", IdSite.ToString().ToUpper()));
            scriptM.AppendLine(" kindAds.validateSite(url, apiToken, idSite);");
            scriptM.AppendLine(" </script>");

            return scriptM.ToString();
        }

        public FileStreamResult CreateScriptFile(Guid IdSite)
        {
            SiteEntity _site = GetSiteById(IdSite.ToString());

            if (_site != null)
            {
                string datafile = CreateScriptGtm(IdSite);

                if (datafile.Length > 0)
                {
                    var byteArray = System.Text.Encoding.ASCII.GetBytes(datafile);
                    var stream = new System.IO.MemoryStream(byteArray);

                    System.Web.Mvc.FileStreamResult _sfile = new System.Web.Mvc.FileStreamResult(stream, "text/plain");
                    _sfile.FileDownloadName = "scriptGTM.txt";

                    return _sfile;
                }
                else
                {
                    return null;
                }
            }
            return null;
        }

        public FileStreamResult CreateVerificationFile(Guid IdSite)
        {
            SiteEntity _site = GetSiteById(IdSite.ToString());

            if (_site != null)
            {
                string datafile = _site.VerificationString;

                if (datafile.Length > 0)
                {
                    var byteArray = System.Text.Encoding.ASCII.GetBytes(datafile);
                    var stream = new System.IO.MemoryStream(byteArray);

                    System.Web.Mvc.FileStreamResult _sfile = new System.Web.Mvc.FileStreamResult(stream, "text/plain");
                    _sfile.FileDownloadName = "kindads.txt";

                    return _sfile;
                }
                else
                {
                    return null;
                }
            }
            return null;
        }

        public bool SiteWithTxtVerified(Guid IdSite)
        {
            bool result = false;
            SiteRepository siteRepository = new SiteRepository() ;
            SiteEntity _site = siteRepository.FindById(IdSite);
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(_site.URL.ToString() + "/kindads.txt");
            request.Method = "GET";

            try
            {
                System.Net.WebResponse webResponse = request.GetResponse();
                using (System.IO.Stream webStream = webResponse.GetResponseStream())
                {
                    if (webStream != null)
                    {
                        using (System.IO.StreamReader responseReader = new System.IO.StreamReader(webStream))
                        {
                            var _response = responseReader.ReadToEnd();
                            if (_response != null)
                            {
                                if (_response.Trim() == _site.VerificationString.Trim())
                                {
                                    //Verify Site
                                    _site.Verified = true;
                                    siteRepository.Edit(_site);
                                    result = true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                //Do nothing
            }
            return result;
        }


        public void CreateIfNotExist(Guid IdSite, AzureADSiteValidation Properties)
        {
            //Checamos si ya estan las propiedades
            AzureSubscriptionRepository repository = new AzureSubscriptionRepository() ;
            AzureSupcriptionEntity storedEntity = repository.FindById(IdSite);
            if (storedEntity == null)
            {
                //Store data
                AzureSupcriptionEntity entity = new AzureSupcriptionEntity()
                {
                    IdSite = IdSite,
                    ClientAppId = Properties.ClientAppId,
                    SubscriptionId = Properties.SubscriptionId,
                    TenantId = Properties.TenantId,
                    AppKey = Properties.AppKey
                };

                repository.Add(entity);
                
            }
        }
      
        public bool SiteWithADVerified(Guid IdSite, AzureADSiteValidation Properties)
        {
            bool result = false;
            SiteEntity site = GetSiteById(IdSite.ToString());
            string SiteUrl = site.URL;
            string Token = string.Empty;

            Token = ADManager.GetToken(Properties.TenantId, Properties.ClientAppId, Properties.AppKey);
            if (Token != string.Empty)
            {
                CreateIfNotExist(IdSite, Properties);
                result = ADManager.ValidateSite(SiteUrl, Token, Properties.SubscriptionId);                
            }
            return result;
        }
    }
}
