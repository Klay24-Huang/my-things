var funcList;
var NowEditID = 0;
var OperatorID = "";
var UserGroupID = "";
var UserName = "";
var UserAccount = "";
var UserPWD = "";
var StartDate = "";
var EndDate = "";

$(document).ready(function () {

    var func = $("#funcName").val();

    if (func != "") {
        funcList = JSON.parse(func);
        console.log(funcList);
    }
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
        $("#ddlObj").val(Mode);
        $("#ddlObj").trigger("change");
        if (Mode == "Add") {
            $("#btnSend").text("儲存");
            $("#UserPWD").prop("disabled", "");
            $("#btnReview").show();
        } else if(Mode=="Edit") {
            $("#btnSend").text("查詢");
            $("#UserPWD").prop("disabled", "disabled");
            $("#btnReview").hide();
        }
    }
    $("#btnSet").on("click", function () {
        var text = $(this).text();
        if (text == "全選") {
            $(".form-check-input").each(function () {
                $(this).prop("checked", "checked");
            });
            $(this).text("取消全選");
        } else {
            $(".form-check-input").each(function () {
                $(this).prop("checked", "");
            })
            $(this).text("全選");
        }
    })
    $("#ddlObj").on("change", function () {
        var Mode = $(this).val();
        console.log("Mode=" + Mode);
        switch (Mode) {
            case "":
                init();
                break;
            case "Add":
                $("#liPower").hide();
                $("#UserPWD").prop("disabled", "");
                $("#btnSend").show();
                $("#btnReset").show();
                $("#btnSend").text("儲存");

                break;
            case "Edit":
                $("#UserPWD").prop("disabled", "disabled");
                $("#liPower").hide();
                $("#btnSend").show();
                $("#btnReset").show();
                $("#btnSend").text("查詢");

                break;
        }
    });
    $("#ddlOperator").on("change", function () {
        console.log($(this).val())
        var value = $(this).val();
        $("#ddlUserGroup").empty();
        if (value != "") {
            var Mode = $("#ddlObj").val();
            if (Mode == "Edit") {
                $("#justSearch").val(1)
                $("#UserPWD").prop("disabled", "disabled");
            }
               
            $("#frmUserMaintain").submit();
        }
    })
    $("#ddlUserGroup").on("change", function () {
        var Account = $("#Account").val();
        if ($(this).val() != "") {
         
            //BE_QueryFuncByUserGroup
            ShowLoading("資料查詢中…");
            var obj = new Object();
            obj.UserID = Account;
            obj.UserGroupID = $(this).val();
            //  DoAjax(obj, "BE_QueryFunc", "查詢權限發生錯誤");
            DoAjaxAfterCallBack(obj, "BE_QueryFuncByUserGroup", "查詢權限發生錯誤", SetMaintain);
        } else {
            $(".form-check-input").each(function () {
                $(this).prop("checked", "");
            })
        }
    });
    $("#btnSend").on("click", function () {
        var Mode = $("#ddlObj").val();
        var OperatorID = $("#ddlOperator").val();
        var Account = $("#Account").val();
        var UserGroupID = $("#ddlUserGroup").val();
        var UserAccount = $("#UserAccount").val();
        var UserName = $("#UserName").val();
        var StartDate = $("#StartDate").val();
        var EndDate = $("#EndDate").val();
        var UserPWD = $("#UserPWD").val();
        var flag = true;
        var errMsg = "";
        var hasClick = false;
        ShowLoading("資料處理中");
        var checkList = [OperatorID, UserGroupID, UserAccount, UserName, UserPWD, StartDate, EndDate];
        var errMsgList = ["業者別未選擇", "使用者群組未選擇", "員工編號未填", "員工姓名未填", "密碼未填", "有效日期（起）未填", "有效日期（迄）未填"];
        var len = checkList.length;
    
        if (flag) {
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
                var obj = new Object();
                obj.SEQNO = 0;
                obj.UserID = Account;
                obj.Operator = OperatorID;
                obj.UserGroupID = UserGroupID;
                obj.Power = GetPower();
                obj.UserAccount = UserAccount;
                obj.UserName = UserName;
                obj.UserPWD = UserPWD;
                obj.StartDate = StartDate.replace(/\//g, "").replace(/\-/g, "");
                obj.EndDate = EndDate.replace(/\//g, "").replace(/\-/g, "");
                obj.Mode = Mode;
                DoAjaxAfterReload(obj, "BE_HandleUserMaintain", "新增/修改權限發生錯誤")
            } else {
                var hasTerm = false;
                for (var i = 0; i < len; i++) {
                    if (checkList[i] != "") {
                        hasTerm=true
                        break;
                    }
                }
                if (hasTerm == false) {
                    flag = false;
                    errMsg = "至少要有一個搜尋條件";
                }
                if (flag) {
                    if (StartDate != "" && EndDate != "") {
                        if (StartDate > EndDate) {
                            flag = false;
                            errMsg = "起始日期大於結束日期";
                        }
                    }
                   
                }
                if (flag) {
                    $("#justSearch").val(0)
                    $("#frmUserMaintain").submit();
                } else {
                    disabledLoading(errMsg);
                }
            }
        } else {

            disabledLoading(errMsg);
        }
       
               


    })

})
function DoEdit(Id) {
   
    if (NowEditID > 0) {
        //先還原前一個
        /*
         var NowEditID = 0;
         
        var OperatorID = "";
        var UserGroupID = "";
        var UserName = "";
        var UserAccount = "";
        var UserPWD = "";
        var StartDate = "";
        var EndDate = "";
         */
        $("#UserAccount_" + NowEditID).val(UserAccount).hide();
        $("#UserName_" + NowEditID).val(UserName).hide();
        $("#UserPWD_" + NowEditID).val("").hide();
        $("#UserGroupName_" + NowEditID).empty().hide();
        $("#OperatorName_" + NowEditID).empty().hide();
        $("#OperatorName_" + NowEditID).unbind("change");

        $("#StartDate_" + NowEditID).val(StartDate).hide();
        $("#EndDate_" + NowEditID).val(EndDate).hide();
        $("#btnReset_" + NowEditID).hide();
        $("#btnSave_" + NowEditID).hide();
        $("#btnEdit_" + NowEditID).show();

    }

    NowEditID = Id;
    UserAccount = $("#UserAccount_" + Id).val();
    UserName = $("#UserName_" + Id).val();
    UserPWD = $("#UserPWD_" + Id).val();
    UserGroupID = $("#UserGroupID_" + Id).val();
    OperatorID = $("#OperatorID_" + Id).val();
    StartDate = $("#StartDate_" + Id).val();
    EndDate = $("#EndDate_" + Id).val();

    $("#UserAccount_" + Id).show();
    $("#UserName_" + Id).show();
    $("#UserPWD_" + Id).show();
    $("#UserGroupName_" + Id).show();
    $("#OperatorName_" + Id).show();
    $("#ddlOperator").find('option').clone().appendTo('#OperatorName_' + Id);
    $("#OperatorName_" + Id).val(OperatorID);
    $("#OperatorName_" + Id).on("change", function () {
        if ($(this).val() != "0" && $(this).val() != "") {
            var Account = $("#Account").val();
            // var Id = $(this).attr("id").val().split("_")[1];
            $("#UserGroupName_" + NowEditID).empty();
            var obj = new Object();
            obj.UserID = Account;
            obj.OperatorID = $(this).val();
            obj.UserGroupID = $("#UserGroupID_" + NowEditID).val();
            obj.NowID = NowEditID;
            DoAjaxAfterCallBack(obj, "BE_GetUserGroupByOperator","查詢使用者群組發生錯誤", SetUserGroupData);
        } else {
            $("#UserGroupName_" + NowEditID).empty();
        }
    });
    $("#OperatorName_" + Id).val(UserGroupID);
    $("#OperatorName_" + Id).trigger("click");

    $("#StartDate_" + Id).show();
    $("#EndDate_" + Id).show();

    $("#btnReset_" + Id).show();
    $("#btnSave_" + Id).show();
    $("#btnEdit_" + Id).hide();
}
function SetUserGroupData(data) {
    console.log(data);
    var len = data.UserGroup.length;
    for (var i = 0; i < len; i++) {
        optText = data.UserGroup[i].UserGroupName;
        optValue = data.UserGroup[i].USEQNO;
        $('#UserGroupName_'+data.NowID).append(`<option value="${optValue}">${optText}</option>`);
    }
    $('#UserGroupName_' + data.NowID).val(data.UserGroupID);
   /*
     optText = 'New elemenet';
        optValue = 'newElement';
        $('#selectId').append(`<option value="${optValue}">${optText}</option>`);
    */
}
function DoReset(Id) {

    $("#UserAccount_" + Id).val(UserAccount).hide();
    $("#UserName_" + Id).val(UserName).hide();
    $("#UserPWD_" + Id).val("").hide();
    $("#UserGroupName_" + Id).val(UserGroupID).hide();
    $("#OperatorName_" + Id).val(OperatorID).hide();
    $("#StartDate_" + Id).val(StartDate).hide();
    $("#EndDate_" + Id).val(EndDate).hide();

    $("#btnReset_" + Id).hide();
    $("#btnSave_" + Id).hide();
    $("#btnEdit_" + Id).show();
     NowEditID = 0;
     OperatorID = "";
     UserGroupID = "";
     UserName = "";
     UserAccount = "";
     UserPWD = "";
     StartDate = "";
     EndDate = "";
}
function DoSave(Id) {
    UserAccount = $("#UserAccount_" + Id).val();
    UserName = $("#UserName_" + Id).val();
    UserPWD = $("#UserPWD_" + Id).val();
    UserGroupID = $("#UserGroupName_" + Id).val();
    OperatorID = $("#OperatorName_" + Id).val();
    StartDate = $("#StartDate_" + Id).val();
    EndDate = $("#EndDate_" + Id).val();
    var Account = $("#Account").val();

    var flag = true;
    var errMsg = "";

    ShowLoading("資料處理中");
    var checkList = [OperatorID, UserGroupID, UserAccount, UserName,  StartDate, EndDate];
    var errMsgList = ["業者別未選擇", "使用者群組未選擇", "員工編號未填", "員工姓名未填",  "有效日期（起）未填", "有效日期（迄）未填"];

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
        if (!CheckStorageIsNull(OperatorID)) {
            OperatorID = $("#OperatorID_" + Id).val();
        }
        if (!CheckStorageIsNull(UserGroupID)) {
            UserGroupID = $("#UserGroupID_" + Id).val();
        }
    }
    if (flag) {
  
        var obj = new Object();
        obj.SEQNO = Id;
        obj.UserID = Account;
        obj.UserGroupID = UserGroupID;
        obj.Power = GetPower();
        obj.UserAccount = UserAccount;
        obj.Operator = OperatorID;
        obj.UserName = UserName;
        obj.UserPWD = UserPWD;
        obj.StartDate = StartDate.replace(/\//g, "").replace(/\-/g, "");
        obj.EndDate = EndDate.replace(/\//g, "").replace(/\-/g, "");
        obj.Mode = Mode;
        DoAjaxAfterReload(obj, "BE_HandleUserMaintain", "修改使用者者發生錯誤");
    } else {
        disabledLoading(errMsg)
    }

}
function showPower(data) {
    var dataList = JSON.parse(data);
    $(".form-check-input").each(function () {
        $(this).prop("checked", "");
    })
    var RootLen = dataList.length;
    console.log(RootLen);
    for (var i = 0; i < RootLen; i++) {
        var SubLen = dataList[i].PowerList.length;
        for (var j = 0; j < SubLen; j++) {
            var objName = dataList[i].MenuCode + "_" + dataList[i].SubMenuCode + "_" + dataList[i].PowerList[j].Code;
            console.log(objName);
            if (parseInt(dataList[i].PowerList[j].hasPower) == 1) {
                $("#" + objName).prop("checked", "checked");
            }

        }
    }
    console.log(dataList);
    $("#btnReview").trigger("click");
}
function SetMaintain(data) {
    console.log("call back")
    $("#btnReview").show();
    //$("#btnSend").text("修改後儲存");
    console.log(data);
    var RootLen = data.Power.length;
    console.log(RootLen);
    for (var i = 0; i < RootLen; i++) {
        var SubLen = data.Power[i].PowerList.length;
        for (var j = 0; j < SubLen; j++) {
            var objName = data.Power[i].MenuCode + "_" + data.Power[i].SubMenuCode + "_" + data.Power[i].PowerList[j].Code;
            console.log(objName);
            if (parseInt(data.Power[i].PowerList[j].hasPower) == 1) {
                $("#" + objName).prop("checked", "checked");
            }

        }
    }
}
function GetPower() {
    var realDataArray = new Array();
    var Len = funcList.length;
    var PowerArr = new Array();
    var hasClick = false;
    for (var i = 0; i < Len; i++) {
        var tmpObj = new Object();
        var tmp = funcList[i].split("_");
        tmpObj.MenuCode = tmp[0];
        tmpObj.SubMenuCode = tmp[1];
        tmpObj.PowerCode = tmp[2];
        tmpObj.hasPower = (($("#" + funcList[i]).prop("checked")) ? 1 : 0)
        if (tmpObj.hasPower == 1) {
            hasClick = true;
        }
        PowerArr.push(tmpObj);
    }

    var PowerArrLen = PowerArr.length;
    if (PowerArrLen > 0) {
        var PowerObj = new Object();
        PowerObj.MenuCode = PowerArr[0].MenuCode;
        PowerObj.SubMenuCode = PowerArr[0].SubMenuCode;
        PowerObj.PowerList = new Array();
        var PowerListObj = new Object();
        PowerListObj.Code = PowerArr[0].PowerCode
        PowerListObj.hasPower = PowerArr[0].hasPower
        PowerObj.PowerList.push(PowerListObj);
        realDataArray.push(PowerObj);
        for (var j = 1; j < PowerArrLen; j++) {
            isSame = (element) => element.MenuCode == PowerArr[j].MenuCode && element.SubMenuCode == PowerArr[j].SubMenuCode;
            var index = realDataArray.findIndex(isSame);
            if (index > -1) {
                var tmpPowerListObj = new Object();
                tmpPowerListObj.Code = PowerArr[j].PowerCode
                tmpPowerListObj.hasPower = PowerArr[j].hasPower
                realDataArray[index].PowerList.push(tmpPowerListObj);
            } else {
                var PowerObj = new Object();
                PowerObj.MenuCode = PowerArr[j].MenuCode;
                PowerObj.SubMenuCode = PowerArr[j].SubMenuCode;
                PowerObj.PowerList = new Array();
                var tmpPowerListObj = new Object();
                tmpPowerListObj.Code = PowerArr[j].PowerCode
                tmpPowerListObj.hasPower = PowerArr[j].hasPower
                PowerObj.PowerList.push(tmpPowerListObj);
                realDataArray.push(PowerObj);
            }
        }
        console.log(realDataArray);
        return realDataArray;
    }
}
function init() {
    $("#btnReview").hide();
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