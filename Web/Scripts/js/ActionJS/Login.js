$(document).ready(function () {
    $(".datePicker").flatpickr();
    $('.dateTimePicker').flatpickr(
        {
            enableTime: true,
            dateFormat: "Y-m-d H:i",
        }
    );
    var IsSuccess = $("#IsSuccess").val();
    var LoginMessage = $("#LoginMessage").val();
    if (IsSuccess != "-1") {
        showLogin(parseInt(IsSuccess), LoginMessage);
    }
    $("#btnLogin").on("click", function () {
        $.busyLoadFull("show", {
            text: "登入中",
            fontawesome: "fa fa-cog fa-spin fa-3x fa-fw"
        });
    })


})

function showLogin(IsSuccess, message) {
    if (IsSuccess == 1) {
        swal({
            title: 'SUCCESS',
            text: message,
            type: 'success',
            icon: 'success'
        }).then(function (value) {
            window.location.href = "../CarDataInfo/CarDashBoard";
        });
    } else {
        swal({
            title: 'Fail',
            text: message,
            icon: 'error'
        });
    }
}
/**
* 禁止輸入非數字
*
*/
function validate(evt) {
    if (evt.keyCode != 8) {

        var theEvent = evt || window.event;
        var key = theEvent.keyCode || theEvent.which;
        key = String.fromCharCode(key);
        console.log("key:" + key);
        var regex = /[0-9]/;
        if (!regex.test(key)) {
            theEvent.returnValue = false;

            if (theEvent.preventDefault)
                theEvent.preventDefault();
        }
    }
}