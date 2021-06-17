var Obj;
var ID_1, ID_2, Car_1, Car_2, Motor_1, Motor_2, Self_1, F01, Signture_1, Other_1, Business_1;
var ID_1_Audit, ID_2_Audit, Car_1_Audit, Car_2_Audit, Motor_1_Audit, Motor_2_Audit, Self_1_Audit, F01_Audit, Signture_1_Audit, Other_1_Audit, Business_1_Audit;
var ID_1_Reason = "", ID_2_Reason = "", Car_1_Reason = "", Car_2_Reason = "", Motor_1_Reason = "", Motor_2_Reason = "", Self_1_Reason = "", F01_Reason = "", Signture_1_Reason = "", Other_1_Reason = "", Business_1_Reason = "";
var field = [ID_1, ID_2, Car_1, Car_2, Motor_1, Motor_2, Self_1, F01, Signture_1, Other_1, Business_1];
var fieldAudit = [ID_1_Audit, ID_2_Audit, Car_1_Audit, Car_2_Audit, Motor_1_Audit, Motor_2_Audit, Self_1_Audit, F01_Audit, Signture_1_Audit, Other_1_Audit, Business_1_Audit];
var fieldReason = [ID_1_Reason, ID_2_Reason, Car_1_Reason, Car_2_Reason, Motor_1_Reason, Motor_2_Reason, Self_1_Reason, F01_Reason, Signture_1_Reason, Other_1_Reason, Business_1_Reason];
var fieldName = ["ID_1", "ID_2", "Car_1", "Car_2", "Motor_1", "Motor_2", "Self_1", "F01", "Other_1", "Business_1", "Signture_1"];
var fieldCName = ["身份證正面", "身份證背面", "汽車駕照正面", "汽車駕照背面", "機車駕照正面", "機車駕照背面", "自拍照", "法定代理人同意書", "其他證件", "企業用戶", "簽名檔"];
var fieldLen = field.length;
var Account = "";
$(function () {
    Account = $("#Account").val();

    //20210105 ADD BY ADAM REASON.city有值就綁定
    //if (CityID != "0" && AreaID != "0") {
    if (CityID != "0") {
        SetCityHasSelected($("#City"), CityID, $("#Area"), AreaID);
        SetArea($("#Area"), $("#City").val())
    } else {
        if (CityID == "0") {
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
            //console.log(textName);
            if ($(this).val() == "-1") {
                //console.log($(this).attr("name"));
                $("#" + textName).prop("readonly", "");
            } else {
                $("#" + textName).val("");
                $("#" + textName).prop("readonly", "readonly");
            }
        });
    }

    $('#btnAllCheckPIC').click(function () {
        for (var i = 0; i < fieldLen; i++) {
            //console.log("#" + fieldName[i] + "_AuditOK")
            $("#" + fieldName[i] + "_AuditOK").prop("checked", true);
            $("#" + fieldName[i] + "_RejectReason").val("");
            $("#" + fieldName[i] + "_RejectReason").prop("readonly", "readonly");
        }
    });

    $("#btnSend").on("click", function () {
        ShowLoading("資料處理中…");
        Account = $("#Account").val();
        var flag = true;
        var errMsg = "";
        var SendObj = new Object();
        var Driver = new Array();
        var SPECSTATUS = $.trim($("#SPECSTATUS").val());
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
        //console.log($("#SPECSTATUS").val())


        //20201125 UPD BY JERRY 增加欄位處理
        var MEMHTEL = $("#MEMHTEL").val();
        var MEMCOMTEL = $("#MEMCOMTEL").val();
        var MEMCONTRACT = $("#MEMCONTRACT").val();
        var MEMCONTEL = $("#MEMCONTEL").val();
        var MEMEMAIL = $("#MEMEMAIL").val();
        var HasVaildEMail = $("#HasVaildEMail_OK").prop("checked") ? 1 : 0;
        var MEMMSG = $("#MEMMSG_OK").prop("checked") ? 'Y' : 'N';

        //20210115 UPD BY 堂尾鰭 增加備註欄位處理
        var MEMONEW = $("#MEMO_NEW").val();
        
        //console.log("AuditStatus=" + AuditStatus);
        $("input[name='Driver']:checked").each(function () {
            console.log($(this).val());
            if ($(this).val() != 'CarDriver1') {
                Driver.push($(this).val());
            }
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
            if (SPECSTATUS != "00" && SPECSTATUS != "" && SPECSTATUS != "01") {
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
            if ($.trim(Area) == "") {
                flag = false;
                errMsg = "請輸入地區";
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

            var y = new Date();
            //console.log(y.getFullYear());
            //console.log(y.getFullYear() - ('1997-01-16 00:00:00.000').substr(0, 4));
            //console.log($("#Birth").val())
            //20210312唐加判斷:20歲以下才要審
            if ($.trim(Obj.F01_Audit) == "0" && (y.getFullYear() - $("#Birth").val().substr(0, 4)) < 20) {
                flag = false;
                errMsg = "請勾選【法定代理人】的照片審核結果";
            }
            /* 20210312唐註解，天0說不要
            if ($.trim(Obj.Other_1_Audit) == "") {
                flag = false;
                errMsg = "請勾選【其他證件】的照片審核結果";
            }
            */
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
            SendObj.AuditStatus = AuditStatus;//-1不通過，否則就通過
            SendObj.NotAuditReason = NotAuditReason;
            SendObj.RejectReason = RejectReason;
            SendObj.UserID = Account;
            SendObj.SendMessage = SendMessage;
            SendObj.IsNew = IsNew; //1是新加入，不然就是變更身分

            //20201125 UPD BY JERRY 增加欄位處理
            SendObj.MEMHTEL = MEMHTEL;
            SendObj.MEMCOMTEL = MEMCOMTEL;
            SendObj.MEMCONTRACT = MEMCONTRACT;
            SendObj.MEMCONTEL = MEMCONTEL;
            SendObj.MEMEMAIL = MEMEMAIL;
            SendObj.HasVaildEMail = HasVaildEMail;
            SendObj.MEMMSG = MEMMSG;

            //20210115 UPD BY 堂尾鰭 增加備註欄位處理
            SendObj.MEMONEW = MEMONEW;
            

            DoAjaxAfterGoBack(SendObj, "BE_Audit", "審核發生錯誤");
            
        } else {
            disabledLoadingAndShowAlert(errMsg);
        }
       
        if (parseInt(MobileLen) > 0 || parseInt(HistoryLen) > 0) {
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
            var y = new Date();
            //console.log(y.getFullYear());
            //console.log(y.getFullYear() - ('1997-01-16 00:00:00.000').substr(0, 4));
            //console.log($("#Birth").val())
            //20210312唐加判斷:20歲以下才要審
            if ($.trim(Obj.F01_Audit) == "0" &&  (y.getFullYear() - $("#Birth").val().substr(0, 4)) < 20 ) {
                flag = false;
                errmsg = "請勾選【法定代理人】的照片審核結果";
            }
            /* 20210312唐註解，天0說不要
            if ($.trim(Obj.Other_1_Audit) == "") {
                flag = false;
                errmsg = "請勾選【其他證件】的照片審核結果";
            }
            */
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
    //20210310唐加
    if (BlockMobileLen > 0) {
        $('#btnCheckBlockMobileLen').click();
    }

    setPostbackValue();
})
function ShowPIC(site) {
    if (site != "") {
        window.open(site);
    }
}
function setData() {
    var ObjList = [];
    for (var i = 0; i < fieldLen; i++) {
        field[i] = $("#" + fieldName[i]).val();
        fieldAudit[i] = $("input[name='" + fieldName[i] + "_AuditStatus']:checked").val();
        fieldReason[i] = $("#" + fieldName[i] + "_RejectReason").val();
        ObjList.push({
            ID:i+1,
            Audit: $("input[name='" + fieldName[i] + "_AuditStatus']:checked").val(),
            Reason: $("#" + fieldName[i] + "_RejectReason").val(),
            Image:''
        });
    }
    ObjList[0].Image = $("#ID_1_PIC").attr('src').toUpperCase().indexOf('FTP') > -1 ? $("#ID_1_PIC").attr('src').substr($("#ID_1_PIC").attr('src').toUpperCase().indexOf('FTP'), 8000) : $("#ID_1_PIC").attr('src');
    ObjList[1].Image = $("#ID_2_PIC").attr('src').toUpperCase().indexOf('FTP') > -1 ? $("#ID_2_PIC").attr('src').substr($("#ID_2_PIC").attr('src').toUpperCase().indexOf('FTP'), 8000) : $("#ID_2_PIC").attr('src');
    ObjList[2].Image = $("#Car_1_PIC").attr('src').toUpperCase().indexOf('FTP') > -1 ? $("#Car_1_PIC").attr('src').substr($("#Car_1_PIC").attr('src').toUpperCase().indexOf('FTP'), 8000) : $("#Car_1_PIC").attr('src');
    ObjList[3].Image = $("#Car_2_PIC").attr('src').toUpperCase().indexOf('FTP') > -1 ? $("#Car_2_PIC").attr('src').substr($("#Car_2_PIC").attr('src').toUpperCase().indexOf('FTP'), 8000) : $("#Car_2_PIC").attr('src');
    ObjList[4].Image = $("#Motor_1_PIC").attr('src').toUpperCase().indexOf('FTP') > -1 ? $("#Motor_1_PIC").attr('src').substr($("#Motor_1_PIC").attr('src').toUpperCase().indexOf('FTP'), 8000) : $("#Motor_1_PIC").attr('src');
    ObjList[5].Image = $("#Motor_2_PIC").attr('src').toUpperCase().indexOf('FTP') > -1 ? $("#Motor_2_PIC").attr('src').substr($("#Motor_2_PIC").attr('src').toUpperCase().indexOf('FTP'), 8000) : $("#Motor_2_PIC").attr('src');
    ObjList[6].Image = $("#Self_1_PIC").attr('src').toUpperCase().indexOf('FTP') > -1 ? $("#Self_1_PIC").attr('src').substr($("#Self_1_PIC").attr('src').toUpperCase().indexOf('FTP'), 8000) : $("#Self_1_PIC").attr('src');
    ObjList[7].Image = $("#F01_PIC").attr('src').toUpperCase().indexOf('FTP') > -1 ? $("#F01_PIC").attr('src').substr($("#F01_PIC").attr('src').toUpperCase().indexOf('FTP'), 8000) : $("#F01_PIC").attr('src');
    ObjList[8].Image = $("#Other_1_PIC").attr('src').toUpperCase().indexOf('FTP') > -1 ? $("#Other_1_PIC").attr('src').substr($("#Other_1_PIC").attr('src').toUpperCase().indexOf('FTP'), 8000) : $("#Other_1_PIC").attr('src');
    ObjList[9].Image = $("#Business_1_PIC").attr('src').toUpperCase().indexOf('FTP') > -1 ? $("#Business_1_PIC").attr('src').substr($("#Business_1_PIC").attr('src').toUpperCase().indexOf('FTP'), 8000) : $("#Business_1_PIC").attr('src');
    ObjList[10].Image = $("#Signture_1_PIC").attr('src').toUpperCase().indexOf('FTP') > -1 ? $("#Signture_1_PIC").attr('src').substr($("#Signture_1_PIC").attr('src').toUpperCase().indexOf('FTP'), 8000) : $("#Signture_1_PIC").attr('src');

    Obj = new Object();
    Obj.ID_1 = 1;
    Obj.ID_1_new = change(field[0]);
    Obj.ID_1_Audit = ObjList[0].Audit;//fieldAudit[0] ;
    Obj.ID_1_Reason = ObjList[0].Reason;//fieldReason[0];
    Obj.ID_1_Image = ObjList[0].Image;//$("#ID_1_PIC").attr('src');

    Obj.ID_2 = 2;
    Obj.ID_2_new = change(field[1] );
    Obj.ID_2_Audit = ObjList[1].Audit;//fieldAudit[1] ;
    Obj.ID_2_Reason = ObjList[1].Reason;//fieldReason[1];
    Obj.ID_2_Image = ObjList[1].Image;//$("#ID_2_PIC").attr('src');

    Obj.Car_1 = 3;
    Obj.Car_1_new = change(field[2] );
    Obj.Car_1_Audit = ObjList[2].Audit;//fieldAudit[2] ;
    Obj.Car_1_Reason = ObjList[2].Reason;//fieldReason[2];
    Obj.Car_1_Image = ObjList[2].Image;//$("#Car_1_PIC").attr('src');

    Obj.Car_2 = 4;
    Obj.Car_2_new = change(field[3] );
    Obj.Car_2_Audit = ObjList[3].Audit;//fieldAudit[3] ;
    Obj.Car_2_Reason = ObjList[3].Reason;//fieldReason[3];
    Obj.Car_2_Image = ObjList[3].Image;//$("#Car_2_PIC").attr('src');

    Obj.Motor_1 = 5;
    Obj.Motor_1_new = change(field[4] );
    Obj.Motor_1_Audit = ObjList[4].Audit;//fieldAudit[4] ;
    Obj.Motor_1_Reason = ObjList[4].Reason;//fieldReason[4];
    Obj.Motor_1_Image = ObjList[4].Image;//$("#Motor_1_PIC").attr('src');

    Obj.Motor_2 = 6;
    Obj.Motor_2_new = change(field[5] );
    Obj.Motor_2_Audit = ObjList[5].Audit;//fieldAudit[5] ;
    Obj.Motor_2_Reason = ObjList[5].Reason;//fieldReason[5];
    Obj.Motor_2_Image = ObjList[5].Image;//$("#Motor_2_PIC").attr('src');

    Obj.Self_1 = 7;
    Obj.Self_1_new = change(field[6] );
    Obj.Self_1_Audit = ObjList[6].Audit;//fieldAudit[6] ;
    Obj.Self_1_Reason = ObjList[6].Reason;//fieldReason[6];
    Obj.Self_1_Image = ObjList[6].Image;//$("#Self_1_PIC").attr('src');

    Obj.F01 = 8;
    Obj.F01_new = change(field[7] );
    Obj.F01_Audit = ObjList[7].Audit;//fieldAudit[7] ;
    Obj.F01_Reason = ObjList[7].Reason;//fieldReason[7];
    Obj.F01_Image = ObjList[7].Image;//$("#F01_PIC").attr('src');

    Obj.Other_1 = 9;
    Obj.Other_1_new = change(field[8] );
    Obj.Other_1_Audit = ObjList[8].Audit;//fieldAudit[9] ;
    Obj.Other_1_Reason = ObjList[8].Reason;//fieldReason[9];
    Obj.Other_1_Image = ObjList[8].Image;//$("#Other_1_PIC").attr('src');

    Obj.Business_1 = 10;
    Obj.Business_1_new = change(field[9] );
    Obj.Business_1_Audit = ObjList[9].Audit;//fieldAudit[10];
    Obj.Business_1_Reason = ObjList[9].Reason;//fieldReason[10];
    Obj.Business_1_Image = ObjList[9].Image;//$("#Business_1_PIC").attr('src');

    Obj.Signture_1 = 11;
    Obj.Signture_1_new = change(field[10]);
    Obj.Signture_1_Audit = ObjList[10].Audit;//fieldAudit[8] ;
    Obj.Signture_1_Reason = ObjList[10].Reason;//fieldReason[8];
    Obj.Signture_1_Image = ObjList[10].Image;//$("#Signture_1_PIC").attr('src');

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


function aa(detail, actionname) {

    //for (var i = 0; i< 100; i++) {
    //    document.getElementById("myTable").deleteRow(0);
    //}
    $('#myTable tbody td').remove()

    const obj = $.grep(detail, function (n, i) { return n.Action === actionname; });
    console.log(obj)

    //data:
    var pp = $.grep(detail, function (n, i) { return n.Action === actionname; }).length
    //console.log(pp)

    //get table body:
    var tableRef = document.getElementById('myTable').getElementsByTagName('tbody')[0];

    for (let index = 0; index < pp; index++) {
        //insert Row
        tableRef.insertRow().innerHTML =
            "<td>" + obj[index].Action + "</td>" +
            "<td>" + obj[index].MKTime + "</td>" +
            "<td>" + obj[index].Event + "</td>";
    }
}