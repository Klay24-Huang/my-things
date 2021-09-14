$(document).ready(function () {

    $("#btn_accept").on('click', function () {
        $("#accept").val("true");
        $("#frmInviteeResponse").submit();
    })

    $("#btn_refuse").on('click', function () {
        $("#accept").val("false");
        $("#frmInviteeResponse").submit();
    })
})