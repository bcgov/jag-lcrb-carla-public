
var searchMap = null;

var LDB_Licence_String = "Public Store";
  // Options for creating the searchMap. This map instance has the ability to drive search queries via the
  // startIdentifyCrsOperation, which allows a user to draw a rectangle, and the endIdentifyCrsOperation,
  // which bundles opposite corners of this rectangle into the form before submitting it for the server
  // to process. The map also displays any Crs successfully returned from a form search, whether or not
  // the search was driven by drawing a rectangle or by a user manually submitting the form.
  // Crs can be clicked to display a subset of their tabulated information in a popup directly on the map.
  var searchMapOptions = {
    // The ID of the addMap div.
    mapNodeId: 'search-map',
    // Minimum zoom level of the map (i.e., how far it can be zoomed out)
    minZoom: 4,
    // Maximum zoom of the map (i.e., how far it can be zoomed in)
    maxZoom: 17,
    // Bounding lats and longs of the map, corresponding to the lat/long extremes of BC.
    mapBounds: {
      north: 60.0223,
      south: 48.204556,
      west: -139.073671,
      east: -114.033822,
      padding: 0.05 // Margin beyond extremes to pad the bounds with, as a ratio of the total bounding box.
    },
    // ESRI layers associated with the map
    esriLayers: [
      {
        url: 'api/MapServer'
      }
    ],
    // WMS layers associated with the map
    wmsLayers: [
      {
        rootUrl: 'https://openmaps.gov.bc.ca/geo/pub/WHSE_CADASTRE.PMBC_PARCEL_FABRIC_POLY_SVW/ows?',
        format: 'image/png',
        layers: 'pub:WHSE_CADASTRE.PMBC_PARCEL_FABRIC_POLY_SVW',
        styles: 'PMBC_Parcel_Fabric_Cadastre_Outlined',
        transparent: true
      }
    ],
    identifyCrsStartCallback: identifyCrsStartCallback,
    identifyCrsEndCallback: identifyCrsEndCallback
  };

  // Hard-coded names for the hidden lat-long fields that delineate a
  // rectangle drawn by the user as part of an identifyCrs operation.
  var startLatLongFieldName = "start_lat_long";
  var endLatLongFieldName = "end_lat_long";
  var latLongDelimiter = ",";

  function startIdentifyCrsOperation() {
    if (searchMap !== null) {
      searchMap.startIdentifyCrs();
    }
  }

  // We want to alter the map to have crosshairs during identifyCrs,
  // as well as disabling the button until the operation is over.
  function identifyCrsStartCallback() {
    $("#search-map").addClass('identify-Crs-operation');
    $("#identify-Crs-btn").prop("disabled", true);
  }

  // Remove the crosshairs from the map, re-enable the button, gather
  // the corners of the rectangle, and submit the form.
  function identifyCrsEndCallback(startLatLong, endLatLong) {
    $("#search-map").removeClass('identify-Crs-operation');
    $("#identify-Crs-btn").prop("disabled", false);
    if (startLatLong && endLatLong) {
      var startLatLongString = startLatLong.lat + latLongDelimiter + startLatLong.lng;
      var endLatLongString = endLatLong.lat + latLongDelimiter + endLatLong.lng;
      $("[name=" + startLatLongFieldName + "]").val(startLatLongString);
      $("[name=" + endLatLongFieldName + "]").val(endLatLongString);
    }
    $("#id-searchForm").submit();
  }




/**
 * The EstablishmentsMap class provides a Leaflet map with different functionality, depending upon the context in which it is deployed.
 * It currently depends only on Leaflet, esri-leaflet, and JQuery, eschewing any other plugins libraries in the interest of maintainability.
 *
 * A NOTE ON FUNCTIONALITY: This class can be initialised in different ways, which exposes its functionality differently.
 * If sufficient parameters are supplied in map construction, the map can perform the following tasks:
 *  - Draw a single pushpin (i.e., a Leaflet marker) which causes the map to emit AJAX requests to show all Establishments
 *      in the bounding box (except the establishment that is being represented by the pushpin). The map will reissue queries for Establishments
 *      in the bounding box whenever the map is panned or zoomed, provided the pushpin is present.
 *      The pushpin can be fed into the map's initialisation options via establishmentPushpinInit or added/moved
 *      programmatically through the public method placeestablishmentPushpin(). It may be removed via removeestablishmentPushpin().
 *      If the establishmentPushpinMoveCallback is supplied on map init, the pushpin can be moved by dragging, which advertises the
 *      pushpin's latitude and longitude to the callback. The map will centre on the pushpin and reissue queries for surrounding
 *      Establishments whenever the pushpin is moved.
 *  - Allow the user to draw a rectangle via the public startIdentifyEstablishments() method. If the map init supplies an identifyEstablishmentsEndCallback,
 *      the map advertises a pair of latitude/longitude coordinates corresponding to extreme corners of the rectangle as it was when the user
 *      released the mouse button.
 *  - Display an ESRI MapServer layer as a base layer.
 *  - Display an array of WMS tile layers as overlays.
 *  - Draw a static rectangle around Establishments supplied via the drawAndFitBounds() method.
 * The map is able to pan and zoom by default, but this behaviour can be disabled by passing appropriate booleans. Note that if zooming is allowed,
 * the map will always zoom into and out of the centre of the map, regardless if the zoom event arises from zoom buttons or the mouse wheel. Also,
 * the constructor allows the map to set its zoom levels, as establishment as the initial centre or a bounding box to fit (precisely one of these is required 
 * for a given instance).
 * @param options An object conforming to the following scheme (using TS notation):
 * {
 *   mapNodeId: string, // The DOM ID of the div into which the Leaflet map will be placed
 *   esriLayers: [ // ESRI layers associated with the map
 *       {
 *          url: string // A URL to an Esri MapServer map service.
 *       }
 *   ],
 *   wmsLayers: [ // WMS layers associated with the map
 *       {
 *          rootUrl: string, // URL to the OWS service of the WMS layer; e.g., 'https://openmaps.gov.bc.ca/geo/pub/WHSE_CADASTRE.PMBC_PARCEL_FABRIC_POLY_SVW/ows?'
 *          format: string, // Format of the tiles; e.g., 'image/png'
 *          layers: string, // Layers of the OWS service; e.g., 'pub:WHSE_CADASTRE.PMBC_PARCEL_FABRIC_POLY_SVW'.
 *          styles: string, // Styles of the OWS service; e.g., 'PMBC_Parcel_Fabric_Cadastre_Outlined'
 *          transparent: boolean // Whether the tiles are transparent (other than the features drawn upon them)
 *      }
 *   ],
 *   initCentre?: [float], // A two-element array representing the latitude and longitude of the centre of the map. If omitted, the map is fit to mapBounds, so one of these must exist.
 *   canZoom?: bool, // Whether the map can be zoomed. Defaults to true.
 *   canPan?: bool, // Whether the map can be panned after initial load. Defaults to true.
 *   minZoom?: number,  // The minimum zoom level of the map (i.e., how far it can be zoomed out)
 *   maxZoom?: number,  // The maximum zoom level of the map (i.e., how far it can be zoomed in)
 *   mapBounds?: { // Latitude and longitude extremes of the bounding rectangle for the map.
 *      north: float, // The top latitude of the map
 *      south: float, // The bottom latitude of the map
 *      west: float, // The leftmost longitude of the map
 *      east: float, // The rightmost longitude of the map
 *      padding: int // Margin beyond extremes to pad the bounds with, as a percentage of the total bounding box.
 *   },
 *   establishmentPushpinInit?: { // An object for setting the latitude, longitude, and details of a establishmentPushpin on init.
 *      lat: float, // The initial latitude of the pushpin
 *      long: float, // The initial longitude of the pushpin
 *      establishmentDetails: {
 *          guid: string // The GUID of the establishment, for identification and special handling
 *      }
 *   },
 *   establishmentPushpinMoveCallback?: function, // Function to call when the map's establishmentPushpin moves
 *   identifyEstablishmentsStartCallback?: function, // Function to call when an identifyEstablishments operation is started
 *   identifyEstablishmentsEndCallback?: function // Function to call when an identifyEstablishments operation ends
 * }
 */

function EstablishmentsMap(options) {
    'use strict';

    /** Class constants */

    // The URL used to search for Establishments.
    var _SEARCH_URL = 'api/establishments/map';

    // The zoom level beyond which the map issues AJAX queries for Establishments, and beneath which removes AJAX-queried Establishments.
    var _SEARCH_MIN_ZOOM_LEVEL = 14;

    // Leaflet style for the _establishmentMarkers
    var _establishment_OPEN_MARKER_STYLE = {
          radius: 3, // The radius of the circleMarker
          color: '#355A20', // The color of the circleMarker; green for Cannabis.
          fillOpacity: 1.0 // How transparent the circleMarker's fill is
    };

    var _establishment_NOT_OPEN_MARKER_STYLE = {
        radius: 3, // The radius of the circleMarker
        color: '#999999', // The color of the circleMarker; grey for not open.
        fillOpacity: 1.0 // How transparent the circleMarker's fill is
    };

    /** Private members dynamically set */

    // The underlying Leaflet map.
    var _leafletMap = null;

    // The map's maximum bounds. This should be a Leaflet LatLngBounds object.
    var _maxBounds = null;

    // An object containing a pushpin marker and a data schematic for a particular establishment. This indicates a single establishment on the screen that may be editable.
    // The object conforms to:
    // {
    //     pushpinMarker: L.marker, // The Leaflet marker that points to the establishment's location
    //     establishmentDetails: {
    //         guid: string, // The establishment's globally-unique ID, to avoid drawing with other (non-interactive) Establishments
    //     },
    //     establishmentMarker: L.circleMarker // The Leaflet circleMarker that represents the establishment itself.
    // }
    var _establishmentPushpin = null;

    // The callback function for _establishmentPushpin's move event.
    var _establishmentPushpinMoveCallback = null;

    // Markers used to denote Establishments. This var should only be accessed directly, so do not write var newArr = _establishmentMarkers anywhere in the class.
    var _establishmentMarkers = [];

    // Whether the map is undergoing an identifyEstablishments operation.
    var _isIdentifyingEstablishments = false;

    // The callback function for the beginning of an identifyEstablishments operation.
    var _identifyEstablishmentsStartCallback = null;

    // the callback function for the end of an identifyEstablishments operation.
    var _identifyEstablishmentsEndCallback = null;

    // The rectangle to draw on the map during an identifyEstablishments operation.
    var _identifyEstablishmentsRectangle = null;

    // The rectangle to draw when displaying Establishments from drawAndFitBounds()
    var _drawAndFitBoundsRectangle = null;

    // The starting corner of the identifyEstablishmentsRectangle
    var _startCorner = null;

    // The ending corner of the (final) identifyEstablishmentsRectangle
    var _endCorner = null;

    /** Private functions */

    // Convenience method for checking whether a property exists (i.e., is neither null nor undefined)
    var _exists = function (prop) {
        return prop !== null && prop !== void 0;
    };

    // Convenience method for checking whether an object is an array.
    var _isArray = function (arr) {
        return _exists(arr.constructor) && arr.constructor === Array;
    };

    var _setMaxBounds = function (bounds) {
        var maxBounds = null;
        if (_exists(bounds) && _exists(bounds.north) && _exists(bounds.south) && _exists(bounds.west) && _exists(bounds.east)) {
            maxBounds = L.latLngBounds([L.latLng(bounds.north, bounds.west), L.latLng(bounds.south, bounds.east)]);
            if (bounds.padding) {
                maxBounds.pad(bounds.padding);
            }
        }
        return maxBounds;
    };

    // Loads ESRI MapServer services.
    var _loadEsriLayers = function (esriLayers) {
        if (_exists(_leafletMap)) {
            esriLayers.forEach(function (esriLayer){
                if (esriLayer && esriLayer.url) {
                    L.esri.tiledMapLayer({
                    url: esriLayer.url
                    }).addTo(_leafletMap);                               
                }
            });
        }
    };

    // Loads WMS layers.
    var _loadWmsLayers = function (wmsLayers) {
        if (_exists(_leafletMap)) {
            wmsLayers.forEach(function (wmsLayer) {
                if (wmsLayer && wmsLayer.rootUrl) {
                    L.tileLayer.wms(wmsLayer.rootUrl, {
                        format: wmsLayer.format || 'image/png',
                        layers: wmsLayer.layers || '',
                        styles: wmsLayer.styles || '',
                        transparent: wmsLayer.transparent || true
                    }).addTo(_leafletMap);
                }
            });
        }
    };

    // Passes the establishmentPushpin's updated lat/long coordinates to the provided callback function, if it exists.
    var _establishmentPushpinMoveEvent = function (moveEvent) {
        if (_exists(_establishmentPushpinMoveCallback)) {
            var latLng = moveEvent.latlng;
            _establishmentPushpinMoveCallback(latLng);
        }
    };

    // Handles the mousemove event during the identifyEstablishments operation. Specifically, this function draws the interstitial
    // rectangles to help the user see the extent they're querying for Establishments.
    var _mouseMoveForIdentifyEstablishmentsEvent = function (e) {
        if (!_exists(_leafletMap) || !_exists(_startCorner)) {
            return;
        }
        var tempCorner = e.latlng;
        if (_exists(_identifyEstablishmentsRectangle)) {
            _leafletMap.removeLayer(_identifyEstablishmentsRectangle);
        }
        _identifyEstablishmentsRectangle = L.rectangle([_startCorner, tempCorner]);
        _identifyEstablishmentsRectangle.addTo(_leafletMap);
    };

    // Handles the mousedown event during the identifyEstablishments operation. Specifically, and sets the starting corner of
    // the rectangle to be drawn, as establishment as subscribing the map to _mouseMoveForIdentifyEstablishmentsEvent.
    var _mouseDownForIdentifyEstablishmentsEvent = function (e) {
        _leafletMap.dragging.disable();
        _startCorner = e.latlng;
        _leafletMap.on('mousemove', _mouseMoveForIdentifyEstablishmentsEvent);
    };

    // Handles the mouseup event during the identifyEstablishments operation. Specifically, this function re-enables dragging, sets
    // the ending corner of the rectangle, unsubscribes the map from the events, passes the corner info to the callback,
    // and resets the private members associated with the operation.
    var _mouseUpForIdentifyEstablishmentsEvent = function (e) {
        _leafletMap.dragging.enable();
        _endCorner = e.latlng;

        _leafletMap.off('mousedown', _mouseDownForIdentifyEstablishmentsEvent);
        _leafletMap.off('mouseup', _mouseUpForIdentifyEstablishmentsEvent);
        _leafletMap.off('mouseout', _mouseUpForIdentifyEstablishmentsEvent);
        _leafletMap.off('mousemove', _mouseMoveForIdentifyEstablishmentsEvent);
        if (_exists(_identifyEstablishmentsRectangle)) {
            _leafletMap.removeLayer(_identifyEstablishmentsRectangle);
            _identifyEstablishmentsRectangle = null;
        }       
        _isIdentifyingEstablishments = false;
        if (_exists(_identifyEstablishmentsEndCallback)) {
            _identifyEstablishmentsEndCallback(_startCorner, _endCorner);
        }
        _startCorner = null;
        _endCorner = null;
    };

    // Determines whether a given latitude is within the map's bounds.
    var _isLatInBounds = function (lat) {
        if (_exists(_maxBounds)) {
            return _maxBounds.getSouth() <= lat && lat <= _maxBounds.getNorth();
        }
        // If _maxBounds doesn't exist, the latitude is valid.
        return true;
    };

    // Determines whether a given longitude is within the map's bounds.
    var _isLongInBounds = function (long) {
        if (_exists(_maxBounds)) {
            return _maxBounds.getWest() <= long && long <= _maxBounds.getEast();
        }
        // If _maxBounds doesn't exist, the longitude is valid.
        return true;
    };

    // Makes sure the latitude and longitude fit within the map's bounding box, if one exists.
    // If the lat and long are within the map's bounds, they are returned; if they can be corrected by flipping the sign,
    // the negated values are returned. Else { NaN, NaN } is returned along with a console error.
    // Takes a latLong parameter corresponding to { lat: number, long: number }
    var _ensureLatLongIsInBounds = function (latLong) {
        var lat = _exists(latLong.lat) ? latLong.lat : NaN;
        var long = _exists(latLong.long) ? latLong.long : NaN;
        // if (long > 0) {
        //     // Even if there's not a bounding box, we'll flip positive longitudes.
        //     long = -long;
        // }
        if (!_isLatInBounds(lat)){
            lat = NaN;
        }
        if (!_isLongInBounds(long)) {
            long = NaN;
        }
        if (isNaN(lat) || isNaN(long)) {
            console.log("Invalid latitude or longitude. (Lat,Long): ("+latLong.lat+","+latLong.long+")");
            return {lat: NaN, long: NaN};
        }
        return {lat: lat, long: long};
    };

    // Takes latitude and longitude and returns a Leaflet latLng object only if the lat/long are valid within the map's bounding box.
    var _getLatLngInBC = function (rawLat, rawLong) {
        var lat = parseFloat(rawLat);
        var long = parseFloat(rawLong);
        if (_exists(lat) && !isNaN(lat) && _exists(long) && !isNaN(long)) {
            var processedLatLong = _ensureLatLongIsInBounds({lat: lat, long: long});
            if (!isNaN(processedLatLong.lat) && !isNaN(processedLatLong.long)) {
                return L.latLng([processedLatLong.lat, processedLatLong.long]);
            }
        }
        return null;
    };

    // Submits an XHR to return all Establishments within the given latLngBounds.
    var _searchByAjax = function (url, latLngBounds, success) {
        var northWestLatLng = latLngBounds.getNorthWest();
        var southEastLatLng = latLngBounds.getSouthEast();
        var startLatLong = northWestLatLng.lat + "," + northWestLatLng.lng;
        var endLatLong = southEastLatLng.lat + "," + southEastLatLng.lng;
        $.ajax({
            url: url,
            data: {
                'start_lat_long': startLatLong,
                'end_lat_long': endLatLong
            },
            dataType: 'json',
            success: success,
            error: function (xhr, status, error) {
                var msg = "An error occurred while searching for Establishments. STATUS: " + status + ".  ERROR " + error + ".";
                console.log(msg);
            }
        });
    };

    // A establishmentMarker should only be added if it is not the same as the pushpinestablishment, which is handled differently.
    var _canDrawestablishment = function (pushpinestablishmentGuid, establishmentToDrawGuid) {
        if (!(_exists(pushpinestablishmentGuid) && _exists(establishmentToDrawGuid))) {
            return true;
        } else {
            return pushpinestablishmentGuid !== establishmentToDrawGuid;
        }
    };

    // Clears the _establishmentMarkers from the map and resets the array.
    var _clearEstablishments = function () {
        if (_exists(_leafletMap) && _isArray(_establishmentMarkers)) {
            _establishmentMarkers.forEach(function (establishmentMarker) {
                _leafletMap.removeLayer(establishmentMarker);
            });
        }
        // We can confidently overwrite the array because the class should never create any references to _establishmentMarkers.
        _establishmentMarkers = [];
    };

    // Parses the input to generate an internal URL to the establishment details summary page. If the input is not a number (or null),
    // an empty string is returned.
    var _generateestablishmentTagUrl = function (tagNum) {
        var num = parseInt(tagNum);
        if (!_exists(num) || isNaN(num)) {
            return '';
        }
        return '<a href="/establishment/' + num + '">' + num + '</a>';
    };

    // Generates a popup content HTML string for a establishment marker, based on the data that establishment has available.
    var _generateestablishmentMarkerPopupContents = function (establishment) {
        if (!_exists(establishment)) {
            return;
        }
        // contentObj is a dictionary whose keys correspond to the display names of
        // establishment data attributes and whose values correspond to the specific establishment's data.
        // This dictionary's values will in general consist of a (potentially processed)
        // subset of the JSON returned by the Python establishment search service.
      var contentObj = {
        // <img src=assets/placeholder_credential.png height=50 width=50>
        'License': 'License:' + (establishment.license || ''),
//          'Type': '<strong>Non-Medical Cannabis Retail Store</strong>',
        'Name': establishment.name || '',
        'Phone': establishment.phone || '',
        'Street Address': establishment.addressStreet || '',
        'City': establishment.addressCity || '',
        'Postal': establishment.addressPostal || '',
        'Status': establishment.isOpen ? 'Open' : 'Coming Soon'
        };

        // We build the contentString from the contentObj dictionary, using paragraphs as property delimiters.
        var contentString = "<table><tr><td>";
        contentString +="<b><u>" + (establishment.name || '') + "</u> - <font color=\"";

        if (establishment.isOpen) {
            contentString += "#355A20\">OPEN";
        }
        else {  // gray for not open.
            contentString += "#999999\">COMING SOON";
        }
        // <td rowspan=5 align=center valign=middle><img src=assets/BUY_LEGAL_DECAL.png alt=\"Licenced Retail Store\" style=\"width: 112px; height: 100px; max-width:112px; max-height:100px \" height=112 width=100></td>
        contentString += "</font></b>";

              

        contentString +="</td></tr>";
        contentString += "<tr><td>" + (establishment.addressStreet || '') + "</td>";
        if (establishment.license === LDB_Licence_String) {
            contentString += "<td rowspan=3 valign=center><img src = 'assets/LDB_32x32.png' width='32' height='32' alt='BC Cannabis Store'></td>";
        }

        contentString += "</tr>";
        contentString += "<tr><td>" + (establishment.addressCity || '') + ", " + establishment.addressPostal + "</td></tr>";
        contentString += "<tr><td>" + (establishment.phone || '&nbsp;') + "</td></tr>";
        if (establishment.license !== LDB_Licence_String) {
            contentString += "<tr><td>Licence No: " + (establishment.license || '') + "</td></tr>";
        }
        
        contentString += "</table >";
        return contentString;
    };

    // Draws Establishments that can be drawn. Currently a establishment cannot be drawn if it is associated with the establishmentPushpin.
    var _drawEstablishments = function (Establishments) {
        // First we clear any extant markers
        _clearEstablishments();

      var ldbIcon = L.icon({
          iconUrl: 'assets/LDB_32x32.png',


        iconSize: [16, 16], // size of the icon
        iconAnchor: [16, 16], // point of the icon which will correspond to marker's location        
        popupAnchor: [-3, -76] // point from which the popup should open relative to the iconAnchor
      });

        var establishmentPushpinGuid = null;
        // Now we draw the Establishments, checking to prevent a marker from being drawn where a pushpin will be.
        if (_exists(_establishmentPushpin) && _exists(_establishmentPushpin.establishmentDetails) && _exists(_establishmentPushpin.establishmentDetails.guid)) {
            establishmentPushpinGuid = _establishmentPushpin.establishmentDetails.guid;
        }
        Establishments.forEach(function (establishment) {
            var latLong = _getLatLngInBC(establishment.latitude, establishment.longitude);
            var establishmentGuid = establishment.id;
            if (_exists(latLong) && _canDrawestablishment(establishmentPushpinGuid, establishmentGuid))
            {
              // icon marker

                
              var markerStyle = establishment.isOpen ? _establishment_OPEN_MARKER_STYLE : _establishment_NOT_OPEN_MARKER_STYLE;
              var style = $.extend({}, markerStyle, { interactive: !_establishmentPushpin });
              // Markers should only be clickable when there is no establishmentPushpin available.           

                var establishmentMarker = null;

                
                if (establishment.license === LDB_Licence_String) {
                    establishmentMarker = L.marker(latLong, { icon: ldbIcon, interactive: !_establishmentPushpin });
                }
                else {
                
                    establishmentMarker = L.circleMarker(latLong, style);
                }

                establishmentMarker.bindPopup(_generateestablishmentMarkerPopupContents(establishment));
                establishmentMarker.addTo(_leafletMap);
                _establishmentMarkers.push(establishmentMarker);
            }
        });
    };

    // Handles the results of an AJAX call to 'api/establishments/map/'. Currently the expected behaviour is to draw the Establishments
    // that can be drawn.
    var _searchByAjaxSuccessCallback = function (results) {
        var Establishments = JSON.parse(results);
        if (_isArray(Establishments)) {
            _drawEstablishments(Establishments);
        } else {
            console.log("Could not parse Establishments data.");
        }
    };

    // Searches for all Establishments in the map's current bounding box, provided the map is beyond the minimum searching zoom level.
    var _searchEstablishmentsInBoundingBox = function () {
        if (_exists(_leafletMap) && _leafletMap.getZoom() >= _SEARCH_MIN_ZOOM_LEVEL) {
            var mapBounds = _leafletMap.getBounds();
            _searchByAjax(_SEARCH_URL, mapBounds, _searchByAjaxSuccessCallback);
        }
    };

    // Issues a query to fetch Establishments in the bounding box, meant to subscribe to
    // the map's moveend event while a establishmentPushpin is present on the map.
    var _searchBoundingBoxOnMoveEnd = function () {
        _searchEstablishmentsInBoundingBox();
    };

    // When the establishmentPushpin is moved, pan to re-centre the pushpin.
    var _establishmentPushpinMoveEndEvent = function () {
        _leafletMap.panTo(_establishmentPushpin.pushpinMarker.getLatLng());
    };

    // When the map is zoomed with a establishmentPushpin, pan to re-centre the pushpin (which
    // is needed if the map is near the bounding box), and clear the surrounding Establishments
    // if the zoom level is below the minimum search level.
    var _establishmentPushpinZoomEndEvent = function () {
        _leafletMap.panTo(_establishmentPushpin.pushpinMarker.getLatLng());
        if (_leafletMap.getZoom() < _SEARCH_MIN_ZOOM_LEVEL) {
            _clearEstablishments();
        }

    };

    /** Public methods */

    /**
     * Places a establishmentPushpin on the map to help refine the placement of a establishment.
     * When placed by a button click, the map pans and zooms to centre on the marker.
     * @param {any} latLongArray An array of [lat, long], where lat and long specify where the establishmentPushpin will be placed
     * @param {any} establishmentDetails Details for the establishment.
     */
    var placeestablishmentPushpin = function (latLongArray, establishmentDetails) {
        // If the map or the latLng do not exist, bail out.
        if (!_exists(_leafletMap) || !_exists(latLongArray) || !_isArray(latLongArray) || latLongArray.length !== 2) {
            return;
      }

      var greenIcon = L.icon({
        iconUrl: 'assets/placeholder_credential.png',
        

        iconSize: [50, 50], // size of the icon
        iconAnchor: [25, 25], // point of the icon which will correspond to marker's location        
        popupAnchor: [-3, -76] // point from which the popup should open relative to the iconAnchor
      });

        // We ensure the lat/long is in BC, in case it was passed in without checking.
        var latLong = _getLatLngInBC(latLongArray[0], latLongArray[1]);
        // If the latitude and longitude do not fit within the map's maxBounds, bail out.
        if (!_exists(latLong)) {
            return;
        }
        // The map zooms to the its maxZoom to display the pushpin.
        var zoomLevel = _leafletMap.getMaxZoom();
        // If the pushpin exists and the movement is substantive, move the pin. Else if
        // the pushpin does not exist, create it and place it at the coordinates.
        if (_exists(_establishmentPushpin) && _exists(_establishmentPushpin.pushpinMarker)) {
            if (!_establishmentPushpin.pushpinMarker.getLatLng().equals(latLong)) {
                _establishmentPushpin.pushpinMarker.setLatLng(latLong);
            }
        } else {
            _establishmentPushpin = {};
          _establishmentPushpin.pushpinMarker = L.marker(latLong, {
              icon: greenIcon,
              draggable: _exists(_establishmentPushpinMoveCallback) // The pin should only drag if the map's caller has a hook to handle movement
            }).addTo(_leafletMap);

            // The pin should subscribe to move events.
            _establishmentPushpin.pushpinMarker.on('move', _establishmentPushpinMoveEvent);
            _establishmentPushpin.pushpinMarker.on('moveend', _establishmentPushpinMoveEndEvent);
        }
        // If the establishmentDetails properties exist, assign them.
        if (_exists(establishmentDetails) && _exists(establishmentDetails.guid)) {
            _establishmentPushpin.establishmentDetails = establishmentDetails;
        }
        // If the pin exists, the map should refresh the Establishments it displays when it is moved, to provide
        // more information to aid in establishment placement without having to load too many Establishments at once.
        _leafletMap.on('moveend', _searchBoundingBoxOnMoveEnd);

        // If the pin exists, zoomend should re-centre the pin and clear Establishments if the zoom level
        // is beneath the _SEARCH_MIN_ZOOM_LEVEL.
        _leafletMap.on('zoomend', _establishmentPushpinZoomEndEvent);

        // Finally, the map should fly to the pin.
        _leafletMap.flyTo(latLong, zoomLevel);
    };

    // Removes the establishmentPushpin from the map and clears it of any extant Establishments.
    var removeestablishmentPushpin = function () {
        if (!_exists(_leafletMap)) {
            return;
        }
        if (_exists(_establishmentPushpin) && _exists(_establishmentPushpin.pushpinMarker)) {
            _leafletMap.removeLayer(_establishmentPushpin.pushpinMarker);
            // Unsubscribe from the pushpin-related events.
            _leafletMap.off('moveend', _searchBoundingBoxOnMoveEnd);
            _leafletMap.off('zoomend', _establishmentPushpinZoomEndEvent);
            _establishmentPushpin = null;
            _clearEstablishments();
        }
    };

    // Displays Establishments and zooms to the bounding box to see all displayed Establishments.
    // limit of Establishments data.
    // Note the Establishments must have valid latitude and longitude data.
    var drawAndFitBounds = function (Establishments) {
        if (!_exists(_leafletMap) || !_exists(Establishments) || !_isArray(Establishments)) {
            return;
        }
        if (_exists(_drawAndFitBoundsRectangle)) {
            _leafletMap.removeLayer(_drawAndFitBoundsRectangle);
            _drawAndFitBoundsRectangle = null;
        }
        _drawEstablishments(Establishments);

        // Once Establishments are drawn, we draw a (static) rectangle that encompasses them, with a bit of
        // a padded buffer to include Establishments on the edges of the rectangle.
        var buffer = 0.00005;
        var padding = 0.01;
        
        // With the above constants, we get the bounds of the _establishmentMarkers and pad them with the buffer and padding.
        var markerBounds = L.featureGroup(_establishmentMarkers).getBounds();
        var northWestCorner = L.latLng(markerBounds.getNorthWest().lat + buffer, markerBounds.getNorthWest().lng - buffer);
        var southEastCorner = L.latLng(markerBounds.getSouthEast().lat - buffer, markerBounds.getSouthEast().lng + buffer);
        markerBounds = L.latLngBounds([northWestCorner, southEastCorner]).pad(padding);

        // Draw the new rectangle to enclose the markers.
        _drawAndFitBoundsRectangle = L.rectangle(markerBounds, {
            fillOpacity: 0, // The fill should be transparent.
            interactive: false // Users should click through the rectangle to the markers.
        });
        //_drawAndFitBoundsRectangle.addTo(_leafletMap);

        // Now that the rectangle is drawn, fit the map to it.
        _leafletMap.fitBounds(markerBounds);
    };

    // Starts the identifyEstablishments operation. This operation comprises several events, generally initiated when a user clicks
    // an appropriate button on the Search page. The map's style is dynamically changed so that the mouse pointer turns to
    // crosshairs, and the map itself is prepared in this method to let a user draw a rectangle on it by clicking and dragging
    // over the map. Once the mouse is released, the starting and ending corners of the box are collected, added to the Search
    // form, and submitted for processing.
    var startIdentifyEstablishments = function () {
        if (_exists(_leafletMap) && _isIdentifyingEstablishments) {
            // If the map is in the midst of an Identify, don't start a new one.
            return;
        }
        _isIdentifyingEstablishments = true;
        _startCorner = null;
        _endCorner = null;
        if (_exists(_identifyEstablishmentsStartCallback)) {
            _identifyEstablishmentsStartCallback();
        }
        _leafletMap.on('mousedown', _mouseDownForIdentifyEstablishmentsEvent);
        _leafletMap.on('mouseup', _mouseUpForIdentifyEstablishmentsEvent);
        _leafletMap.on('mouseout', _mouseUpForIdentifyEstablishmentsEvent);
    };

    /** IIFE for construction of a EstablishmentsMap */
    (function (options) {
        options = options || {};
        var mapNodeId = options.mapNodeId;
        if (!_exists(mapNodeId)) {
            // If there's no mapNodeId, we shouldn't initialise the map.
          console.log("ERROR: Map initialisation called but no map node ID provided.");
            return;
        }
        if (_exists(_leafletMap)) {
            // If we already have a map associated with this instance, we remove it.
            _leafletMap.remove();
            _leafletMap = null;
        }

        // Zoom and centre settings
        var minZoom = options.minZoom || 4;
        var maxZoom = options.maxZoom || 17;
        var initCentre = options.initCentre || null;

        // Bools need a stricter check because of JS lazy evaluation
        var canZoom = _exists(options.canZoom) ? options.canZoom : true;
        var canPan = _exists(options.canPan) ? options.canPan : true;
        _maxBounds = _setMaxBounds(options.mapBounds) || void 0;
        _leafletMap = L.map(mapNodeId, {
            minZoom: minZoom,
            maxZoom: maxZoom,
            maxBounds: _maxBounds,
            maxBoundsViscosity: 1.0,
            zoomControl: canZoom,
            scrollWheelZoom: canZoom ? 'center' : false, // We want the map to stay centred on scrollwheel zoom if zoom is enabled.
            keyboardPanDelta: canPan ? 80 : 0
        });
        if (_exists(initCentre) && _isArray(initCentre) && initCentre.length === 2) {
            var rawLat = initCentre[0];
            var rawLong = initCentre[1];
            var centreLatLng = _getLatLngInBC(rawLat, rawLong);
            if (_exists(centreLatLng)) {
                _leafletMap.setView(centreLatLng, maxZoom);
            }
        } else if (_exists(_maxBounds)) {
            _leafletMap.fitBounds(_maxBounds);
        }

        if (!canPan) {
            _leafletMap.dragging.disable();
            _leafletMap.doubleClickZoom.disable();
        }

        if (_exists(options.esriLayers)) {
            _loadEsriLayers(options.esriLayers);
        }
        if (_exists(options.wmsLayers)) {
            _loadWmsLayers(options.wmsLayers);
        }

        // Callbacks
        _establishmentPushpinMoveCallback = options.establishmentPushpinMoveCallback || null;
        _identifyEstablishmentsStartCallback = options.identifyEstablishmentsStartCallback || null;
        _identifyEstablishmentsEndCallback = options.identifyEstablishmentsEndCallback || null;

        var establishmentPushpinInit = options.establishmentPushpinInit || null;
        if (_exists(establishmentPushpinInit) && _exists(establishmentPushpinInit.lat) && _exists(establishmentPushpinInit.long)) {
            var details = establishmentPushpinInit.establishmentDetails;
            placeestablishmentPushpin([establishmentPushpinInit.lat, establishmentPushpinInit.long], details);
        }
        
        // TODO: Settle attribution (possibly even external to the map)
        // Position of the attribution control
        //_leafletMap.attributionControl.setPosition('topright');
    }(options));

    // The public members and methods of a EstablishmentsMap.
    return {
        placeestablishmentPushpin: placeestablishmentPushpin,
        removeestablishmentPushpin: removeestablishmentPushpin,
        drawAndFitBounds: drawAndFitBounds,
        startIdentifyEstablishments: startIdentifyEstablishments
    };
}
