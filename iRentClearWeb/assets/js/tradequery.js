$(document).ready(function () {
    $("#btnSearch").on("click", function () {
        var qtype = $('input:radio:checked[name="qtype"]').val(); //$("#qtype").val();
        console.log(qtype);
        var term = $("#term").val();
        var flag = true;
        var errMsg = "";
        if (term === "") {
            flag = false;
            switch (qtype) {
                case "1":
                    errMsg = "身份證字號未輸入";
                    break;
                case "2":
                    errMsg = "訂單編號未輸入";
                    break;
                case "3":
                    errMsg = "授權單號未輸入";
                    break;
            }
        } else {
           // var regex = /^(0)(9)[0-9]{8}$/;
            switch (qtype) {
                case "1":
                    if (/^[A-Z]{1}[1-2]{1}[0-9]{8}$/.test(term)==false) {
                        flag = false;
                        errMsg = "身份證格式不正確";
                    }
                    break;
                case "2":
                    if (/^(H)[0-9]{7}$/.test(term) == false) {
                        flag = false;
                        errMsg = "訂單編號格式不正確(EX:H0000001)";
                    }
                    break;
                case "3":
                    errMsg = "授權單號未輸入";
                    break;
            }
        }
        if (flag) {
            formTradequery.submit();
        } else {
            warningAlert(errMsg, false, 0, "");
        }
    });
    
    $('input[name$="qtype"]').on('click', function () {
        var type = $('input:checked').val();
        switch (type) {
            case '1':
                $('#input').html('<span style="color:red">*</span>身份證字號');
                break;
            case '2':
                $('#input').html('<span style="color:red">*</span>訂單編號');
                break;
            case '3':
                $('#input').html('<span style="color:red">*</span>授權單號');
                break;
        }
    });
});