$(document).ready(function () {
    $("#start").datepicker();
    $("#end").datepicker();
    var CarList = $("#hidCar").val();
    CarautoComplete(CarList, "#objCar", 3);
    $("#btnSend").on("click", function () {
        var manager = $("#manager").val();
        var CarNo = $("#objCar").val();
        var flag = true;
        var errMsg = "";
        var ED = $("#end").val();
        var EH = $("#EH").val();
        var EM = $("#EM").val();
        var SD = $("#start").val();
        var SH = $("#SH").val();
        var SM = $("#SM").val();
        if (EH === "-1") {
            errMsg = "未選擇結束時間【時】 ";
            flag = false;
        }
        if (flag) {
            if (EM === "-1") {
                errMsg = "未選擇結束時間【分】";
                flag = false;
            }
        }
        if (flag) {
            if (ED === "") {
                errMsg = "未選擇結束日期";
                flag = false;
            }
        }
        if (flag) {
            if (SH === "-1") {
                errMsg = "未選擇開始時間【時】 ";
                flag = false;
            }
        }
      
        if (flag) {
            if (SM === "-1") {
                errMsg = "未選擇開始時間【分】";
                flag = false;
            }
        }
     
        if (flag) {
            if (SD === "") {
                errMsg = "未選擇開始日期";
                flag = false;
            }
        }
        if (flag) {
            if (CarNo === "-1") {
                errMsg = "未選擇要清潔的車輛";
                flag = false;
            }
        }
        if (flag) {
            var endTime = new Date(ED + ' ' + EH + ":" + EM + ":00");
            var startTime = new Date(SD + ' ' + SH + ":" +SM + ":00");
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
            blockUI();
            var SendObj = new Object();
            SendObj.manager = manager;

            SendObj.CarNo = CarNo;
            SendObj.ED = ED.replace(/\//g, "") + EH + EM + "00";
            SendObj.SD = SD.replace(/\//g, "") + SH + SM + "00";
SendObj.SpecCode = '1';
            console.log(SendObj);
            var jdata = JSON.stringify({ "para": SendObj });
            GetEncyptData(jdata, insCleanOrderComplete);
        } else {
            warningAlert(errMsg, false, 0, "");
        }
    });
});