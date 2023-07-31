var GLOBAL = {};
var map = null;
GLOBAL.DotNetReference = null;

function initMap() {
    mapboxgl.accessToken = 'pk.eyJ1IjoiamV0c3RyZWFtdGVjaCIsImEiOiJja21kdnFmanoybXcwMnFvY2l1aWc0c3NhIn0.TF31xwLHmK_DrMmzA3Id4w';

    map = new mapboxgl.Map({
        container: 'map',
        style: 'mapbox://styles/mapbox/streets-v11',
        center: [10, 10]
    });
    // Add full screen toggle to map - disabled while we work on "search on move"
    // map.addControl(new mapboxgl.FullscreenControl(), 'top-left');
    // Add zoom and rotation controls to the map.
    map.addControl(new mapboxgl.NavigationControl(), 'top-left');
    map.on('render', () => {
        map.resize();
    });
}

function setFilterEvents() {
    map.on('load', () => {
        map.on('dragend', () => {
            GLOBAL.DotNetReference.invokeMethodAsync('OnMapFilterEvent')
        });
        map.on('wheel', () => {
            GLOBAL.DotNetReference.invokeMethodAsync('OnMapFilterEvent')
        });
    });
}

function setMarkers(data) {
    if (map === null || data === null) return;

    const geojson = {
        type: 'FeatureCollection',
        features: data
    };

    // add markers to map
    for (const feature of geojson.features) {
        // create a HTML element for each feature
        const el = document.createElement('div');
        el.className = 'marker';
        el.innerHTML = `<div class="globe-marker"><div>${feature.properties.title}</div><div class="marker-point"></div></div>`

        // make a marker for each feature and add to the map
        var marker = new mapboxgl.Marker(el)
            .setLngLat(feature.geometry.coordinates)
            .setPopup(
                new mapboxgl.Popup({ offset: 25 }) // add popups
                    .setHTML(`<div>${feature.properties.description}</div>`)
            )
            .addTo(map);

        addMouseOverListener(el, feature.properties.id);
    }
}

function addMouseOverListener(el, id) {
    try {
        const listing = document.getElementById(`listing-card-${id}`);
        let children = el.firstChild.children;

        const primaryColor = getComputedStyle(document.documentElement)
            .getPropertyValue('--colors-primary-500');
        const textColor = getComputedStyle(document.documentElement)
            .getPropertyValue('--colors-text');

        listing.addEventListener("mouseover", (event) => {
            for (let i = 0; i < children.length; i++) {
                children[i].style.backgroundColor = primaryColor;
                children[i].style.color = "white";
                el.style.zIndex = "1";
            }
        }, false);

        listing.addEventListener("mouseout", (event) => {
            for (let i = 0; i < children.length; i++) {
                children[i].style.backgroundColor = "white";
                children[i].style.color = textColor;
                el.style.zIndex = "0";
            }
        }, false);
    }
    catch (err) {
        // Catching error here to log edge case when listing card doesn't load before markers are set.
        console.error(err);
    }
}

function setCircle(data) {
    if (map === null || data === '[]') return;

    const geojson = {
        type: 'FeatureCollection',
        features: data
    };

    // add markers to map
    for (const feature of geojson.features) {
        // create a HTML element for each feature
        const el = document.createElement('div');
        el.className = 'marker';
        el.innerHTML = `<div class="marker-circle"></div>`

        // make a marker for each feature and add to the map
        var marker = new mapboxgl.Marker(el)
            .setLngLat(feature.geometry.coordinates);

        setMarkerScaleByZoom(marker);

        map.on('zoom', () => {
            setMarkerScaleByZoom(marker)
        });

        marker.addTo(map);
    }
}

function setMarkerScaleByZoom(marker) {
    const scale = Math.pow(1.4, map.getZoom());
    const svgElement = marker.getElement().children[0];
    svgElement.style.width = `${scale}px`;
    svgElement.style.height = `${scale}px`;
}

function fitToBounds(data, animate) {
    if (map === null || data === '[]') return;

    const geojson = {
        type: 'FeatureCollection',
        features: data
    };

    // Geographic coordinates of the LineString
    const coordinates = geojson.features[0].geometry.coordinates;

    // Create a 'LngLatBounds' with both corners at the first coordinate.
    const bounds = new mapboxgl.LngLatBounds(
        coordinates,
        coordinates
    );

    // Extend the 'LngLatBounds' to include every coordinate in the bounds result.
    for (const feature of geojson.features) {
        bounds.extend(feature.geometry.coordinates);
    }

    map.fitBounds(bounds, {
        padding: 100,
        animate: animate,
        maxZoom: 14
    });
}

function getBounds() {
    // HACK: Passing back as an array was not working. Resolved to pass back as a string and convert in C#
    return map.getBounds().toArray().toString();
}

function clearMarkers() {
    var els = document.getElementsByClassName('marker');
    if (els.length <= 0) return;

    var count = els.length - 1;
    while (count >= 0) {
        els[count].remove();
        count--;
    }
}

function setCenter(lat, lon, smoothTransition) {
    if (map === null) return;
    if (smoothTransition) {
        map.flyto([lat, lon]);
    }
    else {
        map.setCenter([lat, lon]);
    }
}

function setZoom(zoom) {
    if (map === null) return;
    map.setZoom(zoom);
}