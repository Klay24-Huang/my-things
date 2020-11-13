﻿var funcList;
$(document).ready(function () {

    var func = $("#funcName").val();

    if (func != "") {
        funcList = JSON.parse(func);
        console.log(funcList);
    }

    if (Mode == "") {
        init();
    } else {
        $("#ddlObj").val(Mode);
        $("#ddlObj").trigger("change");
        if (Mode == "Add") {
            $("#btnSend").text("儲存");
            $("#liPower").hide();
        } else if(Mode=="Edit") {
            $("#btnSend").text("查詢");
            $("#liPower").hide();
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
                $("#btnSend").show();
                $("#btnReset").show();
                $("#btnSend").text("儲存");

                break;
            case "Edit":

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
        var flag = true;
        var errMsg = "";
        var hasClick = false;
        if (Mode == "Add") {

            var obj = new Object();
            obj.UserID = Account;
            obj.UserGroupID = UserGroupID;
            obj.Power = GetPower();
            obj.UserAccount = UserAccount;
            obj.UserName = UserName;
            obj.StartDate = StartDate.replace(/\//g, "").replace(/\-/g, "");
            obj.EndDate = EndDate.replace(/\//g, "").replace(/\-/g, "");
            obj.Mode = Mode;
            DoAjaxAfterReload(obj, "BE_HandleUserMaintain", "新增/修改權限發生錯誤")
        } else {

        }
               


    })

})
function SetMaintain(data) {
    console.log("call back")
    $("#liPower").show();
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
    $("#liPower").hide();
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