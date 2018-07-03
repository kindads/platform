var metaMask = (function () {
  // properties -------------------------------------------------------------
  var info = {
    ApiUrl: '',
    EnableTest:1
  }

  var Log = function (message) {
    if (info.EnableTest === 1) {
      console.log(message);
    }
  }

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
        ok();       
      },
      error: function (ex) {
        exception();
      }
    });
  };

  // Private methods -------------------------------------------------------------

  var getBalance = function () {
      // obtain balance with web3
    var balance = 0;
    //todo
    //for test
    balance = 5;
    return balance;
  };

  var currentBalanceIsEnough = function (price) {
    var result = 0;
    var balance = getBalance();
    if (balance >= price) {
      result = 1;
    }
    return result;
  };

  var campaignSettingsAreValid = function (idCampaign) {
    var result = 0;
    //api/Campaign?idCampaign={idCampaign}
    var controller = "Campaign?idCampaign=" + idCampaign;
    Log("Controller:" + controller);

    getFromApi(controller, function () {
      Log("Ok");
    }, function () {
      Log("Error");
      });

    result = 1;
    return result;
  };

  var sendTransaction = function (callback) {
    //senf transaction to web3
    var result = 0;   
   
    var controller = "ProcessCampaign?idCampaign=" + idCampaign;
    Log("Controller:" + controller);

    getFromApi(controller, function () {
      Log("Ok");
      callback();
    }, function () {
      Log("Error");
    });
    return result;
  };

  var processCampaign = function (callback) {
    // send campaign
    var result = 0;

    callback();
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

    currentBalanceEnough: function (price, idCampaign) {
      var result = 0;
      var currentBalanceResult = currentBalanceIsEnough(price);

      if (currentBalanceResult === 1) {
        // check setting are valid
        Log(">>> Current balance is enough");
        result = campaignSettingsAreValid(idCampaign);
      }

      return result;
    },

    payCampaign: function ( callback ) {
      //Send transaction
      sendTransaction(callback);
    },

    sendCampaign: function ( callback ) {
      processCampaign(callback);
    },

    log: function (message) {
      Log(message);
    },

    init: function (url) {
      // API url
      info.ApiUrl = url;
    }
  }
})();
