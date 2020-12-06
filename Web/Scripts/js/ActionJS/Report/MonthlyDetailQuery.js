$(document).ready(function () {
    $("#OrderNum").val(OrderNum);
    $("#start").val(SD);
    $("#end").val(ED);
    $("#userID").val(UserID);
    var hasData = parseInt($("#len").val());
    if (hasData > 0) {
        $('.table').footable({
            "paging": {
                "enabled": true,
                "limit": 3,
                "size": 20
            }
        });
        $("#btnDownload").on("click", function () {
            $("#formMonthlyDetailQueryDownload").submit();
        });
    }

    $('table').footable();
});