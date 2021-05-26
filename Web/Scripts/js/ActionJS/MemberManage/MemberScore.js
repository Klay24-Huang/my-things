$(function () {
    $("#AuditMode").on("change", function () {
        var Mode = $("#AuditMode").val();
        $(".clear").val('');
        switch (Mode) {
            case "0":
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
                $("#AA").show();
                $("#BB").show();
                break;
            case "1":
                $("#NAME").show();
                $("#ID").show();
                $("#ORDER").hide();
                $("#ORDER_I").show();
                $("#DATE").hide();
                $("#Import").hide();
                $("#Choice_0").show();
                $("#Choice_1").show();
                $("#Choice_2").show();
                $("#SCORE").show();
                $("#btnSubmit0").hide();
                $("#btnSubmit1").hide();
                $("#btnSubmit2").show();
                $("#btnSubmit3").hide();
                $("#AA").hide();
                $("#BB").hide();
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
                $("#AA").hide();
                $("#BB").hide();
                break;
        }
    });
    $("#ORDER_I").on("change", function () {
        ShowLoading("資料查詢中…");
        var a = $("#AuditMode").val()
        var b = parseInt(a) + 2
        var c = b.toString();
        //$("#AuditMode").val(c);
        console.log(c)
        console.log($("#AuditMode").val())
        console.log(a)
        console.log(b)
        $("#AuditMode").val('3');
        $("#frmMemberScore").submit();
        disabledLoading();

    });
    $("#btnSubmit0").on("click", function () {
        ShowLoading("資料查詢中…");
        var SD = $("#StartDate").val();
        var ED = $("#EndDate").val();
        var flag = true;
        var errMsg = "";

        if ($("#IDNO").val() == "" && $("#MEMNAME").val() == "" && $("#ORDERNO").val() == "") {
            flag = false;
            errMsg = "請輸入ID或姓名或合約";
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

        if ($("#IDNO").val() == "" && $("#MEMNAME").val() == "" && $("#ORDERNO").val() == "") {
            flag = false;
            errMsg = "請輸入ID或姓名或合約";
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