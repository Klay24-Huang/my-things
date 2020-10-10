var map = L.map('map', {
});
map.setView([25.0270000, 121.545745], 13);


L.tileLayer('http://{s}.tile.osm.org/{z}/{x}/{y}.png', {
        attribution: '&copy; <a href="http://osm.org/copyright">OpenStreetMap</a> contributors'
    }).addTo(map);



var marker=L.marker([25.0270000, 121.545745]).addTo(map);
var latlngs = [
    [25.0270000, 121.545745],
    [25.0370000, 121.535745],
    [25.0270000, 121.525745]
];
var polyline = L.polyline(latlngs);
polyline.addTo(map);
latlngs = [[
    [25.0270000, 121.555745],
    [25.0370000, 121.565745],
    [25.0270000, 121.575745],
    [25.0270000, 121.555745]
]];
var polygon = L.polygon(latlngs);
polygon.addTo(map);


var bounds = [[25.0390000, 121.559745], [25.0490000, 121.578745]];
var rectangle=L.rectangle(bounds);
rectangle.addTo(map);
console.log(rectangle.toGeoJSON());

var circle= L.circle([25.0370000, 121.515745], {radius: 200});
circle.addTo(map);
console.log(circle.toGeoJSON());


var data={
    "type": "MultiPolygon",
    "coordinates": [
            [
            [
                [
                121.5398,
                25.0071
                ],
                [
                121.5831,
                25.0112
                ],
                [
                121.5515,
                25.0294
                ],
                [
                121.5398,
                25.0071
                ]
            ]
            ],
            [
            [
                [
                121.5398,
                25.0071
                ],
                [
                121.5031,
                25.0112
                ],
                [
                121.5515,
                25.0294
                ],
                [
                121.5398,
                25.0071
                ]
            ]
            ]
        ]
}
L.geoJSON(data).addTo(map);

// zoom the map to the polyline