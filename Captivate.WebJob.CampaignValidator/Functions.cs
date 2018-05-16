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

namespace Captivate.WebJob.CampaignValidator
{
    public class Functions
    {
        CampaignManager campaignManager;

        public Functions()
        {
            campaignManager = new CampaignManager();
        }

        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public void ProcessQueueMessage([QueueTrigger("campaignqueue")] string message, TextWriter log)
        {
            //Obtenemos el objeto de la notificacion

            NotificationManager notificationManager = new NotificationManager();
            Notification notification = notificationManager.GetNotification(message);

            string idCampaign = notification.IdCampaignExternal;
            string IdUser = notification.IdUser.ToString();
            try
            {
                if (!String.IsNullOrEmpty(idCampaign))
                {
                    var campaign = campaignManager.GetById(new Guid(idCampaign));
                    var product = campaign.PRODUCT;

                    if (campaignManager.AutorizeCampaign(campaign, IdUser))
                    {
                        switch (product.PARTNER.IdPartner.ToString())
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
                            default:
                                break;

                        }

                        if (!string.IsNullOrEmpty(campaign.IdCampaign3rdParty))
                        {
                            campaignManager.Edit(campaign);
                            campaignManager.Save();
                            message = string.Format("Congratulations your campaign {0} has been approved", campaign.Name);
                            notificationManager.EnqueueMailNotification(campaign.Name, message, IdUser);
                        }
                    }

                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
