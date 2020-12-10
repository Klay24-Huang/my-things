$(function () {
    $("#btnSend").on("click", function () {
        ShowLoading("卡號發送中");
        var CardType = $("#CardType").val();
        var CardNo = $("#CardNo").val();
        var CarNo = $("#CarNo").val();
        var OrderNo = $("#OrderNo").val();
        var flag = true;

        var errMsg = "";
        if (false == CheckStorageIsNull(CardType)) {
            flag = false;
            errMsg = "請選擇卡別";
        } else {
            if (CardType == "-1") {
                flag = false;
                errMsg = "請選擇卡別";
            }
        }
        if (flag) {
            if (CardNo == "") {
                flag = false;
                errMsg = "卡號未填";
            }
        }

        if (flag) {
            if (CardType == "0") { //萬用卡
                if (CarNo == "") {
                    flag = false;
                    errMsg = "車號未填";
                }
            } else {
                //一般卡

                if (flag) {
                    if (OrderNo == "") {
                        flag = false;
                        errMsg = "訂單編號未填";
                    } else {
                        if (false == RegexOrderNo(OrderNo)) {
                            flag = false;
                            errMsg = "訂單編號格式不符（格式：H+7碼數字，未滿7碼左補0)";
                        }
                    }
                }
            }
        }
        if (flag) {
            var Account = $("#Account").val();
            var obj = new Object();
            obj.UserID = Account;
            obj.CardType = CardType;
            obj.OrderNo = OrderNo;
            obj.CardNo = CardNo;
            DoSend(obj);
        } else {
            disabledLoadingAndShowAlert(errMsg);
        }


    });

    $("#CardType").val("1");
});
function DoSend(obj) {
    ShowLoading("資料處理中");
    var Account = $("#Account").val();

    var flag = true;
    var errMsg = "";

    if (flag) {


        var json = JSON.stringify(obj);
        console.log(json);
        var site = jsHost + "BE_SentCardSetting";
        console.log("site:" + site);
        $.ajax({
            url: site,
            type: 'POST',
            data: json,
            cache: false,
            contentType: 'application/json',
            dataType: 'json',           //'application/json',
            success: function (data) {
                $.busyLoadFull("hide");

                if (data.Result == "1") {
                    swal({
                        title: 'SUCCESS',
                        text: data.ErrorMessage,
                        icon: 'success'
                    }).then(function (value) {
                        window.location.reload();
                    });
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
                    text: "刪除萬用卡發生錯誤",
                    icon: 'error'
                });
            }

        });
    } else {
        disabledLoadingAndShowAlert(errMsg);
    }
}