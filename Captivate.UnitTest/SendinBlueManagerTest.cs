using System;
using System.Collections.Generic;
using Captivate.Common.Models.Entities;
using Captivate.Common.Partners.Mail.SendinBlue;
using Captivate.Business.Partners.Mail;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Captivate.UnitTest
{
    [TestClass]
    public class SendinBlueManagerTest
    {
        // primero obtenemos todos los folders
        [TestMethod]
        public void GetAllFolders()
        {
            string ApiKey = "YsQDFHN0nj1x4fht";
            string IdCampaign = string.Empty;
            SendinBlueManager blueManager = new SendinBlueManager(ApiKey);
            blueManager.GetAllFolders();

            Assert.AreEqual(true, true);
        }


        // Obtenemos todos las listas de ese folder
        [TestMethod]
        public void GetAllListsTest()
        {
            string ApiKey = "YsQDFHN0nj1x4fht";
            string IdCampaign = string.Empty;
            SendinBlueManager blueManager = new SendinBlueManager(ApiKey);
            blueManager.GetAllLists(IdCampaign,1);

            Assert.AreEqual(true,true);
        }

      
        // Creacion de la campaña
        [TestMethod]
        public void CreateCampaign()
        {
            string ApiKey = "YsQDFHN0nj1x4fht";
            string IdCampaign = string.Empty;
            SendinBlueManager blueManager = new SendinBlueManager(ApiKey);

            SendinBlueCampaignRequest request = new SendinBlueCampaignRequest
            {
                Category = "My category",
                FromName = "Alberto",
                Name = "Mi camapaña",
                HtmlContent = "<html><body><pre> Corramos con el peje este 4 de julio a festejar a su depa </pre></body></html>",
                ListIds = new List<int>{ 2 },
                Schedule = DateTime.Now.AddMinutes(2),
                Subject = "My subject",
                FromEmail = "angel.alvarad@blockbliss.com",
                ExcludeList = new List<int>()
            };

            CampaignEntity campaign = new CampaignEntity();
            blueManager.CreateCampaign(request, campaign,"1");

            Assert.AreEqual(true, true);
        }

        [TestMethod]
        public void GetAllFoldersTest()
        {
            string ApiKey = "YsQDFHN0nj1x4fht";
            SendinBlueManager blueManager = new SendinBlueManager(ApiKey);
            List<Folder> folders = blueManager.GetAllFolders();

            Assert.AreEqual(true, folders.Count>0);
        }

        [TestMethod]
        public void ValidateApiKeyTest()
        {
            string ApiKeyCorrect = "YsQDFHN0nj1x4fht";
            string ApiKeyWrong = "YsQDFHN0nj1x4ft";

            SendinBlueManager blueManager = new SendinBlueManager(ApiKeyWrong);
            bool result=blueManager.ValidateApiKey();

            Assert.AreEqual(true, result);
        }
    }
}
