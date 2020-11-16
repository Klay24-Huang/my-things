var finalPrice = 0;
$(document).ready(function () {
    $("#panelResult").hide();
    $("#btnQuery").on("click", function () {
        var OrderNo = $("#OrderNo").val();

        var flag = true;
        var errMsg = "";
        ShowLoading("資料查詢中…");
        if (OrderNo == "") {
            flag = false;
            errMsg = "請輸入要修改的訂單編號，格式為H+7碼純數字";
        }
        if (flag) {
            var Account = $("#Account").val();
            var obj = new Object();
            obj.UserID = Account;
            obj.OrderNo = OrderNo;
            DoAjaxAfterCallBackWithOutMessage(obj, "BE_GetOrderModifyInfo", "查詢資料發生錯誤", SetData);
        } else {
            disabledLoadingAndShowAlert(errMsg);
        }

    });
    $("#btnReset").on("click", function () {
        var OrderNo = $("#spn_OrderNo").html();
        console.log(OrderNo);

        var flag = true;
        var errMsg = "";
        ShowLoading("資料查詢中…");
        if (OrderNo == "") {
            flag = false;
            errMsg = "請輸入要修改的訂單編號，格式為H+7碼純數字";
        }
        if (flag) {
            var Account = $("#Account").val();
            var obj = new Object();
            obj.UserID = Account;
            obj.OrderNo = OrderNo;
            DoAjaxAfterCallBackWithOutMessage(obj, "BE_GetOrderModifyInfo", "查詢資料發生錯誤", SetData);
        } else {
            disabledLoadingAndShowAlert(errMsg);
        }
    });

});

function SetData(data) {
    console.log(data);
    var ModifyObj = data.Data.ModifyLog;
    var errMsg = "";
    if (ModifyObj.hasModify != 0) {
        errMsg = "此訂單於【" + ModifyObj.ModifyTime + "】，由【" + ModifyObj.ModifyUserID + "】修改過";
    }
    var OrderObj = data.Data.OrderData;
    var LastOrderObj = data.Data.LastOrderData;
    if (LastOrderObj.LastStopTime != "") {
        $("#spn_LastStopTime").html(LastOrderObj.LastStopTime);
    }
    if (CheckStorageIsNull(OrderObj)) {
        console.log(OrderObj);
        var FS = new Date(OrderObj.FS).Format("yyyy-MM-dd HH:mm:ss")
        var FE = new Date(OrderObj.FS).Format("yyyy-MM-dd HH:mm:ss")
        var FineTime = new Date(OrderObj.FineTime).Format("yyyy-MM-dd HH:mm:ss")
        $("#panelResult").show();
        $("#spn_OrderNo").html("H" +pad(OrderObj.OrderNo,7))
        $("#spn_IDNO").html(OrderObj.IDNO)
        $("#spn_UserName").html(OrderObj.UserName)
        $("#spn_PRONAME").html(OrderObj.PRONAME)
        $("#spn_FS").html(FS)
        $("#spn_FE").html(FE)
        $("#StartDate").val(FS)
        $("#EndDate").val(FE)
        $("#spn_CarRent").html(OrderObj.CarRent);
        $("#pure_price_input").val(OrderObj.CarRent);

        finalPrice = OrderObj.FinalPrice;
        if (LastOrderObj.LastStopTime != "") {
            $("#spn_LastMile").html(LastOrderObj.LastEndMile)
        }
        $("#spn_gift").html(OrderObj.CarPoint);
        $("#spn_moto_gift").html(OrderObj.MotorPoint);
        $("#spn_StartMile").html(OrderObj.StartMile);
        $("#start_mile_input").val(OrderObj.StartMile)
        $("#spn_StopMile").html(OrderObj.StartMile);
        $("#end_mile_input").val(OrderObj.StartMile);
        $("#spn_Mileage").html(OrderObj.Mileage);
        $("#spn_finePrice").html(OrderObj.FinePrice);
        $("#fine_price_input").val(OrderObj.FinePrice)
        $("#spn_eTag").html(OrderObj.eTag);
        $("#spn_finalPrice").html(OrderObj.FinalPrice);
        $("#final_price_input").val(OrderObj.FinalPrice);
      


    }
    
}