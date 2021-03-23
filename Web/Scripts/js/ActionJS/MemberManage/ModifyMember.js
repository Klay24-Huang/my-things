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
    var StartDate = new Date();
    var EndDate = new Date();
    StartDate.setDate(StartDate.getDate() -10);
    $('#idEndNo .form-check-input').click(function () {
        if ($('#StartDate').val() == '') {
            $("#StartDate").val($.format.date(StartDate, 'yyyy-MM-dd'));
        }
        if ($('#EndDate').val() == '') {
            $("#EndDate").val($.format.date(EndDate, 'yyyy-MM-dd'));
        }
        if ($('#AuditType').val() == '' || $('#AuditType').val() == '-1') {
            $('#AuditType').val('0');
        }
    });

    if ($("#IDNO").val() == "") {
        if ($('#StartDate').val() == '') {
            $("#StartDate").val($.format.date(StartDate, 'yyyy-MM-dd'));
        }
        if ($('#EndDate').val() == '') {
            $("#EndDate").val($.format.date(EndDate, 'yyyy-MM-dd'));
        }
        if ($('#AuditType').val() == '' || $('#AuditType').val() == '-1') {
            $('#AuditType').val('0');
        }
    }

    $('#IDNO').change(function () {
        if ($("#IDNO").val()!="") {
            $("#StartDate").val('');
            $("#EndDate").val('');
            $('#AuditType').val('-1');
            $('#AuditError').val('');
        } else {
            if ($('#StartDate').val() == '') {
                $("#StartDate").val($.format.date(StartDate, 'yyyy-MM-dd'));
            }
            if ($('#EndDate').val() == '') {
                $("#EndDate").val($.format.date(EndDate, 'yyyy-MM-dd'));
            }

        }
    });

    setPostbackValue();

});
function GoDetail(IDNO) {
    ShowLoading("資料讀取中…");
    $("#AuditIDNO").val(IDNO);
    $("#frmModifyMemberDetail").submit();
}