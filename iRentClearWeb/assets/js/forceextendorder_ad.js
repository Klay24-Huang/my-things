$(document).ready(function () {
    $("#btnExtend").on("click", function () {
        var OrderNum = $("#OrderNum").val();
        var ExtendD = $("#date").val();
        var EH = $("#EH").val();
        var EM = $("#EM").val();
        var flag = true;
        var errMsg = "";
        if (EH === "-1" ) {
            errMsg = "未選擇時";
            flag = false;
        }
        if(flag){
            if (EM === "-1") {
                errMsg = "未選擇分";
                flag = false;
            }
        }
        if (flag) {
            if (ExtendD === "") {
                errMsg = "未選擇日期";
                flag = false;
            }
        }
        if (flag) {
            if (OrderNum === "") {
                errMsg = "未輸入訂單編號";
                flag = false;
            }
        }
        if (flag) {
            var SendObj = new Object();
            SendObj.OrderNum = OrderNum;
        
            //var ExtendTime=new Date(ExtendD + ' ' + EH+":"+EM+":00")
            SendObj.ExtendTime = ExtendD.replace(/\//g, "") + EH + EM + "00";
            console.log(SendObj);
            var jdata = JSON.stringify({ "para": SendObj });
            GetEncyptData(jdata, forceExtendComplete);
        } else {
            warningAlert(errMsg, false, 0, "");
        }
    });
});
function pad(number, length) {

    var str = '' + number;
    while (str.length < length) {
        str = '0' + str;
    }

    return str;

}