using System;

using System.IO;

using KindAds.Common.Utils;
using Microsoft.Azure.WebJobs;
using KindAds.Business.Partners.Push;
using KindAds.Business.Partners.Mail;
using KindAds.Common.Interfaces;

using KindAds.Business;
using KindAds.Common.Models;

using KindAds.Common.Models.Entities;
using Newtonsoft.Json;
using KindAds.Azure;
using KindAds.Comun.Enums;

namespace KindAds.WebJob.CampaignValidator
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
        public void ProcessQueueMessage([QueueTrigger("%azure-queue-campaign%")] string message, TextWriter log)
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
                                campaign.IdCampaign3rdParty = subscribersManager.SendCampaign(idCampaign);
                                break;
                            case Constants.PROVIDER_PUSH_CREW:
                                PushCrewManager pushCrewManager = new PushCrewManager();
                                campaign.IdCampaign3rdParty = pushCrewManager.SendCampaign(idCampaign);
                                break;
                            case Constants.PROVIDER_MAIL_CHIMP:
                                MailChimpManager mailChimpManager = new MailChimpManager();
                                campaign.IdCampaign3rdParty = mailChimpManager.SendCampaign(idCampaign);
                                break;
                            case Constants.PROVIDER_CAMPAIGN_MONITOR:
                                CampaignMonitorManager campaignMonitorManager = new CampaignMonitorManager();
                                campaign.IdCampaign3rdParty = campaignMonitorManager.SendCampaign(idCampaign);
                                break;
                            case Constants.PROVIDER_AWEBER:
                                AWeberManager aWeberManager = new AWeberManager();
                                campaign.IdCampaign3rdParty = aWeberManager.SendCampaign(idCampaign);
                                break;
                            case Constants.PROVIDER_SEND_GRID:
                                SendGridManager sendGridManager = new SendGridManager();
                                campaign.IdCampaign3rdParty = sendGridManager.SendCampaign(idCampaign);
                                break;
                            case Constants.PROVIDER_ACTIVE_CAMPAIGN:
                                ActiveCampaignManager activeCampaignManager = new ActiveCampaignManager();
                                campaign.IdCampaign3rdParty = activeCampaignManager.SendCampaign(idCampaign);
                                break;
                            case Constants.PROVIDER_GETRESPONSE:
                                GetResponseManager getResponseManager = new GetResponseManager();
                                campaign.IdCampaign3rdParty = getResponseManager.SendCampaign(idCampaign);
                                break;
                            case Constants.PROVIDER_ICONTACT:
                                IContactManager icontactManager = new IContactManager();
                                campaign.IdCampaign3rdParty = icontactManager.SendCampaign(notification.IdCampaignExternal);
                                break;
                            case Constants.PROVIDER_SENDINBLUE:
                                SendinBlueManager sendingBlueManager = new SendinBlueManager();
                                campaign.IdCampaign3rdParty = sendingBlueManager.SendCampaign(notification.IdCampaignExternal);
                                break;
                            case Constants.PROVIDER_PUSH_ENGAGE:
                                PushEngageManger pushEngageManger = new PushEngageManger();
                                campaign.IdCampaign3rdParty = pushEngageManger.SendCampaign(idCampaign);
                                break;
                            case Constants.PROVIDER_ONE_SIGNAL:
                                OneSignalManager oneSignalManager = new OneSignalManager();
                                campaign.IdCampaign3rdParty = oneSignalManager.SendCampaign(idCampaign);
                                break;
                            case Constants.PROVIDER_MAILJET:
                                MailJetManager mailJetManager = new MailJetManager();
                                campaign.IdCampaign3rdParty = mailJetManager.SendCampaign(idCampaign);
                                break;
                            default:
                                break;

                        }

                        if (!string.IsNullOrEmpty(campaign.IdCampaign3rdParty))
                        {
                            campaign.CAT_CAMPAIGN_STATUS_IdStatus = (int) CatCampaignStatusEnum.Authorized;
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
