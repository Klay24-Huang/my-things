var NowEditID = 0;
var UserGroupID = "";
var UserGroupName = "";
var OperatorID = "";
var FuncGroupID = "";
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
              
               // $("#ddlOperator").show();
                $("#btnSend").show();
                $("#btnReset").show();
                $("#btnSend").text("儲存");


                break;
            case "Edit":
               
              //  $("#ddlOperator").show();
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
        var UserGroupID = $("#UserGroupID").val();
        var UserGroupName = $("#UserGroupName").val();
        var Operator = $("#ddlOperator").val();
        var FuncGroup = $("#ddlFuncGroup").val();
        var StartDate = $("#StartDate").val();
        var EndDate = $("#EndDate").val();

        var checkList = [UserGroupID, UserGroupName, Operator, FuncGroup, StartDate, EndDate];
        var errMsgList = ["使用者群組編號未填", "使用者群組名稱未填", "業者別未填", "功能群組未填", "有效日期（起）未填", "有效日期（迄）未填"];
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
                obj.UserGroupID = UserGroupID;
                obj.OperatorID = Operator;
                console.log(obj);
               DoAjaxAfterSubmitNonShowMessageAndNowhide(obj, "BE_CheckUserGroup", "驗證使用群組編號發生錯誤", $("#UserGroupMaintain"));
            } else {
                $("#UserGroupMaintain").submit();
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
        $("#ddlOperator").val(tmpOperatorID)
        $("#ddlFuncGroup").val(tmpFuncGroupID)
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
        $("#UserGroupID_" + NowEditID).val(UserGroupID).hide();
        $("#UserGroupName_" + NowEditID).val(UserGroupName).hide();
        $("#StartDate_" + NowEditID).val(StartDate).hide();
        $("#EndDate_" + NowEditID).val(EndDate).hide();
        $("#OperatorName_" + NowEditID).empty().hide();
        $("#FuncGroupName_" + NowEditID).empty().hide();
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

    UserGroupID = $("#UserGroupID_" + Id).val();
    UserGroupName = $("#UserGroupName_" + Id).val();
    OperatorID = $("#OperatorID_" + Id).val();
    FuncGroupID = $("#FuncGroupID_" + Id).val();
    StartDate = $("#StartDate_" + Id).val();
    EndDate = $("#EndDate_" + Id).val();

    //  }
    $("#UserGroupID_" + Id).show();
    $("#UserGroupName_" + Id).show();
    $("#OperatorName_" + Id).empty();
    $("#ddlOperator").find('option').clone().appendTo('#OperatorName_' + Id);
    $("#OperatorName_" + Id).val(OperatorID).show();
    $("#ddlFuncGroup").find('option').clone().appendTo('#FuncGroupName_' + Id);
    $("#FuncGroupName_" + Id).val(FuncGroupID).show();
    $("#StartDate_" + Id).show();
    $("#EndDate_" + Id).show();

    $("#btnReset_" + Id).show();
    $("#btnSave_" + Id).show();
    $("#btnEdit_" + Id).hide();
}
function DoReset(Id) {

    $("#UserGroupID_" + Id).val(UserGroupID).hide();
    $("#UserGroupName_" + Id).val(UserGroupName).hide();
    $("#OperatorName_" + Id).empty().hide();
    $("#FuncGroupName_" + Id).empty().hide();
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
    var USEQNO = Id;
    UserGroupID = $("#UserGroupID_" + Id).val();
    UserGroupName = $("#UserGroupName_" + Id).val();
    OperatorID = $("#OperatorName_" + Id).val();
    FuncGroupID = $("#FuncGroupName_" + Id).val();
    StartDate = $("#StartDate_" + Id).val();
    EndDate = $("#EndDate_" + Id).val();

    var Account = $("#Account").val();

    var flag = true;
    var errMsg = "";
    ShowLoading("資料處理中");
    var checkList = [UserGroupID, UserGroupName, OperatorID, FuncGroupID, StartDate, EndDate];
    var errMsgList = ["使用者群組編號未填", "使用者群組名稱未填","業者別未選擇","功能群組未選擇", "有效日期（起）未填", "有效日期（迄）未填"];
    var len = checkList.length;
    for (var i = 0; i < len; i++) {
        if (checkList[i] == "") {
            flag = false;
            errMsg = errMsgList[i];
            break;
        }
    }
    if (flag) {
        if (OperatorID == "") {
            flag = false;
            errMsg = "業者別未選擇";
        }
    }
    if (flag) {
        if (FuncGroupID == "") {
            flag = false;
            errMsg = "功能群組未選擇";
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
        obj.SEQNO = USEQNO;
        obj.UserGroupID = UserGroupID;
        obj.OperatorID = OperatorID;
        obj.FuncGroupID = FuncGroupID;
        obj.UserGroupName = UserGroupName;
        obj.StartDate = StartDate.replace(/\//g, "").replace(/\-/g, "");
        obj.EndDate = EndDate.replace(/\//g, "").replace(/\-/g, "");

        DoAjaxAfterReload(obj, "BE_UPDUserGroup", "修改使用者群組發生錯誤");
    }

}
function init() {
    $("#btnReview").hide();
    $("#btnSend").hide();
    $("#btnReset").hide();
    $("#UserGroupID").val("");
    $("#UserGroupID").prop("disabled", "");
    $("#UserGroupName").val("");
    $("#UserGroupName").prop("disabled", "");
    $("#StartDate").val("");
    $("#StartDate").prop("disabled", "");
    $("#EndDate").val("");
    $("#EndDate").prop("disabled", "");
    $("#ddlObj").val("");
    $("#ddlOperator").val("");
    $("#panelResult").hide();

}