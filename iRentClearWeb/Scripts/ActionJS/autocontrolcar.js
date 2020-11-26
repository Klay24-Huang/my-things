$(document).ready(function () {
    var opt = {
        dateFormat: 'yy-mm-dd'
    };
  
    $("#divReturnDate").hide();
    $("#type").on("change", function () {
        var checkValue = $(this).val();
        console.log(checkValue);
        if (checkValue === "1") {
            $("#divReturnDate").show();
            //  $("#returnDate").datetimepicker(opt);
            $("#returnDate").datepicker(opt);
            var nowDate = new Date();
            var HH = (nowDate.getHours() < 10) ? "0" + nowDate.getHours().toString() : nowDate.getHours().toString();
            var MM = (nowDate.getMinutes() < 10) ? "0" + nowDate.getMinutes().toString() : nowDate.getMinutes().toString();
            var SS = (nowDate.getSeconds() < 10) ? "0" + nowDate.getSeconds().toString() : nowDate.getSeconds().toString();
            $("#HH").val(HH);
            $("#MM").val(MM);
            $("#SS").val(SS);
        } else {
            $("#divReturnDate").hide();
        }
    });
    $("#btnSend").on("click", function () {
        var OrderNum = $("#OrderNum").val();
        var type = $("#type").val();
        var Mode = $("#Mode").val();
        var returnDate = $("#returnDate").val();
        var flag = true;
        var errMsg = "";
        var nowDate=new Date();
        var UserID = $("#UserID").val();
        if (OrderNum === "") {
            flag = false;
            errMsg = "交易序號不能為空白";
        }
        if (type === "-1") {
            flag = false;
            errMsg = "請選擇動作類型";
        }
        if (Mode === "-1") {
            flag = false;
            errMsg = "請選擇動作用途";
        }
        if (flag) {
            flag = /^(H)[0-9]{1,7}$/.test(OrderNum);
            if (flag === false) {
                errMsg = "訂單編號格式不符";
            }
        }
        if (flag) {
            if (type === "1") {
                if (returnDate == "") {
                    flag = false;
                    errMsg = "請輸入強制還車時間";
                }
            } else {
                returnDate = "";
            }
        }
        if (flag) {
            var SendObj = new Object();
            SendObj.OrderNum = OrderNum;
            blockUI();
            if (returnDate === "") {
                returnDate = nowDate.getFullYear().toString() + "-" + (nowDate.getMonth()+1).toString() + "-" + nowDate.getDate().toString() + " " + nowDate.getHours().toString() + ":" + nowDate.getMinutes().toString() + ":00";
            } else {
                returnDate = returnDate + " " + $("#HH").val() + ":" + $("#MM").val() + ":" + $("#SS").val();
            }
            console.log(returnDate);

            //var ExtendTime=new Date(ExtendD + ' ' + EH+":"+EM+":00")
            SendObj.type = type;
            SendObj.Mode = Mode;
            SendObj.returnDate = returnDate;
			 SendObj.UserID = UserID;
            console.log(SendObj);
            var jdata = JSON.stringify({ "para": SendObj });
            GetEncyptData(jdata, carControlComplete);
        } else {
            warningAlert(errMsg, false, 0, "");
        }
    
    });
});