var NowEditID = 0;

$(document).ready(function () {

    SetStation($("#Banner"), $("#BannerName"));

    //讓修改後回到此頁面時重里整理，但第一次進來不會耶
    setTimeout(function () {
        $('.btnSend').click();
    }, 500);

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
    //console.log('qq:' + SEQNO)
    //console.log('qqaa:' + typeof (SEQNO))

    //var a = parseInt(SEQNO);
    //console.log('www:' + a)
    //console.log('rrrr:' + typeof (a))
    $("#MaintainBanner").val(SEQNO);
    //console.log('oooooo:' + $("#MaintainBanner").val())
    $("#frmBannerMaintain").submit();
}
function GoAddNew() {
    window.location.href = "BannerInfoAdd";
}

function DoEdit(Id) {
    if (NowEditID > 0) {
        $("#Queue_" + NowEditID).empty().hide();
        $("#btnReset_" + NowEditID).hide();
        $("#btnSave_" + NowEditID).hide();
        $("#btnEdit_" + NowEditID).show();
    }

    NowEditID = Id;

    $("#Queue_" + Id).show();

    $("#btnReset_" + Id).show();
    $("#btnSave_" + Id).show();
    $("#btnEdit_" + Id).hide();
}
function DoReset(Id) {
    $("#Queue_" + Id).val("").hide();

    $("#btnReset_" + Id).hide();
    $("#btnSave_" + Id).hide();
    $("#btnEdit_" + Id).show();
    NowEditID = 0;
}
function DoSave(Id) {
    Queue = $("#Queue_" + Id).val();
    var Account = $("#Account").val();

    ShowLoading("資料處理中");

    var obj = new Object();
    obj.UserID = Account;
    obj.SEQNO = Id;
    obj.Queue = Queue;
    DoAjaxAfterReload(obj, "BE_BannerSort", "修改使用者者發生錯誤");
}
