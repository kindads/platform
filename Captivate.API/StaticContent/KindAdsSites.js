
var kindAds = (function () {

    // private object same as: Captivate.Comun.Models.SiteValidationToken
    var siteData = {
        RequestUrl: '',
        Token: '',
        SiteId: '',
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
        validateSite: function (url, apiToken, idSite) {
            siteData.RequestUrl = url;
            siteData.Token = apiToken;
            siteData.SiteId = idSite;
            siteData.SiteUrl = location.hostname;
            send();
        }
    }
})();

