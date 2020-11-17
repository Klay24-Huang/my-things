$(function () {
    var now = new Date();
    SetStationNoShowName($("#StationID"));
    SetCar($("#CarNo"))
    if (errorLine == "ok") {

    } else {
        if (errorMsg != "") {
            ShowFailMessage(errorMsg);
        }
    }
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
})
function ViewDetail(OrderNo, Mode) {
    $("#DetailOrderNo").val(OrderNo);
    if (Mode == 0) {
        document.getElementById("showDetail").action = "ContactDetail"
    } else {
        document.getElementById("showDetail").action = "ContactMotorDetail"
    }
    $("#showDetail").submit()
    console.log(OrderNo);
}
function setStation(StationID) {
    $('#StationID').val(StationID);
}