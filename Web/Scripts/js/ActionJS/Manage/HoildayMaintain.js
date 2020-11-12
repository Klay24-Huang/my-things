$(function () {
    $("#HoildayYear").on("change", function () {
        ShowLoading("資料查詢中");
        $("#frmHoildayMaintain").submit();
    });
    $("#Season").on("change", function () {
        ShowLoading("資料查詢中");
        $("#frmHoildayMaintain").submit();
    });
    $("#btnReset").on("click", function () {
        $("input[name='chkHoilday']").prop("checked", false);
       // SyncData();
    });
    $("#btnRestore").on("click", function () {
        $("input[name='chkHoilday']").prop("checked", false);
        SyncData();
    })
    $("#btnSetDef").on("click", function () {
        $("input[name='chkHoilday']").each(function () {
         //   console.log($(this).val());
            var isHoilday = $(this).attr('data-hoilday');
            if (isHoilday == "1") {
                $(this).prop("checked", true);
            }
        });
    });
    $("#btnSend").on("click", function () {
        var Account = $("#Account").val();
        var HoildayYear = $("#HoildayYear").val();
        var HoildaySeason = $("#Season").val();
     
        var obj = new Object();
        obj.UserID = Account;
        obj.QueryYear = parseInt(HoildayYear);
        obj.QuerySeason = parseInt(HoildaySeason) + 1;
        var HoildayArray = new Array();
        $("input[name='chkHoilday']").each(function () {
            //   console.log($(this).val());
            if ($(this).prop("checked")) {
                var HoildayObj = new Object();
                HoildayObj.HolidayYearMonth = $(this).val().substr(0,6);
                HoildayObj.HolidayDate = $(this).val();
                HoildayArray.push(HoildayObj)
            }

        });
        obj.Hoilday = HoildayArray;
        var json = JSON.stringify(obj);
        console.log(json);
        var site = jsHost + "BE_HandleHoilday";
        console.log("site:" + site);
        $.ajax({
            url: site,
            type: 'POST',
            data: json,
            cache: false,
            contentType: 'application/json',
            dataType: 'json',           //'application/json',
            success: function (data) {
                $.busyLoadFull("hide");

                if (data.Result == "1") {
                    swal({
                        title: 'SUCCESS',
                        text: data.ErrorMessage,
                        icon: 'success'
                    }).then(function (value) {
                   
                        disabledLoading();
                    });
                } else {

                    swal({
                        title: 'Fail',
                        text: data.ErrorMessage,
                        icon: 'error'
                    });
                }
            },
            error: function (e) {
                $.busyLoadFull("hide");
                swal({
                    title: 'Fail',
                    text: "修改假日發生錯誤",
                    icon: 'error'
                });
            }

        });
   
    });
    SyncData();
});
function SyncData() {
    var HoildayYear = $("#HoildayYear").val();
    var HoildaySeason = $("#Season").val();
    var Account = $("#Account").val();
    if (HoildayYear != "" && HoildaySeason != "" && Account !== "") {
        ShowLoading("資料讀取中");

        var obj = new Object();
        obj.UserID = Account;
        obj.QueryYear = parseInt(HoildayYear);
        obj.QuerySeason = parseInt(HoildaySeason) + 1;
        var json = JSON.stringify(obj);
        console.log(json);
        var site = jsHost + "BE_GetHoilday";
        console.log("site:" + site);
        $.ajax({
            url: site,
            type: 'POST',
            data: json,
            cache: false,
            contentType: 'application/json',
            dataType: 'json',           //'application/json',
            success: function (data) {
                $.busyLoadFull("hide");

                if (data.Result == "1") {
                    //swal({
                    //    title: 'SUCCESS',
                    //    text: data.ErrorMessage,
                    //    icon: 'success'
                    //}).then(function (value) {
                        console.log(data.Data.holidays.length)
                        if (false == isNaN(data.Data.holidays.length)) {
                            var len = data.Data.holidays.length
                            for (var i = 0; i < len; i++) {
                                var objName = "#chkHoilday_" + data.Data.holidays[i].HolidayDate
                                console.log(objName);
                                $(objName).prop("checked", true);
                            }
                        }

                        disabledLoading();
                    //});
                } else {

                    swal({
                        title: 'Fail',
                        text: data.ErrorMessage,
                        icon: 'error'
                    });
                }
            },
            error: function (e) {
                $.busyLoadFull("hide");
                swal({
                    title: 'Fail',
                    text: "查詢假日資料發生錯誤",
                    icon: 'error'
                });
            }

        });
    } else {
        disabledLoading();
    }
}