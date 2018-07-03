using Captivate.Azure;
using Captivate.Common.Models;
using Captivate.Comun.Models;
using Captivate.Comun.Models.Entities;
using Captivate.DataAccess.Mappers;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.DataAccess.Repositories
{   
    public class PublisherAdsRepository : DGenericRepository<PublisherAdsEntity>
    {
        public PublisherAdsRepository(string connectionStringName = "KindAdsDefaultConnection") : base(connectionStringName)
        {
            LoadProfileMapper(typeof(SiteMapper));
        }

        public PublisherAdsEntity GetSingle(int Id)
        {
            var query = GetAll().FirstOrDefault(x => x.Id == Id);
            return query;
        }

        public List<DefaultAds> GetAllDefaultAds(Guid Id)
        {
            List<DefaultAds> defaultAds = new List<DefaultAds>();
            using (var cnn = DBConnection)
            {
                cnn.Open();
                defaultAds = cnn.Query<DefaultAds>("sp_MoneyAds_GetDefaultAds", new { IdUser = Id }, commandType: CommandType.StoredProcedure).ToList();
            }

            return defaultAds;
        }

        public List<DefaultAds> GetAllStickyAds(Guid Id)
        {
            List<DefaultAds> defaultAds = new List<DefaultAds>();
            using (var cnn = DBConnection)
            {
                cnn.Open();
                defaultAds = cnn.Query<DefaultAds>("sp_MoneyAds_GetStickyAds", new { IdUser = Id }, commandType: CommandType.StoredProcedure).ToList();
            }
            return defaultAds;
        }

        public bool AddAds(string Id, Comun.Models.ViewModel.DefaultAds model)
        {
            bool result = false;
            using (var cnn = DBConnection)
            {
                //Todo: crate sp
                cnn.Open();
                cnn.Query<DefaultAds>("sp_MoneyAds_AddAds", new { IdUser = new Guid(Id), IdSite=model.IdSite ,JavascriptId=model.javascriptId, AdsText=model.text, UrlImage=model.image, Name=model.name , AdsTypeId = model.typeSelected }, commandType: CommandType.StoredProcedure);
                result = true;
            }
            return result;
        }

        public bool IsActive(int pId, int adsId, Guid Idsite)
        {
            bool result = false;
            using (var cnn = DBConnection)
            {
                cnn.Open();
                cnn.Query<DefaultAds>("sp_MoneyAds_ActivateAds", new { AdsTypeId = adsId , IdPublisherAds = pId , IdSite=Idsite }, commandType: CommandType.StoredProcedure);
                result = true;
            }
            return result;
        }

        public DefaultConfig GetDefaultConfig(string User, string Site)
        {
            DefaultConfig config = new DefaultConfig();
            using (var cnn = DBConnection)
            {
                cnn.Open();
                config=cnn.Query<DefaultConfig>("sp_MoneyAds_GetDefaultConfig", new { IdUser = User, IdSite = Site }, commandType: CommandType.StoredProcedure).FirstOrDefault();
                config = config == null ? new DefaultConfig() : config;
            }
            return config;
        }

        public StickyConfig GetStickyConfig(string User, string Site)
        {
            StickyConfig config = new StickyConfig();
            using (var cnn = DBConnection)
            {
                cnn.Open();
                config=cnn.Query<StickyConfig>("sp_MoneyAds_GetStickyConfig", new { IdUser = User, IdSite = Site }, commandType: CommandType.StoredProcedure).FirstOrDefault();
                config = config == null ? new StickyConfig() : config;
            }
            return config;
        }


        public void GetClickBehavior(DefaultConfig config)
        {
            string TableName = "Styles";
            string PartitionName = "DefaultAds";
            string RowNameAdsDiv = "ClicBehavior";

            DefaultHtmlEntity styleClicBehavior = TableManager.GetClicBehavior(TableName, PartitionName, RowNameAdsDiv);

            config.ClicBehavior = string.Format(styleClicBehavior.Html);

        }

        public void GetStickyHtml(StickyConfig config)
        {
            // Obtenerlo de configuracion
            string TableName = "Styles";
            string PartitionName = "StickyAds";
            string RowNameAdsDiv = "AdsDiv";
            string RowNameCloseDiv = "CloseDiv";

            StickyHtmlEntity styleAdsDiv = TableManager.GetAdsHtml(TableName, PartitionName, RowNameAdsDiv);
            StickyHtmlEntity styleCloseDiv = TableManager.GetAdsHtml(TableName, PartitionName, RowNameCloseDiv);

            config.AdsHtml= string.Format(styleAdsDiv.Html, config.JsId);
            config.CloseHtml = string.Format(styleCloseDiv.Html, config.JsId);
        }
    }
}
