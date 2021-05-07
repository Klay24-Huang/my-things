$(document).ready(function () {
  
    SetStation($("#StationID"), $("#StationName"));

    $("#btnSend").on("click", function () {
        ShowLoading("資料查詢中");
        var StationID = $("#StationID").val();
        var isMach = $("#NotMach").prop("checked");
        var isMach2 = $("#NotMach2").prop("checked");
        console.log(isMach);
        if (isMach == false && StationID == "" && isMach2 == false) {
            disabledLoadingAndShowAlert("請至少選擇一個查詢條件");
        } else {
            $("#frmStationSetting").submit();
        }
    });
    $("#btnAdd").on("click", function () {
        GoAddNew();
    });
    
    var hasData = parseInt(ResultDataLen);
    if (hasData > 0) {
        $('.table').footable({
            "paging": {
                "enabled": true,
                "limit": 3,
                "size": 20
            }
        });
    }
});
function goMaintain(StationID) {
    $("#MaintainStationID").val(StationID);
    $("#frmStationMaintain").submit();
}
function GoAddNew() {
    window.location.href = "StationInfoAdd";
}
