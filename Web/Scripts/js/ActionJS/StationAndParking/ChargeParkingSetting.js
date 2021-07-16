var NowEditID = 0;
var ParkingName = "";
var ParkingAddress = "";
var Latitude = 0.0;
var Longitude = 0.0;
var OpenTime = "";
var CloseTime = "";
var SettingPrice = 0;
var Operator = "";

$(document).ready(function () {
    var hasData = parseInt($("#len").val());
    //console.log(hasData);
    if (hasData > 0) {
        $('.table').footable({
            "paging": {
                "enabled": true,
                "limit": 3,
                "size": 20
            }
        });
    }

    //20210511唐加
    $("#btnExplode").on("click", function () {
        ShowLoading("資料查詢中…");

        $("#ExplodeParkingName").val($("#ParkingName").val());
        disabledLoading();
        $("#frmChargeParkingSettingExplode").submit();
    });
})

//20210511唐加
function DoIn(Id) {
    var SParkingName = $("#ParkingName_" + Id).val();
    var SParkingAddress = $("#ParkingAddress_" + Id).val();
    var SLatitude = $("#Latitude_" + Id).val();
    var SLongitude = $("#Longitude_" + Id).val();
    var SOpenTime = $("#OpenTime_" + Id).val();
    var SCloseTime = $("#CloseTime_" + Id).val();
    var Id = $("#Id_" + Id).val();
    var obj = new Object();
    obj.ParkingName = SParkingName;
    obj.ParkingAddress = SParkingAddress;
    obj.Longitude = SLongitude;
    obj.Latitude = SLatitude;
    obj.OpenTime = SOpenTime + ":00";
    obj.CloseTime = SCloseTime + ":00";
    obj.Id = Id;
    var json = JSON.stringify(obj);
    //console.log(json);
    var site = jsHost2 + "BE_InsertChargeParking";
    //console.log("site:" + site);
    $.ajax({
        url: site,
        type: 'POST',
        data: json,
        cache: false,
        contentType: 'application/json',
        dataType: 'json',
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
                text: "加入調度停車場發生錯誤",
                icon: 'error'
            });
        }
    });
}

function DoReset(Id) {
    if (NowEditID > 0) {
        NowEditID = 0;
        $("#ParkingName_" + Id).val(ParkingName).hide();
        $("#ParkingAddress_" + Id).val(ParkingAddress).hide();
        $("#Latitude_" + Id).val(Latitude).hide();
        $("#Longitude_" + Id).val(Longitude).hide();
        $("#OpenTime_" + Id).val(OpenTime).hide();
        $("#CloseTime_" + Id).val(CloseTime).hide();
        $("#Operator_" + Id).val(Operator).hide();
        $("#SettingPrice_"+Id).val(SettingPrice).hide();
    } else {
        $("#ParkingName_" + Id).hide();
        $("#ParkingAddress_" + Id).hide();
        $("#Latitude_" + Id).hide();
        $("#Longitude_" + Id).hide();
        $("#OpenTime_" + Id).hide();
        $("#CloseTime_" + Id).hide();
        $("#Operator_" + Id).hide();
        $("#SettingPrice_"+Id).hide();
    }

    $("#btnReset_" + Id).hide();
    $("#btnSave_" + Id).hide();
    $("#btnEdit_" + Id).show();
}

function DoEdit(Id) {
    if (NowEditID > 0) {
        //先還原前一個
        $("#ParkingName_" + NowEditID).val(ParkingName).hide();
        $("#ParkingAddress_" + NowEditID).val(ParkingAddress).hide();
        $("#Latitude_" + NowEditID).val(Latitude).hide();
        $("#Longitude_" + NowEditID).val(Longitude).hide();
        $("#OpenTime_" + NowEditID).val(OpenTime).hide();
        $("#CloseTime_" + NowEditID).val(CloseTime).hide();
        $("#Operator_" + NowEditID).val(Operator).hide();
        $("#SettingPrice_" + NowEditID).val(SettingPrice).hide();
        $("#btnReset_" + NowEditID).hide();
        $("#btnSave_" + NowEditID).hide();
        $("#btnEdit_" + NowEditID).show();
    }
    NowEditID = Id;
    ParkingName = $("#ParkingName_" + Id).val();
    ParkingAddress = $("#ParkingAddress_" + Id).val();
    Latitude = $("#Latitude_" + Id).val();
    Longitude = $("#Longitude_" + Id).val();
    OpenTime = $("#OpenTime_" + Id).val();
    CloseTime = $("#CloseTime_" + Id).val();
    Operator=$("#Operator_" + Id).val();
    SettingPrice=$("#SettingPrice_" + Id).val();

    $("#ParkingName_" + Id).show();
    $("#ParkingAddress_" + Id).show();
    $("#Latitude_" + Id).show();
    $("#Longitude_" + Id).show();
    $("#OpenTime_" + Id).show();
    $("#CloseTime_" + Id).show();
    $("#Operator_" + Id).show();
    $("#SettingPrice_" + Id).show();
    $("#btnReset_" + Id).show();
    $("#btnSave_" + Id).show();
    $("#btnEdit_" + Id).hide();
}

function DoSave(Id) {
    ShowLoading("資料處理中");
    var Account = $("#Account").val();
    var SParkingName = $("#ParkingName_" + Id).val();
    var SParkingAddress = $("#ParkingAddress_" + Id).val();
    var SLatitude = $("#Latitude_" + Id).val();
    var SLongitude = $("#Longitude_" + Id).val();
    var SOpenTime = $("#OpenTime_" + Id).val();
    var SCloseTime = $("#CloseTime_" + Id).val();
    var SOperator = $("#Operator_" + Id).val();
    var SSettingPrice = $("#SettingPrice_" + Id).val();
    var flag = true;
    var errMsg = "";
    var checkList = [SParkingName, SParkingAddress, SOpenTime, SCloseTime, SLatitude, SLongitude, SSettingPrice, SOperator];
    var errMsgList = ["停車場名稱未填", "停車場地址未填", "開放日期(起)未填", "開放日期(迄)未填", "緯度未填", "經度未填","收費金額未填","營運商未填"];
    for (var i = 0; i < checkList.length; i++) {
        if (checkList[i] == "") {
            errMsg = errMsgList[i];
            flag = false;
            break;
        }
    }
    if (flag) {
        var obj = new Object();
        obj.UserID = Account;
        obj.ParkingID = Id;
        obj.ParkingName = SParkingName;
        obj.ParkingAddress = SParkingAddress;
        obj.Longitude = SLongitude;
        obj.Latitude = SLatitude;
        obj.OpenTime = SOpenTime + ":00";
        obj.CloseTime = SCloseTime + ":00";
        obj.Price = SSettingPrice;
        obj.Operator = SOperator;
        var json = JSON.stringify(obj);
        //console.log(json);
        var site = jsHost + "BE_HandleChargeParking";
        //console.log("site:" + site);
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
                    text: "修改停車便利付停車場發生錯誤",
                    icon: 'error'
                });
            }
        });
    } else {
        disabledLoadingAndShowAlert(errMsg);
    }
}