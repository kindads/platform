
var kindAds = (function () {

    // private object same as: KindAds.Comun.Models.SiteValidationToken
    var siteData = {
        RequestUrl: '',
        Token: '',
        AudienceId: '',
        SiteUrl: '',
        Ip: '',
        UserAgent: ''
    };

    // private methods
    var send = function () {
        $.ajax({
            type: 'POST',
            url: siteData.RequestUrl,
            data: siteData,
            dataType: 'json',
            success: function (result) {
                console.log("Congratulations!!, site information would be sending to verified site");
            },
            error: function (ex) {
                console.log(ex);
                console.log("Something goes wrong!!");
            }
        });
    };

    // public methods
    return {
        validateSite: function (url, apiToken, idAudience) {
            siteData.RequestUrl = url;
            siteData.Token = apiToken;
            siteData.AudienceId = idAudience;
            siteData.SiteUrl = location.hostname;
            send();
        }
    }
})();

