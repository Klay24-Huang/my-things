$(function () {
    var Mode = $("#AuditMode").val();
    switch (Mode) {
        case "1":
            $("#DATE").hide();
            $("#DATE2").hide();
            $("#btnSubmitE").hide();
            $("#btnSubmitQ").show();
            break;
        case "2":
            $("#DATE").show();
            $("#DATE2").show();
            $("#btnSubmitE").show();
            $("#btnSubmitQ").hide();
    }

    $("#btnSubmitQ").on("click", function () {
        ShowLoading("資料查詢中…");
        var flag = true;
        var errMsg = "";
        //if ($("#MEMACCOUNT").val() == "") {
        //    flag = false;
        //    errMsg = "請輸入會員帳號";
        //}
        Account = $("#Account").val();
        var SendObj = new Object();
        var ReceiveObj = new Object();
        var SPSD = $("#StartDate").val().replace(/\-/g, '');
        var SPED = $("#EndDate").val().replace(/\-/g, '');
        var SPSD2 = $("#StartDate2").val().replace(/\-/g, '');
        var SPED2 = $("#EndDate2").val().replace(/\-/g, '');
        var MEMACCOUNT = $("#MEMACCOUNT").val();

        if (flag) {
            SendObj.SPSD = SPSD;
            SendObj.SPED = SPED;
            SendObj.SPSD2 = SPSD2;
            SendObj.SPED2 = SPED2;
            SendObj.MEMACCOUNT = MEMACCOUNT;

            ReceiveObj = DoAjaxAfterGoBack_GG(SendObj, "BE_IrentPaymentDetail", "查詢發生錯誤");
            console.log("poi")
            console.log(ReceiveObj)
            //disabledLoading();
        } else {
            disabledLoadingAndShowAlert(errMsg);
        }
    });
});