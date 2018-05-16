﻿using System;
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
            KindadsContext context = new KindadsContext();            
            telemetria = new Trace();
            CampaignRepository = new CampaignRepository { Context = context };
            ProductRepository = new ProductRepository { Context = context };
        }

        public string ValidateCampaign(string IdCampaig,string IdUser)
        {
            bool result = false;
            CampaignManager campaignManager = new CampaignManager();
            NotificationManager notificationManager = new NotificationManager();

            //Obtenemos los datos 

            CampaignEntity campaign = CampaignRepository.FindBy(c => c.IdCampaign == new Guid(IdCampaig)).FirstOrDefault();
            ProductEntity product = ProductRepository.FindBy(p => p.IdProduct == campaign.PRODUCT_IdProduct).FirstOrDefault();
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

            //result = String.IsNullOrEmpty(responseSends.sends[0].sendId) ? false : campaignManager.AutorizeCampaign(campaign);
            result = String.IsNullOrEmpty(responseSends.sends[0].sendId);

            //Validacion para enviar correo
            //if (result == true)
            //{
            //    string message = string.Format("Campaign '{0}' modify successfully", campaign.Name);
            //    notificationManager.EnqueueMailNotification(campaign.Name, message, IdUser);
            //}
            return responseSends.sends[0].sendId;
        }
    }
}