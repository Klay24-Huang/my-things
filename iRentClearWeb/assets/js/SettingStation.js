var host = "http://113.196.107.238/"
function doSetting() {
    $.busyLoadFull("show", {
        spinner: "cube-grid"
    });
    console.log("hello Setting");
    var StationID = new Array();

    $("input[name='StationID[]']").each(function () {
        if ($(this).prop("checked")) {
            StationID.push($(this).val());
        }
    });
    console.log(StationID.length);

    var URL = host + "iMotoWebAPI/api/SettingManageStation";// GetCleanCar
    var StationIDStr = "";
    if (StationID.length > 0) {
        StationIDStr = JSON.stringify(StationID);
    }

    var jsonData = "";
    if (StationID.length > 0) {
        jsonData =JSON.stringify({ "para": { "Account": $("#Account").val(), "StationID": StationID} });
    } else {
        jsonData =JSON.stringify({ "para": { "Account": $("#Account").val()} });
    }
    
    console.log(jsonData);
    $.ajax({
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: jsonData,
        url: URL,
        error: function (xhr, error) { console.log(xhr.responseText + "," + xhr.status + "," + error); $.busyLoadFull("hide", { animate: "fade" }); },
        success: function (JsonData) {
            console.log(JsonData.Result + "," + JsonData.ErrMsg);
            console.log(JsonData);
            if (JsonData.Result === "0") {
                $.busyLoadFull("hide", { animate: "fade" });
                swal({
                    title: '設定據點成功',
                    text: data.ErrMsg,
                    showCancelButton: false,
                    type: "success",
                    position: "center-left",
                    customClass: "aaaa"
                }).then(() => {
                    window.location.reload();
                });

            } else {
                $.busyLoadFull("hide", { animate: "fade" });
                swal({
                    title: '設定資料發生錯誤',
                    text: data.ErrMsg,
                    showCancelButton: false,
                    type: "error",
                    position: "center-left",
                    customClass: "aaaa"
                }).then(() => {
                    window.location.reload();
                });
                console.log("false");

            }
        }
    });
}
function doDownload() {
    $.busyLoadFull("show", {
        spinner: "cube-grid"
    });
    console.log("hello Download");
    $("#frmDownload").submit();
    $.busyLoadFull("hide", { animate: "fade" });
   
}
$(document).ready(function () {
    let count = 10; 
    $.busyLoadFull("show", {
        spinner: "cube-grid"
    });

    getData();
  //  $.busyLoadFull("hide", { animate: "fade" });
});

function getData() {
    var URL = host + "iMotoWebAPI/api/GetManageStationSetting";// GetCleanCar
  
    var jsonData = JSON.stringify({ "para": { "Account": $("#Account").val()} });
    console.log(jsonData);

    console.log(URL);
    $.ajax({
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: jsonData,
        url: URL,
        error: function (xhr, error) { console.log(xhr.responseText + "," + xhr.status + "," + error); $.busyLoadFull("hide", { animate: "fade" }); },
        success: function (JsonData) {
            console.log(JsonData.Result + "," + JsonData.ErrMsg);
            console.log(JsonData);
            if (JsonData.Result === "0") {
                console.log("true");
                console.log(JsonData.data);
                var dataLen = JsonData.data.length;
                console.log("dataLen:" + dataLen);
                $.busyLoadFull("hide", { animate: "fade" });
                var tmpHTML = "";
                if (dataLen > 0) {
                    tmpHTML += "<tr align=\"center\"><td colspan=\"3\"><input type=\"button\" name=\"btnSetting\" class=\"btn-danger\" value=\"設定\" onclick=\"doSetting();\" />";
                    tmpHTML += "<input type=\"button\" name=\"btnDownload\" class=\"btn-info\" value=\"下載\"  onclick=\"doDownload();\"/></td></tr>";
                }
                for (var i = 0; i < dataLen; i++) {
                    var isCheck = (JsonData.data[i].isSelected=="1")?" checked":"";
                   if (i % 3 == 0 ) {
                        tmpHTML += "<tr>";
                       tmpHTML += "<td>";
                       tmpHTML += "<label><input type=\"checkbox\" name=\"StationID[]\" value=\"" + JsonData.data[i].StationID + "\" " + isCheck + ">" + JsonData.data[i].StationName + "</label></td>";

                    }  else if (i % 3 == 1) {
                       tmpHTML += "<td>";
                       tmpHTML += "<label><input type=\"checkbox\" name=\"StationID[]\" value=\"" + JsonData.data[i].StationID + "\" " + isCheck + ">" + JsonData.data[i].StationName + "</label></td>";
                    
                    } else if (i % 3 == 2) {
                       tmpHTML += "<td>";
                       tmpHTML += "<label><input type=\"checkbox\" name=\"StationID[]\" value=\"" + JsonData.data[i].StationID + "\" " + isCheck + ">" + JsonData.data[i].StationName + "</label></td></tr>";
  
                    }
                }
                if ((dataLen-1) % 3 <2) {
                    tmpHTML += "</tr>";
                }
                
               // console.log("tmpHTML:" + tmpHTML);
                $("#data").html(tmpHTML);
           
            } else {
                $.busyLoadFull("hide", { animate: "fade" });
                swal({
                    title: '取得資料發生錯誤',
                    text: data.ErrMsg,
                    showCancelButton: false,
                    type: "error",
                    position: "center-left",
                    customClass: "aaaa"
                }).then(() => {
                    window.location.reload();
                });
                console.log("false");

            }
        }
    });
   
}