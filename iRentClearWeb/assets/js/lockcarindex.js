$(document).ready(function () {
    $("#setCard").on("click", function () {
        var OrderNum = $("#OrderNum").val();
        var IDNo = $("#IDNO").val();
        var flag = true;
        var errMsg = "";
        

        flag = /^(H)[0-9]{1,7}$/.test(OrderNum);
        if (flag) {
            flag = /^[A-Za-z]{1}[1-2]{1}[0-9]{8}$/.test(IDNo);
            if (flag == false) {
                errMsg = "身份證不正確";
            }
        } else {
            errMsg = "訂單編號不正確";
        }
        if (flag) {
            var SendObj = new Object();
            SendObj.OrderNum = OrderNum;
            blockUI();
            //var ExtendTime=new Date(ExtendD + ' ' + EH+":"+EM+":00")
            SendObj.IDNO = IDNo;
            console.log(SendObj);
            var jdata = JSON.stringify({ "para": SendObj });
            GetEncyptData(jdata, lockcarindexComplete);
        } else {
            warningAlert(errMsg, false, 0, "");
        }
    });
});