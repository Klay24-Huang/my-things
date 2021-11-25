window.onload = function () {
    
    var VuePage = new Vue({
        el: '#VuePage'
        , data: function () {
            var data = {
                form: {}
            };
            return data;
        }
        , methods: {
            // 執行登入按鈕
            DoLogin: function () {
                var self = this;
                var regex = new RegExp(/^(?=.*\d)(?=.*[a-zA-Z]).{6,12}$/);

                // 組合表單資料
                var postData = {};
                postData['phone'] = self.form.Phone;
                postData['pwd'] = self.form.UserPwd;
                if (self.form.UserPwd.match(regex) != null) {
                    // 使用 jQuery Ajax 傳送至後端
                    $.ajax({
                        url: 'DoLogin',
                        method: 'POST',
                        dataType: 'json',
                        data: {
                            phone: postData['phone'],
                            pwd: postData['pwd']
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