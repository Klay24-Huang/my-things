﻿$(function () {
    //var StationList = localStorage.getItem("StationList");
    //var CarList = localStorage.getItem("CarList");
    //if (typeof StationList !== 'undefined' && StationList !== null) {
    //    StationList = JSON.parse(StationList)
    //}
    //if (typeof CarList !== 'undefined' && CarList !== null) {
    //    CarList = JSON.parse(CarList)
    //}
    ////var StationList = $("#hidStation").val();

    //if (StationList.length > 0) {
       
    //    var Station = new Array();
    //    var StationLen = StationList.length;
    //    for (var i = 0; i < StationLen; i++) {
    //        Station.push(StationList[i].StationName + "(" + StationList[i].StationID + ")");
    //    }
    //    $("#StationID").autocomplete({
    //        source: Station,
    //        minLength: 1,
    //        matchCase: true,
    //          select: function (event, ui) {
    //            var data = ui.item.value.split("(");
    //            var contactData = data[1].split(")");
    //              $("#StationID").val(contactData[0]);
            
    //              $("#StationName").html(data[0]);
    //            return false;
    //        }
    //    });

    //}


    //if (CarList.length > 0) {
    //    var Car = new Array();
    //    var CarLen = CarList.length;
    //    for (var i = 0; i < CarLen; i++) {
    //        Car.push(CarList[i].CarNo);
    //    }

    //    $("#CarNo").autocomplete({
    //        source: Car,
    //        minLength: 1,
    //        matchCase: true
    //    });

    //}
    SetStation($("#StationID"), $("#StationName"));
    SetCar($("#CarNo"))
  
    var hasData = parseInt($("#len").val());
    if (hasData > 0) {
        $('.table').footable({
            "paging": {
                "limit": 3,
                "size": 20
            }
        });
    }
    $("#btnSearch").on("click", function () {
        ShowLoading("資料處理中...");
        var flag = true;
        var message = "";
        var CarNo = $("#CarNo").val();
        var StationID = $("#StationID").val();
        var ShowType = $("input[name=ShowType]:checked").val();
      

        if (false==CheckIsUndefined(ShowType)) {
            flag = false;
            message="請選擇顯示方式"
        }
     
        if (flag) {
            var obj = new Object();
            var terms = new Array();
            $("input[name=terms]:checked").each(function () {
                terms.push($(this).val());
            });
            obj.CarNo = CarNo;
            obj.StationID = StationID;
            obj.ShowType = ShowType;
            obj.Terms = terms;
            var json = JSON.stringify(obj);
            $("#queryData").val(json);
            $("#frmCarDashBoard").submit();
        } else {
            disabledLoadingAndShowAlert(message);
        }
        return false;
    })


});
function SendMotoCmd(CID, deviceToken, Action) {
    ShowLoading("發送命令…"); 
    var Account = $("#Account").val();
    var obj = new Object();
    obj.UserId = Account;
    obj.CmdType = Action;
    obj.deviceToken = deviceToken;
    obj.CID = CID;

    var json = JSON.stringify(obj);
    console.log(json);
    var site = jsHost + "SendMotorCMD";
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

            if (data.Result == "1") {
                swal({
                    title: 'SUCCESS',
                    text: data.ErrorMessage,
                    icon: 'success'
                }).then(function (value) {
                    window.location.reload();
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
                text: "發送機車命令發生錯誤",
                icon: 'error'
            });
        }

    });
}
function SendCarCmd(CID, deviceToken, Action, IsCens) {
    ShowLoading("發送命令…"); 
    var Account = $("#Account").val();
    var obj = new Object();
    obj.UserId = Account;
    obj.CmdType = Action;
    obj.deviceToken = deviceToken;
    obj.CID = CID;
    obj.IsCens = IsCens;

    var json = JSON.stringify(obj);
    console.log(json);
    var site = jsHost + "SendCarCMD";
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

            if (data.Result == "1") {
                swal({
                    title: 'SUCCESS',
                    text: data.ErrorMessage,
                    icon: 'success'
                }).then(function (value) {
                    window.location.reload();
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
                text: "發送汽車命令發生錯誤",
                icon: 'error'
            });
        }

    });
}