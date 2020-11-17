$(function () {
    var now = new Date();
    SetStationNoShowName($("#StationID"));
    SetCar($("#CarNo"))
    if (errorLine == "ok") {

    } else {
        if (errorMsg != "") {
            ShowFailMessage(errorMsg);
        }
    }
    var hasData = parseInt($("#len").val());
    if (hasData > 0) {
        $('.table').footable({
            "paging": {
                "enabled": true,
                "limit": 3,
                "size": 20
            }
        });
    }

})
function ViewDetail(OrderNo, Mode) {
    $("#DetailOrderNo").val(OrderNo);
    if (Mode == 0) {
        document.getElementById("showDetail").action="ContactDetail"
    } else {
        document.getElementById("showDetail").action = "ContactMotorDetail"
    }
    $("#showDetail").submit()
    console.log(OrderNo);
}
function CancelOrder(OrderNo) {
    var Account = $("#Account").val();
    var obj = new Object();
    obj.UserID = Account;
    obj.OrderNo = OrderNo;
    var json = JSON.stringify(obj);
    console.log(json);
    var site = jsHost + "BE_CanccelOrder";
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
                text: "取消訂單發生錯誤",
                icon: 'error'
            });
        }

    });
}
function ReBook(OrderNo) {
    var Account = $("#Account").val();
    var obj = new Object();
    obj.UserID = Account;
    obj.OrderNo = OrderNo;
    var json = JSON.stringify(obj);
    console.log(json);
    var site = jsHost + "BE_ChangeCar";
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
                var newCar = data.Data.NewCarNo;
                var successMsg = "新車號：【" + newCar + "】，請客戶重新取車"
                swal({
                    title: 'SUCCESS',
                    text: successMsg,
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
                text: "換車發生錯誤",
                icon: 'error'
            });
        }

    });
}

function setStation(StationID) {
    $('#StationID').val(StationID);
}