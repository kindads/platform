

function getNetworkNameMetamask() {
      var networkId = web3.version.network
      switch (networkId) {
        case "1":
          networkName = "main";
          break;
        case "2":
          networkName = "morden";
          break;
        case "3":
          networkName = "ropsten";
          break;
        case "4":
          networkName = "rinkeby";
          break;
        case "42":
          networkName = "kovan";
          break;
        default:
          networkName = "unknown";
      }
      return networkName;
}


function IsMetamaskInBrowser() {
      if (typeof web3 !== 'undefined') {
        try {
          web3 = new Web3(web3.currentProvider);
          return true;
        }
        catch (err) {
          return false;
        }
      } else {
        return false;
      }
}

function GetCurrentWallet() {
  return web3.eth.coinbase;
}

function CheckBrowserCompatibility() {


  isIE = /*@@cc_on!@@*/false || !!document.documentMode;
  isEdge = !isIE && !!window.StyleMedia;
  if ((!!window.opr && !!opr.addons) || !!window.opera || navigator.userAgent.indexOf(' OPR/') >= 0) {
    return {
      validBrowser: true,
      browserName:'opera'
    };
  }
  if (navigator.userAgent.indexOf("Chrome") != -1 && !isEdge) {
    return {
      validBrowser: true,
      browserName:'chrome'
    };
  }
  else if (navigator.userAgent.indexOf("Firefox") != -1) {
    return {
      validBrowser: true,
      browserName:'firefox'
    };
  }
  else if (navigator.userAgent.indexOf("Safari") != -1 && !isEdge) {
    return {
      validBrowser: false,
      browserName:'safari'
    };
  }
  else if ((navigator.userAgent.indexOf("MSIE") != -1) || (!!document.documentMode == true)) {
    return {
      validBrowser: false,
      browserName:'ie'
    };
  }
  else if (isEdge) {
    return {
      validBrowser: false,
      browserName:'edge'
    };
  }
  else {
    return {
      validBrowser: false,
      browserName:'other-browser'
    };
  }
}
