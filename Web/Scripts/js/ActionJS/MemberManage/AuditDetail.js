var Obj;
var ID_1, ID_2, Car_1, Car_2, Motor_1, Motor_2, Self_1, F01, Signture_1, Other_1, Business_1;
var ID_1_Audit, ID_2_Audit, Car_1_Audit, Car_2_Audit, Motor_1_Audit, Motor_2_Audit, Self_1_Audit, F01_Audit, Signture_1_Audit, Other_1_Audit, Business_1_Audit;
var ID_1_Reason = "", ID_2_Reason = "", Car_1_Reason = "", Car_2_Reason = "", Motor_1_Reason = "", Motor_2_Reason = "", Self_1_Reason = "", F01_Reason = "", Signture_1_Reason = "", Other_1_Reason = "", Business_1_Reason = "";
var field = [ID_1, ID_2, Car_1, Car_2, Motor_1, Motor_2, Self_1, F01, Signture_1, Other_1, Business_1];
var fieldAudit = [ID_1_Audit, ID_2_Audit, Car_1_Audit, Car_2_Audit, Motor_1_Audit, Motor_2_Audit, Self_1_Audit, F01_Audit, Signture_1_Audit, Other_1_Audit, Business_1_Audit];
var fieldReason = [ID_1_Reason, ID_2_Reason, Car_1_Reason, Car_2_Reason, Motor_1_Reason, Motor_2_Reason, Self_1_Reason, F01_Reason, Signture_1_Reason, Other_1_Reason, Business_1_Reason];
var fieldName = ["ID_1", "ID_2", "Car_1", "Car_2", "Motor_1", "Motor_2", "Self_1", "F01", "Signture_1", "Other_1", "Business_1"];
var fieldCName = ["身份證正面", "身份證背面", "汽車駕照正面", "汽車駕照背面", "機車駕照正面", "機車駕照背面", "自拍照", "法定代理人同意書", "簽名檔", "其他證件", "企業用戶"];
var fieldLen = field.length;
var Account = "";
$(function () {
    Account = $("#Account").val();

    if (CityID != "0" && AreaID != "0") {
        SetCityHasSelected($("#City"), CityID, $("#Area"), AreaID);
    } else {
        if (CityID == 0) {
            SetCity($("#City"));
        }
    }
    $("#City").on("change", function () {
        SetArea($("#Area"),$(this).val())
    })
    $("#NotAuditReason").on("change", function () {
        if ($(this).val() == "其他") {
            $("#RejectReason").prop("readonly", "");
        } else {
            //$("#RejectReason").prop("readonly", "readonly");
        }
    });
    $("input[name='AuditStatus']").on("click", function () {
        console.log($(this).val())
        if ($(this).val() == "-1") {
            $("#NotAuditReason").prop("disabled", "");
        } else {
            //$("#NotAuditReason").prop("disabled", "disabled");
            $('#NotAuditReason').val('');
        }
        if ($('#NotAuditReason').val() == '其他') {
            $("#RejectReason").prop("readonly", "");
            $("#RejectReason").prop("disabled", "");
        } else {
            //$("#RejectReason").prop("readonly", "readonly");
            $('#RejectReason').val('');
        }
    });

    //20201124 UPD BY JERRY 增加載入時，欄位狀態處理
    if ($("#AuditReject").prop("checked")) {
        $("#NotAuditReason").prop("disabled", "");
        setTimeout(function () {
            if ($('#NotAuditReason').val() == '其他') {
                $("#RejectReason").prop("readonly", "");
                $("#RejectReason").prop("disabled", "");
            }
        }, 300);
    }


    for (var i = 0; i < fieldLen; i++) {
        $("input[name='" + fieldName[i] + "_AuditStatus']").on("click", function () {
            var textName = $(this).attr("name").replace("AuditStatus", "RejectReason");
            console.log(textName);
            if ($(this).val() == "-1") {
                console.log($(this).attr("name"));
                $("#" + textName).prop("readonly", "");
            } else {
                $("#" + textName).val("");
                $("#" + textName).prop("readonly", "readonly");
            }
        });
    }
    $("#btnSend").on("click", function () {
        ShowLoading("資料處理中…");
        Account = $("#Account").val();
        var flag = true;
        var errMsg = "";
        var SendObj = new Object();
        var Driver = new Array();
        var SPECSTATUS = $("#SPECSTATUS").val();
        var SPSD = $("#StartDate").val().replace(/\-/g,'');
        var SPED = $("#EndDate").val().replace(/\-/g, '');
        var Birth = $("#Birth").val();
        var Mobile = $("#Mobile").val();
        var Addr = $("#Addr").val();
        var InvoiceType = $("#InvoiceType").val();
        var UniCode = $("#UniCode").val();
        var AuditStatus = (CheckStorageIsNull($("input[name='AuditStatus']:checked").val())) ? parseInt($("input[name='AuditStatus']:checked").val()):0;
        var NotAuditReason = $("#NotAuditReason").val();
        var RejectReason = $("#RejectReason").val();
        var Area = $("#Area").val();
        //var SendMessage = (CheckStorageIsNull($("#SendMessage").val())) ? parseInt($("#SendMessage").val()) : 0;
        //20201124 UPD BY JERRY 修改發送簡訊判斷
        var SendMessage = $("#SendMessage").prop("checked") ? 1 : 0;


        //20201125 UPD BY JERRY 增加欄位處理
        var MEMHTEL = $("#MEMHTEL").val();
        var MEMCOMTEL = $("#MEMCOMTEL").val();
        var MEMCONTRACT = $("#MEMCONTRACT").val();
        var MEMCONTEL = $("#MEMCONTEL").val();
        var MEMEMAIL = $("#MEMEMAIL").val();
        var HasVaildEMail = $("#HasVaildEMail_OK").prop("checked") ? 1 : 0;
        var MEMMSG = $("#MEMMSG_OK").prop("checked") ? 'Y' : 'N';
        
        //console.log("AuditStatus=" + AuditStatus);
        $("input[name='Driver']:checked").each(function () {
            console.log($(this).val());
            Driver.push($(this).val());
        });
        if (AuditStatus != -1) {
            if (Driver.length == 0) {
                flag = false;
                errMsg = "請至少選擇一種駕照類型";
            }
        }
        //if (flag) {
        //    $('.sameMobile').each(function () {
        //        if ($.trim($(this).html()) == $.trim(Mobile)) {
        //            flag = false;
        //            errMsg = "手機號碼重複，請確認手機正確性，並修改手機號碼！";
        //        }
        //    });
        //}
        if (flag) {
            if (SPECSTATUS != "00") {
                if (SPSD == "" && SPED == "") {
                    flag = false;
                    errMsg = "請選擇特殊身份有效日期(起迄)";
                } else if (SPSD == "" && SPED != "") {
                    flag = false;
                    errMsg = "請選擇特殊身份有效日期(起)";
                } else if (SPSD != "" && SPED == "") {
                    flag = false;
                    errMsg = "請選擇特殊身份有效日期(迄)";
                }
            }
        }
        if (flag) {
            if (AuditStatus == 0) {
                flag = false;
                errMsg = "請選擇是否審核通過";
            } else if (AuditStatus == -1) {
                if (NotAuditReason == "") {
                    flag = false;
                    errMsg = "請選擇不通過的原因說明";
                } else if (NotAuditReason == "其他" && RejectReason == "") {
                    flag = false;
                    errMsg = "不通過的原因選擇其他，需填寫說明";
                }
            }
        }
        if (flag) {
            if (HasVaildEMail == 1 && MEMEMAIL=="") {
                flag = false;
                errMsg = "請輸入正確的EMAIL欄位";
            } 
        }
        if (flag) {
            if (InvoiceType == "0") {
                flag = false;
                errMsg = "請選擇發票寄送方式";
            }
        }
        if (flag) {
            if (Addr == "") {
                flag = false;
                errMsg = "請輸入地址";
            }
        }
        if (flag) {
            if (Mobile == "") {
                flag = false;
                errMsg = "請輸入手機號碼";
            }
        }
        setData();

        if (flag) {
            if ($.trim(Obj.ID_1_Audit) == "") {
                flag = false;
                errMsg = "請勾選【身份證正面】的照片審核結果";
            }
            if ($.trim(Obj.ID_2_Audit) == "") {
                flag = false;
                errMsg = "請勾選【身份證正面】的照片審核結果";
            }
            if ($.trim(Obj.Car_1_Audit) == "") {
                flag = false;
                errMsg = "請勾選【汽車駕照正面】的照片審核結果";
            }
            if ($.trim(Obj.Car_2_Audit) == "") {
                flag = false;
                errMsg = "請勾選【汽車駕照反面】的照片審核結果";
            }
            if ($.trim(Obj.Motor_1_Audit) == "") {
                flag = false;
                errMsg = "請勾選【機車駕照正面】的照片審核結果";
            }
            if ($.trim(Obj.Motor_2_Audit) == "") {
                flag = false;
                errMsg = "請勾選【機車駕照反面】的照片審核結果";
            }
            if ($.trim(Obj.Self_1_Audit) == "") {
                flag = false;
                errMsg = "請勾選【自拍照】的照片審核結果";
            }
            if ($.trim(Obj.F01_Audit) == "") {
                flag = false;
                errMsg = "請勾選【法定代理人】的照片審核結果";
            }
            if ($.trim(Obj.Other_1_Audit) == "") {
                flag = false;
                errMsg = "請勾選【其他證件】的照片審核結果";
            }
            if ($.trim(Obj.Business_1_Audit) == "") {
                flag = false;
                errMsg = "請勾選【企業用戶】的照片審核結果";
            }
            if ($.trim(Obj.Signture_1_Audit) == "") {
                flag = false;
                errMsg = "請勾選【簽名檔】的照片審核結果";
            }
        }
        if (flag) {
           
            SendObj.ImageData = Obj;
            SendObj.IDNO = $("#IDNO").val();
            SendObj.Driver = Driver;
            SendObj.SPECSTATUS = SPECSTATUS;
            SendObj.SPSD = SPSD;
            SendObj.SPED = SPED;
            SendObj.Birth = Birth;
            SendObj.Mobile = Mobile;
            SendObj.Area = Area;
            SendObj.Addr = Addr;
            SendObj.UniCode = UniCode;
            SendObj.InvoiceType = InvoiceType;
            SendObj.AuditStatus = AuditStatus;
            SendObj.NotAuditReason = NotAuditReason;
            SendObj.RejectReason = RejectReason;
            SendObj.UserID = Account;
            SendObj.SendMessage = SendMessage;
            SendObj.IsNew = IsNew;

            //20201125 UPD BY JERRY 增加欄位處理
            SendObj.MEMHTEL = MEMHTEL;
            SendObj.MEMCOMTEL = MEMCOMTEL;
            SendObj.MEMCONTRACT = MEMCONTRACT;
            SendObj.MEMCONTEL = MEMCONTEL;
            SendObj.MEMEMAIL = MEMEMAIL;
            SendObj.HasVaildEMail = HasVaildEMail;
            SendObj.MEMMSG = MEMMSG;
            

            DoAjaxAfterGoBack(SendObj, "BE_Audit", "審核發生錯誤");
            
        } else {
            disabledLoadingAndShowAlert(errMsg);
        }
       
        if (parseInt(MobileLen) > 0 || parseInt(HistoryLen)>0) {
                $('.table').footable({
                    "paging": {
                        "enabled": true,
                        "limit": 3,
                        "size": 20
                    }
                });
            }
        

    });
    $("#btnSavePIC").on("click", function () {
        ShowLoading("資料暫存中…");
        var flag = true;
        var errmsg = "";
        var reject = 0;
        //第一步先assign value
        for (var i = 0; i < fieldLen; i++) {
            field[i] = $("#" + fieldName[i]).val(); 
        }
        for (var i = 0; i < fieldLen; i++) {
            for (var j = 0; j < fieldLen; j++) {
                if (i != j) {
                    if (field[i] == field[j]) {
                        flag = false;
                        errmsg = fieldCName[i] + "與" + fieldCName[j] + "重覆";
                        break;
                    }
                }
             
            }
            if (flag) {
                fieldAudit[i] = $("input[name='" + fieldName[i] + "_AuditStatus']:checked").val();
                console.log($("input[name='" + fieldName[i] + "_AuditStatus']:checked").val())
                fieldReason[i] = $("#" + fieldName[i] + "_RejectReason").val();
                if ($("input[name='" + fieldName[i] + "_AuditStatus']:checked").val() == "-1" && $("#" + fieldName[i] + "_RejectReason").val() == "") {
                    flag = false;
                    errmsg = fieldCName[i] + "未寫不通過原因";
                    break;
                } else if ($("input[name='" + fieldName[i] + "_AuditStatus']:checked").val() == "-1" && $("#" + fieldName[i] + "_RejectReason").val() != "") {

                    reject = 1;
                } 
            }
        }
        setData();
        if (flag) {
            if ($.trim(Obj.ID_1_Audit) == "") {
                flag = false;
                errmsg = "請勾選【身份證正面】的照片審核結果";
            }
            if ($.trim(Obj.ID_2_Audit) == "") {
                flag = false;
                errmsg = "請勾選【身份證正面】的照片審核結果";
            }
            if ($.trim(Obj.Car_1_Audit) == "") {
                flag = false;
                errmsg = "請勾選【汽車駕照正面】的照片審核結果";
            }
            if ($.trim(Obj.Car_2_Audit) == "") {
                flag = false;
                errmsg = "請勾選【汽車駕照反面】的照片審核結果";
            }
            if ($.trim(Obj.Motor_1_Audit) == "") {
                flag = false;
                errmsg = "請勾選【機車駕照正面】的照片審核結果";
            }
            if ($.trim(Obj.Motor_2_Audit) == "") {
                flag = false;
                errmsg = "請勾選【機車駕照反面】的照片審核結果";
            }
            if ($.trim(Obj.Self_1_Audit) == "") {
                flag = false;
                errmsg = "請勾選【自拍照】的照片審核結果";
            }
            if ($.trim(Obj.F01_Audit) == "") {
                flag = false;
                errmsg = "請勾選【法定代理人】的照片審核結果";
            }
            if ($.trim(Obj.Other_1_Audit) == "") {
                flag = false;
                errmsg = "請勾選【其他證件】的照片審核結果";
            }
            if ($.trim(Obj.Business_1_Audit) == "") {
                flag = false;
                errmsg = "請勾選【企業用戶】的照片審核結果";
            }
            if ($.trim(Obj.Signture_1_Audit) == "") {
                flag = false;
                errmsg = "請勾選【簽名檔】的照片審核結果";
            }
        }
        if (flag) {
            console.log(JSON.stringify(Obj));
            $("#tmpPicData").val(JSON.stringify(Obj));
            $("#picreject").val(reject);
            disabledLoading();
            $('#surrounding_modal').modal('hide')
        } else {
            disabledLoadingAndShowAlert(errmsg);
            console.log(errmsg);
        }
    });

    if (MobileLen > 0) {
        $('#btnCheckSameMobile').click();
    }
    setPostbackValue();
})
function ShowPIC(site) {
    if (site != "") {
        window.open(site);
    }
}
function setData() {
    for (var i = 0; i < fieldLen; i++) {
        field[i] = $("#" + fieldName[i]).val();
        fieldAudit[i] = $("input[name='" + fieldName[i] + "_AuditStatus']:checked").val();
        fieldReason[i] = $("#" + fieldName[i] + "_RejectReason").val();
    }
    Obj = new Object();
    Obj.ID_1 = 1;
    Obj.ID_1_new = change(field[0] );
    Obj.ID_1_Audit =  fieldAudit[0] ;
    Obj.ID_1_Reason = fieldReason[0];
    Obj.ID_1_Image = $("#ID_1_PIC").attr('src');

    Obj.ID_2 = 2;
    Obj.ID_2_new = change(field[1] );
    Obj.ID_2_Audit = fieldAudit[1] ;
    Obj.ID_2_Reason = fieldReason[1];
    Obj.ID_2_Image = $("#ID_2_PIC").attr('src');

    Obj.Car_1 = 3;
    Obj.Car_1_new = change(field[2] );
    Obj.Car_1_Audit = fieldAudit[2] ;
    Obj.Car_1_Reason = fieldReason[2];
    Obj.Car_1_Image = $("#Car_1_PIC").attr('src');

    Obj.Car_2 = 4;
    Obj.Car_2_new = change(field[3] );
    Obj.Car_2_Audit = fieldAudit[3] ;
    Obj.Car_2_Reason = fieldReason[3];
    Obj.Car_2_Image = $("#Car_2_PIC").attr('src');

    Obj.Motor_1 = 5;
    Obj.Motor_1_new = change(field[4] );
    Obj.Motor_1_Audit = fieldAudit[4] ;
    Obj.Motor_1_Reason = fieldReason[4];
    Obj.Motor_1_Image = $("#Motor_1_PIC").attr('src');

    Obj.Motor_2 = 6;
    Obj.Motor_2_new = change(field[5] );
    Obj.Motor_2_Audit = fieldAudit[5] ;
    Obj.Motor_2_Reason = fieldReason[5];
    Obj.Motor_2_Image = $("#Motor_2_PIC").attr('src');

    Obj.Self_1 = 7;
    Obj.Self_1_new = change(field[6] );
    Obj.Self_1_Audit = fieldAudit[6] ;
    Obj.Self_1_Reason = fieldReason[6];
    Obj.Self_1_Image = $("#Self_1_PIC").attr('src');

    Obj.F01 = 8;
    Obj.F01_new = change(field[7] );
    Obj.F01_Audit =  fieldAudit[7] ;
    Obj.F01_Reason = fieldReason[7];
    Obj.F01_Image = $("#F01_PIC").attr('src');

    Obj.Other_1 = 9;
    Obj.Other_1_new = change(field[9] );
    Obj.Other_1_Audit =  fieldAudit[9] ;
    Obj.Other_1_Reason = fieldReason[9];
    Obj.Other_1_Image = $("#Other_1_PIC").attr('src');

    Obj.Business_1 = 10;
    Obj.Business_1_new = change(field[10] );
    Obj.Business_1_Audit = fieldAudit[10];
    Obj.Business_1_Reason =  fieldReason[10];
    Obj.Business_1_Image = $("#Business_1_PIC").attr('src');

    Obj.Signture_1 = 11;
    Obj.Signture_1_new = change(field[8] );
    Obj.Signture_1_Audit = fieldAudit[8] ;
    Obj.Signture_1_Reason = fieldReason[8];
    Obj.Signture_1_Image = $("#Signture_1_PIC").attr('src');
}
function change(NewKind) {
    var type=0
    switch (NewKind) {
        case "ID_1":
            type = 1;
            break;
        case "ID_2":
            type = 2;
            break;
        case "Car_1":
            type = 3;
            break;
        case "Car_2":
            type = 4;
            break;
        case "Motor_1":
            type = 5;
            break;
        case "Motor_2":
            type = 6;
            break;
        case "Self_1":
            type = 7;
            break;
        case "F01":
            type = 8;
            break;
        case "Other_1":
            type = 9;
            break;
        case "Business_1":
            type = 10;
            break;
        case "Signture_1":
            type = 11;
            break;
    }
    return type;
}