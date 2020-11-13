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

    $('.btn-clear').click(function () {
        setTimeout(function () {
            $('form input').val('');
            $('.form-check-input').attr('checked', false);
        },200);
    });
    setTimeout(function () {
        if ($.trim($('AuditMode').val()) != '') {
            $('.btn-submit').click();
        }
    }, 500);
});
function GoDetail(IDNO) {
    ShowLoading("資料讀取車…");
    $("#AuditIDNO").val(IDNO);
    $("#frmAuditDetail").submit();
}