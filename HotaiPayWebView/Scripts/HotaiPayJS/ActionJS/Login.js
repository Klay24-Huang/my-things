window.onload = function () {
    //獲取元素（兩種方式都可以）
    var input = document.getElementById("demo_input")
    var imgs = document.getElementById('eyes');
    //下面是一個判斷每次點選的效果
    var flag = 0;
    imgs.onclick = function () {
        if (flag == 0) {
            input.type = 'text';
            eyes.src = '~\images\eye-regular.svg';//睜眼圖
            flag = 1;
        } else {
            input.type = 'password';
            eyes.src = '~\images/eye-slash-regular.svg';//閉眼圖
            flag = 0;
        }
    }
    //var VuePage = new Vue({
    //    el: '#VuePage'
    //    , data: function () {
    //        var data = {
    //            form: {}
    //        };
    //        return data;
    //    }
    //    , methods: {
    //        // 執行登入按鈕
    //        Login: function () {
    //            var self = this;
    //            var regex = new RegExp(/^(?=.*\d)(?=.*[a-zA-Z]).{6,12}$/);
    //            console.log('哈')
    //            // 組合表單資料
    //            var postData = {};
    //            postData['phone'] = self.form.Phone;
    //            postData['pwd'] = self.form.UserPwd;
    //            console.log("SSSS")
    //            if (self.form.UserPwd.match(regex) != null) {
    //                console.log("OOOOOO")
    //                // 使用 jQuery Ajax 傳送至後端
    //                $.ajax({
    //                    url: 'Login',
    //                    method: 'POST',
    //                    dataType: 'json',
    //                    data: {
    //                        phone: postData['phone'],
    //                        pwd: postData['pwd']
    //                    },
    //                    success: function (datas) {
    //                        if (datas.ErrMsg) {
    //                            alert(datas.ErrMsg);
    //                            return;
    //                        }
    //                        console.log("QQQ")
    //                        console.log(datas)
    //                        alert(datas.ResultMsg);
    //                    },
    //                    error: function (err) {
    //                        $('#ErrorMsg').html(err.responseText);
    //                        $('#ErrorAlert').modal('toggle');
    //                    },
    //                });
    //            } else {
    //                console.log("XXXXX")
    //                var hints = document.getElementById('pwdHint');
    //                hints.textContent = '密碼格式錯誤:需至少有各一個大小寫英文、密碼長度介於6~12碼之間';
    //            }
    //        }
    //    }
    //})
}