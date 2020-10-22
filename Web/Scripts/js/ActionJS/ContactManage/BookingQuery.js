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
function ViewDetail(OrderNo) {
    console.log(OrderNo);
}
function CancelOrder(OrderNo) {
    console.log(OrderNo);
}
function ReBook(OrderNo) {
    console.log(OrderNo);
}