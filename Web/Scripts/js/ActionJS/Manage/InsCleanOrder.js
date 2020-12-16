$(document).ready(function () {
    SetCar($("#CarNo"))

    $("#btnSend").on("click", function () {
        ShowLoading("資料處理中…");
        var manager = $("#manager").val();
        var CarNo = $("#CarNo").val();
        var flag = true;
        var errMsg = "";
        var ED = $("#EndDate").val();

        var SD = $("#StartDate").val();

   
            if (CarNo === "") {
                errMsg = "未選擇要清潔的車輛";
                flag = false;
            }
        if (flag) {
            if (SD == "") {
                errMsg = "未選擇起始時間";
                flag = false;
            }
        }
        if (flag) {
            if (ED == "") {
                errMsg = "未選擇結束時間";
                flag = false;
            }
        }
        if (flag) {
            var endTime = new Date(ED + ":00");
            var startTime = new Date(SD + ":00");
            if (startTime >= endTime) {
                flag = false;
                errMsg = "開始時間不能大於或等於結束時間";
            }
            if (flag) {
                if (new Date() >= startTime) {
                    flag = false;
                    errMsg = "開始時間必需大於現在時間";
                }
            }
        }
        if (flag) {
            if (manager === "") {
                flag = false;
                errMsg = "未輸入管理者帳號";
            }
        }
        if (flag) {
            var SendObj = new Object();
            SendObj.manager = manager;

            SendObj.CarNo = CarNo;
            SendObj.ED = ED.replace(/-/g, "").replace(/:/g,"")  + "00";
            SendObj.SD = SD.replace(/-/g, "").replace(/:/g,"") + "00";
          
            DoAjaxAfterReload(SendObj, "BE_InsCleanOrder", "新增保修清潔預約發生錯誤");
        } else {
            disabledLoadingAndShowAlert(errMsg);
        }
    });
});