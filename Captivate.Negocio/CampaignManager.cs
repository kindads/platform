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
        public CampaignManager()
        {
            KindadsContext context = new KindadsContext();
            telemetria = new Trace();
            repository = new CampaignRepository { Context = context };
            userRepository = new AspNetUserRepository { Context = context };
            transactionCaptRepository = new TransactionCaptRepository { Context = context };
        }

        public CampaignEntity GetById(Guid id)
        {
            return repository.GetById(id);
        }

        public bool AutorizeCampaign(Guid idCampaign,string IdUser)
        {
            bool result = false;
            var campaign = GetById(idCampaign);
            var product = campaign.PRODUCT;
            var user = userRepository.GetByEmail(campaign.AspNetUser.Email);

            try
            {
                var _createTransaction = AsyncHelpers.RunSync<CreateTransactionModel>(() => NethereumHelper.DoTransaction(IdUser,user.WalletAddress, product.AspNetUser.WalletAddress, ((Double)product.Price * (Double)100000000).ToString()));

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
                        CAMPAIGN = campaign
                    };

                    transactionCaptRepository.Add(_transaction);

                    repository.Save();
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

        public void Save()
        {
            repository.Save();
        }
    }
}
