window.onload = function () {
    const input = document.getElementById('confirmPwd');
    const nextStep = document.getElementById('nextStep');

    input.addEventListener('input', updateValue);

    function updateValue(e) {
        nextStep.disabled = false;
        nextStep.className = 'btn btn-blue mb-2';
    }

    var VuePage = new Vue({
        el: '#VuePage'
        , data: function () {
            var data = {
                form: {}
            };
            return data;
        }
        , methods: {
            // 獲取驗證碼按鈕
            DoSignUp: function () {
                var self = this;
                var regex = new RegExp(/^(?=.*\d)(?=.*[a-zA-Z]).{6,12}$/);
                // 組合表單資料
                var postData = {};
                postData['pwd'] = self.form.Pwd;
                postData['pwdConfirm'] = self.form.PwdConfirm;

                if (self.form.UserPwd.match(regex) != null) {
                    // 使用 jQuery Ajax 傳送至後端
                    $.ajax({
                        url: 'DoSignUp',
                        method: 'POST',
                        dataType: 'json',
                        data: {
                            pwd: postData['pwd'],
                            pwdConfirm: postData['pwdConfirm']
                        },
                        success: function (datas) {
                            if (datas.ErrMsg) {
                                alert(datas.ErrMsg);
                                return;
                            }
                            alert(datas.ResultMsg);
                        },
                        error: function (err) {
                            $('#ErrorMsg').html(err.responseText);
                            $('#ErrorAlert').modal('toggle');
                        },
                    });
                } else {
                    var hints = document.getElementById('pwdHint');
                    hints.textContent = '密碼格式錯誤:需至少有各一個大小寫英文、密碼長度介於6~12碼之間';
                }
            }
        }
    })
}