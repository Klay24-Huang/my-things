$(function () {
    if (ResultDataLen > -1) {
        $("#panelResult").show();
        $('.table').footable({
            "paging": {
                "limit": 3,
                "size": 20
            }
        });
    } else {
        $("#panelResult").hide();
    }
})