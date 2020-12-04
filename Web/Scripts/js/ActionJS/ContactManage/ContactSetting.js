var MemberInvoice = {};
var LoveCodeList = [];
$(function () {
    $("#divReturnDate").hide();
    $("#returnInvoiceType").hide();

    $("#LoveCodeList").hide();
    $("#CARRIERID").hide();
    $("#NPOBAN").hide();
    $("#UniCode").hide();
    $("#type").on("change", function () {
        var fp = document.querySelector("#StartDate")._flatpickr;

        if ($(this).val() == "1") {
            $("#divReturnDate").show();

            //console.log($.format.date(new Date(), 'yyyy-MM-dd HH:mm'));
            fp.setDate(new Date());
            $("#StartDate").val($.format.date(new Date(), 'yyyy-MM-dd HH:mm'));
            if ($("#type").val() == '1' && $("#mode").val() == '0') {
                $("#returnInvoiceType").show();
                getMemberInvoice();
            } else {
                $("#returnInvoiceType").hide();
            }
        } else {
            $("#divReturnDate").hide();
            $("#returnInvoiceType").hide();

            //console.log($("#StartDate").val());
            $("#StartDate").val('');
        }
    });
    $("#mode").on("change", function () {
        if ($("#type").val() == '1' && $("#mode").val() == '0') {
            $("#returnInvoiceType").show();
            getMemberInvoice();
        } else {
            $("#returnInvoiceType").hide();
        }
    });

    $("#InvoiceType").on("change", function () {
        initInvoData($("#InvoiceType").val());
    });
    $("#LoveCodeList").on("change", function () {
        $("#NPOBAN").val($("#LoveCodeList").val());
    });
    $("#btnSend").on("click", function () {
        ShowLoading("資料處理中");
        var OrderNo = $("#OrderNo").val();
        var type = $("#type").val();
        var mode = $("#mode").val();
        var ReturnDate = $("#StartDate").val();
        var bill_option = $("#InvoiceType").val();
        var CARRIERID = $("#CARRIERID").val();
        var NPOBAN = $("#NPOBAN").val();
        var unified_business_no = $("#UniCode").val();
        var flag = true;
        var errMsg = "";
        if (OrderNo == "") {
            flag = false;
            errMsg = "訂單編號未填";
        }
        if (flag) {
            if (parseInt(type) < 0) {
                flag = false;
                errMsg = "動作類型未填";
            }
        }
        if (flag) {
            if (parseInt(mode) < 0) {
                flag = false;
                errMsg = "動作用途未填";
            }
        }
        if (flag) {
            if (type == "1" && ReturnDate == "") {
                flag = false;
                errMsg = "還車時間未填"
            } else {
                ReturnDate = ReturnDate + ":00";
            }
            if (type == "1" && (InvoiceType == "0" || InvoiceType == "")) {
                flag = false;
                errMsg = "發票寄送方式未填"
            }
        }
        if (flag) {
            if (type == "1" && mode == "0") {
                switch (bill_option) {
                    case '1':   //捐贈
                        if ($("#LoveCodeList").val() == '') {
                            flag = false;
                            errMsg = "還車時間未填";
                        }
                        break;
                    case '2':   //EMAIL
                    case '3':   //EMAIL
                        break;
                    case '4':   //三聯式發票，請至官網查詢下載
                        if ($("#UniCode").val() == '') {
                            flag = false;
                            errMsg = "統一編號未填";
                        }
                        break;
                    case '5':   //手機條碼載具
                    case '6':   //自然人憑證載具
                        if ($("#CARRIERID").val() == '') {
                            flag = false;
                            errMsg = bill_option == '4' ? '手機條碼載具未填' :'自然人憑證載具未填';
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        if (flag) {
            var Account = $("#Account").val();
            var obj = new Object();
            obj.UserID = Account;
            obj.OrderNo = OrderNo;
            obj.type = parseInt(type);
            obj.Mode = parseInt(mode);
            obj.returnDate = ReturnDate;
            obj.bill_option = bill_option;
            obj.CARRIERID = bill_option == '4' ? '\\' + CARRIERID : CARRIERID;
            obj.NPOBAN = NPOBAN;
            obj.unified_business_no = unified_business_no;
            var json = JSON.stringify(obj);
            console.log(json);
            DoAjaxAfterReload(obj, "BE_ContactSetting", "執行強取強還發生錯誤");
        } else {
            disabledLoadingAndShowAlert(errMsg);
        }

    });

    var getMemberInvoice = function () {
        if ($('#type').val() != "1") {

        }
        var flag = true;
        var errMsg = "";
        if (flag) {
            if ($("#OrderNo").val() == "") {
                flag = false;
                errMsg = "預約編號未輸入";
            }
        }
        if (flag) {
            if ($("#type").val() == "-1") {
                flag = false;
                errMsg = "動作類型未輸入";
            }
        }

        if (flag) {
            ShowLoading("開始查詢會員訂單資料");
            // window.location.href = "../CarDataInfo/CarDashBoard";
            //載入車輛
            var site = jsHost + "GetMemberInvoiceSetting";
            $.ajax({
                url: site,
                type: 'POST',
                data: '{"OrderNo":"'+$("#OrderNo").val()+'"}',
                cache: false,
                contentType: 'application/json',
                dataType: 'json',           //'application/json',
                success: function (data) {
                    $.busyLoadFull("hide");
                    console.log(data);
                    if (data.Result == "1") {
                        if (data.Data.MemberInvoice != null && data.Data.MemberInvoice.length > 0) {
                            MemberInvoice = data.Data.MemberInvoice[0];
                        } else {
                            MemberInvoice = {};
                        }
                        if (data.Data.LoveCodeList != null && data.Data.LoveCodeList.length > 0) {
                            LoveCodeList = data.Data.LoveCodeList;
                            LoveCodeList.sort(function (a, b) {
                                return a.LoveCode > b.LoveCode ? 1 : -1;
                            });
                        } else {
                            LoveCodeList = [];
                        }
                        setInvoData();
                    } else {

                        swal({
                            title: 'Fail',
                            text: data.ErrorMessage,
                            icon: 'error'
                        });
                    }
                },
                error: function (e) {
                    $.busyLoadFull("hide");
                    swal({
                        title: 'Fail',
                        text: "載入會員資料發生錯誤",
                        icon: 'error'
                    });
                }
            });
        }
        else {
            disabledLoadingAndShowAlert(errMsg);
        }
    };
});
var initInvoData = function (invoiceType) {
    $("#LoveCodeList").hide();
    $("#CARRIERID").hide();
    $("#NPOBAN").hide();
    $("#UniCode").hide();

    switch (invoiceType) {
        case '1':   //捐贈
            $("#LoveCodeList").show();
            break;
        case '2':   //EMAIL
        case '3':   //二聯
            break;
        case '4':   //三聯式發票，請至官網查詢下載
            $("#UniCode").show();
            break;
        case '5':   //手機條碼載具
        case '6':   //自然人憑證載具
            $("#CARRIERID").show();
            break;
        default:
            break;
    }
};
var setInvoData = function () {
    $('#InvoiceType').val(MemberInvoice.InvoiceType);

    initInvoData($("#InvoiceType").val());
    $('#CARRIERID').val(MemberInvoice.CARRIERID);
    $('#NPOBAN').val(MemberInvoice.NPOBAN);
    $('#UniCode').val(MemberInvoice.UniCode);

    var loveListOption = $.map(LoveCodeList, function (row) {
        return '<option value="' + row.LoveCode + '">(' + row.LoveCode + ')' + row.LoveName + '</option>';
    });
    $('#LoveCodeList').html('<option value="">請選擇</option> \n' + loveListOption.join(' \n'));
    $('#LoveCodeList').val(MemberInvoice.NPOBAN);
};