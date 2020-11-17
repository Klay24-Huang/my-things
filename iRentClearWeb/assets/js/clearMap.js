var host = "http://113.196.107.238/"
var latlng = { lat: 25.046891, lng: 121.516602 }; // 給一個初始位置
var map;
var markers = [];
var table;
var nowSelMode = "moto";
var nowMode = "location";
var nowMarker;
var infowindow = null;
var cityCircle;
var METERS_PER_MILE = 1609.34;
var raidus = 3500;
var roundObj = new Object;
var timeFlag = true;
var delayTime = 1 * 60 * 1000;
var lastLocation, nowLocation;
function initMap() {
    console.log("on initMap");
    map = new google.maps.Map(document.getElementById('map'), {
        zoom: 14, //放大的倍率
        center: latlng, //初始化的地圖中心位置
        disableDefaultUI: true,
        clickableIcons: false,
        draggable: true,
        keyboardShortcuts: false
    });

    let lastLocation = localStorage.getItem('lastLocation');

    console.log("lastLocation:" + lastLocation);
    nowSelMode = localStorage.getItem('nowSelMode');
    console.log(nowSelMode);
    if (nowSelMode == null) {
        nowSelMode = "moto";
    }
    if (lastLocation != null) {
        nowLocation = JSON.parse(lastLocation);
        console.log("initMap目前在:" + nowLocation.lat + "," + nowLocation.lng);
        var pos = {
            lat: nowLocation.lat,
            lng: nowLocation.lng
        };
        nowMarker = new google.maps.Marker({
            position: pos,
            icon: '../assets/img/icon_people.png',
            map: map
        });

        map.setZoom(14);
        map.setCenter(pos);
    } else {

    }
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            var pos = {
                lat: position.coords.latitude,
                lng: position.coords.longitude
            };
            nowLocation.lat = position.coords.latitude;
            nowLocation.lng = position.coords.longitude;
            localStorage.setItem('lastLocation', JSON.stringify(nowLocation));
            nowMarker = new google.maps.Marker({
                position: pos,
                icon: '../assets/img/icon_people.png',
                map: map
            });

            map.setZoom(14);
            map.setCenter(pos);
            nowMarker.setPosition(pos);

        });
    } else {
        alert("不支援定位！");


    }
    CustomMarker.prototype = new google.maps.OverlayView();
    if (nowSelMode == "moto") {
        raidus = 1000;
    } else if (nowSelMode == "maintain") {
        raidus = 10000;
    }
    console.log("nowSelMode:" + nowSelMode);
    var sunCircle = {
        strokeColor: "#c3fc49",
        strokeOpacity: 0.8,
        strokeWeight: 2,
        fillColor: "#c3fc49",
        fillOpacity: 0.35,
        map: map,
        center: pos,
        radius: raidus // in meters
    };
    
    cityCircle = new google.maps.Circle(sunCircle);

    google.maps.event.addListener(map, 'click', function (event) {
        console.log("click:" + event.latLng);
        cityCircle.setCenter(event.latLng);
        clearMarkers()
        map.setCenter(event.latLng);
        if (!nowMarker) {
            var pos = {
                lat: event.latLng.lat(),
                lng: event.latLng.lng()
            };
            nowMarker = new google.maps.Marker({
                position: pos,
                icon: '../assets/img/icon_people.png',
                map: map
            });
        } else {
            nowMarker.setPosition(event.latLng);

        }
        for (var i = 0; i < markers.length; i++) {
            markers[i].setMap(null);
        }
        markers.length = 0;
        var lastLocation = new Object;
        lastLocation.lat = event.latLng.lat();
        lastLocation.lng = event.latLng.lng();
        localStorage.setItem('lastLocation', JSON.stringify(lastLocation));
        //getAround(event.latLng.lat(), event.latLng.lng(), raidus);
        getCarData(event.latLng.lat(), event.latLng.lng());


    });

    google.maps.event.addListener(map, 'touchend', function (event) {
        console.log("touchend:" + event.latLng);
        cityCircle.setCenter(event.latLng);
        clearMarkers()
        for (var i = 0; i < markers.length; i++) {
            markers[i].setMap(null);
        }
        markers.length = 0;
        var lastLocation = new Object;
        lastLocation.lat = event.latLng.lat();
        lastLocation.lng = event.latLng.lng();
        localStorage.setItem('lastLocation', JSON.stringify(lastLocation));
       // getAround(event.latLng.lat(), event.latLng.lng(), raidus);
        getCarData(event.latLng.lat(), event.latLng.lng());


    });

} //init_end

function getAround(lat, lon, raidus) {
    var latitude = lat;
    var longitude = lon;
    var degree = (24901 * 1609) / 360.0;
    var raidusMile = raidus;
    var dpmLat = 1 / degree;
    var radiusLat = dpmLat * raidusMile;
    var minLat = latitude - radiusLat;
    var maxLat = latitude + radiusLat;
    var mpdLng = degree * Math.cos(latitude * (Math.PI / 180));
    var dpmLng = 1 / mpdLng;
    var radiusLng = dpmLng * raidusMile;
    var minLng = longitude - radiusLng;
    var maxLng = longitude + radiusLng;
    roundObj.minLng = minLng;
    roundObj.maxLng = maxLng;
    roundObj.minLat = minLat;
    roundObj.maxLat = maxLat;
    var lastLocation = new Object;
    lastLocation.lat = lat;
    lastLocation.lng = lon;
    localStorage.setItem('lastLocation', JSON.stringify(lastLocation));
    console.log("目前在:" + lastLocation);
    timeFlag = true;
    if (nowSelMode == "nor") {

        let moto = JSON.parse(localStorage.getItem('nor'));
        if (moto) {
            if (moto.length > 0) {
                deleteMarkers()
                var norLen = moto.length;
                for (var i = 0; i < norLen; i++) {
                    drawMark(moto[i].Lat, moto[i].Lng, moto[i], 0);
                }
                showMarkers();

            }
        }
    } else if (nowSelMode == "any") {

        let moto = JSON.parse(localStorage.getItem('any'));
        console.log("len=>" + moto);
        if (moto) {
            if (moto.length > 0) {
                deleteMarkers()
                var norLen = moto.length;
                for (var i = 0; i < norLen; i++) {
                    drawMark(moto[i].Lat, moto[i].Lng, moto[i], 3);
                }
                showMarkers();

            }
        }
    } else if (nowSelMode == "moto") {

        let moto = JSON.parse(localStorage.getItem('moto'));
        console.log("len=>" + moto);

        if (moto) {
            if (moto.length > 0) {
                deleteMarkers()
                var norLen = moto.length;
                for (var i = 0; i < norLen; i++) {
                    drawMark(moto[i].Lat, moto[i].Lng, moto[i], 4);
                }
                showMarkers();

            }
        }

    }
}
$(document).ready(function () {
    console.log("on ready");
    getNowLocation();
    var listMode = ""
    if (localStorage.getItem('nowMode') == null || localStorage.getItem('nowMode') == "") {
        localStorage.setItem('nowMode', nowMode);
    } else {
        nowMode = localStorage.getItem('nowMode');
        if (nowMode != "location") {
            nowMode = "location";
            localStorage.setItem('nowMode', nowMode);
        }
    }
    nowSelMode = localStorage.getItem('nowSelMode');
    if (nowSelMode == null) {
        localStorage.setItem('nowSelMode', "moto");
        nowSelMode = "moto";
    } else {
        if (nowSelMode == "") {
            localStorage.setItem('nowSelMode', "moto");
            nowSelMode = "moto";
        }
    }
    localStorage.removeItem('nor');
    localStorage.removeItem('any');
    localStorage.removeItem('moto');
    if (nowMode == "location") {
        $("#location").hide();
        $("#divlist").hide();
        $("#listIcon").hide();
    }

  
    //  cityCircle.setCenter(nowLocation);
  //  getAround(nowLocation.lat, nowLocation.lng, raidus);
    //var timeoutID = window.setInterval((() => checkReloadData()), delayTime);

    $("#btnNormal").on("touchend", function () {
        //console.log("Normal touchend");
        listMode = "nor";
        nowSelMode = "nor";
        console.log("nor");
        localStorage.setItem('nowSelMode', "nor");
        //for (var i = 0; i < markers.length; i++) {
        //    markers[i].setMap(null);
        //}
        //markers.length = 0;
        //getCarData(nowLocation.lat, nowLocation.lng);
        window.location.href = "Map";
    });
    $("#btnNormal").on("click", function () {
        //console.log("Normal touchend");
        listMode = "nor";
        nowSelMode = "nor";
        console.log("nor");
        localStorage.setItem('nowSelMode', "nor");
        //for (var i = 0; i < markers.length; i++) {
        //    markers[i].setMap(null);
        //}
        //markers.length = 0;
        //getCarData(nowLocation.lat, nowLocation.lng);
        window.location.href = "Map";
    });


    $("#btnAny").on("touchend", function () {
        console.log("Any touchend");
        listMode = "any";
        nowSelMode = "any";
        localStorage.setItem('nowSelMode', "any");
        console.log("any");
        //for (var i = 0; i < markers.length; i++) {
        //    markers[i].setMap(null);
        //}
        //markers.length = 0;
        //getCarData(nowLocation.lat, nowLocation.lng);
        window.location.href = "Map";
    });
    $("#btnAny").on("click", function () {
        console.log("Any click");
        listMode = "any";
        nowSelMode = "any";
        localStorage.setItem('nowSelMode', "any");
        console.log("any");
        //for (var i = 0; i < markers.length; i++) {
        //    markers[i].setMap(null);
        //}
        //markers.length = 0;
        //getCarData(nowLocation.lat, nowLocation.lng);
        window.location.href = "Map";
    });
    $("#btnMoto").on("touchend", function () {
        console.log("motor touchend");
        listMode = "moto";
        nowSelMode = "moto";
        localStorage.setItem('nowSelMode', "moto");
        //console.log("moto");
        //for (var i = 0; i < markers.length; i++) {
        //    markers[i].setMap(null);
        //}
        //markers.length = 0;
        //getCarData(nowLocation.lat, nowLocation.lng);
        window.location.href = "Map";
    });
    $("#btnMoto").on("click", function () {
        console.log("motor click");
        listMode = "moto";
        nowSelMode = "moto";
        localStorage.setItem('nowSelMode', "moto");
        console.log("moto");
        //for (var i = 0; i < markers.length; i++) {
        //    markers[i].setMap(null);
        //}
        //markers.length = 0;
        //getCarData(nowLocation.lat, nowLocation.lng);
        window.location.href = "Map";
    });
    $("#btnMaintenance").on("click", function () {
        console.log("maintain click");
        listMode = "maintain";
        nowSelMode = "maintain";
        localStorage.setItem('nowSelMode', "maintain");
        console.log("moto");
        //for (var i = 0; i < markers.length; i++) {
        //    markers[i].setMap(null);
        //}
        //markers.length = 0;
        //getCarData(nowLocation.lat, nowLocation.lng);
        window.location.href = "Map";
    });

    $("#btnList").on("click", function () {
        nowMode = "list";
        localStorage.setItem("nowMode", nowMode);
        window.location.href = "Index";
    });
    $("#btnList").on("touchend", function () {
        nowMode = "list";
        localStorage.setItem("nowMode", nowMode);
        window.location.href = "Index";
    });
});
function getNowLocation() {
    $.busyLoadFull("show", {
        spinner: "cube-grid"
    });
    var URL = host + "iMotoWebAPI/api/MaintainUserGetLatLng";
    var jsonData = JSON.stringify({ "para": { "Account": $("#Account").val() } });
    console.log(jsonData);
    console.log(URL);
    $.ajax({
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: jsonData,
        url: URL,
        error: function (xhr, error) { $.busyLoadFull("hide", { animate: "fade" }); console.log(xhr.responseText + "," + xhr.status + "," + error); },
        success: function (JsonData) {
            $.busyLoadFull("hide", { animate: "fade" });
            console.log(JsonData.Result + "," + JsonData.ErrMsg);
            console.log(JsonData);
            if (JsonData.Result === "0") {

                console.log("true");
                console.log(JsonData.data);
                if (JsonData.data != null) {
                    var nowLocation = new Object;
                    nowLocation.lng = JsonData.data.Lng;
                    nowLocation.lat = JsonData.data.Lat;

                    var lastLocation = new Object;
                    lastLocation.lat = JsonData.data.Lat;
                    lastLocation.lng = JsonData.data.Lng;
                    localStorage.setItem('lastLocation', JSON.stringify(lastLocation));
                    let lastLocation2 = localStorage.getItem('lastLocation');
                    if (lastLocation2 != null) {
                        nowLocation = JSON.parse(localStorage.getItem('lastLocation'));
                        console.log("目前在:" + nowLocation.lat + "," + nowLocation.lng);
                    } else {
                        nowLocation = new Object();
                        nowLocation.lat = latlng.lat;
                        nowLocation.lng = latlng.lng;
                    }
                    markers.length = 0;
                    getCarData(JsonData.data.Lat, JsonData.data.Lng);
                } else {
                    var lastLocation = new Object;
                    lastLocation.lat = latlng.lat;
                    lastLocation.lng = latlng.lng;
                    localStorage.setItem('lastLocation', JSON.stringify(lastLocation));
                    getCarData(latlng.lat, latlng.lng);
                }
               
                initMap();
            } else {
                console.log("false");
                initMap();

            }
        }
    });
}
function getCarData(lat, lng) {
    $.busyLoadFull("show", {
        spinner: "cube-grid"
    });
    localStorage.removeItem('nor');
    localStorage.removeItem('any');
    localStorage.removeItem('moto');
    clearMarkers()
    var URL = host + "iMotoWebAPI/api/GetCleanCarByLatLng";
    var NowType = 0;
    var radius = 3.5;
    if (nowSelMode == "any") {
        NowType = 1;
    } else if (nowSelMode == "moto") {
        NowType = 4;
        radius = 1;
    } else if (nowSelMode == "maintain") {
        NowType = 5;
        radius = 10;
    }
    var jsonData = JSON.stringify({ "para": { "Account": $("#Account").val(), "NowType": NowType, "Lng": lng, "Lat": lat, "raduis": radius } });
    console.log(jsonData);

    console.log(URL);
    $.ajax({
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: jsonData,
        url: URL,
        error: function (xhr, error) { $.busyLoadFull("hide", { animate: "fade" }); console.log(xhr.responseText + "," + xhr.status + "," + error); },
        success: function (JsonData) {
            $.busyLoadFull("hide", { animate: "fade" });
            console.log(JsonData.Result + "," + JsonData.ErrMsg);
            console.log(JsonData);
            if (JsonData.Result === "0") {
               
                console.log("true");
                console.log(JsonData.data);
                var dataLen = JsonData.data.length;
                console.log(dataLen);
                for (var i = 0; i < dataLen; i++) {
                    if (JsonData.data[i].total > 0) {
                        console.log(JsonData.data[i].total);
                        if (NowType == 5) {
                            localStorage.setItem('maintain', JSON.stringify(JsonData.data[i].CarList));
                        } else {
                            if (JsonData.data[i].projType == 0) {
                                localStorage.setItem('nor', JSON.stringify(JsonData.data[i].CarList));

                            } else if (JsonData.data[i].projType == 3) {
                                localStorage.setItem('any', JSON.stringify(JsonData.data[i].CarList));
                            } else {
                                localStorage.setItem('moto', JSON.stringify(JsonData.data[i].CarList));
                            }
                        }
                    
                        console.log(nowMode);
                        if (nowMode == "location") {
                            if (nowSelMode == "nor" && JsonData.data[i].projType == 0) {
                                for (var j = 0; j < JsonData.data[i].total; j++) {

                                    drawMark(JsonData.data[i].CarList[j].Lat, JsonData.data[i].CarList[j].Lng, JsonData.data[i].CarList[j], JsonData.data[i].projType);
                                }
                                // showMarkers();
                            } else if ((nowSelMode == "any" && JsonData.data[i].projType == 3)) {
                                for (var j = 0; j < JsonData.data[i].total; j++) {

                                    drawMark(JsonData.data[i].CarList[j].Lat, JsonData.data[i].CarList[j].Lng, JsonData.data[i].CarList[j], JsonData.data[i].projType);
                                }
                                //  showMarkers();
                            } else if ((nowSelMode == "moto" && JsonData.data[i].projType == 4)) {
                                for (var j = 0; j < JsonData.data[i].total; j++) {

                                    drawMark(JsonData.data[i].CarList[j].Lat, JsonData.data[i].CarList[j].Lng, JsonData.data[i].CarList[j], JsonData.data[i].projType);
                                }

                            } else if (nowSelMode == "maintain") {
                                for (var j = 0; j < JsonData.data[i].total; j++) {

                                    drawMark(JsonData.data[i].CarList[j].Lat, JsonData.data[i].CarList[j].Lng, JsonData.data[i].CarList[j], JsonData.data[i].projType);
                                }
                            }
                            showMarkers();
                        }


                    }

                }
                //if (nowMode == "list") {
                //    // showList();
                //    $("#list").click();

                //} else {
                //    $("#location").click();
                //}
             

            } else {
                console.log("false");
          
            }
        }
    });
}
function drawMark(lat, lng, obj, type) {
    //  var map = new google.maps.Map(document.getElementById('map');
   // console.log("drawMark" + obj.CarNo);
    //  var map = document.getElementById('map')
    obj.CarNo = obj.CarNo.replace(/ /g, "");
    obj.CarNo = obj.CarNo.replace(/\"/g, "");
    var orderStatus = (obj.OrderStatus == 0) ? "未出租" : "出租中【" + obj.OrderStatus + "】";
    var contentString = '<div><div>車號：' + obj.CarNo + '</div>';
    contentString += '<div>所在據點：' + obj.NowStationID + '(' + obj.NowStationName + ')</div>';
    contentString += '<div>狀態：' + orderStatus + '</div>';
    var isLowPow = (parseInt(obj.LowPowStatus, 10) == 1) ? "電瓶12V電壓不足" : "";
    if (parseInt(obj.LowPowStatus, 10) == 1 && nowSelMode == "moto") {
        isLowPow = "電池平均電量不足";
    }
    if (parseInt(obj.isNoResponse, 10) == 1) {
        if (isLowPow == "") {
            isLowPow = "車機一小時未回應";
        } else {
            isLowPow += "、車機一小時未回應";
        }
    }
    var isLongTime = (parseInt(obj.afterRentDays, 10) > 3) ? "三天以上未租" : "";
    if (isLowPow != "" ) {
        contentString += "<div>告警：" + isLowPow+"</div>";
    }
    if (isLongTime != "") {
        contentString += "<div>三天以上未租：√</div>";
    }
    //if(isMobile()){
    contentString += '<div class="text-center"><span class="btn btn-danger text-center" id="booking_' + obj.CarNo + '" ontouchend=booking("' + obj.CarNo + '"); onclick=booking("' + obj.CarNo + '");>預約</span></div></div>';
    //}else{
    // contentString += '<div class="text-center"><span class="btn btn-danger text-center" id="booking_'+obj.CarNo+'" onclick=booking("' + obj.CarNo  + '");>預約</span></div></div>';
    //}



    var uluru = { lat: lat, lng: lng };
    var marker;
    var icon = "";
    var title = "";

    if (type == 0) {
        if (obj.OrderStatus >0) {
            icon = "../assets/img/icn_pin_orange.png";
            
            title = '同站租還';
        } else if (obj.LowPowStatus == "1" && obj.isNoResponse == "0") {
            icon = "../assets/img/irent_icon 2-21.png";
            title = '同站租還';
        } else if (obj.isNoResponse == "1") {
            
            icon = "../assets/img/icn_pin_purple.png";
            title = '同站租還';
        } else if (parseInt(obj.afterRentDays,10) > 3) {
            icon = "../assets/img/icn_pin_green.png";
            title = '同站租還';
        } else if (obj.isNeedMaintenance == "1") {
            icon = "../assets/img/icn_pin_dark_green.png";
            title = '同站租還';
        }  else {
            icon = "../assets/img/icn_pin_gray.png";
            title = '同站租還';
        }
    } else if (type == 3) {
        if (obj.OrderStatus > 0) {

            icon = "../assets/img/icn_carr_orange.png";
            title = '路邊租還';
        } else if (obj.LowPowStatus == "1" && obj.isNoResponse == "0") {
            icon = "../assets/img/irent_icon 2-21.png";
            title = '路邊租還';
        } else if (obj.isNoResponse == "1") {
            icon = "../assets/img/icn_carr_purple.png";

            title = '路邊租還';
        } else if (parseInt(obj.afterRentDays, 10) > 3) {
            icon = "../assets/img/icn_carr_green.png";
            title = '路邊租還';
        } else if (obj.isNeedMaintenance == "1") {
            icon = "../assets/img/icn_carr_dark_green.png";
            title = '路邊租還';
        } else {
            icon = "../assets/img/icn_carr_gray.png";
            title = '路邊租還';
        }
    } else {
        if (obj.OrderStatus > 0) {
            icon = "../assets/img/icon_motopin_orange.png";
            title = '機車';
        } else if (obj.LowPowStatus == "1" && obj.isNoResponse == "0") {
            icon = "../assets/img/irent_icon 2-21.png";
            title = '機車';
        } else if (obj.isNoResponse == "1") {
            icon = "../assets/img/icon_motopin_purple.png";

            title = '機車';
        } else if (parseInt(obj.afterRentDays, 10) > 3) {

            icon = "../assets/img/icon_motopin_green.png";
            title = '機車';
        } else if (obj.isNeedMaintenance == "1") {
            icon = "../assets/img/icon_motopin_dark_green.png";
            title = '機車';
        } else {
            icon = "../assets/img/icon_motopin_gray.png";
            
            title = '機車'
        }
    }
    var d1 = Date.parse(obj.LastClean);
    var d2 = Date.now();
    var diffDate = parseInt((d2 - d1) / 86400000);
    //var r = ((Math.random() * 6) % 6) / 100000;
    //var tmp = (uluru.lat + r);
    //if (tmp >= roundObj.minLat && tmp < roundObj.maxLat) {
    //    uluru.lat += r;
    //}

    marker = new google.maps.Marker({
        position: uluru,
        label: {
            text: diffDate + ',' + obj.AfterRent,
            color: 'red',
            fontSize: '14px',
        },
        labelOrigin: new google.maps.Point(50, 100),
        labelClass: "labels", // the CSS class for the label
        icon: {
            url: icon,
            labelOrigin: new google.maps.Point(20, -5)
        },
        title: title
    });
    marker.content = contentString;

    marker.addListener('click', function () {
        //infowindow.content = marker.content;   
        if (infowindow) {
            infowindow.close();
        }
        //console.log(marker.content);
        console.log("show");
        timeFlag = false;
        infowindow = new google.maps.InfoWindow({ content: marker.content });

        infowindow.open(map, marker);
    });

    //if ((lat >= roundObj.minLat && lat < roundObj.maxLat) && lng >= roundObj.minLng && lng < roundObj.maxLng) {
        markers.push(marker);
    //}
}
function setMapOnAll(map) {
    console.log("bbb");
    console.log("markers length=" + markers.length);
    for (var i = 0; i < markers.length; i++) {
        markers[i].setMap(map);
    }
}
// Sets the map on all markers in the array.
//function setMapOnAll(map) {
//    for (var i = 0; i < markers.length; i++) {
//        markers[i].setMap(map);
//    }
//}

// Removes the markers from the map, but keeps them in the array.
function clearMarkers() {
    // markers.length = 0;
    console.log("call clearMarkers");
    setMapOnAll(null);
}

// Shows any markers currently in the array.
function showMarkers() {
    console.log("aaa")
    setMapOnAll(map);
}

// Deletes all markers in the array by removing references to them.
function deleteMarkers() {
    clearMarkers();
    markers = [];
}

function booking(CarNo) {
    console.log(CarNo);
    $.busyLoadFull("show", {
        spinner: "cube-grid"
    });
    var flag = false;
    var errMsg = "";
    var obj;
    if (CarNo == "") {
        $.busyLoadFull("hide", { animate: "fade" });
        swal({
            title: "發生錯誤",
            text: "發生錯誤，請重新整理網頁",
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#DD6B55",
            confirmButtonText: '確定'
        });
    } else {
        console.log("nowSelMode:" + nowSelMode);
        if (nowSelMode == "maintain") {
            let maintain = JSON.parse(localStorage.getItem('maintain'));
            console.log("maintain=" + maintain);
            obj = maintain.find(function (element) {
                return element.CarNo.replace(/ /g, "").replace(/\"/g, "") == CarNo;
            });
        }else if (nowSelMode == "nor") {
            let nor = JSON.parse(localStorage.getItem('nor'));
            console.log("nor=" + nor);
            obj = nor.find(function (element) {
                return element.CarNo.replace(/ /g, "").replace(/\"/g, "") == CarNo;
            });
        } else if (nowSelMode == "any") {
            let any = JSON.parse(localStorage.getItem('any'));
            console.log("any=" + any);
            obj = any.find(function (element) {
                return element.CarNo.replace(/ /g, "").replace(/\"/g, "") == CarNo;
            });
        } else {
            let moto = JSON.parse(localStorage.getItem('moto'));
            console.log("moto=" + moto);
            obj = moto.find(function (element) {
                return element.CarNo.replace(/ /g, "").replace(/\"/g, "") == CarNo;
            });
        }
  
        if (obj) {
            console.log(obj);
            flag = true;
            $("#CarNo").val(CarNo);
            $("#BookingStart").val(obj.BookingStart);
            $("#BookingEnd").val(obj.BookingEnd);
        }
        //} else {
        //    nor = JSON.parse(localStorage.getItem('any'));

        //    var obj = nor.find(function (element) {
        //        return element.CarNo.replace(/ /g, "").replace(/\"/g, "") == CarNo;
        //    });
        //    if (obj) {
        //        console.log(obj);
        //        flag = true;
        //        $("#CarNo").val(CarNo);
        //        $("#BookingStart").val(obj.BookingStart);
        //        $("#BookingEnd").val(obj.BookingEnd);
        //    } else {
        //        nor = JSON.parse(localStorage.getItem('moto'));
        //        var obj = nor.find(function (element) {
        //            return element.CarNo == CarNo;
        //        });
        //        if (obj) {
        //            flag = true;
        //            $("#CarNo").val(CarNo);
        //            $("#BookingStart").val(obj.BookingStart);
        //            $("#BookingEnd").val(obj.BookingEnd);
        //            console.log(obj);
        //        }
        //    }

        //}
        if (flag) {
            $.busyLoadFull("hide", { animate: "fade" });
            frmBooking.submit();
        } else {
            $.busyLoadFull("hide", { animate: "fade" });
            swal({
                title: "發生錯誤",
                text: "發生錯誤，請重新整理網頁",
                type: "warning",
                showCancelButton: true,
                confirmButtonColor: "#DD6B55",
                confirmButtonText: '確定'
            });
        }
        // frmBooking.submit();
    }

}
function CustomMarker(latlng, map, args, icon) {
    this.latlng = latlng;
    this.args = args;
    this.icon = icon;
    this.setMap(map);
}



CustomMarker.prototype.draw = function () {

    var self = this;

    var div = this.div;

    if (!div) {

        div = this.div = document.createElement('div');

        div.className = 'marker';

        div.style.position = 'absolute';
        div.style.cursor = 'pointer';
        div.style.width = '20px';
        div.style.height = '20px';
        div.style.background = 'blue';

        if (typeof (self.args.marker_id) !== 'undefined') {
            div.dataset.marker_id = self.args.marker_id;
        }

        google.maps.event.addDomListener(div, "click", function (event) {
            alert('You clicked on a custom marker!');
            google.maps.event.trigger(self, "click");
        });

        var panes = this.getPanes();
        panes.overlayImage.appendChild(div);
    }

    var point = this.getProjection().fromLatLngToDivPixel(this.latlng);

    if (point) {
        div.style.left = (point.x - 10) + 'px';
        div.style.top = (point.y - 20) + 'px';
    }
};

CustomMarker.prototype.remove = function () {
    if (this.div) {
        this.div.parentNode.removeChild(this.div);
        this.div = null;
    }
};

CustomMarker.prototype.getPosition = function () {
    return this.latlng;
};
