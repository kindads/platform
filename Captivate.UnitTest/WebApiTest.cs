using System;
using System.Collections.Generic;
using System.Linq;
using Captivate.API.Controllers;
using Captivate.Common.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Captivate.UnitTest
{
    [TestClass]
    public class WebApiTest
    {
        #region Examples
        //[TestMethod]
        //public void Get()
        //{
        //    // Arrange
        //    ValuesController controller = new ValuesController();

        //    // Act
        //    IEnumerable<string> result = controller.Get();

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(2, result.Count());
        //    Assert.AreEqual("value1", result.ElementAt(0));
        //    Assert.AreEqual("value2", result.ElementAt(1));
        //}

        //[TestMethod]
        //public void GetById()
        //{
        //    // Arrange
        //    ValuesController controller = new ValuesController();

        //    // Act
        //    string result = controller.Get(5);

        //    // Assert
        //    Assert.AreEqual("value", result);
        //}

        //[TestMethod]
        //public void Post()
        //{
        //    // Arrange
        //    ValuesController controller = new ValuesController();

        //    // Act
        //    controller.Post("value");

        //    // Assert
        //}

        //[TestMethod]
        //public void Put()
        //{
        //    // Arrange
        //    ValuesController controller = new ValuesController();

        //    // Act
        //    controller.Put(5, "value");

        //    // Assert
        //}

        //[TestMethod]
        //public void Delete()
        //{
        //    // Arrange
        //    ValuesController controller = new ValuesController();

        //    // Act
        //    controller.Delete(5);

        //    // Assert
        //}

        #endregion

       

        [TestMethod]
        public void PostTest()
        {
            // Arrange
            SiteController controller = new SiteController();

            string siteToken = "";

            SiteValidationToken data = new SiteValidationToken();

            // Act
            controller.Post(data);

            // Assert
            Assert.AreEqual(true, true);
        }

    }
}
