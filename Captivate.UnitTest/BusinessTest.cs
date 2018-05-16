using System;
using Captivate.Comun.Models;
using Captivate.Negocio.Email;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Captivate.UnitTest
{
    [TestClass]
    public class BusinessTest
    {
        [TestMethod]
        public void SendMailTest()
        {
            MailManager mailManager = new MailManager();
            MailMessage message = new MailMessage();

            message.Body = "Prueba de correo";
            message.Destination = "angel.alvarado@blockbliss.com";
            message.Subject = "Prueba";

            mailManager.SendAsync(message).GetAwaiter().GetResult();

            Assert.AreEqual(true, true);

        }
    }
}
