//initial map
var map = L.map('map').setView([25.0270000, 121.545745], 11);
// Set up the OSM layer
L.tileLayer(
    'https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    maxZoom: 20,
    errorTileUrl: 'http://bpic.588ku.com/element_pic/16/12/07/706f7ff4f15725b17ba1d30d384e6468.jpg'
}).addTo(map);
//options..?
//http://leaflet-extras.github.io/leaflet-providers/preview/index.html
//https://leafletjs.com/plugins.html#basemap-providers

// Initialise the FeatureGroup to store editable layers
var editableLayers = new L.FeatureGroup();
map.addLayer(editableLayers);
//initial draw plugin and toll settings
var drawPluginOptions = {
    position : 'topright',
    draw: {
        // Turns off these drawing tool
        polyline: false,
        circle: false, 
        rectangle: false,
        marker: false,
        circlemarker:false,
        polygon: {
            allowIntersection: false, // Restricts shapes to simple polygons
            drawError: {
                color: '#e1e100', // Color the shape will turn when intersects
                message: '<strong>Polygon draw does not allow intersections!<strong> (allowIntersection: false)' // Message that will show when intersect
            },
            shapeOptions: {
                // color: '#D7000F'
            }
        }
    },
    edit: {
        featureGroup: editableLayers, //REQUIRED!!
        edit: true
    },
    
};

//save polygon id
var polyLayersId = [];

//load data from backend
//L.geoJSON(data).addTo(map);

//layers data already exist
var polyLayers = [];
var polygon1 = L.polygon([
    [[22.73264868398435, 120.28450012207031],
    [22.72837380478485, 120.28450012207031],
    [22.723307108275556, 120.28604507446288],
    [22.727502979677855, 120.27437210083008],
    [22.732094540515035, 120.28209686279295],
    [22.732569520769065, 120.29059410095213],
    [22.73241119420103, 120.29239654541016],
    [22.721644557594896, 120.29093742370605]]
]);
polyLayers.push(polygon1)
var polygon2 = L.polygon([
    [24.512642, 121],
    [24.520387, 121.1],
    [24.509116, 121.2]
]);
polyLayers.push(polygon2);
// Add the layers to the editable feature group 
for(let layer of polyLayers) {
    editableLayers.addLayer(layer);
    
    polyLayersId.push(editableLayers.getLayerId(layer));
    // console.log(layer._renderer._center);
    renderData(layer);
}
//two functions for buttons
function setView(e){
    // console.log(e);
    //get center of polygon
    let center = editableLayers.getLayer(polyLayersId[e.dataset.index]).getBounds().getCenter();
    // set view
    map.setView(center, 12);
};
function removeLayer(e){
    // console.log(e.dataset.layerid);
    let targetId = Number(e.dataset.layerid),
        targetIndex = polyLayersId.indexOf(targetId);
    editableLayers.removeLayer(targetId);
    // console.log(map._layers);
    polyLayersId.splice(targetIndex,1);
    document.querySelector('#polygonData').innerHTML = "";
    polyLayersId.forEach(function(value){
        let layer = editableLayers.getLayer(value);
        // console.log(layer);
        renderData(layer);
    });
    // console.log(polyLayersId);
}

//render html
function renderData(layer){
    var str = `
                <div class="card p-2 mb-1">
                    <p> 
                        ${polyLayersId.indexOf(editableLayers.getLayerId(layer))+1}
                    </p>
                    <button type="button" class="btn btn-primary mb-1 setViews" 
                        data-index="${polyLayersId.indexOf(editableLayers.getLayerId(layer))}" 
                        data-layerId="${editableLayers.getLayerId(layer)}" 
                        onclick="setView(this)">移至
                    </button>
                    <button type="button" class="btn btn-danger" 
                        data-index="${polyLayersId.indexOf(editableLayers.getLayerId(layer))}" 
                        data-layerId="${editableLayers.getLayerId(layer)}"
                        onclick="removeLayer(this)"
                        >刪除
                    </button>
                </div>
                `;
    document.querySelector('#polygonData').insertAdjacentHTML( 'beforeend', str );
}
// $('.leaflet-pane.leaflet-overlay-pane').attr('id', 'polygonPath');

// Initialise the draw control and pass it the FeatureGroup of editable layers
var drawControl = new L.Control.Draw(drawPluginOptions);
map.addControl(drawControl);

var drawnItems = new L.FeatureGroup();
map.addLayer(drawnItems);
var drawControl = new L.Control.Draw({
    edit: {
        featureGroup: drawnItems
    }
});
//while polygon created
map.on('draw:created', function(e) {
    let layer = e.layer;

    editableLayers.addLayer(layer);
    polyLayersId.push(editableLayers.getLayerId(layer));
    renderData(layer);
});
//while polygon edit
map.on('draw:edited', function (e) {
    // let layers = e.layers;
    // layers.eachLayer(function (layer) {
        //do whatever you want; most likely save back to db
        // console.log(layer);
    // });
});