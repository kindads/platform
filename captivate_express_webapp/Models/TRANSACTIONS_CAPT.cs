//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace captivate_express_webapp.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TRANSACTIONS_CAPT
    {
        public int IdTransaction { get; set; }
        public string HashFrom { get; set; }
        public string HashTo { get; set; }
        public string Amount { get; set; }
        public string BlockDate { get; set; }
        public string RegisterDate { get; set; }
        public string HashTransaction { get; set; }
        public string Gas { get; set; }
        public short TRANSACTION_TYPE_IdTransactionType { get; set; }
    
        public virtual TRANSACTION_TYPE TRANSACTION_TYPE { get; set; }
        public virtual CAMPAIGN CAMPAIGN { get; set; }
    }
}
