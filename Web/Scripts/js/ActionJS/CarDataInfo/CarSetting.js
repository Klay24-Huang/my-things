var CarNo = "";
var StationID = "";

$(function () {
    SetStationNoShowName($("#StationID"));
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
    console.log(hasData);
    if (hasData > 0) {
        $('.table').footable({
            "paging": {
                "limit": 3,
                "size": 20
            }
        });
    }
});

function DoReset(Id) {
    if (CarNo !="") {
        CarNo ="";
        $("#Station_" + Id).val(StationID).hide();

    } else {
        $("#Station_" + Id).hide();

    }

    $("#btnReset_" + Id).hide();
    $("#btnSave_" + Id).hide();
    $("#btnEdit_" + Id).show();

}
function DoEdit(Id) {
    if (CarNo !="") {
        //先還原前一個
        $("#Station_" + CarNo).val(StationID).hide();

        $("#btnReset_" + CarNo).hide();
        $("#btnSave_" + CarNo).hide();
        $("#btnEdit_" + CarNo).show();
    }
    //再開啟下一個

    CarNo = Id;
    StationID = $("#Station_" + Id).val();
    SetStationNoShowName($("#Station_" + Id));

    $("#Station_" + Id).show();


    $("#btnReset_" + Id).show();
    $("#btnSave_" + Id).show();
    $("#btnEdit_" + Id).hide();

}
function DoSave(Id) {
    ShowLoading("資料處理中");
    var Account = $("#Account").val();
    var SStationID = $("#Station_" + Id).val();

    var flag = true;
    var errMsg = "";
    var checkList = [SStationID];
    var errMsgList = ["保有據點代碼未填"];
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
        obj.StationId = SStationID;
        obj.CarNo = CarNo;

        var json = JSON.stringify(obj);
        console.log(json);
        var site = jsHost + "BE_HandleCarSetting";
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
                    text: "修改車輛資料時發生錯誤",
                    icon: 'error'
                });
            }

        });
    } else {
        disabledLoadingAndShowAlert(errMsg);
    }
}