var siteModule = (function () {

  var data = {
    GtmToken: '',
    IdSite: ''
  };

  var createSite = {
    URL: '',
    protocoloSelected: '',
    categorySelected: '',
    Name:''
  }

  //Private methods
  var getGTMToken = function (IdSite) {

    $.ajax({
      url: "GetGoogleTagManagerToken",
      type: "POST",
      data: JSON.stringify(
        {
          IdSite: IdSite
        }),
      cache: false,
      contentType: "application/json",
      success: function (response) {
        if (response.success) {

          $("#GtmToken").val(response.Token);
          $("#IdSite").val(IdSite);

          data.IdSite = IdSite;
          data.GtmToken = response.Token;
        }
      },
      error: function (xhr, error, status) {
        //Error
      }
    });
  };

  var sendAzureValidation = function (IdSite, ClientAppId, SubscriptionId, TenantId, AppKey,type) {
    console.log("Type value");
    console.log(type);

    $.ajax({
      url: "VerifySiteAzure",
      type: "POST",
      data: JSON.stringify(
        {
          IdSite: IdSite,
          ClientAppId: ClientAppId,
          SubscriptionId: SubscriptionId,
          TenantId: TenantId,
          AppKey: AppKey,
          Type: type
        }),
      cache: false,
      contentType: "application/json",
      success: function (response) {
        if (response.success) {
          $("#SiteVerify").show();
          $("#SiteNoVerify").hide();
          $("#SiteVerifySuccess").show();
          $("#SiteVerifyFailure").hide();
        } else {
          $("#SiteVerify").show();
          $("#SiteNoVerify").hide();
          $("#SiteVerifyFailure").show();
          $("#SiteVerifySuccess").hide();
        }

      },
      error: function (xhr, error, status) {

        $("#SiteVerify").show();
        $("#SiteNoVerify").hide();
        $("#SiteVerifyFailure").show();
        $("#SiteVerifySuccess").hide();
      }
    });
  };

  var sendGoogleTagManagerValidation = function (IdSite,type) {

    $.ajax({
      url: "VerifySiteTxtAndGTM",
      type: "POST",
      data: JSON.stringify({ IdSite: IdSite, Type: type }),
      cache: false,
      contentType: "application/json",
      success: function (response) {
        if (response.success) {
          $("#SiteVerify").show();
          $("#SiteNoVerify").hide();
          $("#SiteVerifySuccess").show();
          $("#SiteVerifyFailure").hide();
        } else {
          $("#SiteVerify").show();
          $("#SiteNoVerify").hide();
          $("#SiteVerifyFailure").show();
          $("#SiteVerifySuccess").hide();
        }

      },
      error: function (xhr, error, status) {
        $("#SiteVerify").show();
        $("#SiteNoVerify").hide();
        $("#SiteVerifyFailure").show();
        $("#SiteVerifySuccess").hide();
      }
    });
  };

  var sendTxtValidation = function (IdSite,type) {

    $.ajax({
      url: "VerifySiteTxtAndGTM",
      type: "POST",
      data: JSON.stringify({ IdSite: IdSite, Type: type }),
      cache: false,
      contentType: "application/json",
      success: function (response) {
        if (response.success) {
          $("#SiteVerify").show();
          $("#SiteNoVerify").hide();
          $("#SiteVerifySuccess").show();
          $("#SiteVerifyFailure").hide();
        } else {
          $("#SiteVerify").show();
          $("#SiteNoVerify").hide();
          $("#SiteVerifyFailure").show();
          $("#SiteVerifySuccess").hide();
        }
      },
      error: function (xhr, error, status) {

        $("#SiteVerify").show();
        $("#SiteNoVerify").hide();
        $("#SiteVerifyFailure").show();
        $("#SiteVerifySuccess").hide();
      }
    });
  };

  //Public methods
  return {
    // delete sites
    deleteSiteRow: function (idSite, url, isSiteVerified) {

      var r = confirm("The site " + url + " will be deleted. Are you sure?");
      if (r === true) {
        var urlCall = 'DeleteSite';
        $.ajax({
          url: urlCall,
          data: { IdSite: idSite },
          success: function (data) {
            if (!data.isDeleted) {
              alert('The site was not deleted because it has associated products.');
            }
            else {
              alert('Site deleted successfully')
              if (isSiteVerified)
                $("#pnlGridVerifyAjax").submit();
              else
                $("#pnlGridPendingAjax").submit();
            }
          },
          error: function (data) {
            if (!data.isDeleted)
              alert('The site was not deleted because it has associated products.');
            else
              alert('Error!');
          }
        });
      }

    },
    // create section
    ready: function () {  
      $('[data-toggle="tooltip"]').tooltip();
      $('.link-dashboard').addClass('active');
    },

    bindingForm: function () {     
      $('#formSite').submit(function (e) {

        // Get values
        createSite.URL = this.viewModel_URL.value;
        createSite.protocoloSelected = this.viewModel_protocoloSelected.value;
        createSite.categorySelected = this.viewModel_categorySelected.value;
        createSite.Name = this.viewModel_Name.value;

        if ($("#formSite").valid()) {
          e.preventDefault();
          $.ajax({
            url: this.action,
            type: "POST",
            data: JSON.stringify(
              {
                URL: createSite.URL,
                protocoloSelected: createSite.protocoloSelected,
                categorySelected: createSite.categorySelected,
                Name: createSite.Name
              }),
            cache: false,
            contentType: "application/json",
            success: function (response) {
              console.log("Response:"+ response.success);
              if (response.success) {
                $('#lblTitle').append('Congratulations!');
                $('#lblBody').append(response.message);
                $('#lblSiteUrl').append($('#viewModel_protocoloSelected').val().concat($('#viewModel_URL').val()));
                $("#pnlInfoSite").show();
                $("#idSite").val(response.idSite);
                $('#myModal').modal('show');
              } else {
                $('#lblTitle').append('Error');
                $('#lblBody').append(response.error);
                $("#pnlInfoSite").hide();
                $('#myModal').modal('show');
              }
            },
            error: function (xhr, error, status) {
              $('#lblTitle').append('Error');
              $('#lblBody').append(status);
              $("#pnlInfoSite").hide();
              $('#myModal').modal('show');
            }
          });
        }
      });      
    },

    //Show section
    readyShowSites: function () {
      $('.link-dashboard').addClass('active');
      $('.datagrid thead th:contains("Delete")').addClass('icon-delete');
    },

    bindingOnClickShowSites: function () {
      $("#pnlGridPending").on("click", "thead th a, tfoot a", function (e) {
        e.preventDefault();
        var param = $(this).attr('href').split('?')[1];
       
        $.ajax({
          url: 'ShowSitesPending' + '?' + param,
          type: 'GET',
          data: '',
          dataType: 'html',
          success: function (data) {
            $('#pnlGridPending').html(data);
          },
          error: function () {
            alert('Error!');
          }
        });
      });

      $("#pnlGridVerify").on("click", "thead th a, tfoot a", function (e) {
        e.preventDefault();
        var param = $(this).attr('href').split('?')[1];
        var url = 'ShowSitesVerify' + '?' + param;
        $.ajax({
          url: url,
          type: 'GET',
          data: '',
          dataType: 'html',
          success: function (data) {
            $('#pnlGridVerify').html(data);
          },
          error: function () {
            alert('Error!');
          }
        });
      });
    },
    
    //Validate section
    chooseValidation: function (type) {
      $('#TypeValidation').val(type);


      if (type === 1) {
        $('#AzureForm').show();
        $('#validateChooseOptionBack').show();

        $('#descriptionAzure').show();
        $('#descripcionGoogleTagManager').hide();
        $('#descripcionTxt').hide();
      }
      else if (type === 2) {
        data.IdSite = $("#IdSite").val();
        getGTMToken(data.IdSite);
        $('#validateChooseOptionBack').show();
        $('#AzureForm').hide();

        $('#descriptionAzure').hide();
        $('#descripcionGoogleTagManager').show();
        $('#descripcionTxt').hide();       
      }
      else {
        $('#validateChooseOptionBack').show();
        $('#AzureForm').hide();

        $('#descriptionAzure').hide();
        $('#descripcionGoogleTagManager').hide();
        $('#descripcionTxt').show();
      }
      $('#siteValidationOptions').hide();
    },
    goBackSiteValidation: function () {
      $('#validateChooseOptionBack').hide();
      $('#AzureForm').hide();
      $('#siteValidationOptions').show();

      $('#descriptionAzure').hide();
      $('#descripcionGoogleTagManager').hide();
      $('#descripcionTxt').hide();
    },
    goBack: function () {
      window.history.back();
    },
    bindingFormSite: function () {

      $('#formValidateSite').submit(function (e) {
        e.preventDefault();

        if (this.type.value === '1') {

          //Send to Azure validation
          var IdSite = this.IdSite.value;
          var ClientAppId = this.ad_ClientAppId.value;
          var SubscriptionId = this.ad_SubscriptionId.value;
          var TenantId = this.ad_TenantId.value;
          var AppKey = this.ad_AppKey.value;
          
          sendAzureValidation(IdSite, ClientAppId, SubscriptionId, TenantId, AppKey, 1);
        }
        else if (this.type.value === '2') {

          //Send to google tag manager validation
          sendGoogleTagManagerValidation(this.IdSite.value, 2);
        }
        else if (this.type.value === '3') {

          //Send to txt validation
          sendTxtValidation(this.IdSite.value, 3);
        }        
      });
    }
  }
})();
