var carid = '@ViewData["carid"]';
var objStation = '@ViewData["objStation"]';
var SD = '@ViewData["SDate"]';
var ED = '@ViewData["EDate"]';
var UserID = '@ViewData["userID"]';
var isHandle = '@ViewData["isHandle"]';
$("#carid").val(carid);
$("#objStation").val(objStation);
$("#start").val(SD);
$("#end").val(ED);
$("#isHandle").val(isHandle);
$("#userID").val(UserID);
var hasData = '@feedBackLen';
$(function () {
    var now = new Date();
    SetStationNoShowName($("#objStation"));
    SetCar($("#carid"))
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
        $("#btnDownload").on("click", function () {
            $("#formFeedBackDownload").submit();
        });
    }
    if (SD !== "" || ED !== "" || UserID !== "" || isHandle !== "" || carid != "" || objStation != "") {
        $('#content').show();

    }

})

function doHandle(id, opt) {
    var flag = true;
    var descript = $("#handleDescript" + id).val();
    console.log(descript);
    if (descript == "") {
        swal({
            title: '處理失敗',
            text: "未輸入回饋處理",
            showCancelButton: false,
            type: "error",
            position: "center-left",
            customClass: "aaaa"
        });
    } else {
        var jsonData = JSON.stringify({ "para": { "handleDescript": descript, "feedBackID": id, "User": opt } });
        var title = "確認做此處理";
        var msg = "確定送出";
        swal({
            title: title,
            text: msg,
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#DD6B55",
            confirmButtonText: '確定'
        }).then((isConfirm) => {
            if (isConfirm) {
                blockUI();
                $.ajax({
                    url: 'http://113.196.107.238/iMotoWebAPI/api/feedBackHandle',
                    type: 'POST',
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    data: jsonData,
                    async: true,
                    success: function (data, status) {
                        console.log(data);
                        if (data.ErrorCode == "000000") {
                            $.unblockUI();
                            swal({
                                title: '',
                                text: '處理成功',
                                showCancelButton: false,
                                type: "success",
                                position: "center-left",
                                customClass: "aaaa"
                            }).then(() => {
                                window.location.reload();
                            });
                        }
                        else {
                            $.unblockUI();
                            swal({
                                title: '處理失敗',
                                text: data.ErrMsg,
                                showCancelButton: false,
                                type: "error",
                                position: "center-left",
                                customClass: "aaaa"
                            });
                        }

                    },
                    error: function (xhr, option, error) {
                        $.unblockUI();
                        swal({
                            title: '處理失敗',
                            text: data.ErrMsg,
                            showCancelButton: false,
                            type: "error",
                            position: "center-left",
                            customClass: "aaaa"
                        });
                    }

                });
            } else {

            }
        });
    }
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