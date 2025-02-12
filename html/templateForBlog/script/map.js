(function (ns) {
    /*globals google, map, $*/
    "use strict";
    var map,
        gpsLatLng,
        marker;

    function initMap() {
        loadJSONConfig(function (config) {
            try {
                map = new google.maps.Map(document.getElementById('map-canvas'),
                    {
                        center: { lat: 50.4038916, lng: 7.2542361 },
                        scrollwheel: true,
                        zoom: 20
                    });

                var kmlLayer = new google.maps.KmlLayer({
                    url: config.KmlFileName,
                    map: map
                });

                ns.map = map;
            } catch (e) {
                console.log(e);
                setTimeout(function () {
                    if (typeof google !== 'object') {
                        location.reload();
                    }
                }, 1000);
            }
        });
    }

    function loadJSONConfig(callback) {
        $.getJSON("config.json",
            function (data) {
                callback(data);
            }).fail(function (xhr, status, error) {
                alert("An AJAX error occured: " + xhr.statusCode().status + "\nError: " + error);
            });
    }

    ns.initMap = initMap;

})(window.milosev);