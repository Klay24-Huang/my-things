$(document).ready(function () {
    $('input[name="qtype"]').on('click', function () {
        var type = $(this).val();
        console.log(type);
        switch (type) {
            case '0':

                $('#editor').hide();

                break;
            case '1':

                $('#editor').show();

                break;
        }
    });
    if ($('input[name = "qtype"]').val() == "0") {
        $('#editor').hide();
    } else {
        $('#editor').show();
    }

    var hasData = parseInt($("#len").val());
    console.log("hasData=" + hasData);
    if (hasData > 0) {
        $("#ShowSetCardPanel").show();
        $('.table').footable({
            "paging": {
                "enabled": true,
                "limit": 3,
                "size": 20
            }
        });
    } else {
        $("#ShowSetCardPanel").hide();
    }
    $("#btnSave").on("click", function () {
        saveNews();
    });
   // $('input[name="qtype"]').toggle("click");
});
//新增優惠訊息
function saveNews() {
    ShowLoading("資料處理中…");
var now = new Date();
var SD = $("#StartDate").val();
var ED = $("#EndDate").val();
var Title = $("#Title").val();
var Content = $("#Content").val();

var Mode = $("#Mode").val();
var NewsID = $("#NewsID").val()
var Account=$("#Account").val()
var NewsType = $('input[name$="qtype"]:checked').val(); //$('input[name$="qtype"]').val();
var NewsClass = $('input[name$="qclass"]:checked').val(); //$('input[name$="qtype"]').val();
var BeTop = $('input[name$="betop"]:checked').val(); //$('input[name$="qtype"]').val();
var url = $("#url").val();
var Start = new Date(SD + ':00');
var End = new Date(ED + ':00');
var errMsg = "";
var flag = true;
var checkList = new Array(SD, ED, Title, Content, url);
var errList = new Array("開始日期不能為空白", "結束日期不能為空白", "標題不能為空白", "內文不能為空白", "網址不能為空白");
var len = 3;
if (NewsType == 1) {
    len = 4;
}
if (Mode < 2) {
    for (i = 0; i <= len; i++) {
        if (checkList[i] === "") {
            flag = false;
            errMsg = errList[i];
            break;
        }
    }
}
if (Mode > 0) {
    if (NewsID === "" || NewsID == 0) {
        flag = false;
        errMsg = "請先選擇要修改或刪除的訊息";
    }
}
if (flag) {
    if (NewsType === 1) {
        flag = checkUrl(url);
        if (false === flag) {
            errMsg = "不是合法的url";
        }
    }
    if (flag) {
        if (now > Start) {
            flag = false;
            errMsg = "開始時間不能小於現在日期";
        }
        if (flag) {
            if (Start > End) {
                flag = false;
                errMsg = "開始時間不能大於結束時間";
            }
        }
    }

}

if (flag) {
    var SendObj = new Object();
    SendObj.UserID = Account;
    SendObj.Content = Content;
    SendObj.ED = ED;
    SendObj.SD = SD;
    SendObj.Title = Title;
    SendObj.URL = url;
    SendObj.NewsType = NewsType;
    SendObj.NewsClass = NewsClass;
    SendObj.NewsID = NewsID;
    SendObj.Mode = Mode;
    SendObj.BeTop = BeTop;
    $("#NewsID").val(0);
    DoAjaxAfterSubmit(SendObj, "BE_NewsHandle", "推播處理發生錯誤", $("#frmNews"))
  
} else {
    disabledLoadingAndShowAlert(errMsg);
}

console.log("sd=" + SD + ";ed=" + ED + ";title=" + Title + ";content=" + Content + ";newstype=" + NewsType + ";url=" + url + ";startTime=" + Start + ";endTime=" + End);
}
function EditNews(NewsID) {
    console.log(NewsID);
    $("#NewsID").val(NewsID);
    $("#Content").hide();
    frmNews.submit();
}
function DelNews(NewsID,SD,ED) {
    ShowLoading("資料處理中…");
 
    if (NewsID > 0) {

        var Account = $("#Account").val()
        var SendObj = new Object();
        SendObj.Content = "刪除";
        SendObj.SD = SD;
        SendObj.ED =ED;
        SendObj.Title = "刪除";
        SendObj.URL = "";
        SendObj.NewsType = 0;
        SendObj.NewsID = NewsID;
        SendObj.Mode = 2;
        SendObj.UserID = Account;
        DoAjaxAfterReload(SendObj, "BE_NewsHandle", "推播處理發生錯誤");
    }
}