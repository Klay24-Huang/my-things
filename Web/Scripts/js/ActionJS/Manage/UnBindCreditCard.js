$(function () {
    $("#btnSend").on("click", function () {
        ShowLoading("資料處理中");
        var Account = $("#Account").val();
        var IDNO = $("#IDNO").val();

        var flag = true;
        var errMsg = "";
        var checkList = [IDNO];
        var errMsgList = ["身份證未輸入"];
        for (var i = 0; i < checkList.length; i++) {
            if (checkList[i] == "") {
                errMsg = errMsgList[i];
                flag = false;
                break;
            }
        }
        if (flag) {
            var obj = new Object();
            obj.UserID = Account;
            obj.IDNO = IDNO;
            var json = JSON.stringify(obj);
            console.log(json);
            var site = jsHost + "BE_UnBindCreditCard";
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
                        text: "信用卡解除綁定發生錯誤",
                        icon: 'error'
                    });
                }

            });
        } else {
            disabledLoadingAndShowAlert(errMsg);
        }
    })
});