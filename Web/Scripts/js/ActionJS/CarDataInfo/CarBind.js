var NowEditID = 0;
var CID = "";

$(document).ready(function () {

    $("#exampleDownload").on("click", function () {
        window.location.href = "../Content/example/CarBindExample.xlsx";
    });
    $("#btnUpload").on("click", function () {
        $("#Mode").val("Add");
        $("#frmCarBind").submit();
    })
    $("#btnExplode").on("click", function () {
        $("#Mode").val("Explode");
        $("#frmCarBind").submit();
    });
    $("#btnSend").on("click", function () {
        $("#Mode").val("Query");
        console.log($("#Mode").val());
        DoQuery();
        //$("#frmCarBind").submit();
    })
    var hasData = parseInt($("#len").val());
    var Mode = $("#Mode").val();
    console.log(hasData);
    if (hasData > 0) {
        ShowLoading("資料讀取中...");
        $('.table').footable({
            "paging": {
                "limit": 3,
                "size": 20
            }
        });
       
    }

    if (Mode == "Add") {
        if (errorLine == "ok") {
            ShowSuccessMessage("匯入成功");
        } else {
            if (errorMsg != "") {
                ShowFailMessage(errorMsg);
            }
        }
    }
    disabledLoading();
});
function DoQuery() {
    var BindStatus = $("#BindStatus").val();
    //20201207唐改...我之前改的還沒push，不小心全刪了....只能重改....
    /*
    if (CheckIsUndefined(BindStatus)) {
        if (parseInt(BindStatus) < 0) {
            ShowFailMessage("請選擇使用狀態");
        } else {
            $("#frmCarBind").submit();
        }
    } else {
        ShowFailMessage("請選擇使用狀態");
    }*/
    $("#frmCarBind").submit();
    console.log("BindStatus="+BindStatus);

} 

function DoReset(Id) {
    if (NowEditID > 0) {
        NowEditID = 0;
        $("#CID_" + Id).empty().hide();

    } else {
        $("#CID_" + Id).hide();
    }

    $("#btnReset_" + Id).hide();
    $("#btnSave_" + Id).hide();
    $("#btnEdit_" + Id).show();

}
function DoEdit(Id) {
    if (NowEditID > 0) {
        //先還原前一個
        $("#CID_" + NowEditID).empty().hide();

        $("#btnReset_" + NowEditID).hide();
        $("#btnSave_" + NowEditID).hide();
        $("#btnEdit_" + NowEditID).show();
    }

    NowEditID = Id;
 //   CID = $("#CID_" + Id).val();
 
    //  }
    var $options = $("#ddlMachine_0 > option").clone();

    $("#CID_" + Id).append($options);
    $("#CID_" + Id).show();


    $("#btnReset_" + Id).show();
    $("#btnSave_" + Id).show();
    $("#btnEdit_" + Id).hide();

}
function DoSave(Id) {
    ShowLoading("資料處理中");
    var Account = $("#Account").val();

    var flag = true;
    var errMsg = "";

    var sCIDVal = $("#CID_" + Id).val();
    if (sCIDVal === '') {
        flag = false
        errMsg = 'CID不可空白';
    }

    if (flag) {
        var obj = new Object();
        obj.UserID = Account;
        obj.CID = $("#CID_"+Id).val();
        obj.CarNo = $("#CarNo_" + Id).val();

        var json = JSON.stringify(obj);
        console.log(json);
        var site = jsHost + "BE_HandleCarMachine";
        console.log("site:" + site);
        $.ajax({
            url: site,
            type: 'POST',
            data: json,
            cache: false,
            contentType: 'application/json',
            dataType: 'json',           //'application/json',
            success: function (data) {
                $.busyLoadFull("hide");

                if (data.Result == "1") {
                    swal({
                        title: 'SUCCESS',
                        text: data.ErrorMessage,
                        icon: 'success'
                    }).then(function (value) {
                        window.location.reload();
                    });
                } else {

                    swal({
                        title: 'Fail',
                        text: data.ErrorMessage,
                        icon: 'error'
                    });
                }
            },
            error: function (e) {
                $.busyLoadFull("hide");
                swal({
                    title: 'Fail',
                    text: "綁定車機發生錯誤",
                    icon: 'error'
                });
            }

        });
    } else {
        disabledLoadingAndShowAlert(errMsg);
    }
}