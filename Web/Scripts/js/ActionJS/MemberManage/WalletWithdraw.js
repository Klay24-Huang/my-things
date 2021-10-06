$(document).ready(function () {

    //獲取銀行代碼
    $.ajax({
        url: "https://raw.githubusercontent.com/wsmwason/taiwan-bank-code/master/data/taiwanBankCodeTT.json",
        type: 'GET',
        cache: false,
        dataType: "json",
        success: function (data) {
            console.log(data);
            for (var i = 0; i < data.length; i++) {
                var option = $("<option>").val(data[i].code).text(data[i].code + data[i].name);
                $("#RVBANK").append(option);

            }
        },
        error: function () {
            ShowFailMessage("銀行代碼載入失敗");
        }
    })

})

function checkInvoiceMode() {
    var invoiceMode = $("#invoiceMode").val();
    var NPOBAN = $("#li_NPOBAN");
    var carrier = $("#li_carrier");
    if (invoiceMode == "1") {
        NPOBAN.show();
        carrier.hide()
    } else if (invoiceMode == "5" || invoiceMode == "6") {
        NPOBAN.hide();
        carrier.show();
    } else {
        NPOBAN.hide();
        carrier.hide();
    }
}

function checkData() {
    event.preventDefault();
    ShowLoading("資料處理中...")
    var IDNO = $("#IDNO").val();
    var cashAmount = $("#cashAmount").val();
    var invoiceMode = $("#invoiceMode").val();
    var RVBANK = $("#RVBANK").val();
    var RVACNT = $("#RVACNT").val();
    var RV_NAME = $("#RV_NAME").val();

    if (IDNO == "" || cashAmount == "" || invoiceMode == null || RVBANK == "" || RVACNT == "" || RV_NAME == "") {
        ShowFailMessage("必填欄位不得空白");
    } else {
        $("#frmWalletWithdraw").submit();
    }
}