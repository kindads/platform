
var Web3 = require('web3');
var web3 = undefined;

var checkIfTransactionAreMined = function (transactionId, context, callback) {

  var result = 0;

  // create web instance
  if (typeof web3 !== 'undefined') {
    web3 = new Web3(web3.currentProvider);
  } else {
    // set the provider you want from Web3.providers
    web3 = new Web3(new Web3.providers.HttpProvider("http://localhost:8545"));
  }

  // ciclo
  var ciclo = 1;

  while (ciclo === 1) {
    var transaction = web3.eth.getTransaction(transactionId);

    if (transaction.blockNumber != null) {
      ciclo = 0;
      context.res = {
        status: 200, /* Defaults to 200 */
        body: "1"
      };
      result = 1;      
    }

    ciclo = 0;
  }

  callback(result, context);
};

module.exports = function (context, req) {
  context.log('JavaScript HTTP trigger function processed a request.');

  if (req.query.transactionId || (req.body && req.body.transactionId)) {

    var transactionId = req.body.transactionId;

    checkIfTransactionAreMined(transactionId, context, function (result, context) {
      context.res = {
        status: 200, /* Defaults to 200 */
        body:  result
      };
    });    

  }
  else {

    // creamos la respuesta
    context.res = {
      status: 400,
      body: "Please pass a transactionId on the query string or in the request body"
    };
  }
  context.done();
};
