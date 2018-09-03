var injectedAds = (function () {

    var Configuration = {
        IdUser: '',
        IdSite: ''
    };

    var InyectConfig = {
        defaultConfig: {
            Name: '',
            Text: '',
            Image: '',
            JsId: '' 
        },
        stickyConfig: {
            Name: '',
            Text: '',
            JsId: '',
            CloseHtml: '',
            AdsHtml:''
        }
    }

    var Urls = {
        Api:''
    }

    var SetDefaultAds = function () {
        console.log(">> Set Default Ads");
        var data = InyectConfig.defaultConfig;

        console.log(">> Text to insert:" + data.Text);
        console.log(" >> JSId to find:" + data.JsId);

        // get element
        var defaultElement = document.getElementById(data.JsId);

         // insert text data in JsId div
        var defaulElement = defaultElement.innerHTML = data.Text;

        defaultElement.onclick = function () {

            // construct data to send
            var Info = {
                IdUser: Configuration.IdUser,
                IdSite: Configuration.IdSite
            };

            var Url = Urls.Api + 'DefaultInfo';
            console.log(Url);

            SendToApi(Url, Info, function () {
                console.log("OK");
            }, function () {
                console.log("Error");
            });
        }  

    };

    var SetStickyAds = function () {
        console.log("Set Sticky Ads");
        var data = InyectConfig.stickyConfig;

        document.body.innerHTML += InyectConfig.stickyConfig.AdsHtml;
        document.body.innerHTML += InyectConfig.stickyConfig.CloseHtml;
        document.getElementById(InyectConfig.stickyConfig.JsId).innerHTML = data.Text;
    };

    var GetInyectConfig = function () {

        var Url = Urls.Api + 'Configuration/GetInyectConfig?IdUser=' + Configuration.IdUser + '&IdSite=' + Configuration.IdSite;

        console.log("Url Inyected config");
        console.log(Url);

        $.ajax({
            type: 'GET',
            url: Url,
            success: function (response) {
                InyectConfig = response;

                if (InyectConfig.defaultConfig.Name !== '') {
                    SetDefaultAds();
                }
                if (InyectConfig.stickyConfig.Name !== '') {
                    SetStickyAds();
                }
            },
            error: function (ex) {
                //Send to azure table
            }
        });
    };

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

    //public methods
    return {
        closeAds: function () {

            // remove element
            var element = document.getElementById(InyectConfig.stickyConfig.JsId);
            element.parentNode.removeChild(element);

            // remove element
            element = document.getElementById(InyectConfig.stickyConfig.JsId+"Close");
            element.parentNode.removeChild(element);

            // construct data to send
            var Info = {
                IdUser: Configuration.IdUser,
                IdSite: Configuration.IdSite
            };

            
            SendToApi(Urls.Api + 'Sticky/PostClicInfo', Info, function () {
                console.log("OK");
            }, function () {
                console.log("Error");
            });

        },
        loadConfiguration: function (Config,Url) {
            Configuration = Config;
            Urls.Api = Url;
            console.log(">> Config geoLocationMetrics.js");
            console.log(">> IdUser:" + Configuration.IdUser);
            console.log(">> IdSite:" + Configuration.IdSite);
            console.log("Loading [injected ads] configuration");

            GetInyectConfig();
        },
        init: function () {
            //binding to DOM
            console.log("Init injected metrics");
        }
    }
})();