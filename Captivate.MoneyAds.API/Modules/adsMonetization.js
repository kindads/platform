var moneyAdsMetrics = (function () {

    var loadDeviceMetricsScript = function () {
        console.log("loading [device metrics] javascript file");

        $.getScript(Url.deviceMetric).
            done(function (script, textStatus) {
                console.log("device metric javascript file load succefully");
                Metrics.device = deviceMetrics;
                Metrics.device.loadConfiguration(Url.mobileDetect, Api.Url, Config);
                Metrics.device.init();
            }).
            fail(function (jqxhr, settings, exception) {
                //Send to azure table
            });
    };

    var loadGeoLocationMetricsScript = function () {
        console.log("loading [geo location metrics] javascriot file");

        $.getScript(Url.geoLocationMetric).
            done(function (script, textStatus) {
                console.log("geo location metric javascript file load succefully");
                Metrics.geoLocation = geoLocationMetrics;
                Metrics.geoLocation.loadConfiguration(Api.Url, Config);
                Metrics.geoLocation.init();
            }).
            fail(function (jqxhr, settings, exception) {
                //Send to azure table
            });
    };

    var loadTimeMetricsScript = function () {
        console.log("loading [time metrics] javascript file");

        $.getScript(Url.timeMetric).
            done(function (script, textStatus) {
                console.log("time metric javascript file load succefully");
                Metrics.time = timeMetrics;
                Metrics.time.loadConfiguration(Config);
                Metrics.time.init();
            }).
            fail(function (jqxhr, settings, exception) {
                //Send to azure table
            });
    };

    var loadInjectedAds = function () {
        console.log("loading [injected ads] javascript file");

        $.getScript(Url.injectedAds).
            done(function (script, textStatus) {
                console.log("injected ads javascript file load succefully");
                Metrics.injected = injectedAds;
                Metrics.injected.loadConfiguration(Config, configuration.ApiUrl);
                Metrics.injected.init();
            }).
            fail(function (jqxhr, settings, exception) {
                //Send to azure table
            });
    };

    // Is a must set value in script
    var Api = {
        RequestUrl: '',
        Url:''
    };

    // Is a must set valie in script
    var Url = {
        deviceMetric: '',
        geoLocationMetric: '',
        timeMetric: '',
        injectedAds: '',

        mobileDetect:''
    };

    // Is a must set value in script 
    var Config = {
        IdUser: '',
        IdSite:''
    };

    var Metrics = {
        device:{},
        geoLocation:{},
        time:{},
        injected:{}
    }

    //Public methods
    return {

        closeAds: function () {
            console.log(Metrics.injected);
            Metrics.injected.closeAds();
        },
        loadConfiguration: function (configuration) {

            // set values
            Api.RequestUrl = configuration.RequestUrl;
            Api.Url = configuration.ApiUrl;

            Url.deviceMetric = configuration.deviceMetric;
            Url.geoLocationMetric = configuration.geoLocationMetric;
            Url.timeMetric = configuration.timeMetric;
            Url.injectedAds = configuration.injectedAds;
            Url.mobileDetect = configuration.mobileDetect;

            console.log(">>> Configuration Url:" + configuration.ApiUrl);

            Config.IdUser = configuration.IdUser;
            Config.IdSite = configuration.IdSite;

            Api.RequestUrl = Api.RequestUrl + Config.IdUser + '&IdSite=' + configuration.IdSite;

            console.log("Request url:");
            console.log(Api.RequestUrl);

            $.ajax({
                type: 'GET',
                url: Api.RequestUrl,
                success: function (result) {

                    console.log(result);

                    if (result.EnableDeviceMetrics === 1) {
                        loadDeviceMetricsScript(Config);
                    }
                    if (result.EnableGeoLocationMetrics === 1) {
                        loadGeoLocationMetricsScript(Config);
                    }
                    if (result.EnableTimeMetricsScript === 1) {
                        loadTimeMetricsScript(Config);
                    }
                    if (result.EnableInjectedAds === 1) {
                        loadInjectedAds(Config);
                    }
                },
                error: function (ex) {
                    //Send to azure table
                }
            });

        }       
       
    }
})();