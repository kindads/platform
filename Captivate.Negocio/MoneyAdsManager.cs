using Captivate.Azure;
using Captivate.Business;
using Captivate.Common.Interfaces;
using Captivate.Common.Models.Entities;
using Captivate.Comun.Models;
using Captivate.Comun.Models.Entities;
using Captivate.Comun.Models.ViewModel;
using Captivate.DataAccess;
using Captivate.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Captivate.Negocio
{
    public class MoneyAdsManager : ITelemetria
    {
        public ITrace telemetria { set; get; }

        public MoneyAdsViewModel viewModel { set; get; }

        public string IdUser { set; get; }


        public MoneyAdsManager()
        {
            telemetria = new Trace();
            viewModel = new MoneyAdsViewModel();
        }


        public SelectList Types()
        {
            AdsTypeRepository repository = new AdsTypeRepository();
            List<AdsTypeEntity> types = repository.GetAll().ToList();
            return new SelectList(types, "Id", "AdsDescription");
        }

        public MoneyAdsViewModel GetDefaultAds(string idUser, int page = 1, string sort = "URL", string sortdir = "ASC")
        {
            MoneyAdsViewModel viewModel = new MoneyAdsViewModel();
            PublisherAdsRepository repository = new PublisherAdsRepository();
            viewModel.defaultAds = repository.GetAllDefaultAds(new Guid(idUser));
            viewModel.PageSize = 4;
            viewModel.TotalRecord = viewModel.defaultAds.Count();
            return viewModel;
        }

        public MoneyAdsViewModel GetStickyAds(string idUser, int page = 1, string sort = "URL", string sortdir = "ASC")
        {
            MoneyAdsViewModel viewModel = new MoneyAdsViewModel();
            PublisherAdsRepository repository = new PublisherAdsRepository();
            viewModel.defaultAds = repository.GetAllStickyAds(new Guid(idUser));
            viewModel.PageSize = 4;
            viewModel.TotalRecord = viewModel.defaultAds.Count();
            return viewModel;
        }

        public bool ContainOneActiveAds(string idUser)
        {
            bool result = false;
            PublisherAdsRepository repository = new PublisherAdsRepository();
            List<Comun.Models.DefaultAds> defaultAds =  repository.GetAllDefaultAds(new Guid(idUser));
            List<Comun.Models.DefaultAds> stickyAds = repository.GetAllStickyAds(new Guid(idUser));

            //Contamos
            result = ((from ads in defaultAds where ads.Status == "Active" select ads).ToList().Count() > 0 ||
                     (from ads in stickyAds where ads.Status == "Active" select ads).ToList().Count() > 0);

            return result;
        }

        public SelectList Sites()
        {
            SiteRepository repository = new SiteRepository();
            List<SiteEntity> allSites = repository.GetAll().ToList();
            List<SiteEntity>  userSites = (   from site in allSites
                                              where site.AspNetUsers_Id == IdUser
                                              select site).ToList();
            return new SelectList(userSites, "IdSite", "URL");
        }

        public bool AddAds(string IdUser, Comun.Models.ViewModel.DefaultAds model)
        {
            bool result = false;
            PublisherAdsRepository repository = new PublisherAdsRepository();
            result = repository.AddAds(IdUser,model);
            return result;
        }

        public bool IsAlive(int Id)
        {
            bool result = false;
            PublisherAdsRepository repository = new PublisherAdsRepository();
            PublisherAdsEntity entity = repository.FindById<int>(Id);
            entity.IsAlive = false;
            repository.Edit(entity);
            result = true;
            return result;
        }

        public bool IsActive(int IdPublisherAds)
        {
            bool result = false;
            PublisherAdsRepository repository = new PublisherAdsRepository();
            MoneyAdsRepository moneyRespository = new MoneyAdsRepository();

            PublisherAdsEntity entity = repository.FindById<int>(IdPublisherAds);
            MoneyAdsEntity moneyEntity = moneyRespository.FindById<int>(entity.MoneyAdsId);
            Guid Idsite = entity.IdSite;

            result = repository.IsActive(IdPublisherAds, moneyEntity.AdsTypeId, Idsite);
            return result;
        }

        public SiteEntity GetSiteById(string IdSite)
        {
            SiteRepository repository = new SiteRepository();
            SiteEntity site = repository.FindById(new Guid(IdSite));
            return site;
        }

        private string GetScript(string IdUser, string IdSite,string environment)
        {
            //string environment = "-dev";
            string blobContainer = string.Format("moneyads{0}",environment);
           
            StringBuilder raw = new StringBuilder();
            raw.AppendLine();
            raw.AppendLine(string.Format("<script src=\"https://kindadsscripts.blob.core.windows.net/{0}/adsMonetization.js\"></script>", blobContainer));
            raw.AppendLine("<script type=\"text/javascript\">");
            raw.AppendLine();
            raw.AppendLine(" var configuration =");
            raw.AppendLine("    {");
            if (environment.Equals("-prod"))
            {
                raw.AppendLine("     ApiUrl: 'https://captivatemoneyadsapi.azurewebsites.net/api/',");
            }
            else
            {
                raw.AppendLine(string.Format("   ApiUrl: 'https://captivatemoneyadsapi{0}.azurewebsites.net/api/',", environment));
            }
            raw.AppendLine(string.Format("   RequestUrl: 'https://captivatemoneyadsapi{0}.azurewebsites.net/api/Configuration?IdUser=',",environment));
            raw.AppendLine(string.Format("   deviceMetric: 'https://kindadsscripts.blob.core.windows.net/{0}/deviceMetrics.js',",blobContainer));
            raw.AppendLine(string.Format("   geoLocationMetric: 'https://kindadsscripts.blob.core.windows.net/{0}/geoLocationMetrics.js',",blobContainer));
            raw.AppendLine(string.Format("   timeMetric: 'https://kindadsscripts.blob.core.windows.net/{0}/timeMetrics.js',",blobContainer));
            raw.AppendLine(string.Format("   injectedAds: 'https://kindadsscripts.blob.core.windows.net/{0}/injectedAds.js',",blobContainer));
            raw.AppendLine(string.Format("   mobileDetect: 'https://kindadsscripts.blob.core.windows.net/{0}/mobile-detect.js',",blobContainer));
            raw.AppendLine(string.Format("   IdUser: '{0}',",IdUser.ToUpper()));
            raw.AppendLine(string.Format("   IdSite: '{0}' ",IdSite.ToUpper()));
            raw.AppendLine("     };");
            raw.AppendLine();
            raw.AppendLine("    moneyAdsMetrics.loadConfiguration(configuration);");
            raw.AppendLine("</script>");

            return raw.ToString();
        }

       

        private string CreateTelemetryScriptContent(string IdUser,string IdSite)
        {
            string script = "algo";
            string environment = string.Empty;

#if DEV  || DEBUG         
            environment = "-dev";
#elif QA
           environment = "-qa";
#elif STAGING
            environment = "-staging";
#else
            environment = "-prod";
#endif
            script = GetScript(IdUser,IdSite,environment);
            return script;
        }

        public FileStreamResult CreateTelemetryScript(string IdUser, string IdSite)        
        {
            SiteEntity _site = GetSiteById(IdSite.ToString());

            if (_site != null)
            {
                string datafile = CreateTelemetryScriptContent(IdUser,IdSite);

                if (datafile.Length > 0)
                {
                    var byteArray = System.Text.Encoding.ASCII.GetBytes(datafile);
                    var stream = new System.IO.MemoryStream(byteArray);

                    System.Web.Mvc.FileStreamResult _sfile = new System.Web.Mvc.FileStreamResult(stream, "text/plain");
                    _sfile.FileDownloadName = "adsMonetizationScript.txt";

                    return _sfile;
                }
                else
                {
                    return null;
                }
            }
            return null;
        }


    }
}
