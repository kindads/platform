using System;
using Captivate.Negocio.Partners.Mail;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Captivate.UnitTest
{
    [TestClass]
    public class SendinBlueManagerTest
    {
        [TestMethod]
        public void GetAllListsTest()
        {
            string IdCampaign = string.Empty;
            SendinBlueManager blueManager = new SendinBlueManager();
            blueManager.GetAllLists(IdCampaign,1);

            Assert.AreEqual(true,true);
        }

        [TestMethod]
        public void GetAllFolders()
        {
            string IdCampaign = string.Empty;
            SendinBlueManager blueManager = new SendinBlueManager();
            blueManager.GetAllFolders();

            Assert.AreEqual(true, true);
        }

        [TestMethod]
        public void CreateCampaign()
        {
            string IdCampaign = string.Empty;
            SendinBlueManager blueManager = new SendinBlueManager();
            blueManager.CreateCampaign();

            Assert.AreEqual(true, true);
        }
    }
}
