

window.onload = function () {
    //取得觸發iCon
    var imgs = document.getElementById('align-self-center');
    var row = document.getElementById('w-100');
    //取得網址列參數
    var getUrlString = location.href;
    var url = new URL(getUrlString);
    var url_HCToken = url.searchParams.get('HCToken');
    var url_PhoneNo = url.searchParams.get('PhoneNo');
    var url_IDNO = url.searchParams.get('IDNO');

    var flag = 0;

    var VuePage = new Vue({
        el: '#bindNewCard'
        , data: function () {
            var data = {
                form: {}
            };
            return data;
        }
        , methods: {
            // 將網址列Token傳送至HotaiPayController查詢卡清單
            DoLogin: function () {
                var self = this;
                // 組合表單資料
                var postData = {};
                postData['HCToken'] = url_HCToken;
                if (url_HCToken != null && url_HCToken != '') {
                    // 使用 jQuery Ajax 傳送至後端
                    $.ajax({
                        url: 'DoCreditStart',
                        method: 'POST',
                        dataType: 'json',
                        data: {
                            HCToken: postData['HCToken']
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
                    alert("Failed to get AccessToken");
                }
            }
        }
    })
}

//點擊新增信用卡事件：導向至bind-newcard供使用者選擇綁中信/非中信卡
function goAddNewCard() {

    //TODO 連結的值須帶過去

    document.location.href ="/HotaiPay/BindNewCard";


    var Today = new Date();
    alert("今天日期是 " + Today.getFullYear() + " 年 " + (Today.getMonth() + 1) + " 月 " + Today.getDate() + " 日");
}