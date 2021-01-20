$(function () {
    var now = new Date();
    SetStationNoShowName($("#StationID"));
    SetCar($("#CarNo"))
    var SpecCode = $("#SpecCode").val();
    if (SpecCode != "") {
        $("#Mode").val(SpecCode);
    }
    if (errorLine == "ok") {

    } else {
        if (errorMsg != "") {
            ShowFailMessage(errorMsg);
        }
    }
    var hasData = parseInt($("#len").val());
    if (hasData > 0) {
        $('.table').footable({
            "paging": {
                "enabled": true,
                "limit": 3,
                "size": 20
            }
        });

    }


})
function DoCancel(OrderNo) {
    ShowLoading("執行取消…");
    var obj = new Object();
    var Account = $("#Account").val();
    obj.UserID = Account;
    obj.OrderNo = OrderNo;
    obj.type = 2;
    obj.Mode = 1;
  
    DoAjaxAfterReload(obj, "BE_ContactSetting", "執行取消發生錯誤");
}