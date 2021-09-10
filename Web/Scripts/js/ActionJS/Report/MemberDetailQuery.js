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

        var flag = true;
        var errMsg = "";
        var SD = $("#StartDate").val();
        var ED = $("#EndDate").val();
        if (SD !== "" && ED !== "") {
            if (SD > ED) {
                flag = false;
                errMsg = "起始日期不可大於結束日期";
            } else {
                var GetDateDiff = DateDiff(SD, ED);
                if (GetDateDiff > 30) {
                    flag = false;
                    errMsg = "時間區間不可大於30天，撈太多資料有效能issue";
                }
            }
        } else {
            flag = false;
            errMsg = "未選擇日期";
        }

        if (flag) {
            console.log($("#AuditMode").val())
            //$("#ExplodeSDate").val($("#StartDate").val());
            //$("#ExplodeEDate").val($("#EndDate").val());
            //$("#ExplodeId").val($("#idEndNo").val());
            //$("#ExplodeAuditMode").val($("#AuditMode").val());
            //$("#frmMemberDetailExplode").submit();
            $("#frmMemberDetail").submit();
            disabledLoading();
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
