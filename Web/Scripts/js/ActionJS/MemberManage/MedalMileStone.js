$(function () {

    $("#AuditMode").on("change", function () {
        var Mode = $("#AuditMode").val();
        switch (Mode) {
            case "0":
                $("#Choice").hide();
                $("#btnSubmit").text("查詢");
                break;
            case "1":
                $("#Choice").show();
                $("#btnSubmit").text("新增");
                break;
        }
    });

    $("#btnSubmit").on("click", function () {
        ShowLoading("資料查詢中…");
        var flag = true;
        //console.log('aaaaaa')
        if ($("#IDNO").val() == "") {
            flag = false;
            errMsg = "ID沒填";
        }

        if (flag) {
            disabledLoading();
            $("#frmMedalMileStone").submit();
            //console.log('ssssss')
        } else {
            disabledLoadingAndShowAlert(errMsg);
        }

        var Mode = $('#modeData').val();//$("#AuditMode").val();
        if (Mode == "Add") {
            if (errorLine == "ok") {
                ShowSuccessMessage("新增成功");
            } else {
                if (errorMsg != "") {
                    ShowFailMessage(errMsg);
                }
            }
        }
    });

});
