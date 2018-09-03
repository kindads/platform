var deviceMetrics = (function () {

    var Configuration = {
        IdUser: '',
        IdSite: ''
    };

    // Is a must set valie in script
    var Url = {
        mobileDetect: '',
        Api:''
    };

    var MobileInfo = {};

    var SendToApi = function (url, info, ok, exception) {
        $.ajax({
            type: 'POST',
            url: url,
            data: info,
            dataType: 'json',
            success: function (response) {
                ok()
            },
            error: function (xhr, error, status) {
                exception();
            }
        });
    };

    var ShowMobileInfo = function () {
        console.log("Show device info");

        //Send data to web API
        var Info = {
            Mobile: MobileInfo.mobile(),
            Phone: MobileInfo.phone(),
            Tablet: MobileInfo.tablet(),
            Os: MobileInfo.os(),
            IdUser: Configuration.IdUser,
            IdSite: Configuration.IdSite
        };

        SendToApi(Url.Api + 'MobileInfo', Info, function () {
            console.log("OK");
        }, function () {
            console.log("Error");
        });


    };

    var ShowBrowserInfo = function () {
        var Navigator = window.navigator;

        console.log("Name of browser:" + Navigator.appName);
        console.log("Version of browser:" + Navigator.appVersion);
        console.log("Language:" + Navigator.language);
        console.log("Platform browser:" + Navigator.platform);
        console.log("User Agent:" + Navigator.userAgent);

        //Send to web API
        var Info = {
            BrowserName: Navigator.appName,
            BrowserVersion: Navigator.appVersion,
            Language: Navigator.language,
            Platform: Navigator.platform,
            UserAgent: Navigator.userAgent,
            IdUser: Configuration.IdUser,
            IdSite: Configuration.IdSite
        };

        SendToApi(Url.Api + 'BrowserInfo', Info, function () {
            console.log("OK");
        }, function () {
            console.log("Error");
        });
    }

    //public methods
    return {
        loadConfiguration: function (mobileUrl, ApiUrl, Config) {
            Configuration = Config;
            console.log(">> Config deveMetrics.js");
            console.log(Configuration);
            console.log("Loading [device] configuration");

            Url.mobileDetect = mobileUrl;
            Url.Api = ApiUrl;

            $.getScript(Url.mobileDetect).
                done(function (script, textStatus) {
                    console.log("Done mobile-detect.js");
                    console.log("User agent:" + window.navigator.userAgent);
                    MobileInfo = new MobileDetect(window.navigator.userAgent);
                    ShowMobileInfo();
                    ShowBrowserInfo();
                }).
                fail(function (jqxhr, settings, exception) {
                    //Send to azure table
                });
        },
        init: function () {
            //binding to DOM
            console.log("Init device metrics");
        }
    }
})();