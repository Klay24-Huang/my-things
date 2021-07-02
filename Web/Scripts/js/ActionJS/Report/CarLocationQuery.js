$(function () {

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

    var checkDate = function () {
        var StartTime = $("#Time_Start").val();
        var EndTime = $("#Time_End").val();
        if (Date.parse(StartTime).valueOf() > Date.parse(EndTime).valueOf()) {
            //disabledLoadingAndShowAlert("起始時間不得大於結束時間");
            //return false;
            return "起始時間不得大於結束時間";
        }
        if (StartTime == "" || EndTime == "") {
            //disabledLoadingAndShowAlert("日期格不得為空");
            //return false;
            return "日期格不得為空"
        }
        if ((Date.parse(EndTime) - Date.parse(StartTime)) / 86400000 > 31) {
            //disabledLoadingAndShowAlert("間隔不得大於31天");
            //return false;
            return "間隔不得大於31天"
        }
        return "true";
    }

    //countLoadingTime = function () {
    //    var loadingTime = 120;
    //    var StartTime = $("#Time_Start").val();
    //    var EndTime = $("#Time_End").val();
    //    if ((Date.parse(EndTime) - Date.parse(StartTime)) / 86400000 < 26)
    //    {
    //        if ((Date.parse(EndTime) - Date.parse(StartTime)) / 86400000 < 20)
    //        {
    //            if ((Date.parse(EndTime) - Date.parse(StartTime)) / 86400000 < 15)
    //            {
    //                if ((Date.parse(EndTime) - Date.parse(StartTime)) / 86400000 < 10)
    //                {
    //                    if ((Date.parse(EndTime) - Date.parse(StartTime)) / 86400000 < 5) {
    //                        loadingTime = 10;
    //                        return loadingTime;
    //                    }
    //                    loadingTime = 30;
    //                    return loadingTime;
    //                }
    //                loadingTime = 60;
    //                return loadingTime;
    //            }
    //            loadingTime = 90;
    //            return loadingTime;
    //        }
    //        return loadingTime;
    //    }
    //}

    $("#Clear").on("click", function () {
        $('#Time_Start').val("")
        $('#Time_End').val("")
    });

    $("#ExportCar").on("click", function () {
        ShowLoading("資料處理中...");
        var message = checkDate();
        //var message = "";
        var StartTime = $("#Time_Start").val();
        var EndTime = $("#Time_End").val();
        $('#IsCar').val("true");

        if (message == "true") {
            var obj = new Object();
            obj.StartTime = StartTime;
            obj.EndTime = EndTime;
            var json = JSON.stringify(obj);
            $("#queryData").val(json);
            $("#frmCarDashBoard").submit();

            setTimeout(function () {
                window.location.reload();
            }, 120000);
        } else {
            disabledLoadingAndShowAlert(message);
        }
        $.busyLoadFull("hide");
        return false;
    });

    $("#ExportMoto").on("click", function () {
        ShowLoading("資料處理中...");
        var message = checkDate();
        //var message = "";
        var StartTime = $("#Time_Start").val();
        var EndTime = $("#Time_End").val();


        if (message == "true") {
            var obj = new Object();

            obj.StartTime = StartTime;
            obj.EndTime = EndTime;
            var json = JSON.stringify(obj);
            $("#queryData").val(json);
            $("#frmCarDashBoard").submit();

            setTimeout(function () {
                window.location.reload();
            }, 120000);
        } else {
            disabledLoadingAndShowAlert(message);
        }
        $.busyLoadFull("hide");
        return false;
    });

});