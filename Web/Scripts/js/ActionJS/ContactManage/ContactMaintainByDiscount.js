$(document).ready(function () {
    $("#panelResult").hide();
    $("#btnQuery").on("click", function () {
        var OrderNo = $("#OrderNo").val();

        var flag = true;
        var errMsg = "";
        ShowLoading("資料查詢中…");
        if (OrderNo == "") {
            flag = false;
            errMsg = "請輸入要修改的訂單編號，格式為H+7碼純數字";
        }
        if (flag) {
            var Account = $("#Account").val();
            var obj = new Object();
            obj.UserID = Account;
            obj.OrderNo = OrderNo;
            DoAjaxAfterCallBack(obj, "BE_GetOrderModifyInfo", "查詢資料發生錯誤", SetData);
        } else {
            disabledLoadingAndShowAlert(errMsg);
        }

    });
});
function SetData(data) {
    console.log(data);
}