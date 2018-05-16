using captivate_express_webapp.Models;
using captivate_express_webapp.Models.Publisher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Threading.Tasks;
using System.Linq.Dynamic;
using Captivate.DataAccess;
using Captivate.Comun.Models.Entities;

namespace captivate_express_webapp.Services
{
  public class SiteService
  {
    private KindadsEntities _context;
    public KindadsContext context;
    public SiteService()
    {
       context = new KindadsContext();
      _context = new KindadsEntities();
    }

    public Guid? CreateSite(CreateSiteModel createSiteModel, string userId)
    {
      CategoryRepository categoryRepository = new CategoryRepository(){ Context = context };
      var allCategory = categoryRepository.GetAll();
      CategoryEntity _category = allCategory.FirstOrDefault(x => x.IdCategory == createSiteModel.CategoryTypeSelect);

      AspNetUserRepository aspNetUserRepository = new AspNetUserRepository() { Context = context };
      AspNetUserEntity _aspnetuser = aspNetUserRepository.GetAll().FirstOrDefault(x => x.Id == userId);

      SiteEntity _site = new SiteEntity
      {
        Name = createSiteModel.Name,
        URL = createSiteModel.URL,
        IdSite = Guid.NewGuid(),

        AspNetUsers_Id = userId,
        Verified = false,
        VerificationString = Guid.NewGuid().ToString(),
        RegistrationDate = DateTime.Now,
        IsActive = true,
      };

      SiteRepository siteRepository = new SiteRepository() { Context = context };
      siteRepository.Add(_site);

      try
      {
        siteRepository.Save();
        CategorySiteEntity categorySiteEntity = new CategorySiteEntity()
        {
          CATEGORY_IdCategory = _category.IdCategory,
          SITEs_IdSite = _site.IdSite
        };

        CategorySiteRepository categorySiteRepository = new CategorySiteRepository() { Context = context };
        categorySiteRepository.Add(categorySiteEntity);
        categorySiteRepository.Save();

        return _site.IdSite;
      }
      catch (System.Data.Entity.Validation.DbEntityValidationException e)
      {
        foreach (var eve in e.EntityValidationErrors)
        {
          String msg = String.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
              eve.Entry.Entity.GetType().Name, eve.Entry.State);
          foreach (var ve in eve.ValidationErrors)
          {
            msg = msg + String.Format("- Property: \"{0}\", Error: \"{1}\"",
                ve.PropertyName, ve.ErrorMessage);
          }
        }
        throw;
      }
      catch (Exception e)
      {
        return null;
      }
    }

    public bool VerifySite(Guid IdSite)
    {
      SITE _site = (SITE)(from d in _context.SITES where d.IdSite.Equals(IdSite) select d).FirstOrDefault();

      if (_site.VerificationString.Length > 0)
      {
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
                    _context.SITES.Attach(_site);
                    _context.Entry(_site).State = EntityState.Modified;
                    return _context.SaveChanges() > 0;
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
      }
      return false;
    }

    public System.Web.Mvc.FileStreamResult CreateVerificationFile(Guid IdSite)
    {
      SITE _site = (SITE)(from d in _context.SITES where d.IdSite.Equals(IdSite) select d).FirstOrDefault();

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

    public async Task<List<SITE>> GetSitesByIdUser(string idUser)
    {
      return await (from r in _context.SITES where r.AspNetUsers_Id.Equals(idUser) select r).ToListAsync();
    }

    public ShowSitesViewModel GetSitesPendingByIdUser(string idUser, int page = 1, string sort = "URL", string sortdir = "ASC")
    {      
      int pageSize = 4; 
      ShowSitesViewModel m = new ShowSitesViewModel();
      SiteRepository siteRepository = new SiteRepository { Context = context };
      List<SiteEntity> sitesAvailable = siteRepository.GetAll().Where(x => x.IsActive.Value).ToList();
      m.PageSize = pageSize;
      m.TotalRecord = (from r in sitesAvailable where r.AspNetUsers_Id.Equals(idUser) && r.Verified == false select r).Count(); 
      m.NoOfPages = (m.TotalRecord / m.PageSize) + ((m.TotalRecord % m.PageSize) > 0 ? 1 : 0);
      m.ListSitesPending = sitesAvailable.OrderBy(sort + " " + sortdir).Where(p => p.AspNetUsers_Id.Equals(idUser) && p.Verified == false).Skip((page - 1) * m.PageSize).Take(m.PageSize).ToList();
      return m;      
    }

    public ShowSitesViewModel GetSitesVerifyByIdUser(string idUser, int page = 1, string sort = "URL", string sortdir = "ASC")
    {
      int pageSize = 4;
      ShowSitesViewModel m = new ShowSitesViewModel();
      SiteRepository siteRepository = new SiteRepository { Context = context };
      List<SiteEntity> sitesAvailable = siteRepository.GetAll().Where(x => x.IsActive.Value).ToList();
      m.PageSize = pageSize;
      m.TotalRecord = (from r in sitesAvailable where r.AspNetUsers_Id.Equals(idUser) && r.Verified == true select r).Count();
      m.NoOfPages = (m.TotalRecord / m.PageSize) + ((m.TotalRecord % m.PageSize) > 0 ? 1 : 0);
      m.ListSitesVerify = sitesAvailable.OrderBy(sort + " " + sortdir).Where(p => p.AspNetUsers_Id.Equals(idUser) && p.Verified == true).Skip((page - 1) * m.PageSize).Take(m.PageSize).ToList();
      return m;
    }

    public async Task<SITE> GetSiteById(string idSite)
    {
      if (!String.IsNullOrEmpty(idSite) && new Guid(idSite) != Guid.Empty)
      {
        return await (from r in _context.SITES where r.IdSite.Equals(new Guid(idSite)) select r).FirstOrDefaultAsync();
      }
      return null;
    }

    public SITE GetSiteByIdSite(string idSite)
    {
      if (!String.IsNullOrEmpty(idSite) && new Guid(idSite) != Guid.Empty)
      {
        return (from r in _context.SITES where r.IdSite.Equals(new Guid(idSite)) select r).FirstOrDefault();
      }
      return null;
    }
    public SiteEntity GetSingleSiteById(Guid IdSite)
    {
      SiteRepository siteRepository = new SiteRepository(){ Context = context };

      SiteEntity site = siteRepository.GetSingle(IdSite);

      return site;
    }
    public SiteEntity UpdateSingleSite(SiteEntity site)
    {
      SiteRepository siteRepository = new SiteRepository() { Context = context };

      siteRepository.Edit(site);
      siteRepository.Save();

      return site;
    }
  }
}
