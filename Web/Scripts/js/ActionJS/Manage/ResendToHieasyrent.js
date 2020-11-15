$(function () {
    $("#btnSend").on("click", function () {
        ShowLoading("資料處理中");
        var OrderNo = $("#OrderNo").val();
        var kind = $("input[name=DoFunc]:checked").val();

        var flag = true;
        var errMsg = "";
        if (OrderNo == "") {
            flag = false;
            errMsg = "訂單編號未填";
        }
        if (flag) {
            if (!CheckStorageIsNull(kind)) {
                flag = false;
                errMsg = "未選擇執行功能";
            }
        }

        if (flag) {
            var Account = $("#Account").val();
            var obj = new Object();
            obj.UserID = Account;
            obj.OrderNo = OrderNo.toUpperCase();
            obj.type = parseInt(kind);
            //obj.Mode = parseInt(mode);
            //obj.returnDate = ReturnDate;
            var json = JSON.stringify(obj);
            console.log(json);
            DoAjaxAfterReload(obj, "BE_HiEasyRentRetry", "執行短租補傳發生錯誤");
        } else {
            disabledLoadingAndShowAlert(errMsg);
        }

    })
});