$(function () {
    if (ResultLen > 0) {
        $('.table').footable({
            "paging": {
                "enabled": true,
                "limit": 3,
                "size": 20
            }
        });
    }
    if (errorLine == "ok") {

    } else {
        if (errorMsg != "") {
            ShowFailMessage(errorMsg);
        }
    }
    $("#frmMotorBatteryStatusQuery").on("submit", function () {
        return checkQueryCondition();
    });
    $('.btn-clear').click(function () {
        setTimeout(function () {
            $('#CarNo').val('');
            $('.datePicker').val('');
        }, 100);
    });
    $("#btnExplode").on("click", function () {
        if (checkQueryCondition()) {
            $("#ExplodeCarNo").val($("#CarNo").val());
            $("#ExplodeSendDate").val($("#SendDate").val());
            disabledLoading();
            $("#frmMotorBatteryStatusExplode").submit();
        }
    });

    var checkQueryCondition = function () {
        ShowLoading("查詢中....");
        var flag = true;
        var errMsg = "";
        var CarNo = $("#CarNo").val();
        var SendDate = $("#SendDate").val();
        if (CarNo == "") {
            flag = false;
            errMsg = "請選擇輸入車號";
        } else if (SendDate == "") {
            flag = false;
            errMsg = "請選擇發送日期";
        }

        if (flag) {
            return true;
        } else {
            disabledLoadingAndShowAlert(errMsg);
            return false;
        }
    };
});