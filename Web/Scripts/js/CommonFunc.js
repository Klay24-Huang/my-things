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