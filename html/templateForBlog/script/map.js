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
        }
        catch (e) {
            console.log(e);
            setTimeout(function () {
                if (typeof google !== 'object') {
                    location.reload();
                }
            }, 1000);
        }
        ns.map = map;
    }

    ns.initMap = initMap;

})(window.milosev);