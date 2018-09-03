using KindAds.Models;
using KindAds.Models.Publisher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Threading.Tasks;
using System.Linq.Dynamic;
using KindAds.DataAccess;
using KindAds.Common.Models.Entities;
using KindAds.Common.Models;
using System.Text;
using Newtonsoft.Json;
using System.Configuration;
using KindAds.Common.Utils;
using KindAds.Common.Models.ViewModel;
using KindAds.Azure;

namespace KindAds.Services
{
  public class SiteService
  {
    private KindadsEntities _context;
 
    public SiteService()
    {
 
      _context = new KindadsEntities();
    }

    //public bool AddToken(SiteViewModel model,Guid IdSite)
    //{
    //  bool result = false;
    //  SiteRepository siteRepository = new SiteRepository() { Context = context  };
    //  SiteEntity site = siteRepository.FindBy(st => st.IdSite == IdSite).FirstOrDefault();
    //  site.Token = CreateToken(site);
    //  siteRepository.Save();
    //  result = true;
    //  return result;
    //}

    //public string CreateToken(SiteEntity site)
    //{
    //  string token = string.Empty;
    //  //Todo
    //  SiteToken siteToken = new SiteToken
    //  {
    //    Name=site.Name,
    //    Url=site.URL,
    //    SiteId=site.IdSite
    //  };

    //  string siteTokenRow=JsonConvert.SerializeObject(siteToken);
    //  token = Security.GetSha256(siteTokenRow);
    //  return token;
    //}



    //public Guid? CreateSite(SiteViewModel createSiteModel, string userId)
    //{
    //  CategoryRepository categoryRepository = new CategoryRepository(){ Context = context };
    //  var allCategory = categoryRepository.GetAll();
    //  CategoryEntity _category = allCategory.FirstOrDefault(x => x.IdCategory == createSiteModel.CategoryTypeSelect);

    //  AspNetUserRepository aspNetUserRepository = new AspNetUserRepository() { Context = context };
    //  AspNetUserEntity _aspnetuser = aspNetUserRepository.GetAll().FirstOrDefault(x => x.Id == userId);

    //  SiteEntity _site = new SiteEntity
    //  {
    //    Name = createSiteModel.Name,
    //    URL = createSiteModel.URL,
    //    IdSite = Guid.NewGuid(),

    //    AspNetUsers_Id = userId,
    //    Verified = false,
    //    VerificationString = Guid.NewGuid().ToString(),
    //    RegistrationDate = DateTime.Now,
    //    IsActive = true,
    //    ValidationType= createSiteModel.type
    //  };

    //  SiteRepository siteRepository = new SiteRepository() { Context = context };
    //  siteRepository.Add(_site);

    //  try
    //  {
    //    siteRepository.Save();
    //    CategorySiteEntity categorySiteEntity = new CategorySiteEntity()
    //    {
    //      CATEGORY_IdCategory = _category.IdCategory,
    //      SITEs_IdSite = _site.IdSite
    //    };

    //    CategorySiteRepository categorySiteRepository = new CategorySiteRepository() { Context = context };
    //    categorySiteRepository.Add(categorySiteEntity);
    //    categorySiteRepository.Save();

    //    return _site.IdSite;
    //  }
    //  catch (System.Data.Entity.Validation.DbEntityValidationException e)
    //  {
    //    foreach (var eve in e.EntityValidationErrors)
    //    {
    //      String msg = String.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
    //          eve.Entry.Entity.GetType().Name, eve.Entry.State);
    //      foreach (var ve in eve.ValidationErrors)
    //      {
    //        msg = msg + String.Format("- Property: \"{0}\", Error: \"{1}\"",
    //            ve.PropertyName, ve.ErrorMessage);
    //      }
    //    }
    //    throw;
    //  }
    //  catch (Exception e)
    //  {
    //    return null;
    //  }
    //}

    //public bool SiteWithToken(Guid IdSite)
    //{
    //  bool result = false;
    //  SiteRepository repository = new SiteRepository() { Context = context };
    //  SiteEntity site = repository.FindBy(s => s.IdSite == IdSite).FirstOrDefault();
    //  result = site.Token == null ? false : true;
    //  return result;
    //}

    //public bool SiteWithTokenVerified(Guid IdSite)
    //{
    //  bool result = false;
    //  SiteRepository repository = new SiteRepository() { Context = context };
    //  SiteEntity site = repository.FindBy(s => s.IdSite == IdSite).FirstOrDefault();
    //  result = (bool)site.Verified;
    //  return result;
    //}

    //public bool SiteWithTxtVerified(Guid IdSite)
    //{
    //  bool result = false;
    //  SITE _site = (SITE)(from d in _context.SITES where d.IdSite.Equals(IdSite) select d).FirstOrDefault();     
    //  System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(_site.URL.ToString() + "/kindads.txt");
    //  request.Method = "GET";

    //  try
    //  {
    //    System.Net.WebResponse webResponse = request.GetResponse();
    //    using (System.IO.Stream webStream = webResponse.GetResponseStream())
    //    {
    //      if (webStream != null)
    //      {
    //        using (System.IO.StreamReader responseReader = new System.IO.StreamReader(webStream))
    //        {
    //          var _response = responseReader.ReadToEnd();
    //          if (_response != null)
    //          {
    //            if (_response.Trim() == _site.VerificationString.Trim())
    //            {
    //              //Verify Site
    //              _site.Verified = true;
    //              _context.SITES.Attach(_site);
    //              _context.Entry(_site).State = EntityState.Modified;
    //              result= _context.SaveChanges() > 0;
    //            }
    //          }
    //        }
    //      }
    //    }
    //  }
    //  catch (Exception Ex)
    //  {
    //    //Do nothing
    //  }
    //  return result;
    //}

    //public int GetValidationType(Guid IdSite)
    //{
    //  int result = 0;
    //  SiteRepository repository = new SiteRepository() { Context = context };
    //  SiteEntity site = repository.FindBy(s => s.IdSite == IdSite).FirstOrDefault();
    //  result = site.ValidationType;
    //  return result;
    //}

    //public bool SiteWithADVerified(Guid IdSite)
    //{
    //  bool result = false;
    //  //Todo
    //  // 1) Obtenemos el sitio
    //  // 2) Obtenemos los datos de validacion del sitio en una nueva tabla SiteValidation

    //  //Verificamos el sitio 
    //  string TenantId = string.Empty;
    //  string ApplicationId = string.Empty;
    //  string AppKey = string.Empty;
    //  string SiteUrl = string.Empty;
    //  string Token = string.Empty;
    //  string SubscriptionId = string.Empty;

    //  Token=ADManager.GetToken(TenantId, ApplicationId, AppKey);
    //  result=ADManager.ValidateSite(SiteUrl,Token, SubscriptionId);
    //  return result;
    //}

    //public bool VerifySite(Guid IdSite)
    //{
    //  bool result = false;     

    //  int ValidationType = GetValidationType(IdSite);

    //  switch(ValidationType)
    //  {
    //    case (int)EnumTypeSiteValidation.AzureAD:
    //      {
    //        result = SiteWithADVerified(IdSite);
    //      }break;
    //    case (int)EnumTypeSiteValidation.Gtm:
    //      {
    //        result= SiteWithTokenVerified(IdSite);
    //      }break;
    //    case (int)EnumTypeSiteValidation.Txt:
    //      {
    //        result= SiteWithTxtVerified(IdSite);
    //      }break;
    //  }
    //  return result;
    //}

    //public System.Web.Mvc.FileStreamResult CreateVerificationFile(Guid IdSite)
    //{
    //  SITE _site = (SITE)(from d in _context.SITES where d.IdSite.Equals(IdSite) select d).FirstOrDefault();

    //  if (_site != null)
    //  {
    //    string datafile = _site.VerificationString;

    //    if (datafile.Length > 0)
    //    {
    //      var byteArray = System.Text.Encoding.ASCII.GetBytes(datafile);
    //      var stream = new System.IO.MemoryStream(byteArray);

    //      System.Web.Mvc.FileStreamResult _sfile = new System.Web.Mvc.FileStreamResult(stream, "text/plain");
    //      _sfile.FileDownloadName = "kindads.txt";

    //      return _sfile;
    //    }
    //    else
    //    {
    //      return null;
    //    }
    //  }
    //  return null;
    //}

    //public async Task<List<SITE>> GetSitesByIdUser(string idUser)
    //{
    //  return await (from r in _context.SITES where r.AspNetUsers_Id.Equals(idUser) select r).ToListAsync();
    //}

    //public Models.Publisher.ShowSitesViewModel GetSitesPendingByIdUser(string idUser, int page = 1, string sort = "URL", string sortdir = "ASC")
    //{      
    //  int pageSize = 4;
    //  Models.Publisher.ShowSitesViewModel m = new Models.Publisher.ShowSitesViewModel();
    //  SiteRepository siteRepository = new SiteRepository { Context = context };
    //  List<SiteEntity> sitesAvailable = siteRepository.GetAll().Where(x => x.IsActive.Value).ToList();
    //  m.PageSize = pageSize;
    //  m.TotalRecord = (from r in sitesAvailable where r.AspNetUsers_Id.Equals(idUser) && r.Verified == false select r).Count(); 
    //  m.NoOfPages = (m.TotalRecord / m.PageSize) + ((m.TotalRecord % m.PageSize) > 0 ? 1 : 0);
    //  m.ListSitesPending = sitesAvailable.OrderBy(sort + " " + sortdir).Where(p => p.AspNetUsers_Id.Equals(idUser) && p.Verified == false).Skip((page - 1) * m.PageSize).Take(m.PageSize).ToList();
    //  return m;      
    //}

    //public Models.Publisher.ShowSitesViewModel GetSitesVerifyByIdUser(string idUser, int page = 1, string sort = "URL", string sortdir = "ASC")
    //{
    //  int pageSize = 4;
    //  Models.Publisher.ShowSitesViewModel m = new Models.Publisher.ShowSitesViewModel();
    //  SiteRepository siteRepository = new SiteRepository { Context = context };
    //  List<SiteEntity> sitesAvailable = siteRepository.GetAll().Where(x => x.IsActive.Value).ToList();
    //  m.PageSize = pageSize;
    //  m.TotalRecord = (from r in sitesAvailable where r.AspNetUsers_Id.Equals(idUser) && r.Verified == true select r).Count();
    //  m.NoOfPages = (m.TotalRecord / m.PageSize) + ((m.TotalRecord % m.PageSize) > 0 ? 1 : 0);
    //  m.ListSitesVerify = sitesAvailable.OrderBy(sort + " " + sortdir).Where(p => p.AspNetUsers_Id.Equals(idUser) && p.Verified == true).Skip((page - 1) * m.PageSize).Take(m.PageSize).ToList();
    //  return m;
    //}

    //public async Task<SITE> GetSiteById(string idSite)
    //{
    //  if (!String.IsNullOrEmpty(idSite) && new Guid(idSite) != Guid.Empty)
    //  {
    //    return await (from r in _context.SITES where r.IdSite.Equals(new Guid(idSite)) select r).FirstOrDefaultAsync();
    //  }
    //  return null;
    //}

    //Quitarlio de product controller
    public SITE GetSiteByIdSite(string idSite)
    {
      if (!String.IsNullOrEmpty(idSite) && new Guid(idSite) != Guid.Empty)
      {
        return (from r in _context.SITES where r.IdSite.Equals(new Guid(idSite)) select r).FirstOrDefault();
      }
      return null;
    }
    //public SiteEntity GetSingleSiteById(Guid IdSite)
    //{
    //  SiteRepository siteRepository = new SiteRepository(){ Context = context };

    //  SiteEntity site = siteRepository.GetSingle(IdSite);

    //  return site;
    //}
    //public SiteEntity UpdateSingleSite(SiteEntity site)
    //{
    //  SiteRepository siteRepository = new SiteRepository() { Context = context };

    //  siteRepository.Edit(site);
    //  siteRepository.Save();

    //  return site;
    //}
  }
}
