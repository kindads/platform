var moneyAddsModule = (function () {

  var data = {
    typeAdd: ''
  };

  return {
    showDefaultAddBody: function () {
      $("#bcDefaultAdd").show();
      $("#bcStickAdd").hide();
      data.typeAdd = '1';
      $("#AdsSelected").html("Default ads");
    },

    showStickAddBody: function () {
      $("#bcDefaultAdd").hide();
      $("#bcStickAdd").show();
      data.typeAdd = '2';
      $("#AdsSelected").html("Sticky ads");
    },

    onSelectTypeAds: function () {
      console.log("Change select");
      var val = $("#comboTypeAds").val();
      if (val === '2') {
        $("#panelDefaultAds").hide();
      }
      else if (val === '1') {
        $("#panelDefaultAds").show();
      }
    },

    ready:function () {
      //disable checkbox
      $("#chkStickAdd").attr("disabled", true);
      $("#chkDefaultAdd").attr("disabled", true);
      data.typeAdd = '1';
      $("#AdsSelected").html("Default ads");

      //Binding fileup change
      $("#fileup").change(function () {
        var file = $('#fileup')[0].files[0]
        if (file) {
          $("#filename").text(file.name);
        }
      });

    },
    bindingForm: function () {
      $("#linkWorkspace").removeClass("active");
      $("#linkMoneyAds").addClass("active");
    },
    bindingGridAds: function () {
      $("#pnlGridDeafultAds").on("click", "thead th a, tfoot a", function (e) {
        e.preventDefault();
        var param = $(this).attr('href').split('?')[1];

        $.ajax({
          url: 'GetDefaultAds' + '?' + param,
          type: 'GET',
          data: '',
          dataType: 'html',
          success: function (data) {
            $('#pnlGridDeafultAds').html(data);
          },
          error: function () {
            alert('Error!');
          }
        });
      });

      $("#pnlGridStickyAds").on("click", "thead th a, tfoot a", function (e) {
        e.preventDefault();
        var param = $(this).attr('href').split('?')[1];
        var url = 'GetStickyAds' + '?' + param;
        $.ajax({
          url: url,
          type: 'GET',
          data: '',
          dataType: 'html',
          success: function (data) {
            $('#pnlGridStickyAds').html(data);
          },
          error: function () {
            alert('Error!');
          }
        });
      });
    },
    activateAdd: function () {
      console.log("Type:" + data.typeAdd);
      if (data.typeAdd == '1') {
       //Show Dafult form
        var valChk = $("#chkDefaultAdd").prop("checked");

        if (valChk == true) {
          $("#chkDefaultAdd").prop("checked", false);
        }
        else {
          $("#chkDefaultAdd").prop("checked", true);
        }        
      }
      else {
        //Show Sticky form
        var valChk = $("#chkStickAdd").prop("checked");

        if (valChk == true) {
          $("#chkStickAdd").prop("checked", false);
        }
        else {
          $("#chkStickAdd").prop("checked", true);
        }   
      }
    }
  }
})();
