$(function () {
    var marker;
    var opt = {
        dateFormat: 'yy-mm-dd',
        showSecond: true,
        timeFormat: 'HH:mm:ss',
        timeOnlyTitle: "選擇時分秒",
        timeText: "時間",
        hourText: "時",
        minuteText: "分",
        secondText: "秒",
        showMillisec: false,
        millisecText: "毫秒",
        timezoneText: "時區",
        currentText: "現在時間",
        closeText: "確定",
        controlType: "select"
    };
    $("#start").datetimepicker(opt);
    $("#end").datetimepicker(opt);
    var len = $("#SearchLen").val();
    if (len > 0) {
        $("#TimeContent").show();
        $("#InforContent").show();

        setValue($('#TimeLine').val());
        for (var i = 0; i < len; i++) {
            GetAddress($("#Lng"+i.toString()).val(),$("#Lat"+i.toString()).val(),"addr"+i.toString());
        }
        $("#TimeLine").on("change", function () {
            console.log($(this).val());
            setValue($(this).val());
          
            // string value = (j + 1).ToString() + ";" + lstEv[j].CarNo + ";" + lstEv[j].Lat.ToString() + ";" + lstEv[j].Lng.ToString() + ";" + lstEv[j].SPEED.ToString();
            //   value += ";" + lstEv[j].CarStatus + ";" + lstEv[j].CCNum + ";" + lstEv[j].FactoryYear + ";" + lstEv[j].GPSStatus + ";" + lstEv[j].GPSTime;
        });
    } else {
        $("#TimeContent").hide();
        $("#InforContent").hide();
    }
    //$("#TimeContent").hide();
    //$("#InforContent").hide();
    $("#btnQuery").on('click', function () {
       // blockUI();
        var Mode = parseInt($("#Mode").val(),10);
        var SD = $("#start").val();
        var ED = $("#end").val();
        var CarNo = $("#ObjCar").val();
        var OrderNum = $("#OrderNum").val();
        var flag = true;
        var errMsg = "";
        if (Mode === 0) {
            if (OrderNum === "") {
                flag = false;
                errMsg = "請輸入訂單編號";
            } else {

            }
        } else {
            if (SD === "") {
                flag = false;
                errMsg = "請輸入起始時間";
            }
            if (flag) {
                if (ED === "") {
                    flag = false;
                    errMsg = "請輸入結束時間";
                }
            }
            if (flag) {
                if (CarNo === "") {
                    flag = false;
                    errMsg = "請輸入車號";
                } else {
                    CarNo = CarNo.toString().toUpperCase();
                }
            }
            if (flag) {
                var SDD = new Date(SD);
                var EDD = new Date(ED);
                if (EDD < SDD) {
                    flag = false;
                    errMsg = "結束時間不能大於開始時間";
                }
            }
           
        }
        if (flag) {
            formbookingQuery.submit();
        } else {
            //   $.unblockUI();
            warningAlert(errMsg, false, 0, "");
        }

    });
    var CarList = $("#hidCar").val();
    console.log("CarList="+CarList);
    if (CarList !== "") {
        var Car = CarList.split(";");
        $("#ObjCar").autocomplete({
            source:Car,
            minLength: 3,
            matchCase: false
        });
    }

    $('input[name$="Mode"]').on('click', function () {
        var type = $(this).val();
        console.log(type);
        switch (type) {
            case "0":
                console.log("A");
                $('#OrderTerm').show();
                $('#CarNoTerm').hide();
                $('#DateTerm').hide();
                break;
            case "1":
                console.log("B");
                $('#OrderTerm').hide();
                $('#CarNoTerm').show();
                $('#DateTerm').show();

                break;
        }
    });

});
function setValue(value) {
    var carData = value.split(";");
    //呼號
    $("#who").val(carData[0]);
    $("#car").val(carData[1]);
    if (parseInt(carData[5], 10) === 0) {
        $("#status").val("可出租");
    } else {
        $("#status").val("出租中");
    }
    if (carData[7] === "") {
        $("#old").val("未設定出廠年份");
    } else {
        var old = new Date().getYear() - parseInt(carData[7], 10);
        $("#old").val(old.toString() + "年多");
    }
    $("#speed").val(carData[4]);
    $("#cc").val(carData[6]);
    $("#update").val(carData[9]);
   // GetAddressForText(carData[3], carData[2], "neer");
    $("#neer").val("");
    console.log("lat:" + carData[2] + ";Lng:" + carData[3]);
    setMap(parseFloat(carData[2]), parseFloat(carData[3]), carData[1]);
}
function setMap(lat, lng, carNo) {
    console.log("lat:" + lat + ";Lng:" + lng);
    deleteMarkers();
 
    var myLatLng = { lat: lat, lng: lng };
    var image = '../assets/img/icon_m_car.png';
    var marker = new google.maps.Marker({
        map: map,
        position: myLatLng,
        icon: image
     });
     map.setCenter(myLatLng);
     map.setZoom(18);
    var infowindow = new google.maps.InfoWindow();
    infowindow.setContent('<b>' + carNo + '<b>');
    // google.maps.event.addListener(marker, 'click', function () {
    infowindow.open(map, marker);
    // });
    map.setCenter(marker.getPosition());
    markers.push(marker);
}
// Sets the map on all markers in the array.
function setMapOnAll(map) {
    for (var i = 0; i < markers.length; i++) {
        markers[i].setMap(map);
    }
}

// Removes the markers from the map, but keeps them in the array.
function clearMarkers() {
    setMapOnAll(null);
}
function deleteMarkers() {
    clearMarkers();
    markers = [];
}