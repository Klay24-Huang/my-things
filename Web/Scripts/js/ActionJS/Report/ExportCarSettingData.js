$(function () {
    SetStation($("#StationID"), $("#StationName"));
    SetCar($("#CarNo"))

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
    var gotoBrnhcd = function (Brnhcd) {
        $('#StationID').val(Brnhcd);
    };

    var checkDate = function () {
        var StartTime = $("#Time_Start").val();
        var EndTime = $("#Time_End").val();
        if (Date.parse(StartTime).valueOf() > Date.parse(EndTime).valueOf()) {
            disabledLoadingAndShowAlert("起始時間不得大於結束時間");
            return false;
        }
        if (StartTime == "" || EndTime == "") {
            disabledLoadingAndShowAlert("日期格不得為空");
            return false;
        }
        if ((Date.parse(EndTime) - Date.parse(StartTime)) / 86400000 > 31) {
            disabledLoadingAndShowAlert("間隔不得大於31天");
            return false;
        }
        return true;
    }

    $("#Clear").on("click", function () {
        $('#StationID').val("");
        $('#Time_Start').val("")
        $('#Time_End').val("")
    });

    $("#btnSearch_X0WR").on("click", function () {
        gotoBrnhcd('X0WR');
    });
    $("#btnSearch_X1JT").on("click", function () {
        gotoBrnhcd('X1JT');
    });
    $("#btnSearch_X1KX").on("click", function () {
        gotoBrnhcd('X1KX');
    });
    $("#btnSearch_X1KZ").on("click", function () {
        gotoBrnhcd('X1KZ');
    });
    $("#btnSearch_X1KY").on("click", function () {
        gotoBrnhcd('X1KY');
    });
    $("#btnSearch_X1ZZ").on("click", function () {
        gotoBrnhcd('X1ZZ');
    });
    $("#btnSearch_X0SR").on("click", function () {
        gotoBrnhcd('X0SR');
    });
    $("#btnSearch_X0R4").on("click", function () {
        gotoBrnhcd('X0R4');
    });
    $("#btnSearch_X0U4").on("click", function () {
        gotoBrnhcd('X0U4');
    });
    $("#btnSearch_X1V4").on("click", function () {
        gotoBrnhcd('X1V4');
    });

    $("#btnSearch").on("click", function () {
        ShowLoading("資料處理中...");
        var flag = checkDate();
        var message = "";     
        var StationID = $("#StationID").val();
        var StartTime = $("#Time_Start").val();
        var EndTime = $("#Time_End").val();

        if (flag) {
            var obj = new Object();

            obj.StationID = StationID;
            obj.StartTime = StartTime;
            obj.EndTime = EndTime;
            var json = JSON.stringify(obj);
            $("#queryData").val(json);
            $("#frmCarDashBoard").submit();
        } else {
            disabledLoadingAndShowAlert(message);
        }
        return false;
    });

    $("#btnExport").on("click", function () {
        ShowLoading("資料處理中...");
        var flag = checkDate();
        var message = "";
        var StationID = $("#StationID").val();
        var StartTime = $("#Time_Start").val();
        var EndTime = $("#Time_End").val();
        $('#isExport').val("true");

        if (flag) {
            var obj = new Object();

            obj.StationID = StationID;
            obj.StartTime = StartTime;
            obj.EndTime = EndTime;
            var json = JSON.stringify(obj);
            $("#queryData").val(json);
            $("#frmCarDashBoard").submit();

            setTimeout(function () {
                window.location.reload();
            }, 1000);
        } else {
            disabledLoadingAndShowAlert(message);
        }
        return false;
    });

});