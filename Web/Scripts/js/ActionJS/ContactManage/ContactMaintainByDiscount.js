var finalPrice = 0;
var Max_Motor_Pointer = 0;
var Max_Car_Pointer = 0;
var ModifyObj;
var OrderObj;
var BonusObj;
var LastOrderObj;
var hasReCal = false;
$(document).ready(function () {
    $("#panelResult").hide();
    $("#btnCal").hide();
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
    $("#btnCal").on("click", function () {
        ShowLoading("計算中…");
        if (CheckStorageIsNull(OrderObj)) {
            var Account = $("#Account").val();
            var MotorPoint = $("#gift_point_moto_input").val();
            var CarPoint = $("#gift_point_input").val();
            if (MotorPoint == "") {
                $("#gift_point_moto_input").val("0");
                MotorPoint = 0;
            }
            if (CarPoint == "") {
                $("#gift_point_input").val("0");
                CarPoint = 0;
            }
            var obj = new Object();
            obj.UserID = Account;
            obj.OrderNo = "H" + pad(OrderObj.OrderNo, 7);
            obj.ProjType = OrderObj.PROJTYPE
            if (OrderObj.PROJTYPE == 4) {
                obj.CarPoint = CarPoint;
                obj.MotorPoint = MotorPoint
            
            } else {
                obj.CarPoint = $("#gift_point_select").val();
                obj.MotorPoint = 0;
            }
            DoAjaxAfterCallBackWithOutMessage(obj, "BE_ReCalculateByDiscount", "重新計算發生錯誤", SetCalData);
        } else {
            disabledLoadingAndShowAlert("請重新取得資料");
        }

    });
    $("#UseStatus").on("change", function () {
        if ($(this).val() == "3") {
            $("#remark_input").prop("readonly", "");
        } else {
            $("#remark_input").prop("readonly", "readonly");
        }
    });
    $("#btnSave").on("click", function () {
        var flag = true;
        var errMsg = "";

        ShowLoading("資料儲存中…");
        if (hasReCal) {
            var UseStatus = $("#UseStatus").val();
            var Remark = $("#remark_input").val();

            if (UseStatus == "-1") {
                flag = false;
                errMsg = "請選擇修改原因";
            } else {
                if (UseStatus == "3" && Remark == "") {
                    flag = false;
                    errMsg = "請輸入備註";
                }
            }

            if (flag) {
                var MotorPoint = $("#gift_point_moto_input").val();
                var CarPoint = $("#gift_point_input").val();
                if (MotorPoint == "") {
                    $("#gift_point_moto_input").val("0");
                    MotorPoint = 0;
                }
                if (CarPoint == "") {
                    $("#gift_point_input").val("0");
                    CarPoint = 0;
                }
                var diffPrice = $("#final_amt").val();
                var finalPrice = $("#final_price_input").val();
                var Account = $("#Account").val();
                var obj = new Object();
                obj.UserID = Account;
                obj.OrderNo = "H" + pad(OrderObj.OrderNo, 7);
                obj.ProjType = OrderObj.PROJTYPE
                obj.DiffPrice = diffPrice;
                obj.FinalPrice = finalPrice
                if (OrderObj.PROJTYPE == 4) {
                    obj.CarPoint = CarPoint;
                    obj.MotorPoint = MotorPoint

                } else {
                    obj.CarPoint = $("#gift_point_select").val();
                    obj.MotorPoint = 0;
                }
                obj.UseStatus = UseStatus;
                if (CheckIsUndefined(Remark)) {
                    obj.Remark = Remark
                }

                DoAjaxAfterReload(obj, "BE_HandleOrderModifyByDiscount", "修改資料發生錯誤");
            } else {
                disabledLoadingAndShowAlert(errMsg);
            }
           

         
        } else {
            disabledLoadingAndShowAlert("請先修改後按下重新計算");
        }
    });

});
function SetCalData(data) {
    console.log(data);
    $("#final_amt").val(data.Data.DiffFinalPrice);
    $("#final_price_input").val(data.Data.NewFinalPrice);
    $("#pure_price_input").val(data.Data.RentPrice);
    hasReCal = true;
}
function SetData(data) {
    console.log(data);
     ModifyObj = data.Data.ModifyLog;
    var errMsg = "";
    if (ModifyObj.hasModify != 0) {
        errMsg = "此訂單於【" + ModifyObj.ModifyTime + "】，由【" + ModifyObj.ModifyUserID + "】修改過";
    }
     OrderObj = data.Data.OrderData;
     LastOrderObj = data.Data.LastOrderData;
     BonusObj = data.Data.Bonus;
    if (LastOrderObj.LastStopTime != "") {
        $("#spn_LastStopTime").html(LastOrderObj.LastStopTime);
    }
    if (CheckStorageIsNull(OrderObj)) {
        console.log(OrderObj);
        var FS = new Date(OrderObj.FS).Format("yyyy-MM-dd HH:mm:ss")
        var FE = new Date(OrderObj.FS).Format("yyyy-MM-dd HH:mm:ss")
        var FineTime = new Date(OrderObj.FineTime).Format("yyyy-MM-dd HH:mm:ss")
        $("#panelResult").show();
        $("#spn_OrderNo").html("H" + pad(OrderObj.OrderNo, 7))
        $("#spn_IDNO").html(OrderObj.IDNO)
        $("#spn_UserName").html(OrderObj.UserName)
        $("#spn_PRONAME").html(OrderObj.PRONAME)
        $("#spn_FS").html(FS)
        $("#spn_FE").html(FE)
        $("#StartDate").val(FS)
        $("#EndDate").val(FE)
        $("#spn_CarRent").html(OrderObj.pure_price);
        $("#pure_price_input").val(OrderObj.pure_price);

        finalPrice = OrderObj.FinalPrice;
        if (LastOrderObj.LastStopTime != "") {
            $("#spn_LastMile").html(LastOrderObj.LastEndMile)
        }
        $("#spn_gift").html(OrderObj.CarPoint);
        $("#spn_moto_gift").html(OrderObj.MotorPoint);
        $("#spn_StartMile").html(OrderObj.SM);
        $("#start_mile_input").val(OrderObj.SM)
        $("#spn_StopMile").html(OrderObj.EM);
        $("#end_mile_input").val(OrderObj.EM);
        $("#spn_Mileage").html(OrderObj.mileage_price);
        $("#spn_finePrice").html(OrderObj.fine_price);
        $("#fine_price_input").val(OrderObj.fine_price)
        $("#spn_eTag").html(OrderObj.eTag);
        $("#spn_finalPrice").html(OrderObj.final_price);
        $("#final_price_input").val(OrderObj.final_price);
        $("#spn_payPrice").html(OrderObj.Paid)

        if (OrderObj.PROJTYPE == 4) {
            $("#gift_point_input").prop("placeholder", "與機車點數合計最多只能使用" + BonusObj.CanUseTotalCarPoint).val(OrderObj.CarPoint);
            $("#gift_point_select").empty().hide();
            $("#gift_point_moto_input").prop("placeholder", "與機車點數合計最多只能使用" + BonusObj.CanUseTotalCarPoint).val(OrderObj.MotorPoint);
            $("#gift_point_moto_input").prop("disabled", "");
            $("#gift_point_input").prop("disabled", "").show();
        } else {
            $("#gift_point_moto_input").prop("disabled", "disabled");
            $("#gift_point_input").hide();
            $("#gift_point_select").empty().show();
            for (var i = 0; i <= BonusObj.CanUseTotalCarPoint; i += 30) {

                $("#gift_point_select").append(`<option value="${i}">${i}</option>`);
            }
            $("#gift_point_select").val(OrderObj.CarPoint)

        }

        if (OrderObj.PROJTYPE == 4) {
            if (data.Data.IsHoliday == 0) {
                $("#spn_OneDay").html(OrderObj.MaxPrice);
            } else {
                $("#spn_OneDay").html(OrderObj.MaxPriceH);
            }
        } else {
            if (data.Data.IsHoliday == 0) {
                $("#spn_OneDay").html(OrderObj.WeekdayPrice);
            } else {
                $("#spn_OneDay").html(OrderObj.HoildayPrice);
            }
        }

        $("#btnCal").show();

    } else {
        $("#btnCal").hide();
    }
    
}
function SetPointer(data) {
    console.log(data)
    Max_Motor_Pointer = parseInt(data.Data.TotalMotorLASTPOINT);
    $("#gift_point_moto_input").prop("readonly", "");
    $("#gift_point_moto_input").prop("max", Max_Motor_Pointer); 

}