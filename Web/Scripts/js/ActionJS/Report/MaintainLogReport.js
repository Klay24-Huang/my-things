$(function () {
    $("#start").val(SD);
    $("#end").val(ED);
    $("#objStation").val(objStation);
    $("#status").val(status);
    $("#userID").val(UserID);
    $("#carid").val(CarNo);
    SetCar($("#CarNo"));
    SetStationNoShowName($("#objStation"));
    var hasData = parseInt($("#len").val());
    if (hasData > 0) {
        $('.table').footable({
            "paging": {
                "enabled": true,
                "limit": 3,
                "size": 20
            }
        });
    }
    $("#btnSend").on("click", function () {
        ShowLoading("資料查詢中…");
        var flag = true;
        var errMsg = "";
        var SD = $("#SDate").val();
        var ED = $("#EDate").val();
        if (SD !== "" && ED !== "") {
            if (SD > ED) {
                flag = false;
                errMsg = "起始日期大於結束日期";
            } else {
                var GetDateDiff = DateDiff(SD, ED);
                if (GetDateDiff > 31) {
                    flag = false;
                    errMsg = "時間區間大於31天";
                }
            }
        } else {
            flag = false;
            errMsg = "未選擇日期";
        }
        if (flag) {
            disabledLoading();
            $("#formMaintainLogReport").submit();
        } else {
            disabledLoadingAndShowAlert(errMsg);
            //ShowFailMessage(errMsg);
        }
    });
    $("#btnDownload").on("click", function () {

        $("#formMaintainLogReportDownload").submit();
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
function showPic(fileName, obj) {
    //  var img = $("#" + obj);
    $("#ReviewImg").attr("src", fileName);
    $("#surrounding_modal").modal();
    //   img.attr("src", fileName);

}