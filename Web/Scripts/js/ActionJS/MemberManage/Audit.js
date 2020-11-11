$(function () {
    var hasData = parseInt(ResultDataLen);
    if (hasData > 0) {
        $('.table').footable({
            "paging": {
                "enabled": true,
                "limit": 3,
                "size": 20
            }
        });
    }

    setPostbackValue();
});
function GoDetail(IDNO) {
    ShowLoading("資料讀取車…");
    $("#AuditIDNO").val(IDNO);
    $("#frmAuditDetail").submit();
}