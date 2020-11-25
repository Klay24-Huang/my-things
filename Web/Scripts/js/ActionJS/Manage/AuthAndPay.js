$(document).ready(function () {
    if (errorLine == "ok") {

    } else {
        if (errorMsg != "") {
            ShowFailMessage(errorMsg);
            $("#footer").hide();
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
        $("#footer").show();
        $("#mytable #checkall").show();
    } else {
        $("#footer").hide();
        $("#mytable #checkall").hide();
    }

    $("#btnSave").on("click", function () {
       
        var Account = $("#Account").val();
        var IDNO = $("#IDNO").val();
        var data = new Array();
        $("#mytable input[type=checkbox]").each(function () {

            console.log($(this).attr("id"));
            if ($(this).attr("id") != "checkall") {
                var obj = new Object();
                var content = $(this).data("content");
                obj.ORDNO = content.ORDNO;  //預約編號
                obj.CNTRNO = content.CNTRNO; //合約編號
                obj.PAYMENTTYPE = content.PAYMENTTYPE; //付款類別
                obj.SPAYMENTTYPE = content.SPAYMENTTYPE; //付款類別中文

                obj.CARNO = content.CARNO; //車號
                obj.TAMT = content.TAMT; //金額
                obj.POLNO = content.POLNO; //罰單編號
                obj.IRENTORDNO = content.IRENTORDNO; //iRent訂單編號
                data.push(obj);
            }

            // totalBill += obj.DUEAMT;
        });
        var baseData = new Object();
        baseData.CUSTID = IDNO;
        baseData.UserID = Account;
        baseData.Data = data;

        var jsonData = JSON.stringify(baseData);
        var title = "";
        var body = "請再次確認金額，是否確定由 " + Account + " 操作取款？";
        swal({
            title: title,
            text: body,
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#DD6B55",
            confirmButtonText: '確定'
        }).then((isConfirm)=> {
                if (isConfirm) {
                    ShowLoading();
                    console.log(jsonData);
                    DoAjaxAfterReload(jsonData, "BE_AutoPayMoney", "取款發生錯誤");
               
            }
            else {

            }

        });
    });

    $("#mytable #checkall").click(function () {
       
        if ($("#mytable #checkall").is(':checked')) {
            totalBill = 0;
            $("#mytable input[type=checkbox]").each(function () {
               
                $(this).prop("checked", true);
                console.log($(this).attr("id"));
                if ($(this).attr("id") != "checkall") {
                    console.log("checkall:" + $(this).data("content"));
                    var obj = $(this).data("content");
                    totalBill += obj.TAMT;
                }

                // totalBill += obj.DUEAMT;
            });
            $("#mytable #checkall").prop("checked", "checked");
        } else {
            $("#mytable input[type=checkbox]").each(function () {
                $(this).prop("checked", "");
                totalBill = 0;
            });
            $("#mytable #checkall").prop("checked", "");
        }
        $("#totalBill").html(totalBill);
    });
    $("#mytable input[type=checkbox]").on("click", function (event) {
        console.log(event);
        if (!event.target.name && typeof event.target.name !== "undefined") {
            console.log("check single:" + $(this).data("content"));
            var obj = $(this).data("content");
            if (this.checked) {
                totalBill += obj.TAMT;
            } else {
                totalBill -= obj.TAMT;
                if ($("#mytable #checkall").is(':checked')) {
                    $("#mytable #checkall").prop("checked", false);
                }
            }
            console.log(event.target.name);
            console.log(typeof event.target.name);
            $("#totalBill").html(totalBill);
        }


    })

    $("[data-toggle=tooltip]").tooltip();

    $("#frmAuthAndPayQuery").on("submit", function () {
        ShowLoading("欠費查詢中…")
        var IDNO = $("IDNO").val();
        if (IDNO == "") {
            disabledLoadingAndShowAlert("身份證未填");
            return false;
        } else {
            return true;
        }


    })
});