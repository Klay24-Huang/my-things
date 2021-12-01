import { data } from "jquery";

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
            // 獲取驗證碼按鈕
            SetSignUpProfile: function () {
                var self = this;
                var inputJson = dataJson[];
                dataJson["name"] = self.form.Name;
                dataJson["email"] = self.form.Email;
                dataJson["birthday"] = self.form.Birth;
                var sexSirCheck = document.getElementById("sex_sir");
                var sexLadyCheck = document.getElementById("sex_lady");
                if (sexSirCheck.checked == true) {
                    dataJson["sex"] = "M";
                } else if (sexLadyCheck.checked == true) {
                    dataJson["sex"] = "F";
                }
                dataJson["id"] = self.form.CustID;
                
                // 組合表單資料
                var postData = {};
                postData['phone'] = self.form.Phone;
                // 使用 jQuery Ajax 傳送至後端
                $.ajax({
                    url: 'SetSignUpProfile',
                    method: 'POST',
                    dataType: 'json',
                    data: JSON.stringify(dataJson),
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

            }
        }
    })
}