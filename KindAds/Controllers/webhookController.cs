using KindAds.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace KindAds.Controllers
{
    public class webhookController : ApiController
    {

    [HttpPost]
    public async System.Threading.Tasks.Task<HttpResponseMessage> eth()
    {

      //Newtonsoft.Json.Linq.JObject response = new Newtonsoft.Json.Linq.JObject();
      Stream req = await Request.Content.ReadAsStreamAsync();
      req.Seek(0, SeekOrigin.Begin);
      string json = new StreamReader(req).ReadToEnd();

      //string jsonvalue = 
      //json = json.Substring(0, json.IndexOf("value"));
      //json = json.Substring(0, json.Length - 1) + "}";

      Models.Wallet.webhookRequest _request = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Wallet.webhookRequest>(json);


      //Add the transaction
      if (_request.@event.ToString().Trim().ToLower() == "transfer")
      {
        KindadsEntities _entities = new KindadsEntities();
        AspNetUser _userFrom = (from _u in _entities.AspNetUsers where _u.WalletAddress.Equals(_request.args.@from) select _u).FirstOrDefault();
        if (_userFrom != null)
        {
          //El usuario existe, es probablemente un cash out, entonces el "TO" NO deberia estar en la base de datos
          AspNetUser _userTo = (from _u in _entities.AspNetUsers where _u.WalletAddress.Equals(_request.args.to) select _u).FirstOrDefault();
          if (_userTo != null)
          {
            //El usuario existe, por lo tanto es una transfer interna
          }
          else
          {
            //El usuario no existe, es un cashout
            TRANSACTIONS_EXTERNAL _transaction = new TRANSACTIONS_EXTERNAL() { Amount = _request.args.value, HashFrom = _request.args.from, HashTo = _request.args.to, HashTransaction = _request.transactionHash, TRANSACTION_TYPE_IdTransactionType = 2, Gas = "0", RegisterDate = Helpers.DateTimeHelper.CurrentDateTimeString(), IdTransactionext = Guid.NewGuid() };
            _entities.TRANSACTIONS_EXTERNAL.Add(_transaction);
            _entities.SaveChanges();
          }
        }
        else
        {
          //El usuario FROM no existe, se verifica que exista en TO, de ser asi, es un cash in
          AspNetUser _userToCashin = (from _u in _entities.AspNetUsers where _u.WalletAddress.Equals(_request.args.to) select _u).FirstOrDefault();
          if (_userToCashin != null)
          {
            //El usuario existe, es un cash in
            //El usuario no existe, es un cashout
            TRANSACTIONS_EXTERNAL _transactionCashin = new TRANSACTIONS_EXTERNAL() { Amount = _request.args.value, HashFrom = _request.args.from, HashTo = _request.args.to, HashTransaction = _request.transactionHash, TRANSACTION_TYPE_IdTransactionType = 2, Gas = "0", RegisterDate = Helpers.DateTimeHelper.CurrentDateTimeString(), IdTransactionext = Guid.NewGuid() };
            _entities.TRANSACTIONS_EXTERNAL.Add(_transactionCashin);
            _entities.SaveChanges();
          }
        }
      }

      HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, "Se ha registrado la transaccion");
      response.Content = new StringContent("hello", System.Text.Encoding.Unicode);
      response.Headers.CacheControl = new CacheControlHeaderValue()
      {
        MaxAge = TimeSpan.FromMinutes(20)
      };
      return response;
    }


    //[HttpGet]
    //public async System.Threading.Tasks.Task<HttpResponseMessage> balance()
    //{

    //  var xx = await Helpers.NethereumHelper.GetBalance("0xd42e518424856c5f0b9bb88b2b3f42597d54826a",true);

    //  HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, "Se ha registrado la transaccion");
    //  response.Content = new StringContent("hello", System.Text.Encoding.Unicode);
    //  response.Headers.CacheControl = new CacheControlHeaderValue()
    //  { 
    //    MaxAge = TimeSpan.FromMinutes(20)
    //  };
    //  return response;
    //}

  }
}
