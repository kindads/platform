//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KindAds.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class CAMPAIGN_SETTINGS
    {
        public System.Guid IdCampaignSetting { get; set; }
        public string SettingName { get; set; }
        public string SettingValue { get; set; }
        public System.Guid CAMPAIGNs1_IdCampaign { get; set; }
    
        public virtual CAMPAIGN CAMPAIGNs1 { get; set; }
    }
}
