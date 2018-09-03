using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Optimization;

namespace KindAds {
  public class BundleConfig
  {
    // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
    public static void RegisterBundles(BundleCollection bundles)
    {
      bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
          "~/Scripts/jquery-{version}.js"
      ));

      bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
          "~/Scripts/jquery.unobtrusive*",
          "~/Scripts/jquery.validate*"
      ));

      bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
          "~/Scripts/knockout-{version}.js",
          "~/Scripts/knockout.validation.js"
      ));

      bundles.Add(new ScriptBundle("~/bundles/app").Include(
          "~/Scripts/sammy-{version}.js",
          "~/Scripts/app/common.js",
          "~/Scripts/app/app.datamodel.js",
          "~/Scripts/app/app.viewmodel.js",
          "~/Scripts/app/home.viewmodel.js",
          "~/Scripts/app/_run.js"
      ));

      // Use the development version of Modernizr to develop with and learn from. Then, when you're
      // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
      bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
          "~/Scripts/modernizr-*"
      ));

      //Específico para pantallas de HOME Y ACCESS Controller
      bundles.Add(new StyleBundle("~/Content/css").Include(
           "~/Content/bootstrap.css",
           "~/Content/Site.css"
      ));

      bundles.Add(new ScriptBundle("~/bundles/customjs").Include(
        "~/Scripts/jquery-{version}.js",
        "~/Scripts/vendor/bootstrap/bootstrap.min.js",
        "~/Scripts/vendor/rangeSlider/rangeslider.min.js",
        "~/Scripts/jquery.unobtrusive-ajax.min.js",
        "~/Scripts/jquery.validate.js",
        "~/Scripts/jHtmlArea-0.8.min.js"
      ));

      bundles.Add(new ScriptBundle("~/bundles/basejs").Include(
        //jquery
        "~/Scripts/jquery-3.3.1.min.js",
        "~/Scripts/jquery.unobtrusive-ajax.min.js",
        "~/Scripts/jquery.validate.min.js"
      ));

      bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
        //bootstrap
        "~/Scripts/vendor/bootstrap/bootstrap.min.js",
        //"~/Scripts/respond-min.js",
        "~/Scripts/vendor/retina/retina.min.js"
      ));

      bundles.Add(new ScriptBundle("~/bundles/signalr").Include(
        //Signal R
        "~/Scripts/jquery.signalR-2.2.3.min.js",
        "~/Scripts/signalr/hubs.js",
        "~/Scripts/signalr/signalRProducts.js"
      ));

    bundles.Add(new ScriptBundle("~/bundles/web3").Include(
        //Web 3
        "~/Scripts/web3/web3Functions.js"
      ));
 
    }
  }
}
