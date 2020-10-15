$(function () {
    SetStation($("#StationID"), $("#StationName"));
    SetCar($("#CarNo"))

    var hasData = parseInt($("#len").val());
    if (hasData > 0) {
        $('.table').footable({
            "paging": {
                "limit": 3,
                "size": 20
            }
        });
    }
})