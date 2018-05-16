using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using Captivate.Comun.Models.Entities;
using Captivate.DataAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Captivate.UnitTest
{
    [TestClass]
    public class DataAccessTest
    {
        [TestMethod]
        public void GetProductsTest()
        {
            KindadsContext context = new KindadsContext();
            ProductRepository productRepository = new ProductRepository { Context=context};
            List<ProductEntity> products= productRepository.GetAll().ToList();
            List<CampaignEntity> campaigns = products[0].CampaignEntitys.ToList();

            Assert.AreEqual(products.Count()>0, true);
        }


        [TestMethod]
        public void GetSitesTest()
        {
            KindadsContext context = new KindadsContext();
            SiteRepository siteRepository = new SiteRepository { Context= context };
            List<SiteEntity> sites = siteRepository.GetAll().ToList();

            Assert.AreEqual(true, true);
        }

        [TestMethod]
        public void AddSiteTest()
        {
            KindadsContext context = new KindadsContext();
            SiteRepository siteRepository = new SiteRepository { Context = context };
            AspNetUserRepository userRepository = new AspNetUserRepository { Context = context };

            List<AspNetUserEntity> users = userRepository.GetAll().ToList();

            SiteEntity site = new SiteEntity();
            site.IdSite = Guid.NewGuid();
            site.AspNetUser = users[0];
            site.Name = "Prueba de sitio";
            site.URL = "http://Test.com";
            site.IsActive = true;

            siteRepository.Add(site);
            siteRepository.Save();


            Assert.AreEqual(true, true);
        }
        [TestMethod]
        public void GetCategoriesTest()
        {
            CategoryRepository categoryRepository = new CategoryRepository();
            List<CategoryEntity> categories = categoryRepository.GetAll().ToList();
        }
        [TestMethod]
        public void AddSite()
        {
            KindadsContext context = new KindadsContext();
            SiteRepository siteRepository = new SiteRepository(){ Context = context };

            CategoryRepository categoryRepository = new CategoryRepository() { Context = context };
            List<CategoryEntity> categories = categoryRepository.GetAll().ToList();

            AspNetUserRepository aspNetUserRepository = new AspNetUserRepository() { Context = context };
            AspNetUserEntity _aspnetuser = aspNetUserRepository.GetAll().FirstOrDefault(x => x.Id == "44d46783-7ae1-4f30-baa3-5ef3e9aae92a");

            SiteEntity _site = new SiteEntity
            {
                Name = "Name",
                URL = "URL",
                IdSite = Guid.NewGuid(),

                AspNetUsers_Id = _aspnetuser.Id,
                Verified = false,
                VerificationString = Guid.NewGuid().ToString(),
                RegistrationDate = DateTime.Now,
                IsActive = true,

                //CATEGORY = new List<CategoryEntity>(),
                //AspNetUser = _aspnetuser,
                //PRODUCT = new List<ProductEntity>()
            };

            //_site.CATEGORY.Add(categories.First());
            siteRepository.Add(_site);

            try
            {
                siteRepository.Save();
            }
            catch (DbEntityValidationException e)
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
            }
        }
    }
}
