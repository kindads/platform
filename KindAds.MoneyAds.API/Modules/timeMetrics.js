var timeMetrics = (function () {

    var Configuration = {
        IdUser: '',
        IdSite: ''
    };

    //public methods
    return {
        loadConfiguration: function (Config) {
            Configuration = Config;
            console.log(">> Config geoLocationMetrics.js");
            console.log(Configuration);
            console.log("Loading [time] configuration");
        },
        init: function () {
            //binding to DOM
            console.log("Init time metrics");
        }
    }
})();