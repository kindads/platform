using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace KindAds.Models.Home
{
  [Serializable]


  public class HomeViewModel
  {

    [Display(Name = "MainTitle", ResourceType = typeof(Resource.Language.Home.Home.Resources))]
    public string MainTitle { get; set; }
  }







}
