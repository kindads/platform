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
    
    public partial class UserDetail
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public System.DateTime RegistrationDate { get; set; }
        public bool IsPremium { get; set; }
        public string UserId { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
    }
}