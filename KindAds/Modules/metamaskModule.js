var metaMask = (function () {
  // properties -------------------------------------------------------------
  var info = {
    ApiUrl: '',
    EnableTest: 1,
    MetamaskIsEnabled: 0,
    LogIn: 0,
    Account: '',
    RowBalance: 0,
    DivideBy: 100000000,
    UserBalance: 0,
    Abi: '',
    ContractAddress: "0xa67c799f22df6a30fc5439836f3c530fc87b6718",
    Wallet:''
  }

  var Log = function (message) {
    if (info.EnableTest === 1) {
      console.log(message);
    }
  }

  var ether = undefined; 

  // Communication methods -------------------------------------------------------------

  var sendToApi = function (controller,info, ok, exception) {
    var urlAction = info.ApiUrl + controller;

    $.ajax({
      type: 'POST',
      url: urlAction,
      data: info,
      dataType: 'json',
      success: function (response) {
        ok();
      },
      error: function (xhr, error, status) {
        exception();
      }
    });
  };


  var getFromApi = function (controller,ok, exception) {
    var urlAction = info.ApiUrl + controller;
    Log(">> Complete url:" + urlAction);

    $.ajax({
      type: 'GET',
      url: urlAction,
      success: function (result) {
        Log("Result is:" + result);
        ok(result);       
      },
      error: function (ex) {
        exception();
      }
    });
  };

  // Private methods -------------------------------------------------------------

  var getBalance = function (callback) {
      // obtain balance with web3
    //var balance = 0;

    //var address = ether.eth.coinbase;

    //ether.eth.getBalance(address, function (err, data) {
    //  info.RowBalance = data;
    //  info.UserBalance = data / info.DivideBy;
    //  callback(info.UserBalance);
    //});

    //balance = info.UserBalance;

    var AbiJson = JSON.parse(info.Abi);
    Log(">>>>> Get KindAds Balance");
    if (info.MetamaskIsEnabled === 1 && info.LogIn === 1) {

      try {
        Log(">>>>> Get script");
        //var address = ether.eth.coinbase;
        var address = info.Wallet;
        //var address = '0x166347e6ca169132f49a2a33254fc90c8e94f976'; // for test
        var kindAdsContract = ether.eth.contract(AbiJson);
        var kindAdsInstance = kindAdsContract.at(info.ContractAddress);        
        var balance = kindAdsInstance.balanceOf(address, function (error, result) {         
          info.RowBalance = result;
          info.UserBalance = info.RowBalance / info.DivideBy;
          Log(">>>>>   Kind Ads balance:" + info.UserBalance);
          callback(info.UserBalance);
        });
      } catch (error) {
        Log(error);
      }
    }

  };

  var currentBalanceIsEnough = function (price, callback, closemodal) {
    var result = 0;
    getBalance(function (balance) {
      Log("Check if is Enough");
      Log(">> Ether balance is:" + balance);
      Log(">> Price is:" + price);

      if (balance >= price) {
        Log("Balance is greather than price");
        result = 1;
        callback(1);
      }
      else {
        Log("No es suficiente");
        closemodal();
      }      
    });   
    return result;
  };

  var campaignSettingsAreValid = function (idCampaign,callback) {
    var result = 0;
    //api/Campaign?idCampaign={idCampaign}
    var controller = "Campaign?idCampaign=" + idCampaign;
    Log("Controller:" + controller);

    getFromApi(controller, function (result) {
      Log(">>> Ok result is:" + result);
      callback(result);
    }, function () {
      Log("Error");
      });

    result = 1;
    return result;
  };

  var sendTransaction = function (callback,idCampaign) {
    // send transaction to web3
    var result = 1;
    callback();
    return result;
  };

  var processCampaign = function (callback, idCampaign) {
    // realizamos la peticion al send controller
    var result = 0;
    Log(">>> IdCampaign:" + idCampaign);
    var controller = "ProcessCampaign?idCampaign=" + idCampaign;
    Log(">>> Controller:" + controller);

    getFromApi(controller, function () {
      Log("Ok");
      result = 1;
      callback();
    }, function () {
      Log("Error");
      });

    return result;
  }

  //Public methods
  return {
    getBalance: function () {
      Log("Get balance");
      // Use: web3.eth.getBalance
    },

    sendTransaction: function () {
      Log("Send transaction");
      // Use: web3.eth.sendTransaction
    },

    bindContract: function () {
      Log("bind contract");
      // Use: web3.eth.contract
    },

    callContractMethod: function () {
      Log("call contract methods");
      // Use instance of contract
    },

    callContractEvent: function () {
      Log("call contract event");
      // Use instance of contract
    },

    currentBalanceEnough: function (price, idCampaign,callback,closemodal) {
      if (info.MetamaskIsEnabled === 1 && info.LogIn === 1) {
        var result = 0;
        var currentBalanceResult = currentBalanceIsEnough(price, function (currentBalanceResult) {
          if (currentBalanceResult === 1) {
            // check setting are valid
            Log(">>> Current balance is enough");
            campaignSettingsAreValid(idCampaign, callback);
          }
        }, closemodal);        
      }      
    },

    chargeCampaign: function ( callback ,idCampaign) {
      //Send transaction
      if (info.MetamaskIsEnabled === 1 && info.LogIn === 1) {
        Log(">> IdCampaign:" + idCampaign);
        sendTransaction(callback, idCampaign);
      }      
    },

    sendCampaign: function (callback, idCampaign) {
      if (info.MetamaskIsEnabled === 1 && info.LogIn === 1 ) {
        Log(">>> Send campaign whit idCampaign:" + idCampaign);
        processCampaign(callback, idCampaign);
      }      
    },

    log: function (message) {
      Log(message);
    },

    init: function (url,abi,wallet,callback) {
      // API url
      var result = 0;
      Log("Init metamask module");
      info.ApiUrl = url;
      info.Abi = abi;
      info.Wallet = wallet;

      try {
        if (typeof web2 === 'undefined') {
          Log("Web 3 creation");
          ether = new Web3(web3.currentProvider);          
          info.MetamaskIsEnabled = 1;
          result = 1;
        }
      }
      catch (err) {
        Log("Metamask is not enable");
        callback();
      }      

      return result;
    },

    // web3.js methods ---------------------------------------------------------

    isConnected: function () {      
      if (info.MetamaskIsEnabled === 1) {
        try {
          var connected = ether.isConnected();
          Log("IsConnected:" + connected);
        }
        catch (err) {
          Log("Error is:" + err);
        }        
      }      
    },

    getCurrentProvider: function () {
      if (info.MetamaskIsEnabled === 1) {
        try {
          var currentProvider = ether.currentProvider;
          Log("Current provider is:");
          Log(currentProvider);
          return currentProvider;
        }
        catch (err) {
          Log("Error is:" + err);
        }        
      }      
    },

    getDefaultAccount: function (callback,succeded) {
      if (info.MetamaskIsEnabled === 1) {
        try {
          var defaultAccount = ether.eth.defaultAccount;
          Log("Default account is:");
          Log(defaultAccount);
          if (defaultAccount != undefined) {
            info.LogIn = 1;
            info.Account = defaultAccount;
            succeded();
          }
          else {
            callback();
          }
          return defaultAccount;
        }
        catch (err) {
        }
      }
    },

    getNetwork: function () {
      if (info.MetamaskIsEnabled === 1 && info.LogIn === 1) {
        var netId = ether.version.network;
        Log("Network is:" + netId);
      }
    },

    getBalance: function (callback) {
      if (info.MetamaskIsEnabled === 1 && info.LogIn === 1) {
        var address = ether.eth.coinbase;
        ether.eth.getBalance(address, function (err, data) {
          info.RowBalance = data;
          info.UserBalance = data / info.DivideBy;
          callback(info.UserBalance);
        });        
      }
    },

    getKindAdsBalance: function (Abi) {

      var AbiJson = JSON.parse(Abi); 
      
      Log(">>>>> Get KindAds Balance");
      if (info.MetamaskIsEnabled === 1 && info.LogIn === 1) {

        try {
          Log(">>>>> Get script");
          var address = ether.eth.coinbase;
          var kindAdsContract = ether.eth.contract(AbiJson);
          var kindAdsInstance = kindAdsContract.at("0xa67c799f22df6a30fc5439836f3c530fc87b6718");
          Log(">>>>>> Contract instance:");
          for (var key in kindAdsInstance) {
            Log(key);
          }
          var balance = kindAdsInstance.balanceOf(address, function (error, result) {
            Log(">>>>>   Kind Ads balance:" + result);
          });
        } catch (error) {
          Log(error);
        }

      }
    }    

   
  }
})();
