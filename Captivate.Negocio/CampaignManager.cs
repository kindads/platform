using Captivate.Business;
using Captivate.Common.Interfaces;
using Captivate.Comun.Enums;
using Captivate.Comun.Models;
using Captivate.Comun.Models.Entities;
using Captivate.DataAccess;
using Captivate.Negocio.Helper;
using System;

namespace Captivate.Negocio
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

        public bool AutorizeCampaign(Guid idCampaign,string IdUser)
        {
            bool result = false;
            var campaign = GetById(idCampaign);
            var product = productRepository.FindById(campaign.PRODUCT_IdProduct) ;
            var userProduct = userRepository.FindById(product.AspNetUsers_Id);
            var user = userRepository.FindById(campaign.AspNetUser_Id);

           
            try
            {
                var _createTransaction = AsyncHelpers.RunSync<CreateTransactionModel>(() => NethereumHelper.DoTransaction(IdUser,user.WalletAddress, userProduct.WalletAddress, ((Double)product.Price * (Double)100000000).ToString()));

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

                
                    result= true;
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

     
    }
}
