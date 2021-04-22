$(function () {

    $("#btnSubmit").on("click", function () {
        ShowLoading("資料查詢中…");
        var flag = true;
        var errMsg = "";
        var IDNO = $("#IDNO").val();

        if (IDNO == "") {
            flag = false;
            errMsg = "身分證未填";
        }

        if (flag) {
            disabledLoading();
            $("#frmEasyWallet").submit();
        } else {
            disabledLoadingAndShowAlert(errMsg);
        }

    });

    $("#btnSend").on("click", function () {
        ShowLoading("資料處理中…");
        Account = $("#Account").val();
        var flag = true;
        var errMsg = "";
        var SendObj = new Object();

        var refundAmount = $("#refundAmount").val();
        var refundReason = $("#refundReason").val();
        var orderNo = $("#orderNo").val();

        if (refundAmount == "") {
            flag = false;
            errMsg = "請輸入退款金額";
        }

        if (flag) {
            if (refundReason == "") {
                flag = false;
                errMsg = "請輸入退款原因";
            }
        }

        if (flag) {
            if (orderNo == "") {
                flag = false;
                errMsg = "請輸入orderNo";
            }
        }

        if (flag) {
            SendObj.refundAmount = refundAmount;
            SendObj.refundReason = refundReason;
            SendObj.orderNo = orderNo;
            SendObj.contractNo = "3202008260000001";
            SendObj.executorId = "irentadmin";

            DoAjaxAfterGoBack_EW(SendObj, "退款失敗");

        } else {
            disabledLoadingAndShowAlert(errMsg);
        }

    });

});