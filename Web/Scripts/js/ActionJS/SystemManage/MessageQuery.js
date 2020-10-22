$(function () {
    var hasData = parseInt($("#len").val());
    var hasRead = parseInt($("#ReadLen").val());
    var CarEventLen = parseInt($("#CarEventLen").val());
    var CardSettingLen = parseInt($("#CardSettingLen").val());
    if (CarEventLen > 0 || hasRead > 0 || CardSettingLen>0) {
        $('.table').footable({
            "paging": {
                "enabled": true,
                "limit": 3,
                "size": 20
            }
        });
    }
    SetCar($("#CarNo"))
    if (errorLine == "ok") {
    
    } else {
        if (errorMsg != "") {
            ShowFailMessage(errorMsg);
        }
    }
    $("#frmMessageQuery").on("submit", function () {
        ShowLoading("查詢中....");
        var flag = true;
        var errMsg = "";
        var ShowType = $("input[name=ShowType]").val();
        console.log(ShowType);
        
        if (false == CheckStorageIsNull(ShowType)) {
            flag = false;
            errMsg = "請選擇顯示方式";
          
        } else {
            var OrderNo = $("#OrderNo").val()
            var CarNo = $("#CarNo").val();
            var SendDate = $("#SendDate").val();
            if (OrderNo == "") {
                if (CarNo != "" && SendDate == "") {
                    flag = false;
                    errMsg = "請選擇發送日期";
                } else if (CarNo == "" && SendDate != "") {
                    flag = false;
                    errMsg = "請選擇輸入車號";
                } else if (CarNo == "" && SendDate == "") {
                    flag = false;
                    errMsg = "請選擇輸入訂單編號或(車號+發送日期)";
                }
            }
        
        }
        if (flag) {
            return true;
        } else {
            disabledLoadingAndShowAlert(errMsg);
            return false;
        }
    });
});