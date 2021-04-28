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

    $("#btnSend_2").on("click", function () {
        ShowLoading("資料處理中…");
        Account = $("#Account").val();
        var flag = true;
        var errMsg = "";
        var SendObj = new Object();

        var refundAmount = $("#refundAmount").val();
        var NAME = $("#NAME").val();
        var merchantOrderNo = $("#merchantOrderNo").val();
        var GIFTNAME = $("#GIFTNAME").val();
        var PBATCHNO = $("#PBATCHNO").val();

        if (refundAmount == "") {
            flag = false;
            errMsg = "請輸入退款金額";
        }

        if (flag) {
            if (NAME == "") {
                flag = false;
                errMsg = "請輸入客人名字";
            }
        }

        if (flag) {
            if (merchantOrderNo == "") {
                flag = false;
                errMsg = "請輸入merchantOrderNo";
            }
        }

        if (flag) {
            if (GIFTNAME == "") {
                flag = false;
                errMsg = "請輸入點數方案名稱";
            }
        }

        if (flag) {
            if (PBATCHNO == "") {
                flag = false;
                errMsg = "請輸入PBATCHNO";
            }
        }

        if (flag) {
            if ($("#IDNO").val() == "") {
                flag = false;
                errMsg = "請輸入身份證";
            }
        }

        if (flag) {
            SendObj.refundAmount = refundAmount;
            SendObj.NAME = NAME;
            SendObj.merchantOrderNo = merchantOrderNo;
            SendObj.GIFTNAME = GIFTNAME;
            SendObj.IDNO = $("#IDNO").val();
            SendObj.PBATCHNO = $("#PBATCHNO").val();

            DoAjaxAfterGoBack_EW_2(SendObj, "退款2失敗");

        } else {
            disabledLoadingAndShowAlert(errMsg);
        }

    });

});