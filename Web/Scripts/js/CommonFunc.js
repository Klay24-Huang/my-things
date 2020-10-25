//判斷是否為undefined
function CheckIsUndefined(obj) {
    if (typeof obj !== 'undefined') {
        return true;
    } else {
        return false;
    }
}
/**
 * 顯示loading
 * @param {any} message
 */
function ShowLoading(message) {
    $.busyLoadFull("show", {
        text: message,
        spinner: "cube-grid"
    });
}
function ShowSuccessMessage(message) {
    swal({
        title: 'SUCCESS',
        text: message,
        icon: 'success'
    });
}
function ShowFailMessage(message) {
    swal({
        title: 'Fail',
        text: message,
        icon: 'error'
    });
}
function ShowMessageAndReload(message,message2,site) {
    swal({
        title: 'SUCCESS',
        text: message,
        icon: 'success'
    }).then(function (value) {
        ShowLoading(message2)
        window.location.href=site;
    });
}
/**
 * 隱藏loading 
 */
function disabledLoading() {
    $.busyLoadFull("hide");
}
/**
 * 隱藏loading後顯示訊息
 * @param {any} message
 */
function disabledLoadingAndShowAlert(message) {
    $.busyLoadFull("hide");
    swal({
        title: 'Fail',
        text: message,
        icon: 'error'
    });
}
/**
 * 基本型ajax
 * @param {any} obj
 * @param {any} API
 * @param {any} FailMessage
 */
function DoAjax(obj, API, FailMessage) {
    var json = JSON.stringify(obj);
    console.log(json);
    var site = jsHost + API;
    console.log("site:" + site);
    $.ajax({
        url: site,
        type: 'POST',
        data: json,
        cache: false,
        contentType: 'application/json',
        dataType: 'json',           //'application/json',
        success: function (data) {
            $.busyLoadFull("hide");

            if (data.Result == "1") {
                swal({
                    title: 'SUCCESS',
                    text: data.ErrorMessage,
                    icon: 'success'
                })
            } else {

                swal({
                    title: 'Fail',
                    text: data.ErrorMessage,
                    icon: 'error'
                });
            }
        },
        error: function (e) {
            $.busyLoadFull("hide");
            swal({
                title: 'Fail',
                text: FailMessage,
                icon: 'error'
            });
        }

    });
}
/**
 * 成功能跳出alert，按下確認後再submit
 * @param {any} obj
 * @param {any} API
 * @param {any} FailMessage
 * @param {any} frmObj
 */
function DoAjaxAfterSubmit(obj, API, FailMessage,frmObj) {
    var json = JSON.stringify(obj);
    console.log(json);
    var site = jsHost + API;
    console.log("site:" + site);
    $.ajax({
        url: site,
        type: 'POST',
        data: json,
        cache: false,
        contentType: 'application/json',
        dataType: 'json',           //'application/json',
        success: function (data) {
            $.busyLoadFull("hide");

            if (data.Result == "1") {
                swal({
                    title: 'SUCCESS',
                    text: data.ErrorMessage,
                    icon: 'success'
                }).then(function (value) {
                    frmObj.submit();
                });
            } else {

                swal({
                    title: 'Fail',
                    text: data.ErrorMessage,
                    icon: 'error'
                });
            }
        },
        error: function (e) {
            $.busyLoadFull("hide");
            swal({
                title: 'Fail',
                text: FailMessage,
                icon: 'error'
            });
        }

    });
}
/**
 * 成功能直接submit，不跳出alert
 * @param {any} obj
 * @param {any} API
 * @param {any} FailMessage
 * @param {any} frmObj
 */
function DoAjaxAfterSubmitNonShowMessage(obj, API, FailMessage, frmObj) {
    var json = JSON.stringify(obj);
    console.log(json);
    var site = jsHost + API;
    console.log("site:" + site);
    $.ajax({
        url: site,
        type: 'POST',
        data: json,
        cache: false,
        contentType: 'application/json',
        dataType: 'json',           //'application/json',
        success: function (data) {
            $.busyLoadFull("hide");

            if (data.Result == "1") {
                    frmObj.submit();
            } else {

                swal({
                    title: 'Fail',
                    text: data.ErrorMessage,
                    icon: 'error'
                });
            }
        },
        error: function (e) {
            $.busyLoadFull("hide");
            swal({
                title: 'Fail',
                text: FailMessage,
                icon: 'error'
            });
        }

    });
}
function DoAjaxAfterSubmitNonShowMessageAndNowhide(obj, API, FailMessage, frmObj) {
    var json = JSON.stringify(obj);
    console.log(json);
    var site = jsHost + API;
    console.log("site:" + site);
    $.ajax({
        url: site,
        type: 'POST',
        data: json,
        cache: false,
        contentType: 'application/json',
        dataType: 'json',           //'application/json',
        success: function (data) {
           // $.busyLoadFull("hide");

            if (data.Result == "1") {
                frmObj.submit();
            } else {

                swal({
                    title: 'Fail',
                    text: data.ErrorMessage,
                    icon: 'error'
                });
            }
        },
        error: function (e) {
            $.busyLoadFull("hide");
            swal({
                title: 'Fail',
                text: FailMessage,
                icon: 'error'
            });
        }

    });
}
/**
 * 執行完AJAX後reload
 * @param {any} obj
 * @param {any} API
 * @param {any} FailMessage
 */
function DoAjaxAfterReload(obj,API,FailMessage) {
    var json = JSON.stringify(obj);
    console.log(json);
    var site = jsHost + API;
    console.log("site:" + site);
    $.ajax({
        url: site,
        type: 'POST',
        data: json,
        cache: false,
        contentType: 'application/json',
        dataType: 'json',           //'application/json',
        success: function (data) {
            $.busyLoadFull("hide");

            if (data.Result == "1") {
                swal({
                    title: 'SUCCESS',
                    text: data.ErrorMessage,
                    icon: 'success'
                }).then(function (value) {
                    window.location.reload();
                });
            } else {

                swal({
                    title: 'Fail',
                    text: data.ErrorMessage,
                    icon: 'error'
                });
            }
        },
        error: function (e) {
            $.busyLoadFull("hide");
            swal({
                title: 'Fail',
                text: FailMessage,
                icon: 'error'
            });
        }

    });
}
/**
 * 執行完將API回傳的資料丟入callback函式內
 * @param {any} obj
 * @param {any} API
 * @param {any} FailMessage
 * @param {any} CallBack
 */
function DoAjaxAfterCallBack(obj, API, FailMessage,CallBack) {
    var json = JSON.stringify(obj);
    console.log(json);
    var site = jsHost + API;
    console.log("site:" + site);
    $.ajax({
        url: site,
        type: 'POST',
        data: json,
        cache: false,
        contentType: 'application/json',
        dataType: 'json',           //'application/json',
        success: function (data) {
            $.busyLoadFull("hide");

            if (data.Result == "1") {
                swal({
                    title: 'SUCCESS',
                    text: data.ErrorMessage,
                    icon: 'success'
                }).then(function (value) {
                    CallBack(data.Data);
                });
            } else {

                swal({
                    title: 'Fail',
                    text: data.ErrorMessage,
                    icon: 'error'
                });
            }
        },
        error: function (e) {
            $.busyLoadFull("hide");
            swal({
                title: 'Fail',
                text: FailMessage,
                icon: 'error'
            });
        }

    });
}

/* autocomplete使用*/
/**
 * 據點autocomplete
 * @param {any} obj 觸發物件
 * @param {any} showNameObj 分割後顯示名稱的地方
 */
function SetStation(obj,showNameObj) {
    var StationList = localStorage.getItem("StationList");
    if (CheckStorageIsNull(StationList)) {
        StationList = JSON.parse(StationList)
    }
    if (StationList.length > 0) {

        var Station = new Array();
        var StationLen = StationList.length;
        for (var i = 0; i < StationLen; i++) {
            Station.push(StationList[i].StationName + "(" + StationList[i].StationID + ")");
        }
        obj.autocomplete({
            source: Station,
            minLength: 1,
            matchCase: true,
            select: function (event, ui) {
                var data = ui.item.value.split("(");
                var contactData = data[1].split(")");
                obj.val(contactData[0]);

                showNameObj.html(data[0]);
                return false;
            }
        });

    }

}
function SetStationNoShowName(obj) {
    var StationList = localStorage.getItem("StationList");
    if (CheckStorageIsNull(StationList)) {
        StationList = JSON.parse(StationList)
    }
    if (StationList.length > 0) {

        var Station = new Array();
        var StationLen = StationList.length;
        for (var i = 0; i < StationLen; i++) {
            Station.push(StationList[i].StationName + "(" + StationList[i].StationID + ")");
        }
        obj.autocomplete({
            source: Station,
            minLength: 1,
            matchCase: true,
            select: function (event, ui) {
                var data = ui.item.value.split("(");
                var contactData = data[1].split(")");
                obj.val(contactData[0]);

           
                return false;
            }
        });

    }

}
/**
 * 車號autocomplete
 * @param {any} obj 觸發物件
 */
function SetCar(obj) {
    var CarList = localStorage.getItem("CarList");
    if (CheckStorageIsNull(CarList)) {
        CarList = JSON.parse(CarList)
    }
    if (CarList.length > 0) {
        var Car = new Array();
        var CarLen = CarList.length;
        for (var i = 0; i < CarLen; i++) {
            Car.push(CarList[i].CarNo);
        }

        obj.autocomplete({
            source: Car,
            minLength: 1,
            matchCase: true
        });

    }
}
/**
 * 專員列表autocomplete
 * @param {any} obj 觸發物件
 * @param {any} showNameObj 分割後顯示名稱的地方
 */
function SetManage(obj,showNameObj) {

}

function CheckStorageIsNull(obj) {
    var flag = false;
    if (typeof obj !== 'undefined' && obj !== null) {
        flag = true;
    }
    return flag;
}
function GetFileExtends(fileName) {
    return (/[.]/.exec(fileName)) ? /[^.]+$/.exec(fileName) : undefined;
}

function resetFileInput(file) {
   
    file.after(file.clone().val(""));
    file.remove();
   // file.val("");
}  

function detailMap(lat, lng) {
    window.open("http://maps.google.com.tw/maps?q=" + lng + "," + lat);
}
function RegexOrderNo(str) {
    var flag = true;
    var patt = /([0-9]{1,})|((H)[0-9]{1,})/g;
    return patt.test(str);
}
function pad(number, length) {

    var str = '' + number;
    while (str.length < length) {
        str = '0' + str;
    }

    return str;

}