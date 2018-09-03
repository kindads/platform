

var migrateMetamask = (function () {
  return {
    init: function () {
      //$.ajax({
      //  type: 'POST',
      //  url: "/en-US/Base/ShowMigrateWallet",
      //  success: function (result) {
      //    if (!(result.success && result.isMigrate)) {
      //      $("#ethereumModal").modal("show");
      //    }
      //  },
      //  error: function (ex) {
      //    alert('Failed.' + ex);
      //  }
      //});

      $("#btnGetWalletMetamask").click(function (event) {
        if (typeof web3 !== 'undefined') {
          web3 = new Web3(web3.currentProvider);
          var address = web3.eth.coinbase;
          if (address != null && address != '') {
            $("#ethereumBtn").hide();
            $("#ethereumAddress").show();
            //$("#ethereumAddress").show();
            //$("#txtEthereumAdress").show();
            //$("#txtEthereumAdress").val(address);
            //$("#btnUpdateWalletMetamask").show();
            //$("#btnGetWalletMetamask").hide();
            //$("#pnlActionWalletMigrate").show();
          } else {
            alert('Please log in metamask');
          }
        } else {
          alert('Please add metamask plugin');
        }
      });

      $("#btnUpdateWalletMetamask").click(function (event) {
        event.preventDefault();
        if (typeof web3 !== 'undefined') {
          web3 = new Web3(web3.currentProvider);
          var address = web3.eth.coinbase;
          $("#pnlActionWalletMigrate").show();
          if (address != null && address != '') {
            $("#txtEthereumAdress").val(address);
          } else {
            alert('Please log in metamask');
          }
        } else {
          alert('Please add metamask plugin');
        }
      });

      $("#btnWalletMigrate").click(function (event) {
        web3 = new Web3(web3.currentProvider);
        var _address = web3.eth.coinbase;

        $.ajax({
          type: 'POST',
          url: "/en-US/Base/MigrateWalletEthereum",
          data: { address: _address },
          success: function (result) {
            if (result.success && result.result) {
              $("#pnlDataWalletMigrateSuccess").show();
              $("#pnlDataWalletMigrate").hide();
            } else {
              alert('Fail');
            }
          },
          error: function (ex) {
          }
        });
      });

      $("#btnCloseWalletMigrate").click(function (event) {
        $("#ethereumModal").modal("hide");
      });

      function isMainNetwork() {
        return ether.version.network == 1;
      }

    },

    GetBalance: function () {
      if (typeof web3 !== 'undefined') {
        web3 = new Web3(web3.currentProvider);
        var address = web3.eth.coinbase;// '0x166347e6ca169132f49a2a33254fc90c8e94f976';
        //var address = '0x166347e6ca169132f49a2a33254fc90c8e94f976';

        var tokenABI = [{ "constant": true, "inputs": [], "name": "name", "outputs": [{ "name": "", "type": "string" }], "payable": false, "stateMutability": "view", "type": "function" }, { "constant": false, "inputs": [{ "name": "_spender", "type": "address" }, { "name": "_value", "type": "uint256" }], "name": "approve", "outputs": [{ "name": "", "type": "bool" }], "payable": false, "stateMutability": "nonpayable", "type": "function" }, { "constant": false, "inputs": [{ "name": "token", "type": "address" }], "name": "reclaimToken", "outputs": [], "payable": false, "stateMutability": "nonpayable", "type": "function" }, { "constant": true, "inputs": [], "name": "totalSupply", "outputs": [{ "name": "", "type": "uint256" }], "payable": false, "stateMutability": "view", "type": "function" }, { "constant": false, "inputs": [{ "name": "_from", "type": "address" }, { "name": "_to", "type": "address" }, { "name": "_value", "type": "uint256" }], "name": "transferFrom", "outputs": [{ "name": "", "type": "bool" }], "payable": false, "stateMutability": "nonpayable", "type": "function" }, { "constant": true, "inputs": [], "name": "INITIAL_SUPPLY", "outputs": [{ "name": "", "type": "uint256" }], "payable": false, "stateMutability": "view", "type": "function" }, { "constant": true, "inputs": [], "name": "decimals", "outputs": [{ "name": "", "type": "uint8" }], "payable": false, "stateMutability": "view", "type": "function" }, { "constant": false, "inputs": [{ "name": "_spender", "type": "address" }, { "name": "_subtractedValue", "type": "uint256" }], "name": "decreaseApproval", "outputs": [{ "name": "", "type": "bool" }], "payable": false, "stateMutability": "nonpayable", "type": "function" }, { "constant": true, "inputs": [{ "name": "_owner", "type": "address" }], "name": "balanceOf", "outputs": [{ "name": "balance", "type": "uint256" }], "payable": false, "stateMutability": "view", "type": "function" }, { "constant": true, "inputs": [], "name": "owner", "outputs": [{ "name": "", "type": "address" }], "payable": false, "stateMutability": "view", "type": "function" }, { "constant": true, "inputs": [], "name": "symbol", "outputs": [{ "name": "", "type": "string" }], "payable": false, "stateMutability": "view", "type": "function" }, { "constant": false, "inputs": [{ "name": "_to", "type": "address" }, { "name": "_value", "type": "uint256" }], "name": "transfer", "outputs": [{ "name": "", "type": "bool" }], "payable": false, "stateMutability": "nonpayable", "type": "function" }, { "constant": false, "inputs": [{ "name": "_spender", "type": "address" }, { "name": "_addedValue", "type": "uint256" }], "name": "increaseApproval", "outputs": [{ "name": "", "type": "bool" }], "payable": false, "stateMutability": "nonpayable", "type": "function" }, { "constant": true, "inputs": [{ "name": "_owner", "type": "address" }, { "name": "_spender", "type": "address" }], "name": "allowance", "outputs": [{ "name": "", "type": "uint256" }], "payable": false, "stateMutability": "view", "type": "function" }, { "constant": false, "inputs": [{ "name": "newOwner", "type": "address" }], "name": "transferOwnership", "outputs": [], "payable": false, "stateMutability": "nonpayable", "type": "function" }, { "constant": false, "inputs": [{ "name": "_spender", "type": "address" }, { "name": "_currentValue", "type": "uint256" }, { "name": "_value", "type": "uint256" }], "name": "safeApprove", "outputs": [{ "name": "success", "type": "bool" }], "payable": false, "stateMutability": "nonpayable", "type": "function" }, { "inputs": [], "payable": false, "stateMutability": "nonpayable", "type": "constructor" }, { "anonymous": false, "inputs": [{ "indexed": true, "name": "previousOwner", "type": "address" }, { "indexed": true, "name": "newOwner", "type": "address" }], "name": "OwnershipTransferred", "type": "event" }, { "anonymous": false, "inputs": [{ "indexed": true, "name": "owner", "type": "address" }, { "indexed": true, "name": "spender", "type": "address" }, { "indexed": false, "name": "value", "type": "uint256" }], "name": "Approval", "type": "event" }, { "anonymous": false, "inputs": [{ "indexed": true, "name": "from", "type": "address" }, { "indexed": true, "name": "to", "type": "address" }, { "indexed": false, "name": "value", "type": "uint256" }], "name": "Transfer", "type": "event" }];
        var tokenAddress = "0xa67c799f22df6a30fc5439836f3c530fc87b6718";
        var tokenInst = web3.eth.contract(tokenABI).at(tokenAddress);

        if (address != null && address != '') {
          tokenInst.balanceOf(address, function (error, result) {
            if (!error) {
              //$("#lblBalance").text(parseFloat(web3.fromWei(result, "ether")));
              $("#lblBalance").text(parseFloat(result) / 100000000);
            }
            else
              alert(error);
          });
        } else {
          $("#lblBalance").text('---');
        }

        //web3.eth.getBalance(address, function (error, balance) {
        //  $("#lblBalance").text(parseFloat(web3.fromWei(balance, "ether")));
        //});

      } else {
        $("#lblBalance").text('---');
      }
    },

    ValidateMetamaskWalletAdvertiser: function (e, idComponent, culture) {
      e.preventDefault();
      $.ajax({
        type: 'POST',
        url: "/" + culture + "/Base/ShowMigrateWallet",
        success: function (result) {
          if (result.success) {
            if (result.isMigrate) {
              window.location.href = "/" + culture + "/Campaign/Index?idProduct=" + idComponent; 
              //$("#productComingSoon").modal("show");
            } else {
              $("#ethereumModal").modal("show");
            }
          } else {
            alert('Error');
          }
        },
        error: function (ex) {
          alert('Error');
        }
      });
    },

    ValidateMetamaskWalletPublisher: function (e, idComponent, culture) {
      e.preventDefault();
      $.ajax({
        type: 'POST',
        url: "/" + culture + "/Base/ShowMigrateWallet",
        success: function (result) {
          if (result.success) {
            if (result.isMigrate) {
              window.location.href = "/" + culture + "/Product/CreateProduct"; 
            } else {
              $("#ethereumModal").modal("show");
            }
          } else {
            alert('Error');
          }
        },
        error: function (ex) {
          alert('Error');
        }
      });
    }

  }

})();
