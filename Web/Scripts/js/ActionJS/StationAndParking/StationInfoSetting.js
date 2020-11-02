$(document).ready(function () {
  
    SetStation($("#StationID"), $("#StationName"));

    $("#btnSend").on("click", function () {
        ShowLoading("資料查詢中");
        var StationID = $("#StationID").val();
        var isMach = $("#NotMach").val();
        if (CheckStorageIsNull(isMach) == false && StationID == "") {
            disabledLoadingAndShowAlert("請至少選擇一個查詢條件");
        } else {
            $("#frmStationSetting").submit();
        }
    });
});
