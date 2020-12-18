var hasReCal = false;
var oldOtherPrice = 0;
$(document).ready(function () {
    $("#panelResult").hide();
    $("#OrderNo").on("blur", function () {
        console.log('c');
        if ($(this).val() != "") {
            $("#OrderNo").val($(this).val().toUpperCase())
        }
    })
    $("#UseStatus").on("change", function () {
        console.log('b');
        if ($(this).val() == "3") {
            $("#remark_input").prop("readonly", "");
        } else {
            $("#remark_input").prop("readonly", "readonly");
        }
    });
    $("#btnQuery").on("click", function () {
        console.log('a');
        var OrderNo = $("#OrderNo").val();

        var flag = true;
        var errMsg = "";
        ShowLoading("資料查詢中…");
        /*20201209唐註解
        if (OrderNo == "") {
            flag = false;
            errMsg = "請輸入要修改的訂單編號，格式為H+7碼純數字a";
        }
        */
        //20201209唐加
        if (OrderNo == "") {
            flag = false;
            errMsg = "訂單編號未填";
        }
        else {
            if (false == RegexOrderNo(OrderNo)) {
                flag = false;
                errMsg = "訂單編號格式不符（格式：H+7碼數字，未滿7碼左補0)";
            }
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
        hasReCal = true;

        var oldPrice = parseInt($("#spn_finalPrice").html());
        var final_price = $("#final_price_input").val();
        var Mileage = $("#Mileage_input").val();
        var pure = $("#pure_price_input").val();
        var SM = $("#start_mile_input").val();
        var EM = $("#end_mile_input").val();
        var FP = $("#fine_price_input").val();
        var SD = $("#StartDate").val();
        var ED = $("#EndDate").val();
        var Other_1 = $("#CarDispatch_input").val();
        var Other_2 = $("#CleanFee_input").val();
        var Other_3 = $("#DestroyFee_input").val();
        var Other_4 = $("#ParkingFee_input").val();
        var Other_5 = $("#DraggingFee_input").val();
        var Other_6 = $("#OtherFee_input").val();
        var Other_7 = $("#ParkingFeeByMachi_input").val();
        if (pure != "" && SM != "" && EM != "" && SD != "" && ED != "" && FP != "" && final_price != "") {
            var OtherPrice = parseInt(Other_1) + parseInt(Other_2) + parseInt(Other_3) + parseInt(Other_4) + parseInt(Other_5) + parseInt(Other_6) + parseInt(Other_7);
            var final_amt = ((oldPrice) - parseInt(final_price)) + (oldOtherPrice - OtherPrice); //(oldPrice ) - (parseInt(final_price) + OtherPrice);


            $("#pure_price_input").prop("readonly", "readonly");
            $("#start_mile_input").prop("readonly", "readonly");
            $("#end_mile_input").prop("readonly", "readonly");
            $("#fine_price_input").prop("readonly", "readonly");
            $("#Mileage_input").prop("readonly", "readonly");
            $("#final_price_input").prop("readonly", "readonly");
            $("#StartDate").prop("disabled", "disabled");
            $("#EndDate").prop("disabled", "disabled");
            $("#CarDispatch_input").prop("readonly", "readonly");
            $("#CleanFee_input").prop("readonly", "readonly");
            $("#DestroyFee_input").prop("readonly", "readonly");
            $("#ParkingFee_input").prop("readonly", "readonly");
            $("#DraggingFee_input").prop("readonly", "readonly");
            $("#OtherFee_input").prop("readonly", "readonly");
            $("#ParkingFeeByMachi_input").prop("readonly", "readonly");
            $("#Insurance_price_input").val(OrderObj.Insurance_price).prop("readonly", "readonly");
            $("#final_amt").val(final_amt);
            console.log(oldPrice);
            disabledLoading();
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
            errMsg = "請輸入要修改的訂單編號，格式為H+7碼純數字b";
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
    $("#final_price_input").on("change", function () {
        hasReCal = false;
    })
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
                var MotorPoint = 0;
                var CarPoint = $("#gift_point_select").val();
                var StartDate = $("#StartDate").val();
                var EndDate = $("#EndDate").val();
                var start_mile = $("#start_mile_input").val();
                var end_mile = $("#end_mile_input").val();
                var fine_price = $("#fine_price_input").val();
                var CarDispatch = $("#CarDispatch_input").val();
                var DispatchRemark = $("#DispatchRemark_input").val();
                var CleanFee = $("#CleanFee_input").val();
                var CleanFeeRemark = $("#CleanFeeRemark_input").val();
                var DestroyFee = $("#DestroyFee_input").val();
                var DestroyFeeRemark = $("#DestroyFeeRemark_input").val();
                var ParkingFee = $("#ParkingFee_input").val();
                var ParkingFeeRemark = $("#ParkingFeeRemark_input").val();
                var DraggingFee = $("#DraggingFee_input").val();
                var DraggingFeeRemark = $("#DraggingFeeRemark_input").val();
                var OtherFee = $("#OtherFee_input").val();
                var OtherFeeRemark = $("#OtherFeeRemark_input").val();
                var ParkingFeeByMachi = $("#ParkingFeeByMachi_input").val();
                var ParkingFeeByMachiRemark = $("#ParkingFeeByMachiRemark_input").val();
                var Insurance_price = $("#Insurance_price_input").val();
                var Mileage = $("#Mileage_input").val();
                
                if ($.trim(CarPoint) == "") {
                    $("#gift_point_select").val("0");
                    CarPoint = 0;
                }
                var diffPrice = $("#final_amt").val();
                var finalPrice = $("#final_price_input").val();
                var Account = $("#Account").val();
                var obj = new Object();
                obj.UserID = Account;
                obj.OrderNo = "H" + pad(OrderObj.OrderNo, 7);
                obj.StartDate = StartDate;
                obj.EndDate = EndDate;
                obj.start_mile = start_mile;
                obj.fine_price = fine_price;
                obj.end_mile = end_mile;
                obj.ProjType = OrderObj.PROJTYPE
                obj.DiffPrice = diffPrice;
                obj.FinalPrice = finalPrice;
                obj.CarDispatch = CarDispatch;
                obj.DispatchRemark = DispatchRemark;
                obj.CleanFee = CleanFee;
                obj.CleanFeeRemark = CleanFeeRemark;
                obj.DestroyFee = DestroyFee;
                obj.DestroyFeeRemark = DestroyFeeRemark;
                obj.ParkingFee = ParkingFee;
                obj.ParkingFeeRemark = ParkingFeeRemark;
                obj.DraggingFee = DraggingFee;
                obj.DraggingFeeRemark = DraggingFeeRemark;
                obj.OtherFee = OtherFee;
                obj.OtherFeeRemark = OtherFeeRemark;
                obj.ParkingFeeByMachi = ParkingFeeByMachi;
                obj.ParkingFeeByMachiRemark = ParkingFeeByMachiRemark;
                obj.Insurance_price = Insurance_price;
                obj.Mileage = Mileage;

                if (OrderObj.PROJTYPE == 4) {
                    obj.CarPoint = CarPoint;
                    obj.MotorPoint = MotorPoint

                } else {
                    obj.CarPoint = $.trim($("#gift_point_select").val()) == '' ? 0 : $("#gift_point_select").val();
                    obj.MotorPoint = 0;
                }
                obj.UseStatus = UseStatus;
                if (CheckIsUndefined(Remark)) {
                    obj.Remark = Remark
                }

                DoAjaxAfterReload(obj, "BE_HandleOrderModify", "修改資料發生錯誤");
            } else {
                disabledLoadingAndShowAlert(errMsg);
            }



        } else {
            disabledLoadingAndShowAlert("請先修改後按下重新計算");
        }
    });
});
function SetData(data) {
    console.log(data);

    ModifyObj = data.Data.ModifyLog;
    console.log("Modify");
    console.log(ModifyObj);
    var errMsg = "";
    if (ModifyObj.hasModify > 0) {
        errMsg = "此訂單於【" + ModifyObj.ModifyTime + "】，由【" + ModifyObj.ModifyUserID + "】修改過";
        ShowSuccessMessage(errMsg);
    }
    OrderObj = data.Data.OrderData;
    LastOrderObj = data.Data.LastOrderData;
    BonusObj = data.Data.Bonus;
    if (OrderObj.PROJTYPE != 4) {
        if (LastOrderObj.LastStopTime != "") {
            $("#spn_LastStopTime").html(LastOrderObj.LastStopTime);
        }
        if (CheckStorageIsNull(OrderObj)) {
            console.log(OrderObj);
            var FS = new Date(OrderObj.FS).Format("yyyy-MM-dd HH:mm:ss")
            var FE = new Date(OrderObj.FE).Format("yyyy-MM-dd HH:mm:ss")
            var FineTime = new Date(OrderObj.FineTime).Format("yyyy-MM-dd HH:mm:ss")
            oldOtherPrice = OrderObj.CarDispatch + OrderObj.CleanFee + OrderObj.DestroyFee + OrderObj.parkingFee + OrderObj.DraggingFee + OrderObj.OtherFee + OrderObj.PARKINGAMT2;
            $("#panelResult").show();
            $("#spn_OrderNo").html("H" + pad(OrderObj.OrderNo, 7))
            $("#spn_IDNO").html(OrderObj.IDNO)
            $("#spn_UserName").html(OrderObj.UserName)
            $("#spn_PRONAME").html(OrderObj.PRONAME)
            $("#spn_FS").html(FS)
            $("#spn_FE").html(FE)
            $("#StartDate").val(FS).prop("disabled", "");
            $("#EndDate").val(FE).prop("disabled", "");
            $("#spn_CarRent").html(OrderObj.pure_price);
            $("#spn_Insurance_price").html(OrderObj.Insurance_price);
            $("#Insurance_price_input").val(OrderObj.Insurance_price).prop("readonly", "");
            $('#spn_InsurancePerHours').html(OrderObj.InsurancePerHours);

            finalPrice = OrderObj.FinalPrice;
            if (LastOrderObj.LastStopTime != "") {
                $("#spn_LastMile").html(LastOrderObj.LastEndMile)
            }
            $("#spn_gift").html(OrderObj.CarPoint);
            $("#spn_moto_gift").html(OrderObj.MotorPoint);
            $("#gift_point_moto_input").val(OrderObj.MotorPoint)
            $("#spn_StartMile").html(OrderObj.SM);

            $("#spn_StopMile").html(OrderObj.EM);


            $("#spn_Mileage").html(OrderObj.mileage_price);
            $("#Mileage_input").val(OrderObj.mileage_price).prop("readonly", "");
            $("#spn_finePrice").html(OrderObj.fine_price);
            $("#spn_payPrice").html(OrderObj.Paid)

            $("#spn_eTag").html(OrderObj.eTag);
            $("#spn_finalPrice").html(OrderObj.final_price + oldOtherPrice);
            $("#pure_price_input").val(OrderObj.pure_price).prop("readonly", "");
            $("#start_mile_input").val(OrderObj.SM).prop("readonly", "");
            $("#end_mile_input").val(OrderObj.EM).prop("readonly", "");
            $("#fine_price_input").val(OrderObj.fine_price).prop("readonly", "");
            $("#final_price_input").val(OrderObj.final_price + oldOtherPrice).prop("readonly", "");
            $("#final_amt").val();
            /*營損開始*/
            $("#CarDispatch_input").val(OrderObj.CarDispatch).prop("readonly", "");
            $("#DispatchRemark_input").val(OrderObj.DispatchRemark);

            $("#CleanFee_input").val(OrderObj.CleanFee).prop("readonly", "");
            $("#CleanFeeRemark_input").val(OrderObj.CleanFeeRemark);

            $("#DestroyFee_input").val(OrderObj.DestroyFee).prop("readonly", "");
            $("#DestroyFeeRemark_input").val(OrderObj.DestroyFeeRemark);

            $("#ParkingFee_input").val(OrderObj.parkingFee).prop("readonly", "");
            $("#ParkingFeeRemark_input").val(OrderObj.ParkingFeeRemark);



            $("#DraggingFee_input").val(OrderObj.DraggingFee).prop("readonly", "");
            $("#DraggingFeeRemark_input").val(OrderObj.DraggingFeeRemark);

            $("#OtherFee_input").val(OrderObj.OtherFee).prop("readonly", "");
            $("#OtherFeeRemark_input").val(OrderObj.OtherFeeRemark);
            $("#ParkingFeeByMachi_input").val(OrderObj.PARKINGAMT2).prop("readonly", "");
            $("#ParkingFeeByMachiRemark_input").val(OrderObj.PARKINGMEMO2);
            oldOtherPrice = OrderObj.CarDispatch + OrderObj.CleanFee + OrderObj.DestroyFee + OrderObj.parkingFee + OrderObj.DraggingFee + OrderObj.OtherFee + OrderObj.PARKINGAMT2;

            /*營損結束*/

            $("#gift_point_input").hide();
            $("#gift_point_select").empty().show();
            if (BonusObj) {
                for (var i = 0; i <= BonusObj.CanUseTotalCarPoint; i += 30) {

                    $("#gift_point_select").append(`<option value="${i}">${i}</option>`);
                }
            }
            $("#gift_point_select").val(OrderObj.CarPoint)

            $("#final_amt").val("");

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

        }
    } else {
        $("#panelResult").hide();
        ShowFailMessage("此訂單非汽車訂單");
    }
   
}