$(document).ready(function () {

    SetStation($("#Banner"), $("#BannerName"));

    $("#btnSend").on("click", function () {
        ShowLoading("資料查詢中");
        $("#frmBannerSetting").submit();
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
function goMaintain(SEQNO) {
    console.log('qq:' + SEQNO)
    console.log('qqaa:' + typeof (SEQNO))

    //var a = parseInt(SEQNO);
    //console.log('www:' + a)
    //console.log('rrrr:' + typeof (a))
    $("#MaintainBanner").val(SEQNO);
    console.log('oooooo:' + $("#MaintainBanner").val())
    $("#frmBannerMaintain").submit();
}
function GoAddNew() {
    window.location.href = "BannerInfoAdd";
}
