var NowEditID = 0;
var Operator = "";
var OperatorName = "";
var OldOperatorICon = "";
var StartDate = "";
var EndDate = "";
var inPicSize = 0;
$(function () {
    
    $("#surrounding_modal").hide();
    if (errorLine == "ok") {

    } else {
        if (errorMsg != "") {
            ShowFailMessage(errorMsg);
        }
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
                $("#fileImport").prop("disabled", "");
                $("#btnSend").show();
                $("#btnReset").show();
                $("#btnSend").text("儲存");

                break;
            case "Edit":
                $("#fileImport").prop("disabled", "disabled");
                // $("#ParkingName").val("");

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
        var Operator = $("#Operator").val();
        var OperatorName = $("#OperatorName").val();
        var StartDate = $("#StartDate").val();
        var EndDate = $("#EndDate").val();
       // var upploader_dom = $("#fileImport").file[0];
        var upploader=$('input[type="file"]').val();
        var checkList = [Operator, OperatorName, StartDate, EndDate];
        var errMsgList = ["加盟業者編號未填", "加盟業者名稱未填", "有效日期（起）未填", "有效日期（迄）未填"];
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
                console.log(upploader);
                if (CheckStorageIsNull(upploader) == false || upploader=="") {

                    flag = false;
                    errMsg = "請上傳logo檔";
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
                obj.Operator = Operator;
                DoAjaxAfterSubmitNonShowMessageAndNowhide(obj, "BE_CheckOperator", "驗證加盟業者編號發生錯誤", $("#frmFranchiseesMaintain"));
            } else {
                $("#frmFranchiseesMaintain").submit();
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
                ShowFailMessage(errMsg);
            }
        }
    }
})
function DoEdit(Id) {
    var OldIcon = $("#btnEdit_" + Id).attr('data-OldIcon');
    if (NowEditID > 0) {
        //先還原前一個
        $("#OperatorAccount_" + NowEditID).val(Operator).hide();
        $("#OperatorName_" + NowEditID).val(OperatorName).hide();
        $("#OperatorICon_" + NowEditID).val("").prop("disabled", "disabled");
        $("#StartDate_" + NowEditID).val(StartDate).hide();
        $("#EndDate_" + NowEditID).val(EndDate).hide();
        $("#hidPic").val("");
        $("#btnReset_" + NowEditID).hide();
        $("#btnSave_" + NowEditID).hide();
        $("#btnEdit_" + NowEditID).show();
        OldOperatorICon = "";
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
    OldOperatorICon = OldIcon;
    Operator = $("#OperatorAccount_" + Id).val();
    OperatorName = $("#OperatorName_" + Id).val();
    StartDate = $("#StartDate_" + Id).val();
    EndDate = $("#EndDate_" + Id).val();

    //  }
    $("#OperatorAccount_" + Id).show();
    $("#OperatorName_" + Id).show();
    $("#StartDate_" + Id).show();
    $("#EndDate_" + Id).show();
    $("#OperatorICon_" + Id).val("").prop("disabled", "");

    $("#btnReset_" + Id).show();
    $("#btnSave_" + Id).show();
    $("#btnEdit_" + Id).hide();
}
function DoReset(Id) {

    $("#OperatorAccount_" + Id).val(Operator).hide();
    $("#OperatorName_" + Id).val(OperatorName).hide();
    $("#OperatorICon_" + Id).val("").prop("disabled", "disabled");
    $("#StartDate_" + Id).val(StartDate).hide();
    $("#EndDate_" + Id).val(EndDate).hide();
    $("#hidPic").val("");
    $("#btnReset_" + Id).hide();
    $("#btnSave_" + Id).hide();
    $("#btnEdit_" + Id).show();
    OldOperatorICon = "";
    NowEditID = 0;
    OldOperatorICon = "";
    Operator = "";
    OperatorName = "";
    StartDate = "";
    EndDate = "";
}
function DoSave(Id) {
    var OldIcon = $("#btnEdit_" + Id).attr('data-OldIcon');
    OldOperatorICon = OldIcon;
    Operator = $("#OperatorAccount_" + Id).val();
    OperatorName = $("#OperatorName_" + Id).val();
    StartDate = $("#StartDate_" + Id).val();
    EndDate = $("#EndDate_" + Id).val();
    var FileStr = $("#hidPic").val();
    var Account = $("#Account").val();

    var flag = true;
    var errMsg = "";
    ShowLoading("資料處理中");
    var checkList = [Operator, OperatorName, StartDate, EndDate];
    var errMsgList = ["加盟業者編號未填", "加盟業者名稱未填", "有效日期（起）未填", "有效日期（迄）未填"];
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
        obj.OperatorID = Id;
        obj.OperatorAccount = Operator;
        obj.OperatorName = OperatorName;
        obj.StartDate = StartDate.replace(/\//g, "").replace(/\-/g, "");
        obj.EndDate = EndDate.replace(/\//g, "").replace(/\-/g, "");
        obj.OperatorICon = FileStr;
        obj.OldOperatorIcon = OldOperatorICon;
        DoAjaxAfterReload(obj, "BE_UPDOperator", "修改加盟業者發生錯誤");
    }

}
function init() {
    $("#btnReview").hide();
    $("#btnSend").hide();
    $("#btnReset").hide();
    $("#fileImport").val("");
    $("#fileImport").prop("disabled", "disabled");
    $("#Operator").val("");
    $("#Operator").prop("disabled", "");
    $("#OperatorName").val("");
    $("#OperatorName").prop("disabled", "");
    $("#StartDate").val("");
    $("#StartDate").prop("disabled", "");
    $("#EndDate").val("");
    $("#EndDate").prop("disabled", "");
    $("#ddlObj").val("");
    $("#panelResult").hide();
    $("#fileImport").on("change", function () {
        var file = this.files[0];
        var fileName = file.name;
        var fileSize = file.size;
        var ext = GetFileExtends(fileName);
        var extName = "";
        if (CheckStorageIsNull(ext)) {
            extName = ext[0];
            console.log(extName.toUpperCase())
        }
        if (extName.toUpperCase() != "PNG") {

            swal({
                title: 'Fail',
                text: "僅允許png格式",
                icon: 'error'
            }).then(function (value) {

                $("#fileImport").val("");
            });
        }
    })
}
function showPic(url) {
    var url = ImgBaseUrl + url;
    $('#tmpENVPIC').attr('src', url);
    $('#tmpENVPIC').show();
    $("#btnReview").trigger("click");
}
function handleFiles(file, id) {
    console.log("call handleFiles");
    if (file != null) {
        console.log(file)
        if (file.length > 0) {
            var tmpfile = file[0];
            console.log("file size=" + file[0].size);

            $("#PicInfo").html("檔案建立日：" + new Date(file[0].lastModified).toLocaleDateString() + " " + new Date(file[0].lastModified).toLocaleTimeString());
            console.log(new Date(file[0].lastModified).toLocaleDateString() + " " + new Date(file[0].lastModified).toLocaleTimeString());
            var reader = new FileReader();
            var fileSize = Math.round(file[0].size / 1024 / 1024);
            inPicSize = fileSize;
            reader.onload = function (e) {
                console.log("call reader.onload ");
                /* $('#show-incarPic').attr('src', e.target.result);
                 var base64 = e.target.result.split(",");
                 $("#hidincarPicType").val(base64[0].replace(/^data:/, ''));
                 $("#hidincarPic").val("⊙"+base64[1]);*/

                var url = reader.result;
                var base64 = e.target.result.split(",")[1];
                console.log("before length:" + base64.length);
                $('#tmpENVPIC').attr('src', url);

            };
            reader.readAsDataURL(tmpfile);


            $('#tmpENVPIC').show();
            console.log("show")
            $("#btnReview").show();
   
        } else {
            $("#btnReview").hide();
            document.getElementById('tmpENVPICc').src = "";
            $("#hidPic").val("");
 

        }

    }

}

document.getElementById("tmpENVPIC").addEventListener('load', function () {

    var cvs = document.createElement('canvas'),
        ctx = cvs.getContext('2d');
    var img = new Image(),
        maxW = 640; //設定最大寬度
    if (this.width > maxW) {
        this.height *= maxW / img.width;
        this.width = maxW;
    }
    // console.log(this.file)
    cvs.width = 640; // this.width;
    cvs.height = 480; // this.height;
    ctx.clearRect(0, 0, cvs.width, cvs.height);
    //        ctx.drawImage(this, 0, 0, this.width, this.height);
    ctx.drawImage(this, 0, 0, 640, 480);
    var compressRate = getCompressRate(1, inPicSize);
    var dataUrl = cvs.toDataURL('image/png', compressRate);
    var base64 = dataUrl.split(",");
    console.log(dataUrl);
    $("#hidPicType").val(base64[0].replace(/^data:/, ''));
    $("#hidPic").val("⊙" + base64[1]);
    console.log("after length:" + base64[1].length);


});
document.getElementById('tmpENVPIC').addEventListener('change', function () {
    console.log("call tmpENVPIC Change");
    var reader = new FileReader();
    var fileSize = Math.round(this.files[0].size / 1024 / 1024);
    reader.onload = function (e) {
        compress(this.files[0], fileSize);
        //呼叫圖片壓縮方法：compress();
    };
    reader.readAsDataURL(this.files[0]);
    //console.log(this.files[0]);
    //以M為單位
    //this.files[0] 該資訊包含：圖片的大小，以byte計算 獲取size的方法如下：this.files[0].size;
}, false);
//最終實現思路：
//1、設定壓縮後的最大寬度 or 高度；
//2、設定壓縮比例，根據圖片的不同size大小，設定不同的壓縮比。
function compress(res, fileSize) { //res代表上傳的圖片，fileSize大小圖片的大小
    var img = new Image(),
        maxW = 640; //設定最大寬度
    img.onload = function () {
        console.log("img load");
        var cvs = document.createElement('canvas'),
            ctx = cvs.getContext('2d');
        if (img.width > maxW) {
            img.height *= maxW / img.width;
            img.width = maxW;
        }
        cvs.width = img.width;
        cvs.height = img.height;
        ctx.clearRect(0, 0, cvs.width, cvs.height);
        ctx.drawImage(img, 0, 0, img.width, img.height);
        var compressRate = getCompressRate(1, fileSize);
        var dataUrl = cvs.toDataURL('image/png', compressRate);
        document.body.appendChild(cvs);
        // console.log(dataUrl);
        console.log("call compress");
    }
    img.src = res;
}
function getCompressRate(allowMaxSize, fileSize) { //計算壓縮比率，size單位為MB
    var compressRate = 1;
    if (fileSize / allowMaxSize > 0.5) {
        compressRate = 0.3;
    } else if (fileSize / allowMaxSize > 0.4) {
        compressRate = 0.4;
    } else if (fileSize / allowMaxSize > 0.3) {
        compressRate = 0.5;
    } else if (fileSize > allowMaxSize) {
        compressRate = 0.6;
    } else {
        compressRate = 0.7;
    }
    return compressRate;
}