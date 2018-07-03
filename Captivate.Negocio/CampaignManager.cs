using Captivate.Business;
using Captivate.Common.Interfaces;

using Captivate.Common.Models;
using Captivate.Common.Models.Entities;
using Captivate.DataAccess;
using Captivate.Business.Helper;
using System;
using Captivate.Comun.Enums;
using Captivate.Azure;
using System.Threading.Tasks;

namespace Captivate.Business
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

        public bool Send(string idCampaign)
        {
            bool result = false;

            //CreateCampaingModel model = new CreateCampaingModel();

            //try
            //{
            //    if (!String.IsNullOrEmpty(idCampaign))
            //    {
            //        var campaign = _service.GetCampaignById(idCampaign);
            //        var product = campaign.PRODUCT;

            //        if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_SUBSCRIBERS)))
            //        {
            //            FillPushNotifSettings(model, campaign);
            //        }
            //        else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_PUSH_CREW)))
            //        {
            //            FillPushNotifSettings(model, campaign);
            //        }

            //        // Creamos el objeto de notificacion
            //        Notification notification = new Notification();
            //        NotificationManager notificationManager = new NotificationManager();

            //        // Enviamos la notificacion para la validacion
            //        notification.IdCampaignExternal = campaign.IdCampaign.ToString();
            //        notification.IdUser = new Guid(Microsoft.AspNet.Identity.IdentityExtensions.GetUserId(User.Identity));

            //        // Encolamos la notificacion para que el webjob la procese
            //        notificationManager.EnqueueCampaignValidator(notification);

            //        //Regresamos el mensaje de procesando
            //        campaign.CAT_CAMPAIGN_STATUS_IdStatus = (int)Captivate.Comun.Enums.CatCampaignStatusEnum.Verify_Proccess;
            //        return Json(new { success = _service.ModifyCampaign(campaign, null, null), message = "Processing campaign" });
            //    }
            //}
            //catch (Exception ex)
            //{
            //    var messageException = telemetria.MakeMessageException(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            //    telemetria.Critical(messageException);
            //    return ResponseError();
            //}

            return result;
        }


    }
}
