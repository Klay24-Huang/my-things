$(document).ready(function () {

    $("#exampleDownload").on("click", function () {
        window.location.href = "../Content/example/ImportCarExample.xlsx";
    });
    if (errorLine == "ok") {
        ShowSuccessMessage("匯入成功");
    } else {
        if (errorMsg != "") {
            ShowFailMessage(errorMsg);
        }
    }
    $("#btnSend").on("click", function () {
        ShowLoading("資料匯入中");
        var flag = true;

        $("#frmImportCarData").submit();


    });

});