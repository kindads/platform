using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Captivate.Common.Models
{
  public class CreateWalletModel
  {
    public string walletaddress { get; set; }
    public string blobname { get; set; }
  }

  public class CreateTransactionModel
  {
    public string hashTransaction { get; set; }
    public string hashFrom { get; set; }
    public string hashTo { get; set; }
    public string amount { get; set; }
  }

  public class webhookRequest
  {
    public string address { get; set; }
    public Double blockNumber { get; set; }
    public string transactionHash { get; set; }
    public string @event { get;set; }
    public hookArgs args { get; set; }

  }
  public class hookArgs
  {
    public string from { get; set; }
    public string to { get; set; }
    public Double value { get; set; }
  }

  public class walletTransactions
  {
    public string hashfrom { get; set; }
    public string hashto { get; set; }
    public Double value { get; set; }
    public string date { get; set; }
    public string hashtransaction { get; set; }
    public string inout { get; set; }
  }

}
