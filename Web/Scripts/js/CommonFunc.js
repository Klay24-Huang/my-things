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
    console.log("json:" + json);
    var site = jsHost + API;
    //var site = "http://localhost:2061/api/BE_HandleUserMaintain" //202012唐測試用
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

            console.log("data:" + data);

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
                    title: 'Fail011',
                    text: data.ErrorMessage,
                    icon: 'error'
                });
            }
        },
        error: function (e) {
            $.busyLoadFull("hide");
            swal({
                title: 'Fail022',
                text: FailMessage,
                icon: 'error'
            });
        }

    });
}
function DoAjaxAfterGoBack(obj, API, FailMessage) {
    var json = JSON.stringify(obj);
    console.log(json);
    var site = jsHost + API;
    //var site = "http://localhost:2061/api/BE_Banner" //202012唐測試用
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
                    history.back();
                });
            } else {

                swal({
                    title: 'Fail1',
                    text: data.ErrorMessage,
                    icon: 'error'
                });
            }
        },
        error: function (e) {
            $.busyLoadFull("hide");
            swal({
                title: 'Fail2',
                text: FailMessage,
                icon: 'error'
            });
        }

    });
}

//20210421唐加，悠遊付退款專用
function DoAjaxAfterGoBack_EW(obj, FailMessage) {
    var json = JSON.stringify(obj);
    //var site = "https://easywallet.ai-irent.net/api/Exec/refundECOrder"
    var site = "http://localhost:60985/api/Exec/refundECOrder"
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
                    history.back();
                });
            } else {

                swal({
                    title: 'Fail1',
                    text: data.ErrorMessage,
                    icon: 'error'
                });
            }
        },
        error: function (e) {
            $.busyLoadFull("hide");
            swal({
                title: 'Fail2',
                text: FailMessage,
                icon: 'error'
            });
        }

    });
}

//20210427唐加，悠遊付退款專用
function DoAjaxAfterGoBack_EW_2(obj, FailMessage) {
    var json = JSON.stringify(obj);
    //var site = "https://easywallet.ai-irent.net/api/Exec/refundECOrder2"
    var site = "http://localhost:60985/api/Exec/refundECOrder2"
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
                    history.back();
                });
            } else {

                swal({
                    title: 'Fail3',
                    text: data.ErrorMessage,
                    icon: 'error'
                });
            }
        },
        error: function (e) {
            $.busyLoadFull("hide");
            swal({
                title: 'Fail4',
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
                    console.log(data.Data);
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
function DoAjaxAfterCallBackWithOutMessage(obj, API, FailMessage, CallBack) {
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
               CallBack(data);
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

function SetCity(obj) {
    var CityList = localStorage.getItem("CityList");
    obj.empty();
    if (CheckStorageIsNull(CityList)) {
        CityList = JSON.parse(CityList);
    }
    obj.append($('<option>', { value: 0, text:"請選擇縣市" }));
    if (CityList.length > 0) {
        var CityLen = CityList.length;
        for (var i = 0; i < CityLen; i++) {
            console.log(CityList[i].CityName);
            obj.append($('<option>', { value: CityList[i].CityID, text: CityList[i].CityName }));
        }
        obj.val(0);
    }
}
/**
 * 
 * @param {any} obj                   目標
 * @param {any} selectValue           選擇的資料
 * @param {any} triggerObj            trigger的物件
 * @param {any} triggerObjSelectValue trigger後物件預設選擇的值
 */
function SetCityHasSelected(obj,selectValue,triggerObj,triggerObjSelectValue) {
    var CityList = localStorage.getItem("CityList");
    var AreaList = localStorage.getItem("AreaList");
    //console.log(CityList);
    obj.empty();
    if (CheckStorageIsNull(CityList)) {
        CityList = JSON.parse(CityList);
    }
    if (CheckStorageIsNull(AreaList)) {
        AreaList = JSON.parse(AreaList);
    }
    if (CityList.length > 0) {
        var CityLen = CityList.length;
        for (var i = 0; i < CityLen; i++) {
            //console.log(CityList[i].CityName);
            obj.append($('<option>', { value: CityList[i].CityID, text: CityList[i].CityName }));
        }
        obj.val(selectValue);
    }
    triggerObj.empty();
    //20210105 ADD BY ADAM REASON.排除地區為空的情況
    if (AreaList.length > 0 && triggerObjSelectValue != "0") {
        
        var tmpArea = AreaList.filter(function (Area) { return Area.CityID == selectValue });
        var tmpAreaLen = tmpArea.length;
        if (tmpAreaLen > 0) {
           
            for (var i = 0; i < tmpAreaLen; i++) {
                //console.log(tmpArea[i].AreaName);
                triggerObj.append($('<option>', { value: tmpArea[i].AreaID, text: tmpArea[i].AreaName }));
            }
            if (triggerObjSelectValue != "" && triggerObjSelectValue != "0") {
                triggerObj.val(triggerObjSelectValue);
            }
           
        }
    }

}
function SetCityHasSelectedhaveZipCode(obj, selectValue, triggerObj, triggerObjSelectValue,zipObj) {
    var CityList = localStorage.getItem("CityList");
    var AreaList = localStorage.getItem("AreaList");
    var ZipCode = "";
    //console.log(CityList);
    obj.empty();
    if (CheckStorageIsNull(CityList)) {
        CityList = JSON.parse(CityList);
    }
    if (CheckStorageIsNull(AreaList)) {
        AreaList = JSON.parse(AreaList);
    }
    if (CityList.length > 0) {
        var CityLen = CityList.length;
        for (var i = 0; i < CityLen; i++) {
            //console.log(CityList[i].CityName);
            obj.append($('<option>', { value: CityList[i].CityID, text: CityList[i].CityName }));
        }
        obj.val(selectValue);
    }
    triggerObj.empty();
    if (AreaList.length > 0) {

        var tmpArea = AreaList.filter(function (Area) { return Area.CityID == selectValue });
        var tmpAreaLen = tmpArea.length;
        if (tmpAreaLen > 0) {

            for (var i = 0; i < tmpAreaLen; i++) {
                console.log(tmpArea[i].AreaName);
                var option = $("<option>", { value: tmpArea[i].AreaID, text: tmpArea[i].AreaName + "(" + tmpArea[i].ZIPCode + ")" });
                option.attr("data-zip", tmpArea[i].ZIPCode);
                triggerObj.append(option);
                if (triggerObjSelectValue == tmpArea[i].AreaID) {
                    ZipCode = tmpArea[i].ZIPCode;
                }
            }
            if (triggerObjSelectValue != "" && triggerObjSelectValue!="0") {
                triggerObj.val(triggerObjSelectValue);
                
                zipObj.val(ZipCode);
            }
           
        }
    }

}
function SetArea(obj, SelectValue) {
    var AreaList = localStorage.getItem("AreaList");
    obj.empty();
    if (CheckStorageIsNull(AreaList)) {
        AreaList = JSON.parse(AreaList);
    }
    if (AreaList.length > 0) {

        var tmpArea = AreaList.filter(function (Area) { return Area.CityID == SelectValue });
        var tmpAreaLen = tmpArea.length;
        if (tmpAreaLen > 0) {

            for (var i = 0; i < tmpAreaLen; i++) {
                console.log(tmpArea[i].AreaName);
                obj.append($('<option>', { value: tmpArea[i].AreaID, text: tmpArea[i].AreaName }));
            }
        }
    }
}
function SetAreaHasZip(obj, SelectValue,zipObj) {
    var AreaList = localStorage.getItem("AreaList");
    var ZipCode = "";
    obj.empty();
    if (CheckStorageIsNull(AreaList)) {
        AreaList = JSON.parse(AreaList);
    }
    if (AreaList.length > 0) {

        var tmpArea = AreaList.filter(function (Area) { return Area.CityID == SelectValue });
        var tmpAreaLen = tmpArea.length;
        if (tmpAreaLen > 0) {
            if (SelectValue == "" || SelectValue == "0") {
                SelectValue = tmpArea[0].AreaID;
            }
            for (var i = 0; i < tmpAreaLen; i++) {
                console.log(tmpArea[i].AreaName);

                var option = $("<option>", { value: tmpArea[i].AreaID, text: tmpArea[i].AreaName + "(" + tmpArea[i].ZIPCode + ")" });
                option.data("data-zip", tmpArea[i].ZIPCode);
             
                obj.append(option);
            
                if (SelectValue == tmpArea[i].AreaID) {
                    ZipCode = tmpArea[i].ZIPCode;
                }
              //  obj.attr("data-zipcode", tmpArea[i].ZIPCode);
            }
        }
        console.log("ZipCode=" + ZipCode);
        if (SelectValue != "") {
            obj.val(SelectValue);
            
            zipObj.val(ZipCode);
        }
      

    }
}
function SetZipCode(obj, SelectedValue) {
    obj.val(SelectedValue);
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
function SetStationByText(obj, showNameObj) {
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

                showNameObj.val(data[0]);
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
function SetManagerStationNoShowName(obj) {
    var StationList = localStorage.getItem("ManagerStationList");
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
function SetManagerStationHasValue(obj,selectedValue) {
    var StationList = localStorage.getItem("ManagerStationList");
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
        if (selectedValue != "") {
            obj.val(selectedValue);
        }
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
    var patt = /^(H|h)[0-9]{1,}$/; //20201208唐改/(H)[0-9]{1,}/g
    return patt.test(str);
}
function pad(number, length) {

    var str = '' + number;
    while (str.length < length) {
        str = '0' + str;
    }

    return str;

}
Date.prototype.Format = function (fmt) {
    var o = {
        "M+": this.getMonth() + 1, //月份 
        "d+": this.getDate(), //日 
        "H+": this.getHours(), //小时 
        "m+": this.getMinutes(), //分 
        "s+": this.getSeconds(), //秒 
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
        "S": this.getMilliseconds() //毫秒 
    };
    if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
}
function clearFileInput(id) {
    var oldInput = document.getElementById(id);

    var newInput = document.createElement("input");

    newInput.type = "file";
    newInput.id = oldInput.id;
    newInput.name = oldInput.name;
    newInput.className = oldInput.className;
    newInput.style.cssText = oldInput.style.cssText;
    newInput.accept = oldInput.accept;
    // TODO: copy any other relevant attributes 

    oldInput.parentNode.replaceChild(newInput, oldInput);
}


//20201109 ADD BY JERRY 增加下拉欄位設值的共同處理
function setPostbackValue() {
    $('select').each(function (i, obj) {
        if ($(obj).attr('value') != null) {
            $(obj).val($(obj).attr('value'));
        }
    });
}

//20201211 ADD BY JERRY 增加依權限產生Menu處理
function initMenu() {
    var PowerList = $.trim(localStorage.getItem("PowerList")) == '' ? [] : $.parseJSON(localStorage.getItem("PowerList"));
    var MenuList = $.trim(localStorage.getItem("MenuList")) == '' ? [] : $.parseJSON(localStorage.getItem("MenuList"));
    //console.log(PowerList);
    console.log(MenuList);


    var menuStr = '<ul class="navbar-nav m-auto">' + '\n';
    for (var i = 0; i < MenuList.length; i++) {
        var menuLi = '<li class="nav-item dropdown">' + '\n';
        if (MenuList[i].lstSubMenu.length > 0) {
            menuLi += '<a class="nav-link dropdown-toggle text-white" href="#" id="navbarDropdown' + MenuList[i].MenuCode+'" role="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">' + MenuList[i].MenuName+'</a >' + '\n';
            menuLi += '<div class="dropdown-menu" aria-labelledby="navbarDropdown">' + '\n';
            for (var x = 0; x < MenuList[i].lstSubMenu.length; x++) {
                var powerList = $.grep(PowerList, function (row) {
                    return row.SubMenuCode == MenuList[i].lstSubMenu[x].SubMenuCode;
                });
                //console.log(powerList);
                if (powerList.length > 0) {
                    var checkList = $.grep(powerList[0].PowerList, function (row) {
                        return row.hasPower == 1;
                        //return 1 == 1;
                    });
                }
                if (checkList.length > 0) {
                    if (MenuList[i].lstSubMenu[x].isNewWindow != 9) {
                        menuLi += '<a class="dropdown-item" href="../' + MenuList[i].lstSubMenu[x].MenuController + '/' + MenuList[i].lstSubMenu[x].MenuAction + '" target="' + (MenuList[i].lstSubMenu[x].isNewWindow == 1 ? '_blank' : '_self') + '">' + MenuList[i].lstSubMenu[x].SubMenuName + '</a>' + '\n';
                    }
                }
            }
            menuLi += '</div>' + '\n';
        } else {
            menuLi += '<a class="nav-link dropdown-toggle text-white" href="#" id="navbarDropdown" role="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">據點管理</a >' + '\n';
        }
        menuLi += '</li>' + '\n';
        menuStr += menuLi;
    }
    menuStr += '</ul>' + '\n';
    $('#navbarSupportedContent').html(menuStr);

    $('#navbarSupportedContent .dropdown-menu').each(function () {
        //console.log($(this).html());
        if ($.trim($(this).html()) == '') {
            $(this).parent().css('display', 'none');
            console.log($(this).parent())
        }
    });
}
$(function () {
    initMenu();
});