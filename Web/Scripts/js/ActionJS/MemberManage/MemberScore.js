﻿$(function () {
    var Today = new Date();
    var Mode = $("#AuditMode").val();
    switch (Mode) {
        case "1":
            $("#NAME").show();
            $("#ID").show();
            $("#ORDER").show();
            $("#ORDER_I").hide();
            $("#DATE").show();
            $("#Import").hide();
            $("#Choice_0").hide();
            $("#Choice_1").hide();
            $("#Choice_2").hide();
            $("#SCORE").hide();
            $("#btnSubmit0").show();
            $("#btnSubmit1").show();
            $("#btnSubmit2").hide();
            $("#btnSubmit3").hide();
            //$("#btnSubmit4").hide();
            $("#AA").show();
            $("#BB").show();
            $("#CC").hide();
            $("#DD").hide();
            $("#memo").hide();
            $("#StartDate").val((Today.getFullYear() - 1) + "-" + (Today.getMonth() + 1) + "-" + Today.getDate());
            $("#EndDate").val(Today.getFullYear() + "-" + (Today.getMonth() + 1) + "-" + Today.getDate());
            break;
        case "0":
            $("#NAME").hide();
            $("#ID").show();
            $("#ORDER").hide();
            $("#ORDER_I").show();
            $("#DATE").hide();
            $("#Import").hide();
            $("#Choice_0").show();
            $("#Choice_1").show();
            $("#Choice_2").hide();
            $("#SCORE").show();
            $("#btnSubmit0").hide();
            $("#btnSubmit1").hide();
            $("#btnSubmit2").show();
            $("#btnSubmit3").hide();
            //$("#btnSubmit4").hide();
            $("#AA").hide();
            $("#BB").hide();
            $("#CC").hide();
            $("#DD").hide();
            $("#memo").show();
            break;
        case "2":
            $("#NAME").hide();
            $("#ID").hide();
            $("#ORDER").hide();
            $("#ORDER_I").hide();
            $("#DATE").hide();
            $("#Import").show();
            $("#Choice_0").hide();
            $("#Choice_1").hide();
            $("#Choice_2").hide();
            $("#SCORE").hide();
            $("#btnSubmit0").hide();
            $("#btnSubmit1").hide();
            $("#btnSubmit2").hide();
            $("#btnSubmit3").show();
            //$("#btnSubmit4").show();
            $("#AA").hide();
            $("#BB").hide();
            //$("#CC").show();
            //$("#DD").show();
            $("#memo").hide();
            break;
    }

    $("#AuditMode").on("change", function () {
        Mode = $("#AuditMode").val();
        $(".clear").val('');
        switch (Mode) {
            case "1":
                $("#NAME").show();
                $("#ID").show();
                $("#ORDER").show();
                $("#ORDER_I").hide();
                $("#DATE").show();
                $("#Import").hide();
                $("#Choice_0").hide();
                $("#Choice_1").hide();
                $("#Choice_2").hide();
                $("#SCORE").hide();
                $("#btnSubmit0").show();
                $("#btnSubmit1").show();
                $("#btnSubmit2").hide();
                $("#btnSubmit3").hide();
                //$("#btnSubmit4").hide();
                $("#AA").show();
                $("#BB").show();
                $("#CC").hide();
                $("#DD").hide();
                $("#memo").hide();
                $("#StartDate").val((Today.getFullYear()-1) + "-" + (Today.getMonth() + 1) + "-" + Today.getDate());
                $("#EndDate").val(Today.getFullYear() + "-" + (Today.getMonth() + 1) + "-" + Today.getDate());
                break;
            case "0":
                $("#NAME").hide();
                $("#ID").show();
                $("#ORDER").hide();
                $("#ORDER_I").show();
                $("#DATE").hide();
                $("#Import").hide();
                $("#Choice_0").show();
                $("#Choice_1").show();
                $("#Choice_2").hide();
                $("#SCORE").show();
                $("#btnSubmit0").hide();
                $("#btnSubmit1").hide();
                $("#btnSubmit2").show();
                $("#btnSubmit3").hide();
                //$("#btnSubmit4").hide();
                $("#AA").hide();
                $("#BB").hide();
                $("#CC").hide();
                $("#DD").hide();
                $("#memo").show();
                break;
            case "2":
                $("#NAME").hide();
                $("#ID").hide();
                $("#ORDER").hide();
                $("#ORDER_I").hide();
                $("#DATE").hide();
                $("#Import").show();
                $("#Choice_0").hide();
                $("#Choice_1").hide();
                $("#Choice_2").hide();
                $("#SCORE").hide();
                $("#btnSubmit0").hide();
                $("#btnSubmit1").hide();
                $("#btnSubmit2").hide();
                $("#btnSubmit3").show();
                //$("#btnSubmit4").show();
                $("#AA").hide();
                $("#BB").hide();
                //$("#CC").show();
                //$("#DD").show();
                $("#memo").hide();
                break;
        }
    });

    $("#ddlOperatorGG").on("change", function () {
        var value = $(this).val();
        $("#ddlUserGroup").empty();
        if (value != "") {
            //var Mode = $("#ddlObj").val();
            //if (Mode == "Edit") {
            //    $("#justSearch").val(1)
            //    $("#UserPWD").prop("disabled", "disabled");
            //}
            $("#justSearch").val(1)
            //$("#AuditMode").val(1)
            $("#frmMemberScore").submit();
        }
    })

    $("#ddlUserGroup").on("change", function () {
        var value = $(this).val();
        $("#scoreman").empty();
        if (value != "") {
            $("#justSearch").val(1)
            $("#frmMemberScore").submit();
        }
    })

    $("#ORDER_I").on("change", function () {
        ShowLoading("資料查詢中…");
        var a = $("#AuditMode").val()
        var b = parseInt(a) + 3
        var c = b.toString();

        $("#AuditMode").val(3); //我只要設值，controller就只會抓到null，但我的邏輯是else就處理，所以null也可以達到我的要求
        if ($("#ORDERNO_I").val() != "") {
            $("#frmMemberScore").submit();
        }
        disabledLoading();
    });

    $("#btnSubmit0").on("click", function () {
        ShowLoading("資料查詢中…");
        var SD = $("#StartDate").val();
        var ED = $("#EndDate").val();
        var flag = true;
        var errMsg = "";
        if ($("#MEMNAME").val() == "" && ($("#IDNO").val() == "" && $("#ORDERNO").val() == "")) {
            flag = false;
            errMsg = "請輸入姓名";
        }
        if ($("#IDNO").val() == "" && $("#ORDERNO").val() != "") {
            flag = false;
            errMsg = "請輸入ID";
        }
        if ($("#ORDERNO").val() != "" && false == RegexOrderNo($("#ORDERNO").val())) {
            flag = false;
            errMsg = "合約編號格式不符（格式：H+數字)";
        }
        if (SD !== "" && ED !== "") {
            if (SD > ED) {
                flag = false;
                errMsg = "起始日期大於結束日期";
            }
            //else {
            //    var GetDateDiff = DateDiff(SD, ED);
            //    if (GetDateDiff > 30) {
            //        flag = false;
            //        errMsg = "時間區間不可大於30天";
            //    }
            //}
        } else {
            flag = false;
            errMsg = "未選擇日期";
        }
        if (flag) {
            $("#frmMemberScore").submit();
            disabledLoading();

        } else {
            disabledLoadingAndShowAlert(errMsg);
        }

    });

    $("#btnSubmit1").on("click", function () {
        ShowLoading("資料查詢中…");
        var SD = $("#StartDate").val();
        var ED = $("#EndDate").val();
        var flag = true;
        var errMsg = "";

        if ($("#IDNO").val() == "" && $("#MEMNAME").val() == "" ) {
            flag = false;
            errMsg = "請輸入ID或姓名";
        }

        if ($("#ORDERNO").val() != "" && false == RegexOrderNo($("#ORDERNO").val())) {
            flag = false;
            errMsg = "合約編號格式不符（格式：H+數字)";
        }

        if (SD !== "" && ED !== "") {
            if (SD > ED) {
                flag = false;
                errMsg = "起始日期大於結束日期";
            }
            else {
                var GetDateDiff = DateDiff(SD, ED);
                if (GetDateDiff > 30) {
                    flag = false;
                    errMsg = "時間區間不可大於30天";
                }
            }
        } else {
            flag = false;
            errMsg = "未選擇日期";
        }
        if (flag) {
            $("#ExplodeIDNO").val($("#IDNO").val());
            $("#ExplodeNAME").val($("#MEMNAME").val());
            $("#ExplodeORDER").val($("#ORDERNO").val());
            $("#ExplodeSDate").val($("#StartDate").val());
            $("#ExplodeEDate").val($("#EndDate").val());
            disabledLoading();
            $("#frmMemberScoreExplode").submit();

        } else {
            disabledLoadingAndShowAlert(errMsg);
        }

    });

    $("#btnSubmit2").on("click", function () {
        ShowLoading("資料查詢中…");
        var flag = true;
        var errMsg = "";

        if ($("#ORDERNO_I").val() == "" && $("#ddlOperatorGG").val() != "其他") {
            flag = false;
            errMsg = "請輸入合約";
        }
        if ($("#ORDERNO_I").val() != "" && $("#ddlOperatorGG").val() != "其他") {
            if (false == RegexOrderNo($("#ORDERNO_I").val())) {
                flag = false;
                errMsg = "合約編號格式不符（格式：H+數字)";
            }
        }


        if ($("#IDNO").val() == "") {
            flag = false;
            errMsg = "請輸入ID";
        }
        if ($("#MEMSCORE").val() == "") {
            flag = false;
            errMsg = "請輸入分數";
        }
        if ($("#sonmemo").val() == "" && $("#ddlOperatorGG").val() == "其他") {
            flag = false;
            errMsg = "請填備註";
        }
        
        if (flag) {
            $("#justSearch").val(0)
            $("#frmMemberScore").submit();
            disabledLoading();

        } else {
            disabledLoadingAndShowAlert(errMsg);
        }
    });

    $("#exampleDownload").on("click", function () {
        window.location.href = "../Content/example/MemberScoreExample.xlsx";
    });

    $("#btnSubmit3").on("click", function () {
        ShowLoading("資料匯入中");
        $("#frmMemberScore").submit();
        disabledLoading();
    });

    $("#fileImport").on("change", function () {
        var file = this.files[0];
        var fileName = file.name;
        var ext = GetFileExtends(fileName);
        var extName = "";
        if (CheckStorageIsNull(ext)) {
            extName = ext[0];
            console.log(extName.toUpperCase())
        }
        if (extName.toUpperCase() != "XLSX" && extName.toUpperCase() != "XLS") {

            swal({
                title: 'Fail',
                text: "僅允許匯入xlsx或xls格式",
                icon: 'error'
            }).then(function (value) {

                $("#fileImport").val("");
            });
        }
    })

    $("#MEMSCORE").val($("#scoreman").val());
});


var DateDiff = function (sDate1, sDate2) { // sDate1 和 sDate2 是 2016-06-18 格式
    var aDate, oDate1, oDate2, iDays
    aDate = sDate1.split("/")
    oDate1 = new Date(aDate[1] + '/' + aDate[2] + '/' + aDate[0]) // 轉換為 06/18/2016 格式
    aDate = sDate2.split("/")
    oDate2 = new Date(aDate[1] + '/' + aDate[2] + '/' + aDate[0])
    iDays = parseInt(Math.abs(oDate1 - oDate2) / 1000 / 60 / 60 / 24) // 把相差的毫秒數轉換為天數
    return iDays;
};


var NowEditID = 0;
function DoEdit(Id) {
    if (NowEditID > 0) {
        //$("#UserSon_" + NowEditID).val(UserSon).hide();
        //$("#UserScore_" + NowEditID).val(UserScore).hide();
        //$("#UserApp_" + NowEditID).val(UserApp).hide();
        $("#UserSon_" + NowEditID).hide();
        $("#UserScore_" + NowEditID).hide();
        $("#UserApp_" + NowEditID).hide();
        $("#btnReset_" + NowEditID).hide();
        $("#btnSave_" + NowEditID).hide();
        $("#btnEdit_" + NowEditID).show();
        $("#btnDel_" + NowEditID).show();
    }

    NowEditID = Id;
    //UserSon = $("#UserAccount_" + Id).val();
    //UserScore = $("#UserName_" + Id).val();
    //UserApp = $("#UserName_" + Id).val();

    $("#UserSon_" + Id).show();
    $("#UserScore_" + Id).show();
    $("#UserApp_" + Id).show();
    $("#btnReset_" + Id).show();
    $("#btnSave_" + Id).show();
    $("#btnEdit_" + Id).hide();
    $("#btnDel_" + Id).hide();
}
function DoReset(Id) {

    $("#UserSon_" + NowEditID).hide();
    $("#UserScore_" + NowEditID).hide();
    $("#UserApp_" + NowEditID).hide();
    $("#btnReset_" + NowEditID).hide();
    $("#btnSave_" + NowEditID).hide();
    $("#btnEdit_" + NowEditID).show();
    $("#btnDel_" + NowEditID).show();

    NowEditID = 0;
    //UserSon = "";
    //UserScore = "";
    //UserApp = "";
}
function DoSave(Id) {
    var UserSon = $("#UserSon_" + Id).val();
    var UserScore = $("#UserScore_" + Id).val();
    var UserApp = $("#UserApp_" + Id).val();
    var SEQ = $("#UserSeq_" + Id).val();
    var Account = $("#Account").val();
    var IDNO = $("#UserId_" + Id).val();

    var flag = true;
    var errMsg = "";

    ShowLoading("資料處理中");
    var checkList = [UserScore, UserApp];
    var errMsgList = ["分數未填", "APP未填"];

    var len = checkList.length;
    for (var i = 0; i < len; i++) {
        if (checkList[i] == "") {
            flag = false;
            errMsg = errMsgList[i];
            break;
        }
    }

    if (UserScore==0) {
        flag = false;
        errMsg = "分數不能是0";
    }

    if (flag) {
        var obj = new Object();
        obj.IDNO = IDNO;
        obj.SON = UserSon;
        obj.SCORE = UserScore;
        obj.APP = UserApp;
        obj.SEQ = SEQ;
        obj.UserID = Account;

        DoAjaxAfterReload(obj, "BE_HandleMemberScore", "修改花生錯誤");
    } else {
        disabledLoadingAndShowAlert(errMsg)
    }

}
function DoDel(Id) {
    var SEQ = $("#UserSeq_" + Id).val();
    var Account = $("#Account").val();
    var IDNO = $("#UserId_" + Id).val();

    ShowLoading("資料處理中");

    var obj = new Object();
    obj.IDNO = IDNO;
    obj.SEQ = SEQ;
    obj.UserID = Account;

    DoAjaxAfterReload(obj, "BE_DeleteMemberScore", "修改花生錯誤");
}