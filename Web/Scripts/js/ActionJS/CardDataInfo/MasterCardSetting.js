$(document).ready(function () {

    $("#exampleDownload").on("click", function () {
        window.location.href = "../Content/example/MasterCardExample.xlsx";
    });
    $("#btnUpload").on("click", function () {
        $("#Mode").val("Add");
        $("#frmMasterSetting").submit();
    })
    $("#btnExplode").on("click", function () {
        $("#Mode").val("Explode");
        $("#frmMasterSetting").submit();
    });
    $("#btnSend").on("click", function () {
        $("#Mode").val("Query");
        console.log($("#Mode").val());
       $("#frmMasterSetting").submit();
    })
    var hasData = parseInt($("#len").val());
    var Mode = $("#Mode").val();
    console.log(hasData);
    if (hasData > 0) {
        ShowLoading("資料讀取中...");
        $('.table').footable();

    }

    if (Mode == "Add") {
        if (errorLine == "ok") {
            ShowSuccessMessage("匯入成功");
        } else {
            if (errorMsg != "") {
                ShowFailMessage(errorMsg);
            }
        }
    }
    disabledLoading();
});
function DeleteOne(ManagerId, CarNo, CardNo) {
    var Account = $("#Account").val();
    var obj = new Object();
    obj.UserID = Account;
    obj.ManagerId = ManagerId;
    obj.Mode = 0;
    obj.CarNo = CarNo;
    obj.CardNo = CardNo;
    DoSend(obj);
}
function DeleteAll(ManagerId, CarNo, CardNo) {
    var Account = $("#Account").val();
    var obj = new Object();
    obj.UserID = Account;
    obj.ManagerId = ManagerId;
    obj.Mode = 0;
    obj.CarNo = CarNo;
    obj.CardNo = CardNo;
    DoSend(obj);
}
function DoSend(obj) {
    ShowLoading("資料處理中");
    var Account = $("#Account").val();

    var flag = true;
    var errMsg = "";

    if (flag) {


        var json = JSON.stringify(obj);
        console.log(json);
        var site = jsHost + "BE_HandleMasterCard";
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
                    text: "刪除萬用卡發生錯誤",
                    icon: 'error'
                });
            }

        });
    } else {
        disabledLoadingAndShowAlert(errMsg);
    }
}