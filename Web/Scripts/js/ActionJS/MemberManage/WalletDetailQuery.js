$(document).ready(function () {
    $(".sort-table").tablesorter();
});

var hasData = parseInt(historyDataLen);
if (hasData > 0) {
    $('.table').footable({
        "paging": {
            "enabled": true,
            "limit": 3,
            "size": 20
        }
    });
}

$("#queryBtn").on("click", function (e) {
    e.preventDefault();
    ShowLoading("資料查詢中");
    var IDNO = $("#IDNO").val();
    var ED = $("#ED").val();
    var SD = $("#SD").val();
    if (IDNO == "" && ED == "" && SD == false) {
        disabledLoadingAndShowAlert("請至少選擇一個查詢條件");
    } else {
        $("#export").val("false");
        $("#frmWalletDetailQuery").submit();
    }
});

$("#exportBtn").on("click", function (e) {
    e.preventDefault();
    var IDNO = $("#IDNO").val();
    var ED = $("#ED").val();
    var SD = $("#SD").val();
    if (IDNO == "" && ED == "" && SD == false) {
        disabledLoadingAndShowAlert("請至少選擇一個查詢條件");
    } else {
        $("#export").val("true");
        $("#frmWalletDetailQuery").submit();
    }
});

$("#Clear").on('click', function () {
    $("#IDNO").val("");
    $("#SD").val("");
    $("#ED").val("");
})