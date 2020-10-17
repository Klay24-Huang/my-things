var NowEditID = 0;
var CID = "";

$(document).ready(function () {

    $("#exampleDownload").on("click", function () {
        window.location.href = "../Content/example/CarBindExample.xlsx";
    });


    var hasData = parseInt($("#len").val());
    console.log(hasData);
    if (hasData > 0) {
        $('.table').footable({
            "paging": {
                "limit": 3,
                "size": 20
            }
        });
    }
    if (Mode == "Add") {
        if (errorLine == "ok") {
            ShowSuccessMessage("匯入成功");
        } else {
            if (errorMsg != "") {
                ShowFailMessage(errMsg);
            }
        }
    }

});
function DoReset(Id) {
    if (NowEditID > 0) {
        NowEditID = 0;
        $("#ParkingName_" + Id).val(ParkingName).hide();
        $("#ParkingAddress_" + Id).val(ParkingAddress).hide();
        $("#Latitude_" + Id).val(Latitude).hide();
        $("#Longitude_" + Id).val(Longitude).hide();
        $("#OpenTime_" + Id).val(OpenTime).hide();
        $("#CloseTime_" + Id).val(CloseTime).hide();
    } else {
        $("#ParkingName_" + Id).hide();
        $("#ParkingAddress_" + Id).hide();
        $("#Latitude_" + Id).hide();
        $("#Longitude_" + Id).hide();
        $("#OpenTime_" + Id).hide();
        $("#CloseTime_" + Id).hide();
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
        $("#btnReset_" + NowEditID).hide();
        $("#btnSave_" + NowEditID).hide();
        $("#btnEdit_" + NowEditID).show();
    }
    //再開啟下一個
    /*    NowEditID = Id;
        ParkingName = $("#ParkingName_" + Id).val();
        ParkingAddress = $("#ParkingAddress_" + Id).val();
        Latitude = $("#Latitude_" + Id).val();
        Longitude = $("#Longitude_" + Id).val();
        OpenTime = $("#OpenTime_" + Id).val();
        CloseTime = $("#CloseTime_" + Id).val();
    } else {*/
    NowEditID = Id;
    ParkingName = $("#ParkingName_" + Id).val();
    ParkingAddress = $("#ParkingAddress_" + Id).val();
    Latitude = $("#Latitude_" + Id).val();
    Longitude = $("#Longitude_" + Id).val();
    OpenTime = $("#OpenTime_" + Id).val();
    CloseTime = $("#CloseTime_" + Id).val();
    //  }
    $("#ParkingName_" + Id).show();
    $("#ParkingAddress_" + Id).show();
    $("#Latitude_" + Id).show();
    $("#Longitude_" + Id).show();
    $("#OpenTime_" + Id).show();
    $("#CloseTime_" + Id).show();

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
    var flag = true;
    var errMsg = "";
    var checkList = [SParkingName, SParkingAddress, SOpenTime, SCloseTime, SLatitude, SLongitude];
    var errMsgList = ["停車場名稱未填", "停車場地址未填", "開放日期(起)未填", "開放日期(迄)未填", "緯度未填", "經度未填"];
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
        var json = JSON.stringify(obj);
        console.log(json);
        var site = jsHost + "BE_HandleTransParking";
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
                    text: "修改停車場發生錯誤",
                    icon: 'error'
                });
            }

        });
    } else {
        disabledLoadingAndShowAlert(errMsg);
    }
}