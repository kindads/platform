using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using Captivate.Common.Models.Entities;
using Captivate.Comun.Models.Entities;
using Captivate.DataAccess;
using Captivate.DataAccess.Repositories;
//using Dapper;
//using DapperExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Captivate.UnitTest
{
    [TestClass]
    public class DataAccessTest
    {
        [TestMethod]
        public void GetProductsTest()
        {

            ProductRepository productRepository = new ProductRepository ();
            List<ProductEntity> products = productRepository.GetAll().ToList();
            List<CampaignEntity> campaigns = products[0].CampaignEntitys.ToList();

            Assert.AreEqual(products.Count() > 0, true);
        }


        [TestMethod]
        public void GetSitesTest()
        {

            SiteRepository siteRepository = new SiteRepository ();
            List<SiteEntity> sites = siteRepository.GetAll().ToList();

            Assert.AreEqual(true, true);
        }

        [TestMethod]
        public void AddSiteTest()
        {
            
            SiteRepository siteRepository = new SiteRepository ();
            AspNetUserRepository userRepository = new AspNetUserRepository ();

            List<AspNetUserEntity> users = userRepository.GetAll().ToList();

            SiteEntity site = new SiteEntity();
            site.IdSite = Guid.NewGuid();
            site.AspNetUser = users[0];
            site.Name = "Prueba de sitio";
            site.URL = "http://Test.com";
            site.IsActive = true;

            siteRepository.Add(site);

            Assert.AreEqual(true, true);
        }
        [TestMethod]
        public void GetCategoriesTest()
        {
            CategoryRepository categoryRepository = new CategoryRepository();
            List<CategoryEntity> categories = categoryRepository.GetAll().ToList();
        }

        [TestMethod]
        public void GetCampaignsTest()
        {

            CampaignRepository repositoty = new CampaignRepository();
            var categories = repositoty.GetAll().ToList();

            Assert.AreEqual(true, true);
        }



   
        [TestMethod]
        public void BuscarXId()
        {
            var tgr = new TagRepository();
            var result = tgr.FindById<int>(1);
        
            tgr.Edit(new TagEntity
            {
                IdTag = 1,
                Description = result.Description + " update descripcion 3"
            });

        }

        [TestMethod]
        public void insertTagTest()
        {
            var tgr = new TagRepository();
            tgr.Add(new TagEntity
            {
                Description = "nueva descripcion2"
            });
        }

        [TestMethod]
        public void ActualizarTagTest()
        {
            var tgr = new TagRepository();
            tgr.Edit(new TagEntity
            {
                IdTag=3,
                Description = "update descripcion 3"
            });
        }

        [TestMethod]
        public void EliminarTagTest()
        {
            var tgr = new TagRepository();
            tgr.Delete(new TagEntity
            {
                IdTag = 2,
                Description = ""
            });
        }

        //        CategoryRepository categoryRepository = new CategoryRepository() { Context = context };
        //        List<CategoryEntity> categories = categoryRepository.GetAll().ToList();

        //        AspNetUserRepository aspNetUserRepository = new AspNetUserRepository() { Context = context };
        //        AspNetUserEntity _aspnetuser = aspNetUserRepository.GetAll().FirstOrDefault(x => x.Id == "44d46783-7ae1-4f30-baa3-5ef3e9aae92a");

        //        SiteEntity _site = new SiteEntity
        //        {
        //            Name = "Name",
        //            URL = "URL",
        //            IdSite = Guid.NewGuid(),

        //            AspNetUsers_Id = _aspnetuser.Id,
        //            Verified = false,
        //            VerificationString = Guid.NewGuid().ToString(),
        //            RegistrationDate = DateTime.Now,
        //            IsActive = true,

        //            //CATEGORY = new List<CategoryEntity>(),
        //            //AspNetUser = _aspnetuser,
        //            //PRODUCT = new List<ProductEntity>()
        //        };

        //        //_site.CATEGORY.Add(categories.First());
        //        siteRepository.Add(_site);

        //        try
        //        {
        //            siteRepository.Save();
        //        }
        //        catch (DbEntityValidationException e)
        //        {
        //            foreach (var eve in e.EntityValidationErrors)
        //            {
        //                String msg = String.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
        //                    eve.Entry.Entity.GetType().Name, eve.Entry.State);
        //                foreach (var ve in eve.ValidationErrors)
        //                {
        //                    msg = msg + String.Format("- Property: \"{0}\", Error: \"{1}\"",
        //                        ve.PropertyName, ve.ErrorMessage);
        //                }
        //            }
        //            throw;
        //        }
        //        catch (Exception e)
        //        {
        //        }
        //    }

        [TestMethod]
        public void Test1()
        {
            var idCampaign = new Guid("3F987790-4D7C-4B33-AA7E-54BA69E52A0F");
            //idCampaign = idCampaign.ToLower();
     
            //CampaignRepository campaignRepository = new CampaignRepository() { Context = context };
            //var campaign = (from c in campaignRepository.Context.Campaigns where c.IdCampaign.ToString().ToLower() == idCampaign.ToString().ToLower() select c).FirstOrDefault();
            
            //CampaignEntity campaign = campaignRepository.GetById(new Guid(idCampaign.ToLower()));
            //var categorys = (from r in campaignRepository.Context.CategorySites join c in campaignRepository.Context.Categories on r.CATEGORY_IdCategory equals c.IdCategory where r.SITEs_IdSite.Equals(campaign.PRODUCT.SITE.IdSite) select c).ToList();
            //var product = campaign.PRODUCT;
        }

        [TestMethod]
        public void AzureSubscriptionGetAllTest()
        {
            //KindadsContext context = new KindadsContext();
            AzureSubscriptionRepository repository = new AzureSubscriptionRepository() ;
            List<AzureSupcriptionEntity> ase = repository.GetAll().ToList();

            Assert.AreEqual(true, true);
        }


        [TestMethod]
        public void AzureSubscriptionAddTest()
        {
            //KindadsContext context = new KindadsContext();
            AzureSubscriptionRepository repository = new AzureSubscriptionRepository();

            AzureSupcriptionEntity entity = new AzureSupcriptionEntity()
            {
                IdSite = new Guid("F5EFFBC7-7024-40CB-A879-0D0D861BCAC6"),
                ClientAppId = "ClientAppId",
                SubscriptionId = "SubscriptionId",
                TenantId = "TenantId",
                AppKey = "AppKey"
            };

            repository.Add(entity);
            //repository.Save();

            Assert.AreEqual(true, true);
        }

        #region Money Ads

        [TestMethod]
        public void AddAdsTypeTest()
        {
            AdsTypeRepository repository = new AdsTypeRepository();
            AdsTypeEntity type = new AdsTypeEntity() { AdsDescription = "Sticky Ads" };

            repository.Add(type);
            Assert.AreEqual(true, true);
        }

        [TestMethod]
        public void GetAllAdsTypesTest()
        {
            AdsTypeRepository repository = new AdsTypeRepository();
            List<AdsTypeEntity> adsTypes = repository.GetAll().ToList();

            Assert.AreEqual(true, adsTypes.Count() > 0);
        }

        [TestMethod]
        public void AddMoneyAdsSettingsTest()
        {
            MoneyAdsSettingsRepository repository = new MoneyAdsSettingsRepository();

            MoneyAdsSettingsEntity adsSettings = new MoneyAdsSettingsEntity()
            {
                Name="My First Sticky ads",
                JavascriptId="",
                AdsText="Campaña del peje sticky",
                UrlImage=""
            };

            repository.Add(adsSettings);
            Assert.AreEqual(true, true);
        }

        [TestMethod]
        public void GetAllMoneyAdsSettingsTest()
        {
            MoneyAdsSettingsRepository repository = new MoneyAdsSettingsRepository();
            List<MoneyAdsSettingsEntity> settings = repository.GetAll().ToList();
            Assert.AreEqual(true, settings.Count > 0);
        }

        [TestMethod]
        public void AddMoneyAdsTest()
        {
            MoneyAdsRepository repository = new MoneyAdsRepository();
            MoneyAdsEntity moneyAds = new MoneyAdsEntity()
            {
                AdsTypeId=2,
                AdsSettingId=2
            };

            repository.Add(moneyAds);
            Assert.AreEqual(true, true);
        }

        [TestMethod]
        public void GetAllMoneyAdsTest()
        {
            MoneyAdsRepository repository = new MoneyAdsRepository();
            List<MoneyAdsEntity> moneyAds = repository.GetAll().ToList();

            Assert.AreEqual(true, moneyAds.Count() > 0);
        }

        [TestMethod]
        public void AddPublisherAdsTest()
        {
            PublisherAdsRepository repository = new PublisherAdsRepository();
            PublisherAdsEntity entidad = new PublisherAdsEntity()
            {
                MoneyAdsId = 2,
                IsActive = true,
                IdUser=new Guid("44f1e76b-24ae-4889-937b-5e1c45575ae8")
            };

            repository.Add(entidad);
            Assert.AreEqual(true, true);
        }

        [TestMethod]
        public void GetAllPublisherAdsTest()
        {
            PublisherAdsRepository repository = new PublisherAdsRepository();
            List<PublisherAdsEntity> ads = repository.GetAll().ToList();
            Assert.AreEqual(true, ads.Count() > 0);
        }

        #endregion
    }


}
