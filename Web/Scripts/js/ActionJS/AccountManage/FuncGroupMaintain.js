﻿var NowEditID = 0;
var FuncGroupID = "";
var FuncGroupName = "";
var StartDate = "";
var EndDate = "";
$(document).ready(function () {
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
    if (Mode == "") {
        init();
    } else {
        $("#ddlObj").trigger("change");
    }

    $("#ddlObj").on("change", function () {
        var Mode = $(this).val();
        console.log("Mode=" + Mode);
        switch (Mode) {
            case "":
                init();
                break;
            case "Add":
             
                $("#btnSend").show();
                $("#btnReset").show();
                $("#btnSend").text("儲存");

                break;
            case "Edit":
          

                $("#btnSend").show();
                $("#btnReset").show();
                $("#btnSend").text("查詢");

                break;
        }
    });
    $("#btnReset").on("click", function () {
        init();
    });

    $("#btnSend").on("click", function () {
        var flag = true;
        var errMsg = "";
        ShowLoading("資料處理中");
        var Account = $("#Account").val();
        var Mode = $("#ddlObj").val();
        var FuncGroupID = $("#FuncGroupID").val();
        var FuncGroupName = $("#FuncGroupName").val();
        var StartDate = $("#StartDate").val();
        var EndDate = $("#EndDate").val();

        var checkList = [FuncGroupID, FuncGroupName, StartDate, EndDate];
        var errMsgList = ["功能群組編號未填", "功能群組名稱未填", "有效日期（起）未填", "有效日期（迄）未填"];
        var len = checkList.length;
        if (Mode == "Add") {

            for (var i = 0; i < len; i++) {
                if (checkList[i] == "") {
                    flag = false;
                    errMsg = errMsgList[i];
                    break;
                }
            }

            if (flag) {
                if (StartDate > EndDate) {
                    flag = false;
                    errMsg = "起始日期大於結束日期";
                }
            }


        } else {
            flag = false;
            errMsg = "請至少要選擇一個查詢欄位";
            for (var i = 0; i < len; i++) {
                if (checkList[i] != "") {
                    flag = true;

                    break;
                }
            }

            if (flag) {
                if (StartDate != "" && EndDate != "") {
                    if (StartDate > EndDate) {
                        flag = false;
                        errMsg = "起始日期大於結束日期";
                    }
                } else {
                    if ((StartDate == "" && EndDate != "")) {
                        flag = false;
                        errMsg = "起始日期未填";
                    } else if ((StartDate != "" && EndDate == "")) {
                        flag = false;
                        errMsg = "結束日期未填";
                    }
                }

            }
        }


        if (flag) {
            if (Mode == "Add") {
                var obj = new Object();
                obj.UserID = Account;
                obj.FuncGroupID = FuncGroupID;
                DoAjaxAfterSubmitNonShowMessageAndNowhide(obj, "BE_CheckFuncGroup", "驗證功能群組編號發生錯誤", $("#FuncGroupMaintainMaintain"));
            } else {
                $("#FuncGroupMaintainMaintain").submit();
            }

        } else {
            disabledLoadingAndShowAlert(errMsg);
        }
    });

    $("#btnReset").on("click", function () {
        init();
    })
    if (Mode != '') {
        $("#ddlObj").val(Mode);
        $("#ddlObj").trigger("change");

    }
    if (ResultDataLen > -1) {
        $("#panelResult").show();
        $('table').footable();
    }
    if (Mode == "Add") {
        if (errorLine == "ok") {
            ShowSuccessMessage("新增成功");
        } else {
            if (errorMsg != "") {
                ShowFailMessage(errorMsg);
            }
        }
    }
})

$(document).on('keyup', '.input_alphanumeric', function (e) {
    let k = e.keyCode;
    console.log(k);
    let str = String.fromCharCode(k);
    console.log(str);
    if (!(str.match(/[0-9a-zA-Z]/) || e.shiftKey || (37 <= k && k <= 40) || k === 8 || k === 46)) {
        console.log("false");
        return false;
    }
});
$(document).on('keydown', '.input_alphanumeric', function (e) {
    let k = e.keyCode;
    console.log(k);
    let str = String.fromCharCode(k);
    console.log(str);
    if (!(str.match(/[0-9a-zA-Z]/) || e.shiftKey || (37 <= k && k <= 40) || k === 8 || k === 46)) {
        console.log("false");
        return false;
    }
});
$(document).on('keypress', '.input_alphanumeric', function (e) {
    let k = e.keyCode;
    console.log(k);
    let str = String.fromCharCode(k);
    console.log(str);
    if (!(str.match(/[0-9a-zA-Z]/) || e.shiftKey || (37 <= k && k <= 40) || k === 8 || k === 46)) {
        console.log("false");
        return false;
    }
});
$(document).on('change', '.input_alphanumeric', function () {
    this.value = this.value.replace(/[^0-9a-zA-Z]+/i, '');
});
function DoEdit(Id) {
    var OldIcon = $("#btnEdit_" + Id).attr('data-OldIcon');
    if (NowEditID > 0) {
        //先還原前一個
        $("#FuncGroupID_" + NowEditID).val(FuncGroupID).hide();
        $("#FuncGroupName_" + NowEditID).val(FuncGroupName).hide();
        $("#StartDate_" + NowEditID).val(StartDate).hide();
        $("#EndDate_" + NowEditID).val(EndDate).hide();
        $("#btnReset_" + NowEditID).hide();
        $("#btnSave_" + NowEditID).hide();
        $("#btnEdit_" + NowEditID).show();
    }
    //再開啟下一個
    /*    NowEditID = Id;
        ParkingName = $("#ParkingName_" + Id).val();
        ParkingAddress = $("#ParkingAddress_" + Id).val();
        Latitude = $("#Latitude_" + Id).val();
        Longitude = $("#Longitude_" + Id).val();
        OpenTime = $("#OpenTime_" + Id).val();
        CloseTime = $("#CloseTime_" + Id).val();
    } else {*/
    NowEditID = Id;

    FuncGroupID = $("#FuncGroupID_" + Id).val();
    FuncGroupName = $("#FuncGroupName_" + Id).val();
    StartDate = $("#StartDate_" + Id).val();
    EndDate = $("#EndDate_" + Id).val();

    //  }
    $("#FuncGroupID_" + Id).show();
    $("#FuncGroupName_" + Id).show();
    $("#StartDate_" + Id).show();
    $("#EndDate_" + Id).show();

    $("#btnReset_" + Id).show();
    $("#btnSave_" + Id).show();
    $("#btnEdit_" + Id).hide();
}
function DoReset(Id) {

    $("#FuncGroupID_" + Id).val(FuncGroupID).hide();
    $("#FuncGroupName_" + Id).val(FuncGroupName).hide();
    $("#StartDate_" + Id).val(StartDate).hide();
    $("#EndDate_" + Id).val(EndDate).hide();
    $("#btnReset_" + Id).hide();
    $("#btnSave_" + Id).hide();
    $("#btnEdit_" + Id).show();

    NowEditID = 0;

    FuncGroupID = "";
    FuncGroupName = "";
    StartDate = "";
    EndDate = "";
}
function DoSave(Id) {
    var SEQNO = Id;
    FuncGroupID = $("#FuncGroupID_" + Id).val();
    FuncGroupName = $("#FuncGroupName_" + Id).val();
    StartDate = $("#StartDate_" + Id).val();
    EndDate = $("#EndDate_" + Id).val();

    var Account = $("#Account").val();

    var flag = true;
    var errMsg = "";
    ShowLoading("資料處理中");
    var checkList = [FuncGroupID, FuncGroupName, StartDate, EndDate];
    var errMsgList = ["功能群組編號未填", "功能群組名稱未填", "有效日期（起）未填", "有效日期（迄）未填"];
    var len = checkList.length;
    for (var i = 0; i < len; i++) {
        if (checkList[i] == "") {
            flag = false;
            errMsg = errMsgList[i];
            break;
        }
    }
    if (flag) {
        if (StartDate > EndDate) {
            flag = false;
            errMsg = "起始日期大於結束日期";
        }
    }
    if (flag) {
        var obj = new Object();
        obj.UserID = Account;
        obj.SEQNO = SEQNO;
        obj.FuncGroupID = FuncGroupID;
        obj.FuncGroupName = FuncGroupName;
        obj.StartDate = StartDate.replace(/\//g, "").replace(/\-/g, "");
        obj.EndDate = EndDate.replace(/\//g, "").replace(/\-/g, "");

        DoAjaxAfterReload(obj, "BE_UPDFuncGroup", "修改功能群組發生錯誤");
    }

}
function init() {
    $("#btnReview").hide();
    $("#btnSend").hide();
    $("#btnReset").hide();
    $("#FuncGroupID").val("");
    $("#FuncGroupID").prop("disabled", "");
    $("#FuncGroupName").val("");
    $("#FuncGroupName").prop("disabled", "");
    $("#StartDate").val("");
    $("#StartDate").prop("disabled", "");
    $("#EndDate").val("");
    $("#EndDate").prop("disabled", "");
    $("#ddlObj").val("");
    $("#panelResult").hide();

}