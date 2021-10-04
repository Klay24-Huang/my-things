var CarNo = "";
var Memo = "";
$(function () {
    SetStation($("#StationID"), $("#StationName"));
    SetCar($("#CarNo"))
    $("#btnSend").on("click", function () {
        if ($("#CarNo").val() == "" && $("#StationID").val() == "") {
            ShowFailMessage("請輸入要查詢的車號或據點");
            return false;
        } else {
            ShowLoading("查詢中....")
        }
    })
    var hasData = parseInt($("#len").val());
    if (hasData > 0) {
        $('.table').footable({
            "paging": {
                "limit": 3,
                "size": 20
            }
        });
    }
})

$(function () {
    SetStation($("#StationID"), $("#StationName"));
    SetCar($("#CarNo"))
    $("#isExport").on("click", function () {
        if ($("#CarNo").val() == "" && $("#StationID").val() == "") {
            ShowFailMessage("請輸入要查詢的車號或據點");
            return false;
        }
    })
})


//function DoReset(Id) {
//    if (CarNo != "") {
//        CarNo = "";
//        $("#Memo_" + Id).val(Memo).hide();

//    } else {
//        $("#Memo_" + Id).hide();

//    }

//    $("#btnReset_" + Id).hide();
//    $("#btnSave_" + Id).hide();
//    $("#btnEdit_" + Id).show();
//    $("#btnShow_" + Id).show();
//    $("#btnSettingOnline_" + Id).show();

//}
//function DoEdit(Id) {
//    if (CarNo != "") {
//        //先還原前一個
//        $("#Memo_" + CarNo).val(Memo).hide();

//        $("#btnReset_" + CarNo).hide();
//        $("#btnSave_" + CarNo).hide();
//        $("#btnEdit_" + CarNo).show();
//        $("#btnShow_" + CarNo).show();
//        $("#btnSettingOnline_" + CarNo).show();
//    }
//    //再開啟下一個

//    CarNo = Id;
//    Memo = $("#Memo_" + Id).val();


//    $("#Memo_" + Id).show();


//    $("#btnReset_" + Id).show();
//    $("#btnSave_" + Id).show();
//    $("#btnEdit_" + Id).hide();
//    $("#btnShow_" + Id).hide();
//    $("#btnSettingOnline_" + Id).hide();

//}

function EditMemo(Id) {
    //if ($("#Select_" + Id).val() == 4) {
    //} else {
    //    $("#Memo_" + Id).hide();
    //}
    $("#Memo_" + Id).show();
}

function CarOffline(Id) {
    $("#Select_" + Id).show();
    $("#Memo_" + Id).show();
    $("#Chk_btnSettingOnline_" + Id).show();
    $("#btnSettingOnline_" + Id).hide();
}

function DoSave(Id) {
    var Account = $("#Account").val();
    var SMemo = $("#Memo_" + Id).val();

    var flag = true;
    var errMsg = "";
    if (SMemo == "") {
        SMemo = "無";
    }
    if (flag) {
        var obj = new Object();
        obj.UserID = Account;
        obj.Memo = SMemo;
        obj.CarNo = Id;

        var json = JSON.stringify(obj);
        console.log(json);
        var site = jsHost + "BE_SetCarMemo";
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
                }
            },
            error: function () {
                swal({
                    title: 'Fail',
                    text: "修改備註發生錯誤",
                    icon: 'error'
                });
            }
        });
    } else {
        disabledLoadingAndShowAlert(errMsg);
    }
}
function SetOnline(Id, Cmd) {
    ShowLoading("資料處理中");
    var Account = $("#Account").val();
    var OffLineReason = "";

    switch ($("#Select_" + Id).val()) {
        case "1":
            OffLineReason = "事故維修"
            break;
        case "2":
            OffLineReason = "車況異常"
            break;
        case "3":
            OffLineReason = "保養驗車"
            break;
        default:
            OffLineReason = "其他"
    }

    if ($("#Select_" + Id).val() == "4") {
        if ($("#Memo_" + Id).val() == "" || $("#Memo_" + Id).val() == "無") {
            ShowFailMessage("備註欄不得空白")
            disabledLoading();
            return
        }
    }

    var flag = true;
    var errMsg = "";

    if (flag) {
        var obj = new Object();
        obj.UserID = Account;
        obj.Online = Cmd;
        obj.CarNo = Id;
        obj.OffLineReason = OffLineReason;

        var json = JSON.stringify(obj);
        console.log(json);
        var site = "http://localhost:2061/api/" + "BE_HandleCarOnline";
        //var site = jsHost + "BE_HandleCarOnline";
        console.log("site:" + site);
        $.ajax({
            url: site,
            type: 'POST',
            data: json,
            cache: false,
            contentType: 'application/json',
            dataType: 'json',           //'application/json',
            success: function (data) {
                //$.busyLoadFull("hide");

                if (data.Result == "1") {
                    DoSave(Id);
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
                    text: "設定車輛上/下線發生錯誤",
                    icon: 'error'
                });
            }

        });
    } else {
        disabledLoadingAndShowAlert(errMsg);
    }
}
function ShowDetail(CarNo) {
    $("#ShowCarNo").val(CarNo);
    $("#frmCarDetail").submit();
}