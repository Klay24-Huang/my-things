$(function () {
    $("#AuditMode").on("change", function () {
        var Mode = $("#AuditMode").val();
        switch (Mode) {
            case "0":
                $("#Choice").hide();
                $("#btnSubmit").show();
                $("#btnSubmit").text("查詢");
                $("#Import").hide();
                $("#ID").show();
                $("#btnSubmit2").hide();
                break;
            case "1":
                $("#Choice").show();
                $("#ID").show();
                $("#Import").hide();
                $("#btnSubmit").show();
                $("#btnSubmit").text("新增");
                $("#btnSubmit2").hide();
                break;
            case "2":
                $("#btnSubmit2").show();
                $("#btnSubmit").hide();
                $("#Import").show();
                $("#ID").hide();
                $("#Choice").hide();
                break;
        }
    });

    $("#btnSubmit").on("click", function () {
        ShowLoading("資料查詢中…");
        var flag = true;

        if ($("#IDNO").val() == "") {
            flag = false;
            errMsg = "ID沒填";
        }

        if (flag) {
            $("#frmMedalMileStone").submit();
            disabledLoading();

            //要花太多時間測試，先跳過
            //var Mode = $("#AuditMode").val();
            //if (Mode == "1") {
            //    var SendObj = new Object();
            //    SendObj.IDNO = $("#IDNO").val();
            //    SendObj.TYPE = $("#ChoiceSelect").val();
            //    var json = JSON.stringify(SendObj);
            //    $.ajax({
            //        url: '@(Url.Action("MedalMileStone", "MemberManage"))',
            //        type: "POST",
            //        data: json,
            //        cache: false,
            //        contentType: 'application/json',
            //        dataType: "json",
            //        success: function (data) {
            //            if (data.Result == "1") {
            //                swal({
            //                    title: 'SUCCESS',
            //                    text: data.ErrorMessage,
            //                    icon: 'success'
            //                }).then(function (value) {
            //                    history.back();
            //                });
            //            } else {

            //                swal({
            //                    title: 'Fail1',
            //                    text: data.ErrorMessage,
            //                    icon: 'error'
            //                });
            //            }
            //        }
            //    });
            //}
            //else {
            //    $("#frmMedalMileStone").submit();
            //    disabledLoading();
            //}

            //沒有效
            //setTimeout回調函數，等2秒再做裡面內容
            //setTimeout(function () {
            //    $("#frmMedalMileStone").submit();
            //    disabledLoading();
            //}, 2000); 

            //var Mode = $("#AuditMode").val();
            //if (Mode == "1") {
            //    if (errorLine == "ok") {
            //        ShowSuccessMessage("新增成功");
            //    } else if (errorLine == "fall") {
            //        ShowFailMessage(errorLine);
            //    }
            //}

        } else {
            disabledLoadingAndShowAlert(errMsg);
        }

    });

    $("#exampleDownload").on("click", function () {
        window.location.href = "../Content/example/MileStoneExample.xlsx";
    });

    $("#btnSubmit2").on("click", function () {
        ShowLoading("資料匯入中");
        $("#frmMedalMileStone").submit();
        disabledLoading();
    });
    $("#fileImport").on("change", function () {
        var file = this.files[0];
        var fileName = file.name;
        var ext = GetFileExtends(fileName);
        var extName = "";
        if (CheckStorageIsNull(ext)) {
            extName = ext[0];
            console.log(extName.toUpperCase())
        }
        if (extName.toUpperCase() != "XLSX" && extName.toUpperCase() != "XLS") {

            swal({
                title: 'Fail',
                text: "僅允許匯入xlsx或xls格式",
                icon: 'error'
            }).then(function (value) {

                $("#fileImport").val("");
            });
        }
    })
    //$("#alert").on("click", function () {
    //    console.log("ssssss")
    //    disabledLoadingAndShowAlert("ssssss");
    //});
});

//function ShowMessage(message) {
//    console.log(message)
//    disabledLoadingAndShowAlert(message);
//}
