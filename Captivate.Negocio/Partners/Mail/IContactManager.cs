using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Captivate.Common.Interfaces;
using Captivate.Business;
using Captivate.DataAccess;
using Captivate.Comun.Interfaces;
using Captivate.Negocio.Partners.IContact;
using Captivate.Comun.Models.Entities;

namespace Captivate.Negocio.Partners.Mail
{
    public class IContactManager : IEmailProviders
    {
        public ITrace telemetria { set; get; }
        public CampaignRepository CampaignRepository { set; get; }
        public ProductRepository ProductRepository { set; get; }
        public IContactManager()
        {
           
            telemetria = new Trace();
            CampaignRepository = new CampaignRepository ();
            ProductRepository = new ProductRepository ();
        }

        public string ValidateCampaign(string IdCampaig,string IdUser)
        {
            bool result = false;
            CampaignManager campaignManager = new CampaignManager();
            NotificationManager notificationManager = new NotificationManager();

            //Obtenemos los datos 

            CampaignEntity campaign = CampaignRepository.FindById(new Guid(IdCampaig));
            ProductEntity product = ProductRepository.FindById(campaign.PRODUCT_IdProduct);
            
            //Todo
            //// Create models
            IContacLogic<ICampaign, IContactPostSendsResponse> contactServiceSends = new IContacLogic<ICampaign, IContactPostSendsResponse>();
            IContacLogic<ICampaign, IContactPostMessagesResponse> contactServiceMessage = new IContacLogic<ICampaign, IContactPostMessagesResponse>();

            IContactRequest requestFrm = contactServiceSends.FillIContactRequest(product.ProductSettingsEntitys);

            //// Creamos el Message
            string cleanBody = IContacLogic<ICampaign, IContactPostMessagesResponse>.CleanStripHTML(campaign.AdText);
            IContactPostMessageRequest requestMessage = new IContactPostMessageRequest
            {

                // Seteamos el objeto requestMessage
                campaignId = Convert.ToInt32(requestFrm.IdCampaign),
                subject = campaign.Name,
                textBody = cleanBody
            };

            // Invocamos
            string MessageId = string.Empty;
            IContactPostMessagesResponse responseMessage = (IContactPostMessagesResponse)contactServiceMessage.CreateMessage(requestMessage, requestFrm);
            //IContactPostMessagesResponse responseMessage = (IContactPostMessagesResponse)contactServiceMessage.CreateMessageToHttpClient(requestMessage,requestFrm);
            requestFrm.IdMessage = responseMessage.messages[0].messageId;

            // Creamos el Sends
            IContactPostSendsRequest requestSends = new IContactPostSendsRequest
            {
                messageId = Convert.ToInt32(requestFrm.IdMessage),
                scheduledTime = contactServiceSends.GetScheduleTime(campaign.StartDate),
                includeListIds = requestFrm.ListId
            };

            // Invocamos
            IContactPostSendsResponse responseSends = (IContactPostSendsResponse)contactServiceSends.CreateSends(requestSends, requestFrm);
            campaign.IdCampaign3rdParty = responseSends.sends[0].sendId;

            result = String.IsNullOrEmpty(responseSends.sends[0].sendId);
            return responseSends.sends[0].sendId;
        }
    }
}
