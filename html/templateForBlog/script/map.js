(function (ns) {
    /*globals google, map*/
    "use strict";
    var map;

    function initMap() {
        try {
            map = new google.maps.Map(document.getElementById('map-canvas'), {
                center: { lat: 34.397, lng: 150.644 },
                scrollwheel: true,
                zoom: 2
            });

            map.data.loadGeoJson("/*geospatialDataFileName*/.geojson", null, function (features) {
                if (!features.length) {
                    console.warn("No GeoJSON features loaded.");
                    return;
                }

                const bounds = new google.maps.LatLngBounds();

                features.forEach(feature => {
                    feature.getGeometry()?.forEachLatLng(latLng => {
                        bounds.extend(latLng);
                    });
                });

                if (!bounds.isEmpty()) {
                    map.fitBounds(bounds);

                    // Prevent excessive zoom for a single point
                    if (features.length === 1) {
                        google.maps.event.addListenerOnce(map, "idle", function () {
                            if (map.getZoom() > 15) {
                                map.setZoom(15);
                            }
                        });
                    }
                }
            });

            ns.map = map;
        }
        catch (e) {
            console.error("Google Maps initialization failed:", e);

            setTimeout(function () {
                if (typeof google !== 'object') {
                    location.reload();
                }
            }, 1000);
        }
    }

    ns.initMap = initMap;

})(window.milosev);