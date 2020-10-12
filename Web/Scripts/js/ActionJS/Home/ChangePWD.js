$(function () {
    var form = $("#frmChangePWD");
    $("#btnSend").on("click", function () {
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
            swal({
                title: 'SUCCESS',
                text: message,
                icon: 'error'
            });
        } else {
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