$(function () {
    $("#Mode").on("change", function () {
        var Mode = $("#Mode").val();
        switch (Mode) {
            case "0":
                $("#Date").show();
                $("#Phone").show();
                $("#Import").hide();
                $("#btnExplode").show();
                $("#btnQuery").show();
                $("#btnSend").hide();
                $("#btnImport").hide();
                break;
            case "1":
                $("#Date").hide();
                $("#Phone").show();
                $("#Import").hide();
                $("#btnExplode").hide();
                $("#btnQuery").hide();
                $("#btnSend").show();
                $("#btnImport").hide();
                break;
            case "2":
                $("#Date").hide();
                $("#Phone").hide();
                $("#Import").show();
                $("#btnExplode").hide();
                $("#btnQuery").hide();
                $("#btnSend").hide();
                $("#btnImport").show();
                break;
        }
    });

    $("#btnQuery").on("click", function () {
        ShowLoading("資料查詢中…");
        var flag = true;
        var errMsg = "";
        var SD = $("#StartDate").val();
        var ED = $("#EndDate").val();

        if ((SD == "" || ED == "") && $("#MobilePhone").val() == "") {
            flag = false;
            errMsg = "請選擇日期或手機號碼2";
        }

        if (SD !== "" && ED !== "") {
            if (SD > ED) {
                flag = false;
                errMsg = "起始日期大於結束日期";
            }
            //else {
            //    var GetDateDiff = DateDiff(SD, ED);
            //    if (GetDateDiff > 31) {
            //        flag = false;
            //        errMsg = "時間區間不可大於31天";
            //    }
            //}
        }

        if (flag) {
            $("#frmBlackList").submit();
            disabledLoading();
        } else {
            disabledLoadingAndShowAlert(errMsg);
            $('.datePicker').val('');
        }
    });

    $("#btnSend").on("click", function () {
        ShowLoading("資料新增中…");
        var flag = true;
        var errMsg = "";

        if ($("#MobilePhone").val() == "") {
            flag = false;
            errMsg = "請輸入手機號碼";
        }

        if (flag) {
            disabledLoading();
            $("#frmBlackList").submit();
        } else {
            disabledLoadingAndShowAlert(errMsg);
        }
    });

    $("#btnExplode").on("click", function () {
        ShowLoading("資料匯出中…");
        var flag = true;
        var errMsg = "";
        var SD = $("#StartDate").val();
        var ED = $("#EndDate").val();

        if ((SD == "" || ED == "") && $("#MobilePhone").val()=="") {
            flag = false;
            errMsg = "請選擇日期或手機號碼";
        }

        if (SD !== "" && ED !== "") {
            if (SD > ED) {
                flag = false;
                errMsg = "起始日期大於結束日期";
            }
            //else {
            //    var GetDateDiff = DateDiff(SD, ED);
            //    if (GetDateDiff > 31) {
            //        flag = false;
            //        errMsg = "時間區間不可大於31天";
            //    }
            //}
        }

        if (flag) {
            $("#ExplodeSDate").val($("#StartDate").val());
            $("#ExplodeEDate").val($("#EndDate").val());
            $("#ExplodeMobilePhone").val($("#MobilePhone").val());
            disabledLoading();
            $("#frmBlackListExplode").submit();
        } else {
            disabledLoadingAndShowAlert(errMsg);
        }
    });

    $("#btnImport").on("click", function () {
        ShowLoading("資料匯入中");
        $("#frmBlackList").submit();
        disabledLoading();
    });

    $("#exampleDownload").on("click", function () {
        window.location.href = "../Content/example/BlackListImport.xlsx";
    });
})

var DateDiff = function (sDate1, sDate2) { // sDate1 和 sDate2 是 2016-06-18 格式
    var aDate, oDate1, oDate2, iDays
    aDate = sDate1.split("/")
    oDate1 = new Date(aDate[1] + '/' + aDate[2] + '/' + aDate[0]) // 轉換為 06/18/2016 格式
    aDate = sDate2.split("/")
    oDate2 = new Date(aDate[1] + '/' + aDate[2] + '/' + aDate[0])
    iDays = parseInt(Math.abs(oDate1 - oDate2) / 1000 / 60 / 60 / 24) // 把相差的毫秒數轉換為天數
    return iDays;
};

function getdetail(detail, Mobile) {
    console.log('b')
    $('#myTable tbody td').remove()

    const obj = $.grep(detail, function (n, i) { return n.Mobile === Mobile; });

    var pp = $.grep(detail, function (n, i) { return n.Mobile === Mobile; }).length

    var tableRef = document.getElementById('myTable').getElementsByTagName('tbody')[0];

    for (let index = 0; index < pp; index++) {
        if (obj[index].Valid == 'N') {
            tableRef.insertRow().innerHTML =
                "<td>" + obj[index].Mobile + "</td>" +
                "<td>" + obj[index].CreateDate + "</td>" +
                "<td>" + obj[index].A_SYSDT + "</td>" +
                "<td>" + obj[index].USERID + "</td>";
        }
        else {
            tableRef.insertRow().innerHTML =
                "<td>" + obj[index].Mobile + "</td>" +
                "<td>" + obj[index].CreateDate + "</td>" +
                "<td>" + "</td>" +
                "<td>" + obj[index].USERID + "</td>";
        }
    }
}

function getaccount(detail, Mobile) {
    $('#myTable2 tbody td').remove()

    const obj = $.grep(detail, function (n, i) {
        return n.MEMTEL === Mobile;
    });

    var pp = $.grep(detail, function (n, i) {
        return n.MEMTEL === Mobile;
    }).length

    var tableRef = document.getElementById('myTable2').getElementsByTagName('tbody')[0];

    for (let index = 0; index < pp; index++) {
        tableRef.insertRow().innerHTML =
            "<td>" + obj[index].MEMIDNO + "</td>" +
            "<td>" + obj[index].MEMCNAME + "</td>" +
            "<td>" + obj[index].MEMADDR + "</td>" +
            "<td>" + obj[index].MEMTEL + "</td>";
    }
}

function DoDel(Mobile) {
    var Account = $("#Account").val();
    var MOBILE = $("#UserId_" + Mobile).val();

    ShowLoading("資料處理中");

    var obj = new Object();
    obj.MOBILE = MOBILE;
    obj.UserID = Account;

    DoAjaxAfterReload(obj, "BE_DeleteBlackList", "刪除發生錯誤");
}