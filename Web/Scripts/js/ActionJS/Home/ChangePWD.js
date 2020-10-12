$(function () {
    console.log("jsHost=" + jsHost);
    var Account = $("#Account").val();
    var form = $("#frmChangePWD");
    var flag = true;
    var message = "";
    $("#btnSend").on("click", function () {
     
            flag = true;
            message = "";
        $.busyLoadFull("show", {
            text:"資料處理中...",
            spinner: "cube-grid"
        });
   

        form.validate({
            rules: {
                OldPwd: { required: true },
                NewPwd: { required: true },
                ConfirmPwd: { required: true}
            }, messages: {
                OldPwd: { required: "請輸入舊密碼" },
                NewPwd: { required: "請輸入新密碼" },
                ConfirmPwd: { required: "請輸入確認新密碼" }

            }


        });
        if (form.validate()) {
            var NewPwd = $("#NewPwd").val();
            var ConfirmPwd = $("#ConfirmPwd").val();
            if (NewPwd != ConfirmPwd) {
                flag = false;
                message = "新密碼與確認新密碼不相同";
            }

        } else {
            flag = false;
            message = "必填欄位未填";
        }
        if (flag) {
            var obj = new Object();
            obj.Account = Account;
            obj.OldPWD = $("#OldPwd").val();
            obj.NewPWD = $("#NewPwd").val();
            var json = JSON.stringify(obj);
            console.log(json);
            var site = jsHost + "BE_ChangePWD";
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
                        text: "更改密碼發生錯誤",
                        icon: 'error'
                    });
                }

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