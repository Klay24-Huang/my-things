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
    SetStationNoShowName($("#StationID"));
    SetCar($("#CarNo"))
    $("#btnSend").on("click", function () {
        CarScheduleSearch();
    });
   drawVisualization();

})
//取得查詢資料
function CarScheduleSearch() {
    ShowLoading("查詢中....");
    var flag = true;
    var errMsg = "";

    var StationID = $('#StationID').val();
    var CarNo = $("#CarNo").val();
    var SDate = $("#StartDate").val();
    var EDate = $("#EndDate").val();
    console.log("StationID=" + StationID);
    if ("" === StationID && ""===CarNo) {
        flag = false;
        errMsg = "請選擇據點或車號";
    }
    if (SDate === '' || EDate === '') {
        flag = false;
        errMsg = "請輸入起迄日期";
    }
    if (SDate > EDate && EDate !== "") {
        errMsg = "開始日期大於結束日期";
        flag = false;
    }

    if (flag) {
        var Account = $("#Account").val();
        var obj = new Object();
        obj.UserID = Account;
        obj.SD = SDate.replace(/\//g, "").replace(/\-/g, "");
        obj.ED = EDate.replace(/\//g, "").replace(/\-/g, "");
        obj.StationID = StationID;
        obj.CarNo = CarNo;
        var json = JSON.stringify(obj);
        console.log(json);
        var site = jsHost + "BE_CarScheduleTimeLog";
        console.log("site:" + site);
        $.ajax({
            url: site,
            type: 'POST',
            data: json,
            cache: false,
            contentType: 'application/json',
            dataType: 'json',           //'application/json',
            success: function (data) {
                $.busyLoadFull("hide");
                console.log(data);
                if (data.Result == "1") {
                    swal({
                        title: 'SUCCESS',
                        text: data.ErrorMessage,
                        icon: 'success'
                    }).then(function (value) {
                        setCarSchedule(data)
                    });
                } else {

                    swal({
                        title: 'Fail',
                        text: data.ErrorMessage,
                        icon: 'error'
                    });
                }
            },
            error: function (e) {
                $.busyLoadFull("hide");
                swal({
                    title: 'Fail',
                    text: "查詢車輛管理時間發生錯誤",
                    icon: 'error'
                });
            }

        });


        // timeline.setItems(items);
    } else {
        disabledLoadingAndShowAlert(errMsg);
    }
    //console.log(carId);


}
function setCarSchedule(JsonData) {
    // drawVisualization();
    console.log(JsonData.Data.length);
    if (typeof groups == 'undefined') {
        groups = new vis.DataSet();
    } else {
        groups.clear();
    }
    //$('#content').show();
    if (typeof items == 'undefined') {
        items = new vis.DataSet();
    } else {
        items.clear();
    }
    var len = JsonData.Data.length;
   // timeline = new vis.Timeline(document.getElementById('visualization'), items, options);
    for (i = 0; i < len; i++) {
        groups.add({
            id: JsonData.Data[i].CarNo,
            content: JsonData.Data[i].CarNo
        });
        timeline.setGroups(groups);
        itemLen = JsonData.Data[i].lstOrder.length;
        console.log(itemLen)
        for (j = 0; j < itemLen; j++) {
            console.log(JsonData.Data[i].lstOrder[j].car_mgt_status + ",BS=" + JsonData.Data[i].lstOrder[j].booking_status + ",CS=" + JsonData.Data[i].lstOrder[j].cancel_status)
            if (JsonData.Data[i].lstOrder[j].car_mgt_status < 17) {
                var className = 'bg-danger';
                var start = new Date(JsonData.Data[i].lstOrder[j].SD);
                var end = new Date(JsonData.Data[i].lstOrder[j].ED);
                console.log("start:" + start + ",end:" + end);
                if (JsonData.Data[i].lstOrder[j].cancel_status > 0 && (JsonData.Data[i].lstOrder[j].booking_status < 1 || JsonData.Data[i].lstOrder[j].booking_status>2)) {
                    className = 'bg-secondary';
                } else if (JsonData.Data[i].lstOrder[j].booking_status === 1 || JsonData.Data[i].lstOrder[j].booking_status === 2 && JsonData.Data[i].lstOrder[j].car_mgt_status==0) {
                    className = 'bg-outline-danger';
                    start = new Date(JsonData.Data[i].lstOrder[j].SD);
                } else if (JsonData.Data[i].lstOrder[j].booking_status === 3 || JsonData.Data[i].lstOrder[j].booking_status === 4) {
                    className = 'bg-primary';
                    start = new Date(JsonData.Data[i].lstOrder[j].FS);
                } else if (JsonData.Data[i].lstOrder[j].car_mgt_status >= 4 && JsonData.Data[i].lstOrder[j].car_mgt_status < 16) {
                    className = 'bg-warning';
                    start = new Date(JsonData.Data[i].lstOrder[j].FS);
                } else if (JsonData.Data[i].lstOrder[j].car_mgt_status === 16 && JsonData.Data[i].lstOrder[j].booking_status === 5) {
                    className = 'bg-success';
                    start = new Date(JsonData.Data[i].lstOrder[j].FS);
                    end = new Date(JsonData.Data[i].lstOrder[j].FE);
                }

                items.add({
                    'type': 'range',
                    'id': JsonData.Data[i].lstOrder[j].OrderNum,
                    'start': start,
                    'end': end,
                    'content': '[' + JsonData.Data[i].lstOrder[j].OrderNum + ']' + $.format.date(start, 'MM/dd HH:mm') + '~' + $.format.date(end, 'MM/dd HH:mm') + '<br/>' + JsonData.Data[i].lstOrder[j].IDNO + ' ' + JsonData.Data[i].lstOrder[j].UName + ' ' + (typeof JsonData.Data[i].lstOrder[j].Mobile === 'undefined' ? '' : JsonData.Data[i].lstOrder[j].Mobile),
                    'group': JsonData.Data[i].CarNo,
                    'className': className
                });
                timeline.setItems(items);
            }

        }
    }
  timeline.setGroups(groups);
 //drawVisualization();
}
//初始化TimeLine object
function drawVisualization() {
    timeline = new vis.Timeline(document.getElementById('visualization'), items, options);
}
//
function switchStack(isStack) {
    options["stack"] = isStack;
    timeline.setOptions(options);
    timeline.redraw();
}