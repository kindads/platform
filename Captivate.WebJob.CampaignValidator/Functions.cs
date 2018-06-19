using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Captivate.Comun.Utils;
using Microsoft.Azure.WebJobs;
using Captivate.Negocio.Partners.Push;
using Captivate.Negocio.Partners.Mail;
using Captivate.Comun.Interfaces;
using Captivate.Negocio.Partners.IContact;
using Captivate.Negocio;
using Captivate.Comun.Models;
using Captivate.Common.Interfaces;
using Captivate.Business;
using Captivate.Comun.Models.Entities;
using Newtonsoft.Json;

namespace Captivate.WebJob.CampaignValidator
{
    public class Functions
    {
        CampaignManager campaignManager;


        public ITrace telemetria { set; get; }
        public Functions()
        {
            campaignManager = new CampaignManager();
            telemetria = new Trace();
        }

        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public void ProcessQueueMessage([QueueTrigger("%campaignQueue%")] string message, TextWriter log)
        {
            //Obtenemos el objeto de la notificacion
            CampaignEntity campaign = new CampaignEntity();
            NotificationManager notificationManager = new NotificationManager();
            Notification notification = notificationManager.GetNotification(message);

            string idCampaign = notification.IdCampaignExternal;
            string IdUser = notification.IdUser.ToString();
            try
            {
                if (!String.IsNullOrEmpty(idCampaign))
                {
                    campaign = campaignManager.GetById(new Guid(idCampaign));
                    var product = campaignManager.FindProductById(campaign.PRODUCT_IdProduct);

                    if (campaignManager.AutorizeCampaign(campaign.IdCampaign, IdUser))
                    {
                    
                        switch (product.PARTNER_IdPartner.ToString())
                        {
                            case Constants.PROVIDER_SUBSCRIBERS:
                                SubscribersManager subscribersManager = new SubscribersManager();
                                campaign.IdCampaign3rdParty = subscribersManager.ValidateCampaign(idCampaign);
                                break;
                            case Constants.PROVIDER_PUSH_CREW:
                                PushCrewManager pushCrewManager = new PushCrewManager();
                                campaign.IdCampaign3rdParty = pushCrewManager.ValidateCampaign(idCampaign);
                                break;
                            case Constants.PROVIDER_MAIL_CHIMP:
                                MailChimpManager mailChimpManager = new MailChimpManager();
                                campaign.IdCampaign3rdParty = mailChimpManager.ValidateCampaign(idCampaign);
                                break;
                            case Constants.PROVIDER_CAMPAIGN_MONITOR:
                                CampaignMonitorManager campaignMonitorManager = new CampaignMonitorManager();
                                campaign.IdCampaign3rdParty = campaignMonitorManager.ValidateCampaign(idCampaign);
                                break;
                            case Constants.PROVIDER_AWEBER:
                                AWeberManager aWeberManager = new AWeberManager();
                                campaign.IdCampaign3rdParty = aWeberManager.ValidateCampaign(idCampaign);
                                break;
                            case Constants.PROVIDER_SEND_GRID:
                                SendGridManager sendGridManager = new SendGridManager();
                                campaign.IdCampaign3rdParty = sendGridManager.ValidateCampaign(idCampaign);
                                break;
                            case Constants.PROVIDER_ACTIVE_CAMPAIGN:
                                ActiveCampaignManager activeCampaignManager = new ActiveCampaignManager();
                                campaign.IdCampaign3rdParty = activeCampaignManager.ValidateCampaign(idCampaign);
                                break;
                            case Constants.PROVIDER_GETRESPONSE:
                                GetResponseManager getResponseManager = new GetResponseManager();
                                campaign.IdCampaign3rdParty = getResponseManager.ValidateCampaign(idCampaign);
                                break;
                            case Constants.PROVIDER_ICONTACT:
                                IContactManager icontactManager = new IContactManager();
                                campaign.IdCampaign3rdParty = icontactManager.ValidateCampaign(notification.IdCampaignExternal, notification.IdUser.ToString());
                                break;
                            case Constants.PROVIDER_SENDINBLUE:
                                SendinBlueManager sendingBlueManager = new SendinBlueManager();
                                campaign.IdCampaign3rdParty = sendingBlueManager.ValidateCampaign(notification.IdCampaignExternal, notification.IdUser.ToString());
                                break;
                            case Constants.PROVIDER_PUSH_ENGAGE:
                                PushEngageManger pushEngageManger = new PushEngageManger();
                                campaign.IdCampaign3rdParty = pushEngageManger.ValidateCampaign(idCampaign);
                                break;
                            case Constants.PROVIDER_ONE_SIGNAL:
                                OneSignalManager oneSignalManager = new OneSignalManager();
                                campaign.IdCampaign3rdParty = oneSignalManager.ValidateCampaign(idCampaign);
                                break;
                            case Constants.PROVIDER_MAILJET:
                                MailJetManager mailJetManager = new MailJetManager();
                                campaign.IdCampaign3rdParty = mailJetManager.ValidateCampaign(idCampaign);
                                break;
                            default:
                                break;

                        }

                        if (!string.IsNullOrEmpty(campaign.IdCampaign3rdParty))
                        {
                            campaignManager.Edit(campaign);
                    
                            message = string.Format("Congratulations your campaign {0} has been approved", campaign.Name);
                            notificationManager.EnqueueMailNotification(campaign.Name, message, IdUser);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                string campaignRaw = JsonConvert.SerializeObject(campaign);
                var messageException = telemetria.MakeMessageException(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                messageException += String.Format(" campaign:{0}", campaignRaw);
                telemetria.Critical(messageException);
            }
        }
    }
}
