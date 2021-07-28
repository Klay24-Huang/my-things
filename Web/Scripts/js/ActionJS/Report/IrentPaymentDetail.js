﻿$(function () {
    $("#AuditMode").on("change", function () {
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
                break;
        }
    });

    $("#btnSubmitQ").on("click", function () {
        ShowLoading("資料查詢中…");
        var flag = true;
        var errMsg = "";
        Account = $("#Account").val();
        //if ($("#MEMACCOUNT").val() == "") {
        //    flag = false;
        //    errMsg = "請輸入會員帳號";
        //}
        var SendObj = new Object();
        var ReceiveObj = new Object();
        //var SPSD = $("#StartDate").val().replace(/\-/g, '');
        //var SPED = $("#EndDate").val().replace(/\-/g, '');
        //var SPSD2 = $("#StartDate2").val().replace(/\-/g, '');
        //var SPED2 = $("#EndDate2").val().replace(/\-/g, '');
        var MEMACCOUNT = $("#MEMACCOUNT").val();

        if (flag) {
            SendObj.MODE = 1;
            SendObj.SPSD = "";
            SendObj.SPED = "";
            SendObj.SPSD2 = "";
            SendObj.SPED2 = "";
            SendObj.SPSD3 = "";
            SendObj.SPED3 = "";
            SendObj.MEMACCOUNT = MEMACCOUNT;
            SendObj.UserID = Account;

            ReceiveObj = DoAjaxAfterGoBack_GG(SendObj, "BE_IrentPaymentDetail", "查詢發生錯誤");
            //console.log(ReceiveObj)
            //console.log(ReceiveObj.Data.Data)            
            aa(ReceiveObj.Data.Data);
            disabledLoading();
        } else {
            disabledLoadingAndShowAlert(errMsg);
        }
    });

    $("#btnSubmitE").on("click", function () {
        ShowLoading("資料查詢中…");
        Account = $("#Account").val();
        var flag = true;
        var errMsg = "";
        var SendObj = new Object();
        var ReceiveObj = new Object();
        var SPSD = $("#StartDate").val().replace(/\-/g, '');
        var SPED = $("#EndDate").val().replace(/\-/g, '');
        var SPSD2 = $("#StartDate2").val().replace(/\-/g, '');
        var SPED2 = $("#EndDate2").val().replace(/\-/g, '');
        var MEMACCOUNT = $("#MEMACCOUNT").val();
        if (SPSD !== "" && SPED !== "") {
            if (SPSD > SPED) {
                flag = false;
                errMsg = "起始日期不可大於結束日期";
            } else {
                var GetDateDiff = DateDiff(SPSD, SPED);
                if (GetDateDiff > 30) {
                    flag = false;
                    errMsg = "時間區間不可大於30天，撈太多資料有效能issue";
                }
            }
        }
        //else {
        //    flag = false;
        //    errMsg = "未選擇日期";
        //}
        if (SPSD2 !== "" && SPED2 !== "") {
            if (SPSD2 > SPED2) {
                flag = false;
                errMsg = "起始日期不可大於結束日期";
            } else {
                var GetDateDiff = DateDiff(SPSD2, SPED2);
                if (GetDateDiff > 30) {
                    flag = false;
                    errMsg = "時間區間不可大於30天，撈太多資料有效能issue";
                }
            }
        }
        //else {
        //    flag = false;
        //    errMsg = "未選擇日期";
        //}
        if (flag) {
            disabledLoading();
            SendObj.MODE = 2;
            SendObj.SPSD = SPSD;
            SendObj.SPED = SPED;
            SendObj.SPSD2 = SPSD2;
            SendObj.SPED2 = SPED2;
            SendObj.SPSD3 = "";
            SendObj.SPED3 = "";
            SendObj.MEMACCOUNT = MEMACCOUNT;
            SendObj.UserID = Account;

            ReceiveObj = DoAjaxAfterGoBack_GG(SendObj, "BE_IrentPaymentDetail", "查詢發生錯誤");
            tableToExcel(ReceiveObj.Data.Data);
        } else {
            disabledLoadingAndShowAlert(errMsg);
        }

    });
});

function aa(detail) {
    console.log(detail)
    //for (var i = 0; i< 100; i++) {
    //    document.getElementById("myTable").deleteRow(0);
    //}
    $('#myTable tbody td').remove()

    const obj = detail;
    //console.log(obj)

    //data:
    var pp = detail.length
    //console.log(pp)

    //get table body:
    var tableRef = document.getElementById('myTable').getElementsByTagName('tbody')[0];

    for (let index = 0; index < pp; index++) {
        //insert Row
        tableRef.insertRow().innerHTML =
            "<td>" + obj[index].CUSTID + "</td>" +
            "<td>" + obj[index].RENT_NO + "</td>" +
            "<td>" + obj[index].IRENT_NO + "</td>" +
            "<td>" + obj[index].MEMO + "</td>" +
            "<td>" + obj[index].ETAG_PARK + "</td>" +
            "<td>" + obj[index].TOTAMT + "</td>";
    }
}


function tableToExcel(detail) {
    //要匯出的json資料
    var jsonData = detail;
    //列標題，逗號隔開，每一個逗號就是隔開一個單元格
    let str = `客戶編號,預約編號,合約編號,付款說明,費用,已付金額,車號,罰單/停車單號,取車日,還車日,還車據點,iRent訂單編號,欠費金額\n`;
    //增加\t為了不讓表格顯示科學計數法或者其他格式
    for (let i = 0; i < jsonData.length; i++) {
        for (let item in jsonData[i]) {
            str += `${jsonData[i][item] + '\t'},`;
        }
        str += '\n';
    }
    //encodeURIComponent解決中文亂碼
    let uri = 'data:text/csv;charset=utf-8,\ufeff' + encodeURIComponent(str);
    //通過建立a標籤實現
    var link = document.createElement("a");
    link.href = uri;
    //對下載的檔案命名
    link.download = "json資料表.csv";
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
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