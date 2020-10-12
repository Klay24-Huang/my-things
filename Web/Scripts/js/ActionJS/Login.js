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
    /**
    * 禁止輸入非數字
    * 
    */
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
})