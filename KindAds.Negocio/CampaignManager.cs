using KindAds.Business;
using KindAds.Common.Interfaces;

using KindAds.Common.Models;
using KindAds.Common.Models.Entities;
using KindAds.DataAccess;
using KindAds.Business.Helper;
using System;
using KindAds.Comun.Enums;
using KindAds.Azure;
using System.Threading.Tasks;
using KindAds.Common.Utils;
using KindAds.Business.Partners.Push;
using KindAds.Business.Partners.Mail;
using Newtonsoft.Json;
using KindAds.DataAccess.Repositories;

namespace KindAds.Business
{
    public class CampaignManager : ITelemetria
    {
        public ITrace telemetria { set; get; }
        public CampaignRepository repository { set; get; }
        public AspNetUserRepository userRepository { set; get; }
        public TransactionCaptRepository transactionCaptRepository { set; get; }


        private readonly ProductRepository productRepository;
        public CampaignManager()
        {
            telemetria = new Trace();
            repository = new CampaignRepository();
            userRepository = new AspNetUserRepository();
            transactionCaptRepository = new TransactionCaptRepository();
            productRepository = new ProductRepository();
        }

        public CampaignEntity GetById(Guid id)
        {
            return repository.FindById(id);
        }


        public string GetAdvertiserWallet(Guid idCampaign)
        {
            string advertiserWallet = string.Empty;
            UserDetailRepository userDetailRepository = new UserDetailRepository();

            var campaign = GetById(idCampaign);
            var product = productRepository.FindById(campaign.PRODUCT_IdProduct);
            var userProduct = userRepository.FindById(product.AspNetUsers_Id);

            var user = userRepository.FindById(campaign.AspNetUser_Id);
            var userDetail = userDetailRepository.GetByUserById(campaign.AspNetUser_Id);

            if(userDetail != null && userDetail.IsMetamask)
            {
                advertiserWallet = user.WalletAddress;
            }

            return advertiserWallet;
        }

        public ProductEntity FindProductById(Guid idProduct)
        {
            return productRepository.FindById(idProduct);
        }

        public bool AutorizeCampaign(Guid idCampaign, string IdUser)
        {
            bool result = false;
            var campaign = GetById(idCampaign);
            var product = productRepository.FindById(campaign.PRODUCT_IdProduct);
            var userProduct = userRepository.FindById(product.AspNetUsers_Id);
            var user = userRepository.FindById(campaign.AspNetUser_Id);


            try
            {
                var balance = Task.Run(() => NethereumHelper.GetBalance(user.WalletAddress, true)).Result;
                if (balance >= product.Price)
                {

                    var _createTransaction = AsyncHelpers.RunSync<CreateTransactionModel>(() => NethereumHelper.DoTransaction(IdUser, user.WalletAddress, userProduct.WalletAddress, ((Double)product.Price * (Double)100000000).ToString()));

                    if (campaign != null && campaign.IdCampaign != Guid.Empty && _createTransaction != null && _createTransaction.hashTransaction.Length > 5)
                    {
                        campaign.CAT_CAMPAIGN_STATUS_IdStatus = (int)CatCampaignStatusEnum.Authorized;
                        repository.Edit(campaign);

                        TransactionsCaptEntity _transaction = new TransactionsCaptEntity()
                        {
                            Amount = _createTransaction.amount,
                            HashFrom = _createTransaction.hashFrom,
                            HashTo = _createTransaction.hashTo,
                            HashTransaction = _createTransaction.hashTransaction,
                            TRANSACTION_TYPE_IdTransactionType = 1,
                            BlockDate = DateTimeHelper.CurrentDateTimeString(),
                            Gas = "0",
                            RegisterDate = DateTimeHelper.CurrentDateTimeString(),
                            CAMPAIGN_IdCampaign = campaign.IdCampaign
                        };

                        transactionCaptRepository.Add(_transaction);


                        result = true;
                    }
                }
            }
            catch (Exception e)
            {
                result = false;
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return result;
        }

        public void Edit(CampaignEntity campaign)
        {
            repository.Edit(campaign);
        }

        public bool ValidateSettings(string idCampaign)
        {
            bool result = false;
            CampaignEntity campaign = new CampaignEntity();
            CampaignManager campaignManager = new CampaignManager();

            try
            {
                if (!String.IsNullOrEmpty(idCampaign))
                {
                    campaign = campaignManager.GetById(new Guid(idCampaign));
                    var product = campaignManager.FindProductById(campaign.PRODUCT_IdProduct);



                    switch (product.PARTNER_IdPartner.ToString())
                    {
                        case Constants.PROVIDER_SUBSCRIBERS:
                            SubscribersManager subscribersManager = new SubscribersManager();
                            result = subscribersManager.SettingsAreValid(idCampaign);
                            break;
                        case Constants.PROVIDER_PUSH_CREW:
                            PushCrewManager pushCrewManager = new PushCrewManager();
                            result = pushCrewManager.SettingsAreValid(idCampaign);
                            break;
                        case Constants.PROVIDER_MAIL_CHIMP:
                            MailChimpManager mailChimpManager = new MailChimpManager();
                            result = mailChimpManager.SettingsAreValid(idCampaign);
                            break;
                        case Constants.PROVIDER_CAMPAIGN_MONITOR:
                            CampaignMonitorManager campaignMonitorManager = new CampaignMonitorManager();
                            result = campaignMonitorManager.SettingsAreValid(idCampaign);
                            break;
                        case Constants.PROVIDER_AWEBER:
                            AWeberManager aWeberManager = new AWeberManager();
                            result = aWeberManager.SettingsAreValid(idCampaign);
                            break;
                        case Constants.PROVIDER_SEND_GRID:
                            SendGridManager sendGridManager = new SendGridManager();
                            result = sendGridManager.SettingsAreValid(idCampaign);
                            break;
                        case Constants.PROVIDER_ACTIVE_CAMPAIGN:
                            ActiveCampaignManager activeCampaignManager = new ActiveCampaignManager();
                            result = activeCampaignManager.SettingsAreValid(idCampaign);
                            break;
                        case Constants.PROVIDER_GETRESPONSE:
                            GetResponseManager getResponseManager = new GetResponseManager();
                            result = getResponseManager.SettingsAreValid(idCampaign);
                            break;
                        case Constants.PROVIDER_ICONTACT:
                            IContactManager icontactManager = new IContactManager();
                            result = icontactManager.SettingsAreValid(idCampaign);
                            break;
                        case Constants.PROVIDER_SENDINBLUE:
                            SendinBlueManager sendingBlueManager = new SendinBlueManager();
                            result = sendingBlueManager.SettingsAreValid(idCampaign);
                            break;
                        case Constants.PROVIDER_PUSH_ENGAGE:
                            PushEngageManger pushEngageManger = new PushEngageManger();
                            result = pushEngageManger.SettingsAreValid(idCampaign);
                            break;
                        case Constants.PROVIDER_ONE_SIGNAL:
                            OneSignalManager oneSignalManager = new OneSignalManager();
                            result = oneSignalManager.SettingsAreValid(idCampaign);
                            break;
                        case Constants.PROVIDER_MAILJET:
                            MailJetManager mailJetManager = new MailJetManager();
                            result = mailJetManager.SettingsAreValid(idCampaign);
                            break;
                        default:
                            break;

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

            return result;
        }

        public bool Send(string idCampaign)
        {
            bool result = false;
            CampaignEntity campaign = new CampaignEntity();
            CampaignManager campaignManager = new CampaignManager();
            try
            {
                if (!String.IsNullOrEmpty(idCampaign))
                {
                    campaign = campaignManager.GetById(new Guid(idCampaign));
                    var product = campaignManager.FindProductById(campaign.PRODUCT_IdProduct);
                    campaign.IdCampaign3rdParty = string.Empty;


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
                            campaign.IdCampaign3rdParty = icontactManager.SendCampaign(idCampaign);
                            break;
                        case Constants.PROVIDER_SENDINBLUE:
                            SendinBlueManager sendingBlueManager = new SendinBlueManager();
                            campaign.IdCampaign3rdParty = sendingBlueManager.SendCampaign(idCampaign);
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

                    //Descomentar
                    if (!string.IsNullOrEmpty(campaign.IdCampaign3rdParty))
                    {
                        campaign.CAT_CAMPAIGN_STATUS_IdStatus = (int)CatCampaignStatusEnum.Authorized;
                        campaignManager.Edit(campaign);
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
            result = campaign.IdCampaign3rdParty == string.Empty ? false : true;

            return result;
        }
    }
}
