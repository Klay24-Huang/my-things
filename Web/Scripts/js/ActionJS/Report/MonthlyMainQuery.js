$(document).ready(function () {
    $("#start").val(SD);
    $("#end").val(ED);
    $("#isHandle").val(isHandle);
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
            $("#formMonthlyQueryDownLoad").submit();
        });
    }

    $('table').footable();
});
