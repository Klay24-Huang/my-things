$(function () {
    $("#divReturnDate").hide();
    $("#type").on("change", function(){
        if ($(this).val() == "1") {
            $("#divReturnDate").show();
        } else {
            $("#divReturnDate").hide();
        }
    })
    $("#btnSend").on("click", function () {
        ShowLoading("資料處理中");
        var OrderNo = $("#OrderNo").val();
        var type = $("#type").val();
        var mode = $("#mode").val();
        var ReturnDate = $("#StartDate").val();
        var flag = true;
        var errMsg = "";
        if (OrderNo == "") {
            flag = false;
            errMsg = "訂單編號未填";
        }
        if (flag) {
            if (parseInt(type) < 0) {
                flag = false;
                errMsg = "動作類型未填";
            }
        }
        if (flag) {
            if (parseInt(mode) < 0) {
                flag = false;
                errMsg = "動作用途未填";
            }
        }
        if (flag) {
            if (type == "1" && ReturnDate == "") {
                flag = false;
                errMsg = "還車時間未填"
            } else {
                ReturnDate = ReturnDate + ":00";
            }
        }
        if (flag) {
            var Account = $("#Account").val();
            var obj = new Object();
            obj.UserID = Account;
            obj.OrderNo = OrderNo;
            obj.type = parseInt(type);
            obj.Mode = parseInt(mode);
            obj.returnDate = ReturnDate;
            var json = JSON.stringify(obj);
            console.log(json);
            DoAjaxAfterReload(obj, "BE_ContactSetting", "執行強取強還發生錯誤");
        } else {
            disabledLoadingAndShowAlert(errMsg);
        }
        
    })
});