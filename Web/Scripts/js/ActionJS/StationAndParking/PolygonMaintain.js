

var has = $("#havePolygon").val();
var myLatLng = { lat: parseInt(lat), lng: parseInt(lng) };
var Color = '#e8e8e8';
var polyLayersId = [];
var polyLayers = [];
var poly = [];
var NowCount = 0;
var ActiveMapColor = "";
var RejectMapColor = "";


$(function () {
    $(".nav-tabs a").click(function () {
        $(this).tab('show');
    });
    ActiveMapColor = $("#MAPColor").val();
    RejectMapColor = $("#RMAPColor").val();
    initMap();
    $(".pick-a-color").pickAColor();
    /*可還車區按新增*/
    $("#btnAddNew").on("click", function () {
        var drawColor = $("#MAPColor").val();
        if (drawColor != "") {
            Color = "#" + drawColor;
            startDraw(0);
        }
    })
    /*可還車區按儲存*/
    $("#btnSaveActive").on("click", function () {
        ShowLoading("資料儲存中…");
        var flag = true;
        var errMsg = "";
        var obj = new Object();
        var StationID = $("#StationID").val();
        var BlockName = $("#BlockName").val();
        var MAPColor = $("#MAPColor").val();
        var SDate = $("#SDate").val();
        var EDate = $("#EDate").val();
        var BlockID = $("#ActiveID").val();
        var checkList = [ BlockName, MAPColor, SDate, EDate];
        var checkListMsg = ["電子柵欄名稱未填", "顏色未填", "有效日期(起)未填", "有效日期(迄)未填"];
        for (var i = 0; i < checkList.length; i++) {
            if (checkList[i] == "") {
                flag = false;
                errMsg = checkListMsg[i];
                break;
            }
        }
        if (flag) {
            obj.polygon = [];
            for (var i = 0; i < polyLayers.length; i++) {
                if (polyLayers[i].polygonType == 0) {
                    var polygonData = new Object();
                    polygonData.RawData = polyLayers[i].polygon;
                  
                    obj.polygon.push(polygonData);
                }
            }
            obj.StationID = StationID;
            obj.BlockID = BlockID;
            obj.BlockName = BlockName;
            obj.MAPColor = MAPColor;
            obj.StartDate = SDate;
            obj.EndDate = EDate;
      
            obj.UserID = Account;
            obj.Mode = 0;
            DoAjax(obj,"BE_HandlePolygon","儲存可還車區域發生錯誤")
        } else {
            disabledLoadingAndShowAlert(errMsg);
        }
    })
    /*不可還車區按新增*/
    $("#btnAddNewReject").on("click", function () {
        var drawColor = $("#RMAPColor").val();
        if (drawColor != "") {
            Color = "#" + drawColor;
            startDraw(1);
        }
    });
    /*不可還車區按儲存*/
    $("#btnSaveReject").on("click", function () {
        ShowLoading("資料儲存中…");
        var flag = true;
        var errMsg = "";
        var obj = new Object();
        var StationID = $("#RStationID").val();
        var BlockName = $("#RBlockName").val();
        var MAPColor = $("#RMAPColor").val();
        var SDate = $("#RSDate").val();
        var EDate = $("#REDate").val();
        var BlockID = $("#RejectID").val();
        var checkList = [BlockName, MAPColor, SDate, EDate];
        var checkListMsg = ["電子柵欄名稱未填", "顏色未填", "有效日期(起)未填", "有效日期(迄)未填"];
        for (var i = 0; i < checkList.length; i++) {
            if (checkList[i] == "") {
                flag = false;
                errMsg = checkListMsg[i];
                break;
            }
        }
        if (flag) {
            obj.polygon = [];
            for (var i = 0; i < polyLayers.length; i++) {
                if (polyLayers[i].polygonType == 1) {
                    console.log(polyLayers[i].polygon);
                    var polygonData = new Object();
                    polygonData.RawData = polyLayers[i].polygon;

                    obj.polygon.push(polygonData);
                }
            }
            obj.StationID = StationID;
            obj.BlockID = BlockID;
            obj.BlockName = BlockName;
            obj.MAPColor = MAPColor;
            obj.StartDate = SDate;
            obj.EndDate = EDate;
       
            obj.UserID = Account;
            obj.Mode = 1;
            DoAjax(obj, "BE_HandlePolygon", "儲存可還車區域發生錯誤")
        } else {
            disabledLoadingAndShowAlert(errMsg);
        }
    })
    
  
});
function initMap(title) {
    map = new google.maps.Map(document.getElementById('map'), {
        center: myLatLng,
        scrollwheel: false,
        zoom: 18,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    });
    currCenter = map.getCenter();
    var marker = new google.maps.Marker({
        map: map,
        position: myLatLng,
        title: title
    });

    var ActiveLatArr = $("#ActiveLat").val();
    var ActiveLngArr = $("#ActiveLng").val();
    var RejectLatArr = $("#RejectLat").val();
    var RejectLngArr = $("#RejectLng").val();
   // console.log(latArr);
 //   console.log(lngArr);
    if (ActiveLatArr != "" && ActiveLngArr != "") {
        var tmplat = ActiveLatArr.split("⊙");
        var tmplng = ActiveLngArr.split("⊙");
        var latlngArr = []

        for (var i = 0; i < tmplat.length; i++) {
            var tmplat2 = tmplat[i].split(",");
            var tmplng2 = tmplng[i].split(",");
            var obj = new Object();
            var polygonArr = new Array();  
            bounds = new google.maps.LatLngBounds();
            for (var j = 0; j < tmplat2.length; j++) {
               // console.log(tmplat2[j] + "," + tmplng2[j]);
                var latlng = new google.maps.LatLng(tmplat2[j], tmplng2[j]);
                latlngArr.push(latlng);
                var polygonObj = new Object();
                polygonObj.lng = tmplng2[j];
                polygonObj.lat = tmplat2[j];
                polygonObj.MAPColor = "#" + ActiveMapColor;
                polygonArr.push(polygonObj);
                bounds.extend(latlng)
                
            }
            obj.centerLat = bounds.getCenter().lat();
            obj.centerLng = bounds.getCenter().lng();
            obj.polygon = polygonArr;
            obj.polygonType = 0;
            polyLayers.push(obj);
            renderData(obj);
            var tmp = new google.maps.Polygon({
                paths: latlngArr,
                strokeColor: MAPColor,
                strokeOpacity: 0.8,
                    strokeWeight: 2,
                fillColor: MAPColor,
                            fillOpacity: 0.35
              });
            tmp.setMap(map);
            poly.push(tmp);
        
            latlngArr.length = 0;
        }
    }
    if (RejectLatArr != "" && RejectLngArr != "") {
        var tmplat = RejectLatArr.split("⊙");
        var tmplng = RejectLngArr.split("⊙");
        var latlngArr = []

        for (var i = 0; i < tmplat.length; i++) {
            var tmplat2 = tmplat[i].split(",");
            var tmplng2 = tmplng[i].split(",");
            var obj = new Object();
            var polygonArr = new Array();
            bounds = new google.maps.LatLngBounds();
            for (var j = 0; j < tmplat2.length; j++) {
                // console.log(tmplat2[j] + "," + tmplng2[j]);
                var latlng = new google.maps.LatLng(tmplat2[j], tmplng2[j]);
                latlngArr.push(latlng);
                var polygonObj = new Object();
                polygonObj.lng = tmplng2[j];
                polygonObj.lat = tmplat2[j];
                polygonObj.MAPColor = "#" + RejectMapColor;
                polygonArr.push(polygonObj);
                bounds.extend(latlng)

            }
            obj.centerLat = bounds.getCenter().lat();
            obj.centerLng = bounds.getCenter().lng();
            obj.polygon = polygonArr;
            obj.polygonType = 1;
            polyLayers.push(obj);
            renderData(obj);
            var tmp = new google.maps.Polygon({
                paths: latlngArr,
                strokeColor: "#" + RejectMapColor,
                strokeOpacity: 0.8,
                strokeWeight: 2,
                fillColor: "#" + RejectMapColor,
                fillOpacity: 0.35
            });
            tmp.setMap(map);
            poly.push(tmp);

            latlngArr.length = 0;
        }
    }
   // startDraw() 

    //}

    //GEvent.addListener(myMap, "click", function (overlay, point) {
    //    if (point) {
    //        //設定標註座標
    //        myMarker.setLatLng(point);
    //        document.getElementById('inLatLng').value = point.toString();
    //    }
    //});

}
//initial map
function startDraw(type) {
    var trueColor = "'" + Color + "'";
    $("#Mode").val(type);
    //$("#toolbar").css("display", "none");
    //$("#set").addClass("btn btn-primary disabled");
    bounds = new google.maps.LatLngBounds();
    var drawingManager = new google.maps.drawing.DrawingManager({
       
        drawingControl: false,
        drawingControlOptions: {
            position: google.maps.ControlPosition.TOP_CENTER,
            drawingModes: [google.maps.drawing.OverlayType.POLYGON]
        },
        polygonOptions: {
            strokeColor: Color,
            strokeOpacity: 0.8,
            strokeWeight: 2,
            fillColor: Color,
            fillOpacity: 0.35,
            clickable: false,
            editable: false
        }

    });
   

    drawingManager.setMap(map);
    drawingManager.setDrawingMode(google.maps.drawing.OverlayType.POLYGON);
    google.maps.event.addListener(drawingManager, 'overlaycomplete', function (e) {

        console.log(e.overlay);
        polygon = e.overlay;
        poly.push(e.overlay);
      //  console.log("overlaycomplete");

        if (drawingManager.getDrawingMode()) {
            drawingManager.setDrawingMode(null);
        }

    });

    google.maps.event.addListener(drawingManager, 'polygoncomplete', function (e) {
        var coordinates = (e.getPath());
        var len = coordinates.getLength();
        var tmpX = coordinates.getAt(0).lng();
        var tmpY = coordinates.getAt(0).lat();

        var bounds = new google.maps.LatLngBounds();
       // bounds.extend(coordinates);
        var Mode = $("#Mode").val();
       
        var obj = new Object();
        var polygonArr = new Array();    
        for (i = 0; i < len; i++) {
            var polygonObj = new Object();
            polygonObj.lng = coordinates.getAt(i).lng();
            polygonObj.lat = coordinates.getAt(i).lat();
          
            if (Mode == "0") {
                polygonObj.MAPColor = "#" + $("#MAPColor").val();
            } else {
                polygonObj.MAPColor = "#" + $("#RMAPColor").val();
            }
            polygonArr.push(polygonObj);
            bounds.extend( new google.maps.LatLng(coordinates.getAt(i).lat(), coordinates.getAt(i).lng()))
        }
        obj.centerLat = bounds.getCenter().lat();
        obj.centerLng = bounds.getCenter().lng();
        obj.polygonType = Mode;
        if (Mode == "0") {
            obj.MAPColor ="#"+$("#MAPColor").val();
        } else {
            obj.MAPColor = "#" + $("#RMAPColor").val();
        }
       
        obj.polygon = polygonArr;
        if (drawingManager.getDrawingMode()) {
            drawingManager.setDrawingMode(null);
        }
        polyLayers.push(obj);
        renderData(obj);
    
        polyLayersId.push(drawingManager)
        //$("#PX").val(tmpX);
        //$("#PY").val(tmpY);


        //$("#save").on("click", function () {
        //    var kind = $("#kind").val();
        //    if ((tmpX != null && tmpY != null) || kind == 3) {
        //        var checkData = new Object();
        //        checkData.PX = tmpX;
        //        checkData.PY = tmpY;
        //        checkData.Mode = $("#Mode").val();
        //        checkData.kind = kind;
        //        checkData.MAPColor = $("#MAPColor").val().replace(/#/g, "");
        //        checkData.SiteID = $("#SiteID").val();
        //        checkData.BlockID = $("#havePolygon").val();
        //        var sendData = new Object();
        //        sendData.para = checkData;
        //        console.log(JSON.stringify(sendData));
        //        GetEncyptData(JSON.stringify(sendData), SaveBlock);

        //    }
        //});
    });
   

}
var polyLayersId = [];

//load data from backend
//L.geoJSON(data).addTo(map);

//layers data already exist
var polyLayers = [];

//two functions for buttons

//function removeLayer(e) {
   
//    let targetId = Number(e.dataset.layerid),
//        targetIndex = polyLayersId.indexOf(targetId);
//    editableLayers.removeLayer(targetId);
 
//    polyLayersId.splice(targetIndex, 1);
//    document.querySelector('#polygonData').innerHTML = "";
//    polyLayersId.forEach(function (value) {
//        let layer = editableLayers.getLayer(value);
     
//        renderData(layer);
//    });
   
//}
function setView(i) {
    var data = polyLayers[i];
    //var json = JSON.stringify(data);
    //console.log(json);
    //var obj = JSON.parse(json);
   // console.log(data.centerLat + "," + data.centerLng);
    var latLng = new google.maps.LatLng(data.centerLat, data.centerLng);
    map.panTo(latLng);
  
}
function removeLayer(objCount) {
   
    var tmp = [];
    var tmplay = [];
    poly[objCount].setMap(null);
    var polyLayersLen = polyLayers.length;
    for (var i = 0; i < polyLayersLen; i++) {
        if (i != objCount) {
            tmp.push(polyLayers[i])
            tmplay.push(poly[i])
        }
    }
    polyLayers.length = 0;
    poly.length = 0;
    polyLayers = tmp;
    poly = tmplay
    polyLayersLen = polyLayers.length;
  
    $("#polygonData").html("");
    NowCount = 0;
    for (var i = 0; i < polyLayersLen; i++) {
        renderData(polyLayers[i])
    }
}
//render html
function renderData(layer) {
  //  console.log(NowCount);
  //  console.log(layer);
    var title = (layer.polygonType == 0) ? "可還車區域" : "不可還車區域";
    var str = `
                <div class="card p-2 mb-1" style="background-color:${layer.polygon[0].MAPColor}" id="polygon_${NowCount}" name="polygon_${NowCount}">
                    <p>
                        ${NowCount + 1}<br>${title}
                    </p>
                    <button type="button" class="btn btn-primary mb-1 setViews"
                        data-index="${NowCount}" 
                        data-layerId="${NowCount}" 
                        data-polygon="${layer}"
                        onclick="setView('${NowCount}')">移至
                    </button>
                    <button type="button" class="btn btn-danger"
                        data-index="${NowCount}" 
                        data-layerId="${NowCount}"
                        data-polygon="${layer}"
                        onclick="removeLayer('${NowCount}')"
                        >刪除
                    </button>
                </div>
                `;
    document.querySelector('#polygonData').insertAdjacentHTML('beforeend', str);
    NowCount += 1;
}
