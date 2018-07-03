var geoLocationMetrics = (function () {

    var Configuration = {
        IdUser: '',
        IdSite: ''
    };

    var Url = {
        Api: '',
        geocodificaciónInversa:'https://maps.googleapis.com/maps/api/geocode/json?latlng='
    };

    var Info = {
        latitude: '',
        longitude: '',
        Address: '',
        IdUser: '',
        IdSite:''
    };

    var GoogleGeocodificacion = {
        Key: 'AIzaSyCz0XcnDBV6w8uNHC0yzfA-yPJR4wrAruk',
        UrlRequest: ''
    };

    var MakeGoogleGeocodificacionUrlRequest = function () {
        GoogleGeocodificacion.UrlRequest = Url.geocodificaciónInversa + Info.latitude + ',' + Info.longitude + '&key=' + GoogleGeocodificacion.Key;
        GetInfo(GoogleGeocodificacion.UrlRequest);
    };

    var GetGeoLocationData = function (response) {
        console.log(response.results[0].formatted_address);
        Info.Address = response.results[0].formatted_address;

        //Send to azure
        SendToApi(Url.Api + 'GeoInfo', Info, function () {
            console.log("OK");
        }, function () {
            console.log("Error");
        });
    };

    var GetInfo = function (url) {
        console.log(">>> Get google location info" + url);

        Info.IdSite = Configuration.IdSite;
        Info.IdUser = Configuration.IdUser;

        $.ajax({
            type: 'GET',
            url: url,
            success: function (response) {
                GetGeoLocationData(response);
            },
            error: function (xhr, error, status) {
                exception();
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

    var onSuccessGeolocating = function (position) {

        console.log(">>> onSuccessGeolocating");

        //Valores
        console.log("Latitude");
        console.log(position.coords.latitude);

        console.log("longitude");
        console.log(position.coords.longitude);

        //Set data
        Info.latitude = position.coords.latitude;
        Info.longitude = position.coords.longitude;

        MakeGoogleGeocodificacionUrlRequest();
    };

    var onErrorGeolocating = function (error) {

        console.log(">>> Error");

        //Send to azure
        switch (error.code) {
            case error.PERMISSION_DENIED:
                console.log('ERROR: User denied access to track physical position!');
                break;

            case error.POSITION_UNAVAILABLE:
                console.log("ERROR: There is a problem getting the position of the device!");
                break;

            case error.TIMEOUT:
                console.log("ERROR: The application timed out trying to get the position of the device!");
                break;

            default:
                console.log("ERROR: Unknown problem!");
                break;
        }
    };

    var GetGeoLocationInfo = function () {
        if (navigator.geolocation) {
           //Todo
            navigator.geolocation.getCurrentPosition(onSuccessGeolocating,
                onErrorGeolocating,
                {
                    enableHighAccuracy: true,
                    maximumAge: 5000,
                    timeout: 10000
                });
        }
        else {
            console.log("Geolocation not available");
        }
    };


    //public methods
    return {
        loadConfiguration: function (ApiUrl, Config) {
            Configuration = Config;
            console.log(">> Config geoLocationMetrics.js");
            console.log(Configuration);

            Url.Api = ApiUrl;
            console.log("Loading [geo location] configuration");
            GetGeoLocationInfo();
        },
        init: function () {
            //binding to DOM
            console.log("Init geo location metrics");
        }
    }
})();