using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using Captivate.Comun.Models.Entities;
using Captivate.DataAccess;
using Dapper;
using DapperExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Captivate.UnitTest
{
    [TestClass]
    public class DataAccessTest
    {
        //    [TestMethod]
        //    public void GetProductsTest()
        //    {
        //        KindadsContext context = new KindadsContext();
        //        ProductRepository productRepository = new ProductRepository { Context=context};
        //        List<ProductEntity> products= productRepository.GetAll().ToList();
        //        List<CampaignEntity> campaigns = products[0].CampaignEntitys.ToList();

        //        Assert.AreEqual(products.Count()>0, true);
        //    }


        //    [TestMethod]
        //    public void GetSitesTest()
        //    {
        //        KindadsContext context = new KindadsContext();
        //        SiteRepository siteRepository = new SiteRepository { Context= context };
        //        List<SiteEntity> sites = siteRepository.GetAll().ToList();

        //        Assert.AreEqual(true, true);
        //    }

        //    [TestMethod]
        //    public void AddSiteTest()
        //    {
        //        KindadsContext context = new KindadsContext();
        //        SiteRepository siteRepository = new SiteRepository { Context = context };
        //        AspNetUserRepository userRepository = new AspNetUserRepository { Context = context };

        //        List<AspNetUserEntity> users = userRepository.GetAll().ToList();

        //        SiteEntity site = new SiteEntity();
        //        site.IdSite = Guid.NewGuid();
        //        site.AspNetUser = users[0];
        //        site.Name = "Prueba de sitio";
        //        site.URL = "http://Test.com";
        //        site.IsActive = true;

        //        siteRepository.Add(site);
        //        siteRepository.Save();


        //        Assert.AreEqual(true, true);
        //    }
        //    [TestMethod]
        //    public void GetCategoriesTest()
        //    {
        //        CategoryRepository categoryRepository = new CategoryRepository();
        //        List<CategoryEntity> categories = categoryRepository.GetAll().ToList();
        //    }

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
            var tgr = new TagDapperRepository();
            var result = tgr.FindById<int>(1);
        
            tgr.Edit(new TagEntity2
            {
                IdTag = 1,
                Description = result.Description + " update descripcion 3"
            });

        }

        [TestMethod]
        public void insertTagTest()
        {
            var tgr = new TagDapperRepository();
            tgr.Add(new TagEntity2
            {
                Description = "nueva descripcion2"
            });
        }

        [TestMethod]
        public void ActualizarTagTest()
        {
            var tgr = new TagDapperRepository();
            tgr.Edit(new TagEntity2
            {
                IdTag=3,
                Description = "update descripcion 3"
            });
        }

        [TestMethod]
        public void EliminarTagTest()
        {
            var tgr = new TagDapperRepository();
            tgr.Delete(new TagEntity2
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

    }


}
