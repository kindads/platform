using Captivate.Comun.Enums;
using Captivate.Comun.Interfaces;
using Captivate.Comun.Models;
using Captivate.Comun.Structures;
using Captivate.Negocio.Utilerias;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Negocio.Email
{
    public class MailManager 
    {
        public Guid IdUser { set; get; }


        public MailManager()
        {
          
        }


        public string GetMailContent(EMailType type)
        {
            String mailContent = String.Empty;
            String pathHtml = String.Empty;

            var path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            path = path.Substring(6);// + @"\Email_templates\captivate_express_webapp_mail.xml";

            Queue<string> nodes = new Queue<string>() { };
            nodes.Enqueue("correo");
            nodes.Enqueue(Enum.GetName(typeof(EMailType), type));
            pathHtml = path + new XmlFileReader().GetNodeContent(path + SRoutes.MailXmlRoute, nodes);

            mailContent = new HtmlFileReader().GetContent(pathHtml);

            return mailContent;
        }

        public Task SendAsync(MailMessage message)
        {
            return configSendGridasync(message);
        }

        private Task configSendGridasync(MailMessage message)
        {
            string apiKey = ConfigurationManager.AppSettings["ApiKeySendGrid"];
            SendGridClient client = new SendGridClient(apiKey);
            EmailAddress from = new EmailAddress("notification-no-response@kindads.io", "Kind Ads");
            string subject = message.Subject;
            EmailAddress to = new EmailAddress(message.Destination, "");
            string htmlContent = message.Body;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlContent);
            return client.SendEmailAsync(msg);
        }
    }

   
}
