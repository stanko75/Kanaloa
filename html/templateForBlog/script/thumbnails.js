(function (ns) {
    /*globals google, $*/
    "use strict";

    function resizeMarkerIcon(size, fileName) {
        ns.markers.forEach(function (marker) {
            if (marker.title.replace(/^.*[\\\/]/, '') == fileName.replace(/^.*[\\\/]/, '')) {
                marker.setIcon({
                    url: fileName,
                    scaledSize: new google.maps.Size(size, size)
                });
            } else {
                marker.setIcon(null);
            }
        });
    }

    $.getJSON("/*picsJson*/Thumbs.json", function (data) {
        var thumbs,
            clicked,
            picsLatLng;
        clicked = false;
        thumbs = $('#thumbnails');
        data.forEach(function (val, key) {
            thumbs.append('<img id="' + key + '"' + ' src=' + val.FileName + ">");
            //thumbs.on('mouseover', '#' + key, function () {
            //    if (!clicked) {
            //        $('#' + key).css("border", "14px solid #333");
            //        ns.map.setZoom(20);
            //        picsLatLng = new google.maps.LatLng(val.Latitude, val.Longitude);
            //        ns.map.setCenter(picsLatLng, 13);
            //        resizeMarkerIcon(70, val.FileName);
            //    }
            //});
            //thumbs.on('mouseout', '#' + key, function () {
            //    if (!clicked) {
            //        $('#' + key).css("border", "");
            //        ns.map.setZoom(/*zoom*/);
            //        resizeMarkerIcon(30, "");
            //    }
            //});
            thumbs.on('click', '#' + key, function () {
                //$('table[style*="width: 555px"]')
                if (clicked && $('#' + key).css('borderWidth').toLowerCase() === '14px') {
                    clicked = false;
                    $('#' + key).css("border", "");
                    ns.map.setZoom(15);
                    resizeMarkerIcon(30, "");
                }
                else {
                    clicked = true;
                }

                if (clicked) {
                    $('img[style*="border: 14px"]').css("border", "");
                    $('#' + key).css("border", "14px solid #333");
                    ns.map.setZoom(20);
                    picsLatLng = new google.maps.LatLng(val.Latitude, val.Longitude);
                    ns.map.setCenter(picsLatLng, 13);
                    resizeMarkerIcon(70, val.FileName);
                }
            });
        });
        ns.map.setZoom(/*zoom*/);
    });
})(window.milosev);