$(function () {
    if (errorLine == "ok") {

    } else {
        if (errorMsg != "") {
            ShowFailMessage(errorMsg);
        }
    }
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
    $("#frmArrearQuery").on("submit", function () {
        ShowLoading("欠費查詢中…")
        var IDNO = $("IDNO").val();
        if (IDNO == "") {
            disabledLoadingAndShowAlert("身份證未填");
            return false;
        } else {
            return true;
        }
        
       
    })
})