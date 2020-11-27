
$(document).ready(function () {
    registerLabel('.form-control', '.form-group');
    $('[name="BookingStart"]').parents('.form-group').addClass('focused');
    $('[name="BookingEnd"]').parents('.form-group').addClass('focused');
    $('[name="CarNo"]').parents('.form-group').addClass('focused');
    //datepicker
    registerDatePickerSingleDateTime('.input-limit-datepickerTime');


    $("#btnSend").on("click", function () {
        var manager = $("#manager").val();
        var CarNo = $("#CarNo").val();
        var flag = true;
        var errMsg = "";
        var SD, SH, SM, ED, EH, EM;
        var SDate, EDate;
        //    SDate = Date.parse($("#BookingStart").val());
        //   EDate = Date.parse($("#BookingEnd").val());
        console.log(SDate + "," + EDate);
        SD = $("#BookingStart").val().split(" ")[0];
        SH = ($("#BookingStart").val().split(" ")[1]).split(":")[0];
        SM = ($("#BookingStart").val().split(" ")[1]).split(":")[1];
        ED = $("#BookingEnd").val().split(" ")[0]; //EDate.getDate();
        EH = ($("#BookingEnd").val().split(" ")[1]).split(":")[0];
        EM = ($("#BookingEnd").val().split(" ")[1]).split(":")[1];
        /* var ED = $("#end").val();
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
         }*/
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
                errMsg = "未輸入管理者帳號";
            }
        }
        if (flag) {
            blockUI();
            var SendObj = new Object();
            SendObj.manager = manager;

            SendObj.CarNo = CarNo;
            SendObj.ED = ED.replace(/-/g, "") + EH + EM + "00";
            SendObj.SD = SD.replace(/-/g, "") + SH + SM + "00";
            SendObj.SpecCode = '1';
            console.log(SendObj);
            var jdata = JSON.stringify({ "para": SendObj });
            GetEncyptData(jdata, insCleanOrderComplete);
        } else {
            warningAlert(errMsg, false, 0, "");
        }
    });
});
function registerLabel(objName, parents) {
    //'.form-control','.form-group'
    $(objName).focus(function () {
        $(this).parents(parents).addClass('focused');
    });
    $(objName).focusout(function () {
        if ($('.tag').text() == '')
            $(this).parents(parents).removeClass('focused');
    });
}
/***功能：註冊datapicker物件，無法選範圍 */
/***傳入參數：objName，物件名稱 */
function registerDatePickerSingleDateTime(objName) {
    $(objName).daterangepicker({
        singleDatePicker: true,
        autoUpdateInput: false,
        buttonClasses: ['btn', 'btn-sm'],
        applyClass: 'btn-danger',
        cancelClass: 'btn-inverse',
        showMeridian: 1,
        autoclose: true,
        timePicker: true,
        timePicker24Hour: true,
        language: 'zh-TW',
        dateLimit: {
            days: 99999
        },
        locale: {
            format: "YYYY-MM-DD HH:mm",
            applyLabel: "確定",
            cancelLabel: "取消",
            fromLabel: "開始日期",
            toLabel: "結束日期",
            customRangeLabel: "自訂日期區間",
            daysOfWeek: ["日", "一", "二", "三", "四", "五", "六"],
            monthNames: ["1月", "2月", "3月", "4月", "5月", "6月",
                "7月", "8月", "9月", "10月", "11月", "12月"],
            firstDay: 1,
            clear: "清除"
        }

    });

    setSingleDayTime(objName);
    setNullDay(objName);
}
function setSingleDayTime(objName) {
    $(objName).on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format('YYYY-MM-DD HH:mm')).parents('.form-group').addClass('focused');
    });
}
/***功能：預設日期 */
/***傳入參數：objName，物件名稱 */
function setNullDay(objName) {
    $(objName).on('cancel.daterangepicker', function (ev, picker) {
        $(this).val('').parents('.form-group').removeClass('focused');
    });
}
function blockUI() {
    console.log("i call");
    $.blockUI({
        message: $("#imgBusy"), centerX: true, centerY: true,
        css: { border: 'none', background: 'none', cursor: 'wait' },
        overlayCSS: { backgroundColor: '#FFFFFF', opacity: 0.5, cursor: 'wait' }
    });
}
function warningAlert(msg, flag, ImgType, site) {
    var errImg = "warning";
    $.unblockUI();
    if (ImgType == 1) {
        errImg = "success";
    }
    if (flag) {
        swal({
            title: "",
            text: msg,
            html: msg,
            type: errImg,
            confirmButtonColor: '#ff0000',
            confirmButtonText: '確定',
            closeOnConfirm: true
        }, function (isConfirm) { window.location.href = site; });
    } else {
        swal({
            title: "",
            text: msg,
            html: msg,
            type: errImg,
            confirmButtonColor: '#ff0000',
            confirmButtonText: '確定',
            closeOnConfirm: true
        }, function (isConfirm) { });
    }
}
function warningAlertSubMit(msg, type, ImgType, site) {
    var errImg = "warning";
    $.unblockUI();
    if (ImgType == 1) {
        errImg = "success";
    }
    if (1 == type) {
        swal({
            title: "",
            text: msg,
            html: msg,
            type: errImg,
            confirmButtonColor: '#ff0000',
            confirmButtonText: '確定',
            closeOnConfirm: true
        }, function (isConfirm) { window.location.href = site; });
    } else if (0 == type) {
        swal({
            title: "",
            text: msg,
            html: msg,
            type: errImg,
            confirmButtonColor: '#ff0000',
            confirmButtonText: '確定',
            closeOnConfirm: true
        }, function (isConfirm) { });
    } else {
        swal({
            title: "",
            text: msg,
            html: msg,
            type: errImg,
            confirmButtonColor: '#ff0000',
            confirmButtonText: '確定',
            closeOnConfirm: true
        }, function (isConfirm) { $("#" + site).submit(); });
    }
}
function GetEncyptData(jdata, callback) {
    console.log("jdata:" + jdata);
    var URL = host + "iMotoWebAPI/api/AesEncrypt/doEncrypt";
    console.log(URL);
    $.ajax({
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: jdata,
        url: URL,
        error: function (xhr, error) { console.log(xhr.responseText + "," + xhr.status + "," + error); },
        success: function (JsonData) {
            console.log(JsonData.Result + "," + JsonData.ErrMsg);
            if (JsonData.Result === "0") {
                callback(JsonData.ErrMsg);
            } else {
                warningAlert(JsonData.ErrMsg, false, 0, "");
            }
        }
    });
}
//新增清潔合約
function insCleanOrderComplete(encryptData) {
    var jdata = JSON.stringify({ "para": encryptData });
    var URL = host + "iMotoWebAPI/api/InsertClean/CleanAdd";
    $.ajax({
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: jdata,
        url: URL,
        error: function (xhr, error) { console.log(xhr.responseText + "," + xhr.status + "," + error); },
        success: function (JsonData) {
            console.log(JsonData.Result + "," + JsonData.ErrMsg);
            if (JsonData.Result === "0") {
                console.log("true");
                //   $.unblockUI();
                warningAlertSubMit(JsonData.ErrMsg, 2, 1, "dataForm");
            } else {
                console.log("false");
                //    $.unblockUI();
                warningAlert(JsonData.ErrMsg, false, 0, "");
            }
        }
    });
}