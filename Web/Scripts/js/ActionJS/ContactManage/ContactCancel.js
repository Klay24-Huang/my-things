$(document).ready(function () {

    $("#btnSend").on("click", function () {

        ShowLoading("資料處理中");
        var OrderNo = $("#OrderNo").val();
        var type ="2";
        var mode = "0";
        var ReturnDate = "";
        var bill_option = "";
        var CARRIERID = "";
        var NPOBAN = "";
        var unified_business_no = "";
        var parkingSpace = "";
        var flag = true;
        var errMsg = "";
        if (OrderNo == "") {
            flag = false;
            errMsg = "訂單編號未填";
        }

        if (flag) {
            var Account = $("#Account").val();
            var obj = new Object();
            obj.UserID = Account;
            obj.OrderNo = OrderNo;
            obj.type = parseInt(type);
            //obj.ParkInfo = parseInt(ParkInfo); //20201209唐加
            obj.Mode = parseInt(mode);
            obj.returnDate = ReturnDate;
            obj.bill_option = bill_option;
            obj.CARRIERID = CARRIERID;
            obj.NPOBAN = NPOBAN;
            obj.unified_business_no = unified_business_no;
            obj.parkingSpace = parkingSpace;
            var json = JSON.stringify(obj);
            console.log(json);
            DoAjaxAfterReload(obj, "BE_ContactSetting", "執行作癈發生錯誤");
        } else {
            disabledLoadingAndShowAlert(errMsg);
        }
    })
})