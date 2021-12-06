window.onload = function () {
    const optCode = document.getElementById('otpCode');
    const nextStep = document.getElementById('nextStep');

    optCode.addEventListener('input', updateValue);

    function updateValue(e) {
        nextStep.disabled = false ;
        nextStep.className = 'btn btn-blue mb-2';
    }

    //nextStep.onclick = function () {
        
        //var postData = {};
        //postData['otpCode'] = document.getElementById('otpCode').value;
        //// 使用 jQuery Ajax 傳送至後端
        //$.ajax({
        //    url: 'CheckOtpCode',
        //    method: 'POST',
        //    dataType: 'json',
        //    data: {
        //        phone: postData['otpCode']
        //    },
        //    success: function (datas) {
        //        if (datas.ErrMsg) {
        //            alert(datas.ErrMsg);
        //            return;
        //        }
        //        alert(datas.ResultMsg);
        //    },
        //    error: function (err) {
        //        $('#ErrorMsg').html(err.responseText);
        //        $('#ErrorAlert').modal('toggle');
        //    },
        //});

        //window.location.href = 'RegisterStep2';
    }

    function GetOtpCode() {

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
            GetOtpCode: function () {
                var self = this;
                
                // 組合表單資料
                var postData = {};
                postData['phone'] = document.getElementById('otpCode').innerText;
                    // 使用 jQuery Ajax 傳送至後端
                    $.ajax({
                        url: 'GetOtpCode',
                        method: 'POST',
                        dataType: 'json',
                        data: {
                            phone: postData['phone']
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
                
            },
             CheckOtpCode: function () {
                var self = this;

                // 組合表單資料
                var postData = {};
                 postData['phone'] = self.form.Phone;
                 postData['otpCode'] = self.form.OtpCode;
                // 使用 jQuery Ajax 傳送至後端
                $.ajax({
                    url: 'CheckOtpCode',
                    method: 'POST',
                    dataType: 'json',
                    data: {
                        phone: postData['phone'],
                        otpCode: postData['otpCode']
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

            }
        }
    })
}