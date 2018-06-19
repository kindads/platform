using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Captivate.API;
using Captivate.API.Controllers;

namespace Captivate.API.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            SiteController controller = new SiteController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Home Page", result.ViewBag.Title);
        }
    }
}
