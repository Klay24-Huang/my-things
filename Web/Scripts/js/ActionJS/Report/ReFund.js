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

    //$("#0bt").on("click", function () {
    //    //ShowLoading("資料查詢中…");
    //    $("#orderNo").val($("#0oNo").text())
    //    $("#NAME").val($("#0mNa").text())
    //    $("#merchantOrderNo").val($("#0mNo").text())
    //    $("#GIFTNAME").val($("#0pNa").text())
    //    $("#refundReason").val($("#0Am").text())
    //});
    //$("#1bt").on("click", function () {
    //    //ShowLoading("資料查詢中…");
    //    $("#orderNo").val($("#1oNo").text())
    //    $("#NAME").val($("#1mNa").text())
    //    $("#merchantOrderNo").val($("#1mNo").text())
    //    $("#GIFTNAME").val($("#1pNa").text())
    //    $("#refundReason").val($("#1Am").text())
    //});
    $("#btnSend").on("click", function () {
        ShowLoading("資料處理中…");
        Account = $("#Account").val();
        var flag = true;
        var errMsg = "";
        var SendObj = new Object();
        var SendObj2 = new Object();
        //悠遊付退款api
        var refundAmount = $("#refundAmount").val();
        var refundReason = $("#refundReason").val();
        var orderNo = $("#orderNo").val();
        //和雲退款
        var NAME = $("#NAME").val();
        var merchantOrderNo = $("#merchantOrderNo").val();
        var GIFTNAME = $("#GIFTNAME").val();

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
            if ($("#IDNO").val() == "") {
                flag = false;
                errMsg = "請輸入身份證";
            }
        }
        
        if (flag) {
            SendObj.refundAmount = refundAmount;
            SendObj.refundReason = refundReason;
            SendObj.orderNo = orderNo;
            //SendObj.contractNo = "3202008260000001";
            SendObj.contractNo = "3202104060000009";
            SendObj.executorId = $("#Account").val();

            DoAjaxAfterGoBack_EW(SendObj, "退款悠遊付失敗");

        } else {
            disabledLoadingAndShowAlert(errMsg);
        }

        if (flag) {
            SendObj2.refundAmount = refundAmount;
            SendObj2.NAME = NAME;
            SendObj2.merchantOrderNo = merchantOrderNo;
            SendObj2.GIFTNAME = GIFTNAME;
            SendObj2.IDNO = $("#IDNO").val();

            DoAjaxAfterGoBack_EW_2(SendObj2, "退款和雲失敗");

        } else {
            disabledLoadingAndShowAlert(errMsg);
        }

    });

    //20210511唐加
    $("#btnExplode").on("click", function () {
        ShowLoading("資料查詢中…");
        var flag = true;
        var errMsg = "";
        var SD = $("#StartDate").val();
        var ED = $("#EndDate").val();


        if (SD !== "" && ED !== "") {
            if (SD > ED) {
                flag = false;
                errMsg = "起始日期大於結束日期";
            }
            else {
                var GetDateDiff = DateDiff(SD, ED);
                if (GetDateDiff > 90) {
                    flag = false;
                    errMsg = "時間區間不可大於90天";
                }
            }
        } else {
            flag = false;
            errMsg = "未選擇日期";
        }
        if (flag) {
            $("#ExplodeSDate").val($("#StartDate").val());
            $("#ExplodeEDate").val($("#EndDate").val());
            disabledLoading();
            $("#frmReFundExplode").submit();
        } else {
            disabledLoadingAndShowAlert(errMsg);
        }
    });

    //放這邊會找不到
    //function DoLoad(i) {
    //    console.log("#bt_" + i)
    //    $("#bt_" + i).on("click", function () {
    //        //ShowLoading("資料查詢中…");
    //        $("#orderNo").val($("#oNo_" + i).text())
    //        $("#NAME").val($("#mNa_" + i).text())
    //        $("#merchantOrderNo").val($("#mNo_" + i).text())
    //        $("#GIFTNAME").val($("#pNa_" + i).text())
    //        $("#refundAmount").val($("#Am_" + i).text())
    //    });
    //}
});

function DoLoad(i) {
    console.log("#bt_" + i)
    $("#bt_" + i).on("click", function () {
        //ShowLoading("資料查詢中…");
        $("#orderNo").val($("#oNo_" + i).text())
        $("#NAME").val($("#mNa_" + i).text())
        $("#merchantOrderNo").val($("#mNo_" + i).text())
        $("#GIFTNAME").val($("#pNa_" + i).text())
        $("#refundAmount").val($("#Am_" + i).text())
    });
}


var DateDiff = function (sDate1, sDate2) { // sDate1 和 sDate2 是 2016-06-18 格式
    var aDate, oDate1, oDate2, iDays
    aDate = sDate1.split("/")
    oDate1 = new Date(aDate[1] + '/' + aDate[2] + '/' + aDate[0]) // 轉換為 06/18/2016 格式
    aDate = sDate2.split("/")
    oDate2 = new Date(aDate[1] + '/' + aDate[2] + '/' + aDate[0])
    iDays = parseInt(Math.abs(oDate1 - oDate2) / 1000 / 60 / 60 / 24) // 把相差的毫秒數轉換為天數
    return iDays;
};