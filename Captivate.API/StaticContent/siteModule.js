var siteModule=(function () {

    return {
        ready: function () {
            $('[data-toggle="tooltip"]').tooltip();
            $('.link-dashboard').addClass('active');
        },
        bindingForm: function () {
            if ($("#formSite").valid()) {
                e.preventDefault();
                $.ajax({
                    url: 'CreateSite',
                    type: this.method,
                    data: new FormData(this),
                    cache: false,
                    contentType: false,
                    processData: false,
                    success: function (response) {
                        if (response.success) {
                            $('#lblTitle').append('Congratulations!');
                            $('#lblBody').append(response.message);
                            $('#lblSiteUrl').append($('#Protocols').val().concat($('#URL').val()));
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
                        $('#lblBody').append(response.error);
                        $("#pnlInfoSite").hide();
                        $('#myModal').modal('show');
                    }
                });
            }
        },
        bindingChanges: function () {
            //Con el  viewModel ya no deberian de necesitarse
            // binding change property for id: Protocols
            $("#Protocols").change(function () {
                $.ajax({
                    type: 'POST',
                    url: 'ProtocolSelecc',
                    data: { id: $("#Protocols").val() },
                    success: function (tags) { },
                    error: function (ex) {
                        alert('Failed to retrieve partners.' + ex);
                    }
                });
                return false;
            });

            // binding change property for id: Categories
            $("#Categories").change(function () {
                $("#Tags").empty();
                $.ajax({
                    type: 'POST',
                    url: 'CategorySelecc',
                    dataType: 'json',
                    data: { id: $("#Categories").val() },
                    success: function (tags) {
                    },
                    error: function (ex) {
                        alert('Failed to retrieve categories.' + ex);
                    }
                });
                return false;
            })
        }
    }

}) ();