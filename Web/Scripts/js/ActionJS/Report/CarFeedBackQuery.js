/*var carid = '@ViewData["carid"]';
var objStation = '@ViewData["objStation"]';
var SD = '@ViewData["SDate"]';
var ED = '@ViewData["EDate"]';
var UserID = '@ViewData["userID"]';
var isHandle = '@ViewData["isHandle"]';*/
$("#carid").val(carid);
$("#objStation").val(objStation);
$("#start").val(SD);
$("#end").val(ED);
$("#isHandle").val(isHandle);
$("#userID").val(UserID)

$(function () {
    var now = new Date();
    SetStationNoShowName($("#objStation"));
    SetCar($("#carid"))
  /*  if (errorLine == "ok") {

    } else {
        if (errorMsg != "") {
            ShowFailMessage(errorMsg);
        }
    }*/
    var hasData = parseInt($("#len").val());
    if (hasData > 0) {
        $('.table').footable({
            "paging": {
                "enabled": true,
                "limit": 3,
                "size": 20
            }
        });
        $("#btnDownload").on("click", function () {
            $("#formFeedBackDownload").submit();
        });
    }
    if (SD !== "" || ED !== "" || UserID !== "" || isHandle !== "" || carid != "" || objStation != "") {
        $('#content').show();

    }


})

function doHandle(id, opt) {
    ShowLoading("資料處理中…");
    var flag = true;
    var errMsg = "";
    var descript = $("#handleDescript" + id).val();
    var obj = new Object();
    console.log(descript);
    if (descript == "") {
        flag = false;
        errMsg = "未輸入回饋處理";
    } else {
        obj.FeedBackID = id;
        obj.HandleDescript = descript;
        obj.UserID = opt;
    }
    if (flag) {
        DoAjaxAfterReload(obj, "BE_FeedBackHandle", "儲存回饋處理發生錯誤");
    } else {
        disabledLoadingAndShowAlert(errMsg);
    }
}
function showPic(fileName, obj) {
  //  var img = $("#" + obj);
    $("#ReviewImg").attr("src", fileName);
    $("#surrounding_modal").modal();
    //   img.attr("src", fileName);
   
}
/*function showPic(fileName, obj) {
    var img = $("#" + obj);
    var site1 = "http://113.196.107.238/iMotoAPI/uploadFeedImages/";
    var site2 = "http://113.196.107.239/iMotoAPI/uploadFeedImages/";
    if (validateImage(site1 + fileName)) {
        img.attr("src", site1 + fileName);
    } else {
        img.attr("src", site2 + fileName);
    }
}
function isHasImg(pathImg) {
    var ImgObj = new Image();
    ImgObj.src = pathImg;
    console.log("site=>" + pathImg + ",fileSize=" + ImgObj.fileSize + ",width=>" + ImgObj.width + ",height=>" + ImgObj.height);
    if (ImgObj.fileSize > 0 || (ImgObj.width > 0 && ImgObj.height > 0)) {
        return true;
    } else {
        return false;
    }
}
function validateImage(url) {
    var xmlHttp;
    if (window.ActiveXObject) {
        xmlHttp = new ActiveXObject("Microsoft.XMLHTTP");
    }
    else if (window.XMLHttpRequest) {
        xmlHttp = new XMLHttpRequest();
    }
    xmlHttp.open("Get", url, false);
    xmlHttp.send();
    if (xmlHttp.status == 404)
        return false;
    else
        return true;
}*/