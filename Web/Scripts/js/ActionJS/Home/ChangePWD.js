$(function () {
    var Account = $("Account").val();
    var form = $("#frmChangePWD");
    $("#btnSend").on("click", function () {
        $("#btnLogin").on("click", function () {
            $.busyLoadFull("show", {
                text: "資料處理中",
                fontawesome: "fa fa-cog fa-spin fa-3x fa-fw"
            });
        })

        form.validate({
            rules: {
                OldPwd: { required: true },
                NewPwd: { required: true },
                ConfirmPwd: { required: true, equalTo: NewPwd }
            }, messages: {
                OldPwd: { required: "請輸入舊密碼" },
                NewPwd: { required: "請輸入新密碼" },
                ConfirmPwd: { required: "請輸入確認新密碼", equalTo: "需與新密碼相同" }

            }


        });
        if (form.validate()) {
            $.busyLoadFull("hide");
            var obj = new Object();
            obj.Account = Account;
            obj.OldPWD = $("#OldPwd").val();
            obj.NewPWD = $("#NewPwd").val();
            var json = JSON.stringify(obj);
            console.log(json);
            var site = "http://www.ryankiki.com/HAAScheduleWebAPI/api/HandCustomer";
            $.ajax({
                url: site,
                type: 'POST',
                data: json,
                cache: false,
                contentType: 'application/json',
                dataType: 'json',           //'application/json',
                success: function (data) {
                    $.busyLoadFull("hide");
                    console.log(data);
                    if (data.Result == "1") {
                        swal({
                            title: 'SUCCESS',
                            text: data.msg,
                            type: 'success'
                        }).then(function (value) {
                            window.location.reload();
                        });
                    } else {
                        $.busyLoadFull("hide");
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
                        text: "更改密碼發生錯誤",
                        icon: 'error'
                    });
                }

            });
            swal({
                title: 'SUCCESS',
                text: message,
                icon: 'error'
            });
        } else {
            $.busyLoadFull("hide");
            swal({
                title: 'Fail',
                text: message,
                icon: 'error'
            });
        }
        return false;
    });
    $("#btnClean").on("click", function () {
        $("#OldPwd").val("");
        $("#NewPwd").val("");
        $("#ConfirmPwd").val("");
    })
 

})