
var latlng = { lat: 25.046891, lng: 121.516602 }; // 給一個初始位置
var map;
var markers = [];
var table;
var nowSelMode = "nor";
var nowMode = "location";
var nowMarker;
var infowindow = null;
var cityCircle;
var METERS_PER_MILE = 1609.34;
var raidus = 3000;
var roundObj = new Object;
var timeFlag = true;
var delayTime = 1 * 60 * 1000;
var lastLocation, nowLocation;
var isFirst = true;
$(document).ready(function () {
 //   var nowMode = "location";
    $.busyLoadFull("show", {
        spinner: "cube-grid"
    });
    var listMode = ""
    nowMode = "list";
    localStorage.setItem("nowMode", nowMode);
    if (localStorage.getItem('nowMode') == null || localStorage.getItem('nowMode') == "") {
        localStorage.setItem('nowMode', nowMode);
    } else {
        nowMode = localStorage.getItem('nowMode');
        if (nowMode != "location" && nowMode != "list") {
            nowMode = "location";
            localStorage.setItem('nowMode', nowMode);
        }
    }
    nowSelMode = localStorage.getItem('nowSelMode');
    if (nowSelMode == null) {
        localStorage.setItem('nowSelMode', "nor");
        nowSelMode = "nor";
    } else {
        if (nowSelMode == "") {
            localStorage.setItem('nowSelMode', "nor");
            nowSelMode = "nor";
        }
    }
    localStorage.removeItem('nor');
    localStorage.removeItem('any');
    localStorage.removeItem('moto');
    localStorage.removeItem('maintain');
    if (nowMode == "location") {
        $("#location").hide();
        $("#divlist").hide();
        $("#listIcon").hide();
    }
    
    getCarData();

   // var timeoutID = window.setInterval((() => checkReloadData()), delayTime);


    $("#btnNormal1").on("touchend", function () {
        console.log("Normal1 touchend");
        listMode = "nor";
        nowSelMode = "nor";
        console.log("nor");
        localStorage.setItem('nowSelMode', "nor");
        localStorage.removeItem('nor');
        getCarData();
        //let nor = JSON.parse(localStorage.getItem('nor'));
        //console.log(nor.length);
        //console.log(nor);
        //if (nor.length > 0) {
        //    showList(nor);
        //}
    });
    $("#btnNormal1").on("click", function () {
       console.log("Normal1 Click");
        listMode = "nor";
        nowSelMode = "nor";
        console.log("nor");
        localStorage.setItem('nowSelMode', "nor");
        localStorage.removeItem('nor');
        getCarData();
        //let nor = JSON.parse(localStorage.getItem('nor'));
        //console.log(nor.length);
        //console.log(nor);
        //if (nor.length > 0) {
        //    showList(nor);
        //}
    });

    $("#btnAny1").on("touchend", function () {
        console.log("Any1 touchend");
        listMode = "any";
        nowSelMode = "any";
        localStorage.setItem('nowSelMode', "any");
        console.log("any");
        localStorage.removeItem('any');
        getCarData();
        //let nor = JSON.parse(localStorage.getItem('nor'));
        //console.log(nor.length);
        //console.log(nor);
        //if (nor.length > 0) {
        //    showList(nor);
        //}
    });
    $("#btnAny1").on("click", function () {
console.log("Any1 click");
        listMode = "any";
        nowSelMode = "any";
        localStorage.setItem('nowSelMode', "any");
        console.log("any");
        localStorage.removeItem('any');
        getCarData();
        //let nor = JSON.parse(localStorage.getItem('nor'));
        //console.log(nor.length);
        //console.log(nor);
        //if (nor.length > 0) {
        //    showList(nor);
        //}
    });
  
    $("#btnMoto1").on("touchend", function () {
        console.log("motor1 touchend");
        listMode = "moto";
        nowSelMode = "moto";
        localStorage.setItem('nowSelMode', "moto");
        console.log("moto");
        localStorage.removeItem('moto');
        getCarData();
        //let moto = JSON.parse(localStorage.getItem('moto'));
        //console.log(moto.length);
        //console.log(moto);
        //if (moto.length > 0) {

        //    showList(moto);
        //}
    });
    $("#btnMoto1").on("click", function () {
        console.log("motor1 click");
        listMode = "moto";
        nowSelMode = "moto";
        localStorage.setItem('nowSelMode', "moto");
        console.log("moto");
        localStorage.removeItem('moto');
        getCarData();
        //let moto = JSON.parse(localStorage.getItem('moto'));
        //console.log(moto.length);
        //console.log(moto);
        //if (moto.length > 0) {

        //    showList(moto);
        //}
    });
    $("#btnMaintenance").on("click", function () {
        console.log("maintain click");
        listMode = "maintain";
        nowSelMode = "maintain";
        localStorage.setItem('nowSelMode', "maintain");
        console.log("maintain");
        localStorage.removeItem('nor');
        localStorage.removeItem('any');
        localStorage.removeItem('moto');
        localStorage.removeItem('maintain');
        getCarData();
    });
    $("#list").on("touchend", function () {
        console.log("list touchend");
        $("#divlist").show();
        $("#location").show();
        $("#map").hide();
        $("#list").hide();
        $("#listIcon").show();
        $("#fixedBox").hide();
        $("#listFixedBox").show();
        let nor = JSON.parse(localStorage.getItem('nor'));
        if (listMode == "any") {
            let nor = JSON.parse(localStorage.getItem('any'));
        }else if (listMode == "maintain") {
                let nor = JSON.parse(localStorage.getItem("maintain"));
        } else {
            let nor = JSON.parse(localStorage.getItem('moto'));
        }
        nowMode = "list";
        localStorage.setItem("nowMode", nowMode);

        getList();
    });
    $("#list").on("click", function () {
        console.log("list click");
        $("#divlist").show();
        $("#location").show();
        $("#map").hide();
        $("#list").hide();
        $("#listIcon").show();
        $("#fixedBox").hide();
        $("#listFixedBox").show();
           let nor = JSON.parse(localStorage.getItem('nor'));
        if (listMode == "any") {
            let nor = JSON.parse(localStorage.getItem('any'));
        } else if (listMode == "maintain") {
            let nor = JSON.parse(localStorage.getItem("maintain"));
        } else {
            let nor = JSON.parse(localStorage.getItem('moto'));
        }
        console.log("nor length:"+nor.length)
        nowMode = "list";
        localStorage.setItem("nowMode", nowMode);
        // showList(nor);
        getList();
    });
    $("#list1").on("touchend", function () {
        console.log("list1 touchend");
        $("#divlist").show();
        $("#location").show();
        $("#map").hide();
        $("#list").hide();
        $("#listIcon").show();
        $("#fixedBox").hide();
        $("#listFixedBox").show();
        let nor = JSON.parse(localStorage.getItem('nor'));
        if (listMode == "any") {
            let nor = JSON.parse(localStorage.getItem('any'));
        } else if (listMode == "maintain") {
            let nor = JSON.parse(localStorage.getItem('maintain'));
        } else {
            let nor = JSON.parse(localStorage.getItem('moto'));
        }
        nowMode = "list";
        localStorage.setItem("nowMode", nowMode);
       // showList(nor);
        getList();
    });
    $("#list1").on("click", function () {
        console.log("list1 click");
        $("#divlist").show();
        $("#location").show();
        $("#map").hide();
        $("#list").hide();
        $("#listIcon").show();
        $("#fixedBox").hide();
        $("#listFixedBox").show();
        let nor = JSON.parse(localStorage.getItem('nor'));
        if (listMode == "any") {
            let nor = JSON.parse(localStorage.getItem('any'));
        } else if (listMode == "maintain") {
            let nor = JSON.parse(localStorage.getItem('maintain'));
        } else {
            let nor = JSON.parse(localStorage.getItem('moto'));
        }
        nowMode = "list";
        localStorage.setItem("nowMode", nowMode);
       // showList(nor);
        getList();
    });
    $("#location").on("touchend", function () {
        //console.log("location touchend");
        //$("#divlist").hide();
        //$("#location").hide();
        //$("#map").show();
        //$("#list").show();
        //$("#listIcon").hide();
        //$("#fixedBox").show();
        //$("#listFixedBox").hide();
        //nowMode = "location";
        //localStorage.setItem("nowMode", nowMode);
        nowMode = "location";
        localStorage.setItem("nowMode", nowMode);
        window.location.href = "Map";
    });
    $("#location").on("click", function () {
        //console.log("location click");
        //$("#divlist").hide();
        //$("#location").hide();
        //$("#map").show();
        //$("#list").show();
        //$("#listIcon").hide();
        //$("#fixedBox").show();
        //$("#listFixedBox").hide();
        //nowMode = "location";
        //localStorage.setItem("nowMode", nowMode);
        nowMode = "location";
        localStorage.setItem("nowMode", nowMode);
        window.location.href = "Map";
    });
    $("#location1").on("touchend", function () {
        //console.log("location1 touchend");
        //$("#divlist").hide();
        //$("#location").hide();
        //$("#map").show();
        //$("#list").show();
        //$("#listIcon").hide();
        //$("#fixedBox").show();
        //$("#listFixedBox").hide();
        nowMode = "location";
        localStorage.setItem("nowMode", nowMode);
        window.location.href = "Map";
    });
    $("#location1").on("click", function () {
//console.log("location click");
//        $("#divlist").hide();
//        $("#location").hide();
//        $("#map").show();
//        $("#list").show();
//        $("#listIcon").hide();
//        $("#fixedBox").show();
//        $("#listFixedBox").hide();
        nowMode = "location";
        localStorage.setItem("nowMode", nowMode);
        window.location.href = "Map";
    });
})
function isMobile() {

    if(/(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|ipad|iris|kindle|Android|Silk|lge |maemo|midp|mmp|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino/i.test(navigator.userAgent) 

    || /1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(navigator.userAgent.substr(0,4))){
  return true;
}else{
   return false;
}

}
function checkReloadData() {
    if (timeFlag) {
        localStorage.removeItem('nor');
        localStorage.removeItem('any');
        localStorage.removeItem('moto');
        console.log("重新讀取");
        getCarData();
    }
}
function showList(obj) {
    console.log("showList()");
    //清空
    $('#carDetailBody').html("");
    if (obj == null) {
        $('#myDataTalbe').footable({ "paging": { "limit": 4 } });
        return;
    }
    //產資料
    var len = obj.length;
    console.log("len="+len);
    
    var tmpHtml = "";
    for (i = 0; i < len; i++) {
        var bgColor = "";
        var OrderStatus = "無租約"
        var d1 = Date.parse(obj[i].LastClean);
        var d2 = Date.now();
        var diffDate = parseInt((d2 - d1) / 86400000);
      
        obj[i].CarNo=obj[i].CarNo.replace(/ /g, "");
        obj[i].CarNo = obj[i].CarNo.replace(/\"/g, "");
        var isLowPow = (parseInt(obj[i].LowPowStatus, 10) == 1) ? "電瓶12V電壓不足" : "";
        if (parseInt(obj[i].LowPowStatus, 10) == 1 && nowSelMode == "moto") {
            isLowPow = "電池平均電量不足";
        }
        if (parseInt(obj[i].isNoResponse, 10) == 1) {
            if (isLowPow == "") {
                isLowPow = "車機一小時未回應";
            } else {
                isLowPow += "、車機一小時未回應";
            }
        }
        var isNeedMaintain = (parseInt(obj[i].isNeedMaintenance, 10) == 1) ? "√" : "";
        var isLongTime = (parseInt(obj[i].afterRentDays, 10) > 3) ? "三天以上未租" : "";
        if (parseInt(obj[i].OrderStatus, 10) != 0) {
            OrderStatus = "租約中【H" + obj[i].OrderStatus + "】";
            bgColor = "background-color:#f7c22a";
        } else if (parseInt(obj[i].LowPowStatus, 10) == 1 || parseInt(obj[i].isNoResponse, 10) == 1) {
            bgColor = "background-color:#b167a6";
        } else if (parseInt(obj[i].afterRentDays, 10) > 3) {
            bgColor = "background-color:#60b631";
        } else if (parseInt(obj[i].isNeedMaintenance, 10) == 1) {
            bgColor = "background-color:#006d49";
        }
      
        var btn = "";
        btn = "<input type='button' ontouchend=booking('" + obj[i].CarNo + "'); onclick=booking('" + obj[i].CarNo + "');  value='預約' />";
        tmpHtml += "<tr style=\"" + bgColor + "\"><td>" + obj[i].CarNo + "</td><td>" + obj[i].NowStationName + "(" + obj[i].NowStationID + ")</td><td>" + OrderStatus + "</td><td>" + diffDate + "</td><td>" + obj[i].AfterRent + "</td>";
        tmpHtml += "<td>" + isLowPow + "</td><td>" + isLongTime + "</td><td>" + isNeedMaintain+"</td><td>" + btn + "</td></tr>";
       
    }
    
     $('#carDetailBody').html(tmpHtml);
  //  $('#myDataTalbe tbody').html(tmpHtml).trigger('footable_redraw');
    if (isFirst) {
        $('#myDataTalbe').footable({ "paging": { "limit": 4 } });
    } else {
        $('#myDataTalbe').trigger('footable_redraw');
    }
 

  

}

function getList() {
    console.log("call getList()");
    let nor = JSON.parse(localStorage.getItem('nor'));
    if (nowSelMode == "any") {
        nor = JSON.parse(localStorage.getItem('any'));

    } else if (nowSelMode == "moto") {
        nor = JSON.parse(localStorage.getItem('moto'));
    } else if (nowSelMode == "maintain") {
        nor = JSON.parse(localStorage.getItem('maintain'));
    }
    if (nor != null) {
        console.log(nowSelMode);
        //   console.log("nor=" + nor.length);
        showList(nor);
    } else {
        showList();
    }

    
}
function getCarData() {

    var URL = jsHost + "MA_GetCleanCarByList";// GetCleanCar
    var NowType = 0;
    if (nowSelMode == "any") {
        NowType = 1;
    } else if (nowSelMode == "moto") {
        NowType = 4;
    } else if (nowSelMode == "maintain") {
        NowType = 5;
    }
    var jsonData = JSON.stringify({ "para": { "Account": $("#Account").val(),"NowType":NowType } });
    console.log(jsonData);

    console.log(URL);
    $.ajax({
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: jsonData,
        url: URL,
        error: function (xhr, error) { console.log(xhr.responseText + "," + xhr.status + "," + error); $.busyLoadFull("hide", { animate: "fade" });},
        success: function (JsonData) {
            console.log(JsonData.Result + "," + JsonData.ErrorMessage);
            console.log(JsonData);
            if (JsonData.Result === "1") {
                console.log("true");
                console.log(JsonData.Data);
                var dataLen = JsonData.Data.length;
                console.log("dataLen:"+dataLen);
                for (var i = 0; i < dataLen; i++) {
                    if (JsonData.Data[i].total > 0) {
                        console.log(JsonData.Data[i].total);
                        if (NowType == 5) {
                            localStorage.setItem('maintain', JSON.stringify(JsonData.Data[i].CarList));
                        } else {
                            if (JsonData.Data[i].projType == 0) {
                                localStorage.setItem('nor', JSON.stringify(JsonData.Data[i].CarList));

                            } else if (JsonData.Data[i].projType == 3) {
                                localStorage.setItem('any', JSON.stringify(JsonData.Data[i].CarList));
                            } else {
                                localStorage.setItem('moto', JSON.stringify(JsonData.Data[i].CarList));
                            }
                        }
                        
                        console.log("nowMode:"+nowMode);
                      
                        /*if (nowMode == "location") {
                            if (nowSelMode == "nor" && JsonData.data[i].projType == 0) {
                                for (var j = 0; j < JsonData.data[i].total; j++) {

                                    drawMark(JsonData.data[i].CarList[j].Lat, JsonData.data[i].CarList[j].Lng, JsonData.data[i].CarList[j], JsonData.data[i].projType);
                                }
                            } else if ((nowSelMode == "any" && JsonData.data[i].projType == 3)) {
                                for (var j = 0; j < JsonData.data[i].total; j++) {

                                    drawMark(JsonData.data[i].CarList[j].Lat, JsonData.data[i].CarList[j].Lng, JsonData.data[i].CarList[j], JsonData.data[i].projType);
                                }
                            } else if ((nowSelMode == "moto" && JsonData.data[i].projType == 4)) {
                                for (var j = 0; j < JsonData.data[i].total; j++) {

                                    drawMark(JsonData.data[i].CarList[j].Lat, JsonData.data[i].CarList[j].Lng, JsonData.data[i].CarList[j], JsonData.data[i].projType);
                                }
                              
                            }
                           
                        } */
                       

                    }

                }
                nowMode = "list";
                if (nowMode == "list") {
                    console.log("call click list")
                    //$("#list").click();    
                    getList();
                    $.busyLoadFull("hide", { animate: "fade" });
                            
                } else {
                    $("#location").click();
                }


            } else {
                console.log("false");

            }
        }
    });
}


function booking(CarNo) {
    $.busyLoadFull("show", {
        spinner: "cube-grid"
    });
    console.log(CarNo);
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
        } else if (nowSelMode == "nor") {
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
        //let nor = JSON.parse(localStorage.getItem('nor'));
        //var obj = nor.find(function (element) {
        //    return element.CarNo.replace(/ /g, "").replace(/\"/g, "") == CarNo;
        //});
        //if (obj) {
        //    console.log(obj);
        //    flag = true;
        //    $("#CarNo").val(CarNo);
        //    $("#BookingStart").val(obj.BookingStart);
        //    $("#BookingEnd").val(obj.BookingEnd);
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
    }

}

