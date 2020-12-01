
$(function () {
    var now = new Date();

    $('#btn_query').on('click', function () {
        Search();

    });

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
var host = "http://113.196.107.238/";
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
    stack: false,
    //zoomMin: 18 * 60 * 1000
    //zoomMax: 24 * 60 * 60 * 1000,
};
//取得查詢資料
function Search() {
    var flag = true;
    var errMsg = "";
    if (typeof groups == 'undefined') {
        groups = new vis.DataSet();
    } else {
        groups.clear();
    }
    var carId = $('#objCar').val();
    var SDate = $("#SDate").val();
    var EDate = $("#EDate").val();
    if ("-1" == carId) {
        flag = false;
        errMsg = "請選擇車輛";
    }
    if (SDate == '' && EDate == '') {
        flag = false;
        errMsg = "請至少要輸入一個日期";
    }
    if (SDate > EDate) {
        errMsg = "開始日期大於結束日期";
        flag = false;
    }

    if (flag) {
        if (typeof items == 'undefined') {
            items = new vis.DataSet();
        } else {
            items.clear();
        }
        groups.add({
            id: 'order',
            content: '訂單編號'
        });



        groups.add({
            id: 'to_obc',
            content: 'COMM'
        });
        groups.add({
            id: carId,
            content: carId
        });

        timeline.setGroups(groups);
        var checkData = new Object();
        checkData.SDate = SDate;
        checkData.EDate = EDate;
        checkData.CarID = carId
        var sendData = new Object();
        sendData.para = JSON.stringify(checkData);

        GetEncyptData(JSON.stringify(sendData), getSendQuery);
       
        timeline.setItems(items);
    } else {
        warningAlert(errMsg, flag, 0, "")
    }
    console.log(carId);


}

function getSendQuery(encryptData) {
    for (mode = 0; mode < 4; mode++) {
              var jdata = JSON.stringify({ "para": encryptData, "mode":mode });
             var URL = host + "iRentAPI/api/MessageTimeLog/GetData";
   
  
   
    $.ajax({
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: jdata,
        url: URL,
        error: function (xhr, error) { console.log(xhr.responseText + "," + xhr.status + "," + error); },
        success: function (JsonData) {
            console.log(JsonData.Result + "," + JsonData.ErrMsg);
            if (JsonData.Result == "0") {
                    var len = parseInt(JsonData.total, 10);

                switch (JsonData.Mode) {
                    case "0":
                        getBooking(JsonData, len);
                        break;
                    case "1":
                        getCommLog(JsonData, len);
                        break;
                    case "2":
                        getGps(JsonData, len);
                        break;
                    case "3":
                        getDoorStatus(JsonData, len);
                        break;
                }
              
                
               

            } else {
                // UPDFail();
                warningAlert(JsonData.ErrMsg, false, 0, "");
            }
        }
    });
    } //end for
}
function getGps(jsonData,len){
    console.log("getGPS");
    var GPSData = jQuery.parseJSON(jsonData.data);
    for (i = 0; i < len; i++) {

        var start = new Date(GPSData[i].create_time * 1000);
        var end = new Date(GPSData[i].s_time * 1000);
        var group = "to_obc";
        var status = "";
        var className = "";

        if (GPSData[i].msg_id === "BS") {
            status = "開機";
            className = "bs";
        } else if (GPSData[i].msg_id === "SS") {
            status = "關機";
            className = "ss";
        } else if (GPSData[i].msg_id === "US") {
            var event = GPSData[i].msg_body;
            if (event.indexOf("1;") == 0) {
                status = "開鎖";
            } else if (event.indexOf("2;") == 0) {
                status = "關鎖";
            } else if (event.indexOf("3;") == 0) {
                status = "刷卡:" + event.split(";")[1].replace(/^0+/, '');
            }
        } else if (GPSData[i].msg_id === 'QR') {
            var event = GPSData[i].msg_body;
            status = "車輛狀態回覆:" + event;
            className = "rs";
        }

        try {
            var item = {
                'id': GPSData[i].create_time + ":GPS" + i.toString() + GPSData[i].msg_id,
                'title': $.format.date(start, 'HH:mm:ss'),
                'start': start,
                'content': '[' + status + ']',
                'group': group,
                'className': className
            };
            if (start < end) {
                item["type"] = "range";
                item["end"] = end;
            }
            items.add(item);
        } catch (err) {
            console.log('getGpsLog');
        }
    }
}
function getDoorStatus(jsonData,len){

    console.log("getDoorStatus");
    var DoorStatusData = jQuery.parseJSON(jsonData.data);
    for (i = 0; i < len; i++) {
        var start = new Date(DoorStatusData[i].create_time);
        var door = DoorStatusData[i].door_status == "1" ? "開門" : "關門";

    try {
        items.add({
            'id': DoorStatusData[i].create_time + ":DoorStatus_" + i.toString() + "_" + DoorStatusData[i].door_status,
            'title': $.format.date(start, 'HH:mm:ss'),
            'start': start,
            'content': "[" + door + "]",
            'group': DoorStatusData[i].car_id
        });
    } catch (err) {
        console.log('getDoorStatus..');
    }
}
}
function getBooking(jsonData, len) {
    var bookingData = jQuery.parseJSON(jsonData.data);
    console.log(bookingData.length);
    for (i = 0; i < len; i++) {
        var className = 'reserve';
        var start = ("" == bookingData[i].start_time) ? "1900-01-01" : new Date(bookingData[i].start_time);
        var end = ("" == bookingData[i].stop_time) ? "1900-01-01" : new Date(bookingData[i].stop_time);
        console.log("start=" + start + ",end=" + end);
        if (bookingData[i].cancel_status == 3 || bookingData[i].cancel_status == 4) {
            className = 'cancel';
        } else if (bookingData[i].booking_status == 1) {
            className = 'maintain';
        } else if (bookingData[i].booking_status == 3 || bookingData[i].booking_status == 4) {
            className = 'extend';
        } else if (bookingData[i].already_lend_car == true) {
            className = 'dispatch';
            start = ("" == bookingData[i].final_start_time) ? "1900-01-01" : new Date(bookingData[i].final_start_time);
            if (bookingData[i].already_return_car == true) {
                className = 'finish';
                end = ("" == bookingData[i].final_stop_time) ? "1900-01-01" : new Date(bookingData[i].final_stop_time);
            }
        }
        var content = "[" + bookingData[i].order_number + "]";
        if (start != "1900-01-01") {
            content += $.format.date(start, 'MM/dd HH:mm');
        }
        if (end != "1900-01-01") {
            content += " ~ " + $.format.date(end, 'MM/dd HH:mm');
        }
        content += "<br/>" + bookingData[i].citizen_id + " " + bookingData[i].name + " " + (typeof bookingData[i].phone_number === 'undefined' ? '' : '0' + bookingData[i].phone_number)

        items.add({
            'type': 'range',
            'id': bookingData[i].order_number,
            'start': start,
            'end': end,
            'content': content,
            'group': 'order',
            'className': className
        });
    }
}
function getCommLog(jsonData,len) {
    var commData = jQuery.parseJSON(jsonData.data);
    console.log("2:" + "Len="+len);

    for (i = 0; i < len; i++) {
        var start = new Date(commData[i].create_time);
        var status = "";
        var group = "to_obc";

        if (commData[i].msg_id === "RS") {
            status = "卡片設定" + commData[i].m_sno + "-" + commData[i].m_sst;
        } else if (commData[i].msg_id === "RR") {
            status = "設定回覆:" + commData[i].msg_body;
            group = commData[i].car_id;
        } else if (commData[i].msg_id === "QS") {
            status = "車輛狀態" + commData[i].m_sno + "-" + commData[i].m_sst;
        } else if (commData[i].msg_id === "LS") {
            status = "遠端控制" + commData[i].m_sno + "-" + commData[i].m_sst;
        } else if (commData[i].msg_id === "LR") {
            group = commData[i].car_id;
            status = "遠端回覆:" + commData[i].msg_body;
        }

        try {
            items.add({
                'id': commData[i].create_time + ":CommLog"+i.toString()+"_" + commData[i].msg_body,
                'title': $.format.date(start, 'HH:mm:ss'),
                'start': start,
                'content': '[' + status + ']',
                'group': group,
                'className': 'rs'
            });
            flag = true;
        } catch (err) {
            console.log('getCommLog=' + err.toString());
        }
    }
}
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