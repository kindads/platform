//import { win32 } from "path";

var languageModule = (function () {

  return {

    bindingForm: function () {
      $("#LanguagesList").empty();
      var cultureSel = $("#hideCultureSel").val();
      $.ajax({
        type: 'POST',
        url: "/" + cultureSel +"/Base/GetLanguages",
        dataType: 'json',
        success: function (elements) {
          if (elements != null && elements != '') {
            //$("#LanguagesList").append('<option value="">' + 'Please select' + '</option>');
            $.each(elements, function (i, state) {
              $("#LanguagesList").append('<option value="' + state.Value + '">' + state.Text + '</option>');
            });
           
            $("#LanguagesList > [value=" + cultureSel + "]").attr("selected", "true");
          } else {
            $("#LanguagesList").append('<option value="">' + 'Please select' + '</option>');
          }
        },
        error: function (ex) {
          alert('Failed to retrieve list languajes.' + ex);
        }
      });

      $("#LanguagesList").change(function () {
        var controllerName = $("#hideControllerName").val();
        var actionName = $("#hideActionName").val();
        var cultureSelection = $("#LanguagesList").val();
        var url = "";

        if (controllerName.toLowerCase() == "home" && (actionName.toLowerCase() == "faq" || actionName.toLowerCase() == "team")) {
          window.location.search = "?culture=" + cultureSelection;
        } else {
          url = $("#LanguagesList").val() + "/" + controllerName + "/" + actionName + window.location.search;
          window.location.pathname = url;
        }

      });

    }
  }

})();
