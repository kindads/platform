using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KindAds.Common.Interfaces;
using KindAds.Business;
using KindAds.DataAccess;

using KindAds.Common.Models.Entities;
using KindAds.Common.Partners.IContact;
using KindAds.Negocio.Partners.Mail;
using KindAds.Azure;

namespace KindAds.Business.Partners.Mail
{
    public class IContactManager 
    {
        public ITrace telemetria { set; get; }
        //public CampaignRepository CampaignRepository { set; get; }
        //public ProductRepository ProductRepository { set; get; }
        public IContactManager()
        {
           
            telemetria = new Trace();
            //CampaignRepository = new CampaignRepository ();
            //ProductRepository = new ProductRepository ();
        }

       
        public bool SettingsAreValid(string IdCampaign)
        {
            bool result = false;
            result = ( ApiAppIdIsValid(IdCampaign)       &&  ApiUsernameIsValid(IdCampaign) && 
                       ApiPasswordIsValid(IdCampaign)    &&  AccountIdIsValid(IdCampaign) && 
                       ClientFolderIdIsValid(IdCampaign) &&  ListIdIsValid(IdCampaign)) ? true : false;
            return result;
        }

        #region setting validation methods

        private bool ApiAppIdIsValid(string IdCampaign)
        {
            bool result = false;
            try
            {
                IContacLogic<ICampaign, IContactPostSendsResponse> service = new IContacLogic<ICampaign, IContactPostSendsResponse>();
                result = service.ApiAppIsValid(IdCampaign);
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return result;
        }

        private bool ApiUsernameIsValid(string IdCampaign)
        {
            bool result = false;
            try
            {
                IContacLogic<ICampaign, IContactPostSendsResponse> service = new IContacLogic<ICampaign, IContactPostSendsResponse>();
                result = service.UsernameIsValid(IdCampaign);
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return result;
        }

        private bool ApiPasswordIsValid(string idCampaign)
        {
            bool result = false;
            try
            {
                IContacLogic<ICampaign, IContactPostSendsResponse> service = new IContacLogic<ICampaign, IContactPostSendsResponse>();
                result = service.PasswordIsValid(idCampaign);
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return result;
        }

        private bool AccountIdIsValid(string idCampaign)
        {
            {
                bool result = false;
                try
                {
                    IContacLogic<ICampaign, IContactPostSendsResponse> service = new IContacLogic<ICampaign, IContactPostSendsResponse>();
                    result = service.AccountIsValid(idCampaign);
                }
                catch (Exception e)
                {
                    var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    telemetria.Critical(messageException);
                }
                return result;
            }
        }

        private bool ClientFolderIdIsValid(string idCampaign)
        {
            bool result = false;
            try
            {
                IContacLogic<ICampaign, IContactPostSendsResponse> service = new IContacLogic<ICampaign, IContactPostSendsResponse>();
                result = service.FolderIsValid(idCampaign);
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return result;
        }

        private bool ListIdIsValid(string idCampaign)
        {
            bool result = false;
            try
            {
                IContacLogic<ICampaign, IContactPostSendsResponse> service = new IContacLogic<ICampaign, IContactPostSendsResponse>();
                result = service.ListIsValid(idCampaign);
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return result;
        }
        #endregion

        [Obsolete]
        public string SendCampaign(string idCampaign)
        {
            bool result = false;
            //CampaignManager campaignManager = new CampaignManager();
            //NotificationManager notificationManager = new NotificationManager();

            ////Obtenemos los datos 

            //CampaignEntity campaign = CampaignRepository.FindById(new Guid(idCampaign));
            //ProductEntity product = ProductRepository.FindById(campaign.PRODUCT_IdProduct);
           
            ////Todo
            ////// Create models
            //IContacLogic<ICampaign, IContactPostSendsResponse> contactServiceSends = new IContacLogic<ICampaign, IContactPostSendsResponse>();
            //IContacLogic<ICampaign, IContactPostMessagesResponse> contactServiceMessage = new IContacLogic<ICampaign, IContactPostMessagesResponse>();
            //List<ProductSettingsEntity> settings= contactServiceSends.GetProductSettingEntity(idCampaign);

            //IContactRequest requestFrm = contactServiceSends.FillIContactRequest(settings);

            ////// Creamos el Message
            //string cleanBody = IContacLogic<ICampaign, IContactPostMessagesResponse>.CleanStripHTML(campaign.AdText);
            //IContactPostMessageRequest requestMessage = new IContactPostMessageRequest
            //{

            //    // Seteamos el objeto requestMessage
            //    campaignId = Convert.ToInt32(requestFrm.IdCampaign),
            //    subject = campaign.Name,
            //    textBody = cleanBody
            //};

            //// Invocamos
            //string MessageId = string.Empty;
            //IContactPostMessagesResponse responseMessage = (IContactPostMessagesResponse)contactServiceMessage.CreateMessage(requestMessage, requestFrm);
            ////IContactPostMessagesResponse responseMessage = (IContactPostMessagesResponse)contactServiceMessage.CreateMessageToHttpClient(requestMessage,requestFrm);
            //requestFrm.IdMessage = responseMessage.messages[0].messageId;

            //// Creamos el Sends
            //IContactPostSendsRequest requestSends = new IContactPostSendsRequest
            //{
            //    messageId = Convert.ToInt32(requestFrm.IdMessage),
            //    scheduledTime = contactServiceSends.GetScheduleTime(campaign.StartDate),
            //    includeListIds = requestFrm.ListId
            //};

            //// Invocamos
            //IContactPostSendsResponse responseSends = (IContactPostSendsResponse)contactServiceSends.CreateSends(requestSends, requestFrm);
            //campaign.IdCampaign3rdParty = responseSends.sends[0].sendId;

            //result = String.IsNullOrEmpty(responseSends.sends[0].sendId);
            //return responseSends.sends[0].sendId;

            return null;
        }
    }
}
