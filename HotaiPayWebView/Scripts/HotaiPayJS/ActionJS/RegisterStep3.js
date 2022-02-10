window.onload = function () {
    let flag = 0;
    $(":checkbox").click(function () {
        var e = document.getElementById("redbtn");
        if (flag === 0) {
            e.disabled = false;
            flag = 1;
        } else {
            e.disabled = true;
            flag = 0
        }
    })

    function checkSubmit(ctl, event) {
        event.preventDefault();
        swal({
            title: "",
            text: "身分證字號、生日與會員服務與權益相關，請確實填寫，完成註冊後不可自行更改。",
            imageUrl: '../images/alert-popup.svg',
            showCancelButton: true,
            cancelButtonText: "取消",
            cancelButtonClass: "btn-outline-gray-small",
            confirmButtonClass: 'btn-blue-small',
            confirmButtonText: '完成註冊'
        }, function (isConfirm) {
            if (isConfirm) {
                $("#ProfileForm").submit();
            } else {
                swal("Cancelled", "You have Cancelled Form Submission!", "error");
            }
        }
        );
    };
}