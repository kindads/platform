using Captivate.Business;
using Captivate.Common.Interfaces;
using captivate_express_webapp.Models;
using captivate_express_webapp.Utils.Configuration;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Web;

namespace captivate_express_webapp.Helpers
{
  public class NethereumHelper : ITelemetria
  {
    public ITrace telemetria { set; get; }

    public NethereumHelper()
    {
      telemetria = new Trace();
    }

    public static string GetUserWalletPassphrase(string usrwallet)
    {
      Services.AccessService _service = new Services.AccessService();
      string _blobName = _service.GetUserTokenAddress(usrwallet);
      return AzureStorageHelper.GetWalletPassphrase(_blobName);
      
    }

    protected static Function GetContractFunction(string _functionName)
    {
      var contractAddress = AppSettings.ContractAddress;

      var web3 = new Nethereum.Web3.Web3(AppSettings.BlockchainURL);
     
      string abiCode = System.Text.Encoding.UTF8.GetString(Resources.abi);
     
      var contract = web3.Eth.GetContract(abiCode, contractAddress);
      return contract.GetFunction(_functionName);
    }

    public static void TransferTokenFrontTest(string FromAddress_Buyer, string ToAddress_Owner, string Amount)
    {
      Task task = System.Threading.Tasks.Task.Run(async () => await (DoTransaction(FromAddress_Buyer, ToAddress_Owner, Amount)));
    }

    public async Task<Models.Wallet.CreateWalletModel> CreateUserWallet()
    {
      Models.Wallet.CreateWalletModel _walletModel = new Models.Wallet.CreateWalletModel();

      try
      {
        string _passphrase = Guid.NewGuid().ToString().Replace("-", "") + Helpers.AzureStorageHelper.GetRandomNumber(1842).ToString();

        string _blobname = AzureStorageHelper.CreateUsrWalletBlobFile(_passphrase);

        var BlockChainUrl = AppSettings.BlockchainURL;
        var web3 = new Nethereum.Web3.Web3(BlockChainUrl);
        var _walletAddress = await web3.Personal.NewAccount.SendRequestAsync(_passphrase);

        _walletModel = new Models.Wallet.CreateWalletModel() { blobname = _blobname, walletaddress = _walletAddress };

      }
      catch (Exception e)
      {
        var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
        telemetria.Critical(messageException);
      }
     
      return _walletModel;
    }

    public async static Task<Models.Wallet.CreateTransactionModel> DoTransaction(string FromAddress_Buyer, string ToAddress_Owner, string Amount)
    {
      Models.Wallet.CreateTransactionModel _transaction = new Models.Wallet.CreateTransactionModel() { amount = Amount, hashFrom = FromAddress_Buyer, hashTo = ToAddress_Owner, hashTransaction = ""};

      var mainAddress = AppSettings.MainAddress;
      var web3 = new Nethereum.Web3.Web3(AppSettings.BlockchainURL);

      try
      {
        
        var unlockMainAddress = await web3.Personal.UnlockAccount.SendRequestAsync(mainAddress, AppSettings.MainAddressPassphrase, 100);
        if (unlockMainAddress)
        {

          HexBigInteger gas = new HexBigInteger(90000);
          HexBigInteger gas2 = new HexBigInteger(0);

          var approvedAddress = await GetContractFunction("approveOwner").SendTransactionAsync(mainAddress, gas, gas2, FromAddress_Buyer, new BigInteger(float.Parse(Amount)));

          if (approvedAddress.Length > 10)
          {
            var functionBalance = GetContractFunction("balanceOf");
            var _balance = await functionBalance.CallAsync<Int64>(FromAddress_Buyer);

            if (Convert.ToDouble(Amount) > Convert.ToDouble(_balance))
            {
              _transaction = null;
              return _transaction;
            }

            var functionFrom = GetContractFunction("transferFrom");
            var _result = await functionFrom.SendTransactionAsync(mainAddress, gas, gas2, FromAddress_Buyer, ToAddress_Owner, new BigInteger(float.Parse(Amount)));
            _transaction.hashTransaction = _result.ToString();
          }
        }
      }
      catch (Exception Ex)
      {
        //System.IO.File.WriteAllText(System.Web.Hosting.HostingEnvironment.MapPath("//logs") + "\\" + DateTimeHelper.GetTimestamp() + ".txt", Ex.Message.ToString());
      }
      finally
      {
        //Then Lock mainAddress again
        await web3.Personal.LockAccount.SendRequestAsync(mainAddress);
      }
      return _transaction;
    }

    public async static Task<string> GetBalance(string _wallet, Boolean fixdecimal)
    {
      try
      {
        var approvedAddress = await GetContractFunction("balanceOf").CallAsync<Int64>(_wallet);
        if (fixdecimal)
        {
          return ((Double)((Double)approvedAddress / (Double)100000000)).ToString();
        }
        else
        {
          return approvedAddress.ToString();
        }

      }
      catch (Exception Ex)
      {
        //Log Error
      }
      return "0";
    }

    [Function("transfer", "bool")]
    public class TransferFunction : Nethereum.Contracts.CQS.ContractMessage
    {
      [Nethereum.ABI.FunctionEncoding.Attributes.Parameter("address", "_to", 1)]
      public string To { get; set; }

      [Nethereum.ABI.FunctionEncoding.Attributes.Parameter("uint256", "_value", 2)]
      public BigInteger TokenAmount { get; set; }
    }

    public async static Task<Boolean> SendGiftTokens(string usrWallet)
    {
      //This is not working yet
      Models.Wallet.CreateTransactionModel _transaction = new Models.Wallet.CreateTransactionModel() { amount = "25", hashFrom ="mainAddres" , hashTo = usrWallet, hashTransaction = "" };

      var mainAddress = AppSettings.MainAddress;
      var web3 = new Nethereum.Web3.Web3(AppSettings.BlockchainURL);

      try
      {
       
        string Amount = "100000000";
        var unlockMainAddress = await web3.Personal.UnlockAccount.SendRequestAsync(mainAddress, AppSettings.MainAddressPassphrase, 100);
        if (unlockMainAddress)
        {

          HexBigInteger gas = new HexBigInteger(90000);
          HexBigInteger gas2 = new HexBigInteger(0);

          var functionBalance = GetContractFunction("balanceOf");
          var _balance = await functionBalance.CallAsync<Int64>(mainAddress);

          if (Convert.ToDouble(Amount) > Convert.ToDouble(_balance))
          {
            _transaction = null;
            return false;
          }

          var functionTransfer = GetContractFunction("transfer");
          var _result = await functionTransfer.SendTransactionAsync(mainAddress, gas, gas2, usrWallet, new BigInteger(float.Parse(Amount)));
          _transaction.hashTransaction = _result.ToString();
        }

        //Se Debe Registrar la transaccion en la DB

      }
      catch (Exception Ex)
      {
        return false;
        //System.IO.File.WriteAllText(System.Web.Hosting.HostingEnvironment.MapPath("//logs") + "\\" + DateTimeHelper.GetTimestamp() + ".txt", Ex.Message.ToString());
      }
      finally
      {
        await web3.Personal.LockAccount.SendRequestAsync(mainAddress);
      }
      return true;
    }
  }
}
