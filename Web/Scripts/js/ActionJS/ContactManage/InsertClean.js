$(document).ready(function () {
    SetCar($("#CarNo"));
    $("#btnReset").on("click", function () {
        $("#CarNo").val("");
        $("#SpecCode").val("-1");
        $("#BookingStart").val("");
        $("#BookingEnd").val("");
    });
    $("#btnSend").on("click", function () {
        ShowLoading("新增保修清潔合約…");
        var manager = $("#Account").val();
        var CarNo = $("#CarNo").val();
        var flag = true;
        var errMsg = "";
        var SD, SH, SM, ED, EH, EM;
        var SDate, EDate;
        var SpecCode="";

        console.log(SDate + "," + EDate);
        SD = $("#BookingStart").val().split(" ")[0];
        SH = ($("#BookingStart").val().split(" ")[1]).split(":")[0];
        SM = ($("#BookingStart").val().split(" ")[1]).split(":")[1];
        ED = $("#BookingEnd").val().split(" ")[0]; //EDate.getDate();
        EH = ($("#BookingEnd").val().split(" ")[1]).split(":")[0];
        EM = ($("#BookingEnd").val().split(" ")[1]).split(":")[1];
        SpecCode = $("#SpecCode").val();
        if (flag) {
            var endTime = $("#BookingEnd").val();// new Date(ED + ' ' + EH + ":" + EM + ":00");
            var startTime = $("#BookingStart").val();// new Date(SD + ' ' + SH + ":" + SM + ":00");
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
                errMsg = "Session失效，操作者帳號遺失";
            }
        }
        if (flag) {
            if (CarNo === "") {
                flag = false;
                errMsg = "未輸入車號"
            }
        }
        if (flag) {
            if (SpecCode == "-1") {
                flag = false;
                errMsg = "請選擇動作類型";
            }
        }
        if (flag) {
            var SendObj = new Object();
            SendObj.manager = manager;

            SendObj.CarNo = CarNo;
            SendObj.ED = ED.replace(/-/g, "") + EH + EM + "00";
            SendObj.SD = SD.replace(/-/g, "") + SH + SM + "00";
            SendObj.SpecCode = SpecCode;

            DoAjaxAfterReload(SendObj,"MA_InsertClean","新增保修清潔合約發生錯誤…")
        } else {
            //warningAlert(errMsg, false, 0, "");
            disabledLoadingAndShowAlert(errMsg)
        }
    });
});