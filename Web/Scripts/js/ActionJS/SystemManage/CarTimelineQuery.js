var timeline;
var encryData;
var groups;
var items;
var orders;
var options = {
    orientation: 'top',
    width: '98%',
    padding: 0,
    type: 'box',
    locales: {
        'zh-tw': {
            current: 'current',
            time: 'time'
        }
    },
    locale: 'zh-tw',
    stack: true,
    //zoomMin: 18 * 60 * 1000
    //zoomMax: 24 * 60 * 60 * 1000,
};

$(function () {
    var now = new Date();
    var StationList = $("#hidStation").val();
    console.log("StationList=" + StationList);
    if (StationList !== "") {
        var Station = StationList.split(";");
        $("#objStation").autocomplete({
            source: Station,
            minLength: 1,
            matchCase: false
        });
    }
    //$('#btn_query').on('click', function () {
    //    CarScheduleSearch();

    //});

    drawVisualization();

    /*
    $('#zoomIn').on('click', function(){
        zoom(-0.2);
    });
    $('#zoomOut').on('click', function(){
        zoom(0.2);
    });
    $('#moveLeft').on('click', function(){
        move(0.2);
    });
    $('#moveRight').on('click', function(){
        move(-0.2);
    });
    */
})

//初始化TimeLine object
function drawVisualization() {
    timeline = new vis.Timeline(document.getElementById('mytimeline'), items, options);
}
//
function switchStack(isStack) {
    options["stack"] = isStack;
    timeline.setOptions(options);
    timeline.redraw();
}