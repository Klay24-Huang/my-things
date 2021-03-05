$(function () {
    //html type="reset"還是不會清除，所以多加這個
    $('.btn-clear').click(function () {
        setTimeout(function () {
            $('.datePicker').val('');
            $('#AuditMode').val(0); //若設定''會產生奇怪的格式轉換問題，什麼null能怎樣啊....，我就改設int
            $('#idEndNo').val('');
        }, 100);
    });

    $("#btnExplode").on("click", function () {
        ShowLoading("資料查詢中…");
        //console.log('Q1' + $("#idEndNo .form-check-input").val());
        //console.log('Q2' + $("#IDNO .form-check-input").val());
        //console.log('Q3' + document.getElementById("idEndNo").innerText);
        //console.log('Q4' + $("#idEndNo").html());
        //console.log('Q5' + document.getElementById("IDNO3").innerText);
        //console.log('Q6' + $("#IDNO3").html());
        //console.log('Q7' + document.getElementById("IDNO4").innerText);
        //console.log('Q8' + $("#IDNO4").html());
        //console.log('Q9' + $("#IDNO3 .form-check-input").val());
        //console.log('Q10' + $("#IDNO4 .form-check-input").val());
        var flag = true;
        var errMsg = "";
        var SD = $("#StartDate").val();
        var ED = $("#EndDate").val();
        if (SD !== "" && ED !== "") {
            if (SD > ED) {
                flag = false;
                errMsg = "起始日期大於結束日期，這是什麼神邏輯";
            } else {
                var GetDateDiff = DateDiff(SD, ED);
                if (GetDateDiff > 2) {
                    flag = false;
                    errMsg = "時間區間不可大於2天，撈太多資料很累耶";
                }
            }
        } else {
            flag = false;
            errMsg = "未選擇日期";
        }

        if (flag) {
            //$("#ExplodeSDate").val($("#StartDate").val());
            //$("#ExplodeEDate").val($("#EndDate").val());
            //$("#ExplodeId").val($("#idEndNo").val());
            //$("#ExplodeAuditMode").val($("#AuditMode").val());
            disabledLoading();
            //$("#frmMemberDetailExplode").submit();
            $("#frmMemberDetail").submit();
        } else {
            disabledLoadingAndShowAlert(errMsg);
        }

    });
});

var DateDiff = function (sDate1, sDate2) { // sDate1 和 sDate2 是 2016-06-18 格式
    var aDate, oDate1, oDate2, iDays
    aDate = sDate1.split("/")
    oDate1 = new Date(aDate[1] + '/' + aDate[2] + '/' + aDate[0]) // 轉換為 06/18/2016 格式
    aDate = sDate2.split("/")
    oDate2 = new Date(aDate[1] + '/' + aDate[2] + '/' + aDate[0])
    iDays = parseInt(Math.abs(oDate1 - oDate2) / 1000 / 60 / 60 / 24) // 把相差的毫秒數轉換為天數
    return iDays;
};
