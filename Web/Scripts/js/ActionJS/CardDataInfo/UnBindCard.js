$(function () {
    $("#btnSend").on("click", function () {
        ShowLoading("卡號解除中");

        var IDNO = $("#IDNO").val();
        var OrderNo = $("#OrderNo").val();
        var flag = true;

        var errMsg = "";

       
          if (flag) {
              if (OrderNo == "") {
                  flag = false;
                  errMsg = "訂單編號未填";
              } else {
                  if (false == RegexOrderNo(OrderNo)) {
                      flag = false;
                      errMsg = "訂單編號格式不符（格式：H+7碼數字，未滿7碼左補0)";
                  }
              }
        }
        if (flag) {
            if (IDNO == "") {
                flag = false;
                errMsg = "身份證未填";
            }
        }
          
        
        if (flag) {
            var Account = $("#Account").val();
            var obj = new Object();
            obj.UserID = Account;
            obj.OrderNo = OrderNo;
            obj.IDNO = CardNo;
            DoSend(obj);
        } else {
            disabledLoadingAndShowAlert(errMsg);
        }
});
function DoSend(obj) {
    ShowLoading("資料處理中");
    var Account = $("#Account").val();

    var flag = true;
    var errMsg = "";

    if (flag) {


        var json = JSON.stringify(obj);
        console.log(json);
        var site = jsHost + "BE_UnBind";
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
                    text: "刪除萬用卡發生錯誤",
                    icon: 'error'
                });
            }

        });
    } else {
        disabledLoadingAndShowAlert(errMsg);
    }
}