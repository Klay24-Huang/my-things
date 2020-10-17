var FID = 0;
var descript = "";
var star = "";
$(function () {

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


function DoReset(Id) {
    if (FID > 0) {
  
        $("#Star_" + Id).val(star).hide();
        $("#Descript_" + Id).val(descript).hide();
        descript = "";
        star = "";

    } else {
        $("#Star_" + Id).hide();
        $("#Descript_" + Id).hide();

    }

    $("#btnReset_" + Id).hide();
    $("#btnSave_" + Id).hide();
    $("#btnEdit_" + Id).show();
    $("#btnShow_" + Id).show();
    $("#btnSettingOnline_" + Id).show();

}
function DoEdit(Id) {
    if (FID >0) {
        //先還原前一個
        $("#Star_" + FID).val(star).hide();
        $("#Descript_" + FID).val(descript).hide();

        $("#btnReset_" + FID).hide();
        $("#btnSave_" + FID).hide();
        $("#btnEdit_" + FID).show();
        $("#btnShow_" + FID).show();
        $("#btnSettingOnline_" + FID).show();
    }
    //再開啟下一個

    FID = Id;
    star = $("#Star_" + Id).val();
    descript = $("#Descript_" + Id).val();


    $("#Star_" + Id).show();
    $("#Descript_" + Id).show();


    $("#btnReset_" + Id).show();
    $("#btnSave_" + Id).show();
    $("#btnEdit_" + Id).hide();
    $("#btnShow_" + Id).hide();
    $("#btnSettingOnline_" + Id).hide();

}

function DoSave(Id) {
    ShowLoading("資料處理中");
    var Account = $("#Account").val();
    var SStar = $("#Star_" + Id).val();
    var SDescript = $("#Descript_" + Id).val();
    var flag = true;
    var errMsg = "";
    if (SStar == "") {
        SMemo = "無";
    }
    if (flag) {
        var obj = new Object();
        obj.UserID = Account;
        obj.Star = SStar;
        obj.Descript = SDescript;
        obj.FID = Id;

        var json = JSON.stringify(obj);
        console.log(json);
        var site = jsHost + "BE_HandleFeedBackKind";
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
                    text: "修改回饋類別發生錯誤",
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


    var flag = true;
    var errMsg = "";

    if (flag) {
        var obj = new Object();
        obj.UserID = Account;
        obj.Online = Cmd;
        obj.FID = Id;

        var json = JSON.stringify(obj);
        console.log(json);
        var site = jsHost + "BE_HandleFeedBackOnline";
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
                    text: "設定回饋類別上/下線發生錯誤",
                    icon: 'error'
                });
            }

        });
    } else {
        disabledLoadingAndShowAlert(errMsg);
    }
}
