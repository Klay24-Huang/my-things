$(function () {
    var Account = $("#Account").val();
    $("#btnSend").on("click", function () {
        ShowLoading("執行延長中....");
        var OrderNum = $("#OrderNo").val();
        var ExtendD = $("#EndDate").val();
        var EH = $("#Hours").val();
        var EM = $("#Mins").val();
        var flag = true;
        var errMsg = "";
        if (EH === "-1") {
            errMsg = "未選擇時";
            flag = false;
        }
        if (flag) {
            if (EM === "-1") {
                errMsg = "未選擇分";
                flag = false;
            }
        }
        if (flag) {
            if (ExtendD === "") {
                errMsg = "未選擇日期";
                flag = false;
            }
        }
        if (flag) {
            if (OrderNum === "") {
                errMsg = "未輸入訂單編號";
                flag = false;
            }
        }
        if (flag) {
            var obj = new Object();
            obj.OrderNo = OrderNum;

            //var ExtendTime=new Date(ExtendD + ' ' + EH+":"+EM+":00")
            obj.ExtendTime = ExtendD.replace(/\//g, "").replace(/\-/g, "")  + EH + EM + "00";
            obj.UserID = Account;
            console.log(obj);
            var json = JSON.stringify(obj);
            var site = jsHost + "BE_HandleExtendCar";
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
                        text: "綁定車機發生錯誤",
                        icon: 'error'
                    });
                }

            });
        } else {
            disabledLoadingAndShowAlert(errMsg);
        }
    });
});