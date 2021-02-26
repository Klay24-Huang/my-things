
var timeFlag = true;
var delayTime = 1 * 60 * 1000;
var userName='@UserName';
var inPicSize=0;
var outPicSize=0;
$(document).ready(function () {
    $("#bookDetailBody").html("");
    $("#showPick").hide();
    $("#pickCarOpt").hide();
    $("#pickMotoOpt").hide();
    $("#returnOpt").hide();
   
  //  localStorage.removeItem("UserName");
  //  localStorage.setItem("UserName", $("#manager").val());
  //let UserName = localStorage.getItem("UserName");
  //   alert($("#manager").val()+","+UserName);
    checkReloadData();
    var timeoutID = window.setInterval((() => checkReloadData()), delayTime);
    

});
function checkReloadData() {
    if (timeFlag) {
        console.log("執行重新判斷可否取車");
        getCarData();
       // window.re
    } else {
        console.log("中斷")
    }
}
function doReturnCar(UserName) {
    console.log("btnFinish click");
   // blockUI();
    $.busyLoadFull("show", {
        spinner: "cube-grid"
    });
 //let UserName = localStorage.getItem("UserName");
    var flag = true;
    var msg = "";
    var MachineNo=$("#hidMachineNo").val();
    var orderNum=$("#hidOrderNum").val();
    var CarNo=$("#hidCarNo").val();
    var isCar=$("#hidIsCar").val();
    var incarclean = ($("#incarclean").prop("checked")) ? 1 : 0;        //車內清潔
    var outcarclean = ($("#outcarclean").prop("checked")) ? 1 : 0;      //車外清潔
    var rescue = ($("#rescue").prop("checked")) ? 1 : 0;                //車輛救授
    var dispatch = ($("#dispatch").prop("checked")) ? 1 : 0;            //車輛調度
    var Anydispatch = ($("#Anydispatch").prop("checked")) ? 1 : 0;      //路邊租還車輛調度
    var remark = $("#remark").val();
    var incarPIC = $("#hidincarPic").val();
    var outcarPIC = $("#hidoutcarPic").val();
    var Maintenance = ($("#Maintenance").prop("checked")) ? 1 : 0;      //保養
    if ((rescue == 1 || dispatch == 1 || Anydispatch == 1 || Maintenance==1) && remark == "") {
        flag = false;
        msg = "備註未填寫";
    }
    if (incarPIC == "" && incarclean==1) {
        if (flag) {
            msg = "車內照未選擇";
        } else {
            msg += ",車內照未選擇";
        }
        flag = false;
    }

    if (outcarPIC == "" && outcarclean==1) {
        if (flag) {
            msg = "車外照未選擇";
        } else {
            msg += ",車外照未選擇";
        }
        flag = false;
    }
   //  alert(CarNo+","+UserName);
    if (flag) {
        
        var URL = jsHost + "MA_CleanCarReturnNew";
        console.log(URL);
        var SendObj = new Object();
        SendObj.UserID = UserName;
        SendObj.IsCar = isCar;
        SendObj.MachineNo = MachineNo;
        SendObj.CarNo = CarNo;
        SendObj.OrderNum = orderNum;
        SendObj.outsideClean = outcarclean;
        SendObj.insideClean = incarclean;
        SendObj.rescue = rescue;
        SendObj.dispatch = dispatch;
        SendObj.Anydispatch = Anydispatch;
        SendObj.remark = remark;
        SendObj.Maintenance = Maintenance;
        SendObj.incarPic = $("#hidincarPic").val();
        SendObj.incarPicType = $("#hidincarPicType").val();
        SendObj.outcarPic = $("#hidoutcarPic").val();
        SendObj.outcarPicType = $("#hidoutcarPicType").val();
        var jdata = JSON.stringify(SendObj);
        console.log(jdata);
 
        $.ajax({
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: jdata,
            url: URL,
            error: function (xhr, error) { console.log(xhr.responseText + "," + xhr.status + "," + error); $.busyLoadFull("hide", { animate: "fade" }); },
            success: function (JsonData) {
                $.busyLoadFull("hide", { animate: "fade" });
                console.log(JsonData.Result + "," + JsonData.ErrorMessage);
                console.log(JsonData);
                if (JsonData.Result === "1") {
                   // $.unblockUI();

                    if (flag) {
                        swal({
                            title: "",
                            text: JsonData.ErrorMessage,
                            html: JsonData.ErrorMessage,
                            type: "success",
                            confirmButtonColor: '#ff0000',
                            confirmButtonText: '確定',
                            closeOnConfirm: true
                        }, function (isConfirm) { window.location.href = "BookingList"; });
                    }
                } else {
                  //  $.unblockUI();

                    if (flag) {
                        swal({
                            title: "",
                            text: JsonData.ErrorMessage,
                            html: JsonData.ErrorMessage,
                            type: "error",
                            confirmButtonColor: '#ff0000',
                            confirmButtonText: '確定',
                            closeOnConfirm: true
                        }, function (isConfirm) { window.location.href = "BookingList"; });
                    }
                }
            }
        });
    } else {
      //  $.unblockUI();
        $.busyLoadFull("hide", { animate: "fade" });
        swal({
            title: "發生錯誤",
            text:msg ,
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#DD6B55",
            confirmButtonText: '確定'
        });
    }
    console.log(incarclean, incarPIC, outcarPIC);

}
function getCarData() {
    $.busyLoadFull("show", {
        spinner: "cube-grid"
    });
    var URL = jsHost + "MA_CleanListQuery";
    console.log(URL);
    var SendObj = new Object();
    SendObj.IDNO = $("#manager").val();
    var jdata = JSON.stringify( SendObj );
    console.log(jdata);
    $.ajax({
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: jdata,
        url: URL,
        error: function (xhr, error) { console.log(xhr.responseText + "," + xhr.status + "," + error); $.busyLoadFull("hide", { animate: "fade" }); },
        success: function (JsonData) {
            console.log(JsonData.Result + "," + JsonData.ErrorMessage);
            console.log(JsonData);
            $.busyLoadFull("hide", { animate: "fade" });
            if (JsonData.Result === "1") {
                localStorage.removeItem('bookingList');
                console.log("true");
                console.log(JsonData.Data);

                var dataLen = JsonData.Data.length;
                if (dataLen > 0) {
                    localStorage.setItem('bookingList', JSON.stringify(JsonData.Data));
                }
                console.log(dataLen);
                var htmlStr = "";
var now = new Date()
       //             console.log(d.getTimezoneOffset());
//alert("now time is :"+ now);

                for (var i = 0; i < dataLen; i++) {
                    console.log(JsonData.Data[i]);
                    htmlStr += "<tr><td>H" + JsonData.Data[i].OrderNum + "</td><td>" + JsonData.Data[i].assigned_car_id + "</td>";
                    //Date.timezoneOffset(-480)
                    var SD = addMinutes(new Date(Date.parse(JsonData.Data[i].start_time)), -30);

                    var ED = addMinutes(new Date(Date.parse(JsonData.Data[i].stop_time)), 15);
//alert("SD="+SD+";ED="+ED);
                   // var now = new Date();
                  //  var d = new Date()
                  //  console.log(d.getTimezoneOffset());
                    console.log("now=" + now);
                    console.log("SD=" + SD);
                    console.log("ED=" + ED);
                    if (JsonData.Data[i].OrderStatus == 0 && JsonData.Data[i].cancel_status == 0) {
                        if (JsonData.Data[i].isOverTime == 1) {
                            htmlStr += "<td>已逾時</td>";
                        } else {
                            if (JsonData.Data[i].CanPick == 1 && JsonData.Data[i].CanCancel == 1) {
                                htmlStr += "<td><span class=\"btn btn-danger\" id='POrder_" + JsonData.Data[i].OrderNum + "' ontouchend=pickCar('" + JsonData.Data[i].OrderNum + "','" + JsonData.Data[i].MachineNo + "','" + JsonData.Data[i].deviceToken + "'," + JsonData.Data[i].IsCar + ",'" + JsonData.Data[i].assigned_car_id + "'); onclick=pickCar('" + JsonData.Data[i].OrderNum + "','" + JsonData.Data[i].MachineNo + "','" + JsonData.Data[i].deviceToken + "'," + JsonData.Data[i].IsCar + ",'" + JsonData.Data[i].assigned_car_id + "');>取車</span><br><span class=\"btn btn-danger\" id='COrder_" + JsonData.Data[i].OrderNum + "' ontouchend=cancelCar('" + JsonData.Data[i].OrderNum + "','" + JsonData.Data[i].MachineNo + "'," + JsonData.Data[i].IsCar + ",'" + JsonData.Data[i].assigned_car_id + "'); onclick=cancelCar('" + JsonData.Data[i].OrderNum + "','" + JsonData.Data[i].MachineNo + "'," + JsonData.Data[i].IsCar + ",'" + JsonData.Data[i].assigned_car_id + "');>取消</span>  </td>";
                            } else if (JsonData.Data[i].CanPick == 0 && JsonData.Data[i].CanCancel == 1) {
                                htmlStr += "<td><span class=\"btn btn-danger\" id='COrder_" + JsonData.Data[i].OrderNum + "' ontouchend=cancelCar('" + JsonData.Data[i].OrderNum + "','" + JsonData.Data[i].MachineNo + "'," + JsonData.Data[i].IsCar + ",'" + JsonData.Data[i].assigned_car_id + "'); onclick=cancelCar('" + JsonData.Data[i].OrderNum + "','" + JsonData.Data[i].MachineNo + "'," + JsonData.Data[i].IsCar + ",'" + JsonData.Data[i].assigned_car_id + "');>取消</span></td>";
                            }
                        }
                        console.log("0");
                    } else if (JsonData.Data[i].OrderStatus == 1) {
                        htmlStr += "<td><span class=\"btn btn-danger\"  id='ROrder_" + JsonData.Data[i].OrderNum + "'  ontouchend=returnCar('" + JsonData.Data[i].OrderNum + "','" + JsonData.Data[i].MachineNo + "','" + JsonData.Data[i].deviceToken + "'," + JsonData.Data[i].IsCar + ",'" + JSON.stringify(JsonData.Data[i]) + "');   onclick=returnCar('" + JsonData.Data[i].OrderNum + "','" + JsonData.Data[i].MachineNo + "','" + JsonData.Data[i].deviceToken + "'," + JsonData.Data[i].IsCar + ",'" + JSON.stringify(JsonData.Data[i]) + "');>還車</span></td>";

                        console.log("1");
                    } else if (JsonData.Data[i].OrderStatus == 2) {
                        htmlStr += "<td>已完成</td>";
                        console.log("2");
                    } else if (JsonData.Data[i].OrderStatus == 3){
                        htmlStr += "<td>已取消</td>";
                    } else if (JsonData.Data[i].OrderStatus == 4) {
                        htmlStr += "<td>取車逾時，系統已取消</td>";
                    } else if (JsonData.Data[i].OrderStatus == 5) {
                        htmlStr += "<td>還車逾時，系統已釋放</td>";  
                    
                    } else {
                        htmlStr += "<td>已逾時</td>";
                        console.log("3");
                    }
                    htmlStr += "<td>" + JsonData.Data[i].Location + "</td>";
                    htmlStr += "<td>" + JsonData.Data[i].start_time.replace("T", " ") + "</td><td>" + JsonData.Data[i].stop_time.replace("T", " ") + "</td>";
                    htmlStr += "</tr>";
                }
                $("#bookDetailBody").html(htmlStr);
        
                $('#bookDetail').footable({ "paging": { "limit": 5},"limit-navigation":5 });
	        // $("#bookDetail").attr("data-paging-limit", 5);
              
                $('#bookDetail').trigger('footable_redraw');
                //    $.unblockUI();
                //  warningAlertSubMit(JsonData.ErrMsg, 2, 1, "formStationManage");

            } else {
                console.log("false");
                // $.unblockUI();
                //    $.unblockUI();
                //  warningAlert(JsonData.ErrMsg, false, 0, "");
            }
           
        }
    });
}
function addMinutes(date, minutes) {
    if(isMobile()){
return new Date(date.getTime() + minutes * 60000);
      
    }else{
        return new Date((date.getTime()+ date.getTimezoneOffset()*60*1000) + minutes * 60000);
    }
    
}
function isMobile() {

    if(/(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|ipad|iris|kindle|Android|Silk|lge |maemo|midp|mmp|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino/i.test(navigator.userAgent) 

    || /1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(navigator.userAgent.substr(0,4))){
  return true;
}else{
   return false;
}

}
function GetEncyptData(jdata, callback) {
    console.log("jdata:" + jdata);
    //  var host = "http://1.34.136.4/";

    var URL = host + "iMotoWebAPI/api/AesEncrypt/doEncrypt";
    console.log(URL);
    $.ajax({
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: jdata,
        url: URL,
        error: function (xhr, error) { console.log(xhr.responseText + "," + xhr.status + "," + error); },
        success: function (JsonData) {
            console.log(JsonData.Result + "," + JsonData.ErrMsg);
            if (JsonData.Result === "0") {
                callback(JsonData.ErrMsg);
            } else {
                warningAlert(JsonData.ErrMsg, false, 0, "");
            }
        }
    });
}
function cancelCar(OrderNum, MachineNo, isCar, CarNo) {
    let bookingList = JSON.parse(localStorage.getItem('bookingList'));
    var nowDate = new Date();
    var obj = bookingList.find(function (element) {
        return element.assigned_car_id.replace(/ /g, "").replace(/\"/g, "") == CarNo;
    });
    var SendObj = new Object();
    SendObj.OrderNo = "H"+OrderNum;
    blockUI();
 
        returnDate = nowDate.getFullYear().toString() + "-" + (nowDate.getMonth() + 1).toString() + "-" + nowDate.getDate().toString() + " " + nowDate.getHours().toString() + ":" + nowDate.getMinutes().toString() + ":00";

    console.log(returnDate);

    //var ExtendTime=new Date(ExtendD + ' ' + EH+":"+EM+":00")
    SendObj.type = 2;
    SendObj.Mode = 1;
    SendObj.returnDate = returnDate;
    SendObj.UserID = $("#Account").val();
  //  console.log(SendObj);
    var jdata = JSON.stringify( SendObj);
    carControlComplete(jdata);
}
//強制取還車
function carControlComplete(encryptData) {
    var jdata =  encryptData ;
    var URL = jsHost + "BE_ContactSetting";
    console.log("URL=" + URL);
    console.log("data:" + jdata);
    $.ajax({
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: jdata,
        url: URL,
        error: function (xhr, error) { console.log(xhr.responseText + "," + xhr.status + "," + error); },
        success: function (JsonData) {
            console.log(JsonData.Result + "," + JsonData.ErrorMessage);
            if (JsonData.Result === "1") {
                console.log("true");
                //   $.unblockUI();
                warningAlertSubMit(JsonData.ErrorMessage, 1, 1, "BookingList");
            } else {
                console.log("false");
                //    $.unblockUI();
                warningAlert(JsonData.ErrorMessage, false, 0, "");
            }
        }
    });
}
function warningAlert(msg, flag, ImgType, site) {
    var errImg = "warning";
    $.unblockUI();
    if (ImgType == 1) {
        errImg = "success";
    }
    if (flag) {
        swal({
            title: "",
            text: msg,
            html: msg,
            type: errImg,
            confirmButtonColor: '#ff0000',
            confirmButtonText: '確定',
            closeOnConfirm: true
        }, function (isConfirm) { window.location.href = site; });
    } else {
        swal({
            title: "",
            text: msg,
            html: msg,
            type: errImg,
            confirmButtonColor: '#ff0000',
            confirmButtonText: '確定',
            closeOnConfirm: true
        }, function (isConfirm) { });
    }
}
function warningAlertSubMit(msg, type, ImgType, site) {
    var errImg = "warning";
    $.unblockUI();
    if (ImgType == 1) {
        errImg = "success";
    }
    if (1 == type) {
        swal({
            title: "",
            text: msg,
            html: msg,
            type: errImg,
            confirmButtonColor: '#ff0000',
            confirmButtonText: '確定',
            closeOnConfirm: true
        }, function (isConfirm) { window.location.href = site; });
    } else if (0 == type) {
        swal({
            title: "",
            text: msg,
            html: msg,
            type: errImg,
            confirmButtonColor: '#ff0000',
            confirmButtonText: '確定',
            closeOnConfirm: true
        }, function (isConfirm) { });
    } else {
        swal({
            title: "",
            text: msg,
            html: msg,
            type: errImg,
            confirmButtonColor: '#ff0000',
            confirmButtonText: '確定',
            closeOnConfirm: true
        }, function (isConfirm) { $("#" + site).submit(); });
    }
}
function pickCar(orderNum, MachineNo,deviceToken, isCar, CarNo) {
    $("#showPick").hide();
    $("#pickCarOpt").hide();
    $("#pickMotoOpt").hide();
    $("#returnOpt").hide();
    $("#BookingList").hide();
$("#hidIsCar").val(isCar);
   $("#hidMachineNo").val(MachineNo);
    $("#hidOrderNum").val(orderNum);
    $("#hidCarNo").val(CarNo);
    let bookingList = JSON.parse(localStorage.getItem('bookingList'));
    console.log(bookingList);
    var obj = bookingList.find(function (element) {
        return element.assigned_car_id.replace(/ /g, "").replace(/\"/g, "") == CarNo;
    });
    var URL = jsHost + "MA_CleanCarStart";
    console.log(URL);
    var SendObj = new Object();
    SendObj.UserID = $("#manager").val();
    SendObj.CarNo = CarNo;
    SendObj.OrderNum = orderNum;
    var jdata = JSON.stringify(SendObj);
    console.log(jdata);
    $.ajax({
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: jdata,
        url: URL,
        error: function (xhr, error) { console.log(xhr.responseText + "," + xhr.status + "," + error); },
        success: function (JsonData) {
            console.log(JsonData.Result + "," + JsonData.ErrorMessage);
            console.log(JsonData);
            if (JsonData.Result === "1") {
                timeFlag = false;
                $("#showPick").show();
                if (isCar == 1) {

                    $("#pickCarOpt").show();

                    if (obj != null) {
                        $("#tdStart").html(obj.start_time.replace("T", " "));
                        $("#tdEnd").html(obj.stop_time.replace("T", " "));
                        $("#tdCarNo").html(CarNo);
                    }
                    //上鎖
                    $("#LockON").on("click", function () {
                        SetLock(MachineNo, deviceToken, 5);
                    });
                    //解鎖
                    $("#LockOff").on("click", function () {
                        SetLock(MachineNo, deviceToken, 4);
                    });
                    //中控上鎖
                    $("#LockDoorON").on("click", function () {
                        SetLock(MachineNo, deviceToken, 3);
                    });
                    //中控解鎖
                    $("#LockDoorOff").on("click", function () {
                        SetLock(MachineNo, deviceToken, 2);
                    });
                    //尋車
                    $("#SearchCar").on("click", function () {
                        searchCarFun(MachineNo, deviceToken);
                    })
                    //以下手機用
                    //上鎖
                    $("#LockON").on("touchend", function () {
                        SetLock(MachineNo, deviceToken, 5);
                    });
                    //解鎖
                    $("#LockOff").on("touchend", function () {
                        SetLock(MachineNo, deviceToken, 4);
                    });
                    //中控上鎖
                    $("#LockDoorON").on("touchend", function () {
                        SetLock(MachineNo, deviceToken, 3);
                    });
                    //中控解鎖
                    $("#LockDoorOff").on("touchend", function () {
                        SetLock(MachineNo, deviceToken, 2);
                    });
                    //尋車
                    $("#SearchCar").on("touchend", function () {
                        searchCarFun(MachineNo, deviceToken);
                    })
                } else {
                    $("#pickMotoOpt").show();
                    if (obj != null) {
                        $("#tdMStart").html(obj.start_time.replace("T", " "));
                        $("#tdMEnd").html(obj.stop_time.replace("T", " "));
                        $("#tdMCarNo").html(CarNo);
                    }
                    $("#AccOn").on("click", function () {
                        SendMotoCmdFun(MachineNo, deviceToken, 1, '1');
                    });
                    $("#AccOff").on("click", function () {
                        SendMotoCmdFun(MachineNo, deviceToken, 2, '1');
                    });
                    //電池架
                    $("#batConvert").on("click", function () {
                        SendMotoCmdFun(MachineNo, deviceToken, 3, '1');
                    });
                    //座墊
                    $("#convertON").on("click", function () {
                        SendMotoCmdFun(MachineNo, deviceToken, 4, '1');
                    });
                    $("#SearchByLight").on("click", function () {
                        SendMotoCmdFun(MachineNo, deviceToken, 6, '1');
                    });
                    $("#SearchBySpeaker").on("click", function () {
                        SendMotoCmdFun(MachineNo, deviceToken, 5, '1');
                    });
                    //以下手機用
                    $("#AccOn").on("touchend", function () {
                        SendMotoCmdFun(MachineNo, deviceToken, 1, '1');
                    });
                    $("#AccOff").on("touchend", function () {
                        SendMotoCmdFun(MachineNo, deviceToken, 2, '1');
                    });
                    //電池架
                    $("#batConvert").on("touchend", function () {
                        SendMotoCmdFun(MachineNo, deviceToken, 3, '1');
                    });
                    //座墊
                    $("#convertON").on("touchend", function () {
                        SendMotoCmdFun(MachineNo, deviceToken, 4, '1');
                    });
                    $("#SearchByLight").on("touchend", function () {
                        SendMotoCmdFun(MachineNo, deviceToken, 6, '1');
                    });
                    $("#SearchBySpeaker").on("touchend", function () {
                        SendMotoCmdFun(MachineNo, deviceToken, 5, '1');
                    });
                }
                $("#returnOpt").show();
                $("#title").html("訂單編號【H" + orderNum + "】<BR>車號【" + CarNo + "】");
            } else {
               // disabledLoadingAndShowAlert(JsonData.ErrorMessage);
                $.busyLoadFull("hide");
                swal({
                    title: 'Fail',
                    text: JsonData.ErrorMessage,
                    icon: 'error'
                });
                $("#BookingList").show();
            }
        }
    });

}
function returnCar(orderNum, MachineNo,deviceToken, isCar, obj) {
console.log("isCar="+isCar);
    $("#pickMotoOpt").hide();
    $("#pickCarOpt").hide();
    $("#BookingList").hide();
    $("#showPick").show();
    $("#returnOpt").show();
    console.log(obj);
    obj = JSON.parse(obj);
    var CarNo = obj.assigned_car_id;
    $("#title").html("訂單編號【H" + orderNum + "】<BR>車號【" + CarNo + "】");
$("#hidIsCar").val(isCar);
   $("#hidMachineNo").val(MachineNo);
    $("#hidOrderNum").val(orderNum);
    $("#hidCarNo").val(CarNo);
    if (isCar == 0) {
        timeFlag = false;
        $("#pickMotoOpt").show();
        $("#tdMStart").html(obj.start_time.replace("T", " "));
        $("#tdMEnd").html(obj.stop_time.replace("T", " "));

        $("#tdMCarNo").html(CarNo);
        $("#AccOn").on("click", function () {
            SendMotoCmdFun(MachineNo, deviceToken, 1, '1');
        });
        $("#AccOff").on("click", function () {
            SendMotoCmdFun(MachineNo, deviceToken, 2, '1');
        });
        //電池架
        $("#batConvert").on("click", function () {
            SendMotoCmdFun(MachineNo, deviceToken, 3, '1');
        });
        //座墊
        $("#convertON").on("click", function () {
            SendMotoCmdFun(MachineNo, deviceToken, 4, '1');
        });
        $("#SearchByLight").on("click", function () {
            SendMotoCmdFun(MachineNo, deviceToken, 6, '1');
        });
        $("#SearchBySpeaker").on("click", function () {
            SendMotoCmdFun(MachineNo, deviceToken, 5, '1');
        });
        //以下手機用
        $("#AccOn").on("touchend", function () {
            SendMotoCmdFun(MachineNo, deviceToken, 1, '1');
        });
        $("#AccOff").on("touchend", function () {
            SendMotoCmdFun(MachineNo, deviceToken, 2, '1');
        });
        //電池架
        $("#batConvert").on("touchend", function () {
            SendMotoCmdFun(MachineNo, deviceToken, 3, '1');
        });
        //座墊
        $("#convertON").on("touchend", function () {
            SendMotoCmdFun(MachineNo, deviceToken, 4, '1');
        });
        $("#SearchByLight").on("touchend", function () {
            SendMotoCmdFun(MachineNo, deviceToken, 6, '1');
        });
        $("#SearchBySpeaker").on("touchend", function () {
            SendMotoCmdFun(MachineNo, deviceToken, 5, '1');
        });
    } else {
        $("#pickCarOpt").show();
        $("#tdStart").html(obj.start_time.replace("T", " "));
        $("#tdEnd").html(obj.stop_time.replace("T", " "));
        $("#tdCarNo").html(CarNo);
        //上鎖
        $("#LockON").on("click", function () {
            SetLock(MachineNo, deviceToken, 5);
        });
        //手機用上鎖
        $("#LockON").on("touchend", function () {
            SetLock(MachineNo, deviceToken, 5);
        });
        //解鎖
        $("#LockOff").on("click", function () {
            SetLock(MachineNo, deviceToken, 4);
        });
        $("#LockOff").on("touchend", function () {
            SetLock(MachineNo, deviceToken, 4);
        });
                    //中控上鎖
                    $("#LockDoorON").on("click", function () {
                        SetLock(MachineNo, deviceToken, 3);
                    });
                    //中控解鎖
                    $("#LockDoorOff").on("click", function () {
                        SetLock(MachineNo, deviceToken, 2);
                    });
    //中控上鎖
                    $("#LockDoorON").on("touchend", function () {
                        SetLock(MachineNo, deviceToken, 3);
                    });
                    //中控解鎖
                    $("#LockDoorOff").on("touchend", function () {
                        SetLock(MachineNo, deviceToken, 2);
                    });
        //尋車
        $("#SearchCar").on("click", function () {
            searchCarFun(MachineNo, deviceToken);
        })
        $("#SearchCar").on("touchend", function () {
            searchCarFun(MachineNo, deviceToken);
        })
    }
    $("#returnOpt").show();
  /*  $("#btnFinish").on("touchend", function () {
        console.log("btnFinish touchend");
        blockUI();
        var flag = true;
        var msg = "";
        var incarclean = ($("#incarclean").prop("checked")) ? 1 : 0;        //車內清潔
        var outcarclean = ($("#outcarclean").prop("checked")) ? 1 : 0;      //車外清潔
        var rescue = ($("#rescue").prop("checked")) ? 1 : 0;                //車輛救授
        var dispatch = ($("#dispatch").prop("checked")) ? 1 : 0;        //車輛調度
        var Anydispatch = ($("#Anydispatch").prop("checked")) ? 1 : 0;        //路邊租還車輛調度
        var remark = $("#remark").val();
        var incarPIC = $("#hidincarPic").val();
        var outcarPIC = $("#hidoutcarPic").val();
        if ((rescue == 1 || dispatch == 1 || Anydispatch == 1) && remark == "") {
            flag = false;
            msg = "備註未填寫";
        }
        if (incarPIC == "") {
            if (flag) {
                msg = "車內照未選擇";
            } else {
                msg += "<br>車內照未選擇";
            }
            flag = false;
        }

        if (outcarPIC == "") {
            if (flag) {
                msg = "車外照未選擇";
            } else {
                msg += "<br>車外照未選擇";
            }
            flag = false;
        }
        if (flag) {
            var URL = host + "iMotoWebAPI/api/CleanCarReturn";
            console.log(URL);
            var SendObj = new Object();
            SendObj.UserID = $("#manager").val();
            SendObj.IsCar = isCar;
            SendObj.MachineNo = MachineNo;
            SendObj.CarNo = CarNo;
            SendObj.OrderNum = orderNum;
            SendObj.outsideClean = outcarclean;
            SendObj.insideClean = incarclean;
            SendObj.rescue = rescue;
            SendObj.dispatch = dispatch;
            SendObj.Anydispatch = Anydispatch;
            SendObj.remark = remark;
            SendObj.incarPic = $("#hidincarPic").val();
            SendObj.incarPicType = $("#hidincarPicType").val();
            SendObj.outcarPic = $("#hidoutcarPic").val();
            SendObj.outcarPicType = $("#hidoutcarPicType").val();
            var jdata = JSON.stringify({ "para": SendObj });

            console.log(jdata);
            $.ajax({
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                data: jdata,
                url: URL,
                error: function (xhr, error) { console.log(xhr.responseText + "," + xhr.status + "," + error); },
                success: function (JsonData) {
                    console.log(JsonData.Result + "," + JsonData.ErrMsg);
                    console.log(JsonData);
                    if (JsonData.Result === "0") {
                        $.unblockUI();

                        if (flag) {
                            swal({
                                title: "",
                                text: JsonData.ErrMsg,
                                html: JsonData.ErrMsg,
                                type: "success",
                                confirmButtonColor: '#ff0000',
                                confirmButtonText: '確定',
                                closeOnConfirm: true
                            }, function (isConfirm) { window.location.href = "BookingList"; });
                        }
                    } else {
                        $.unblockUI();

                        if (flag) {
                            swal({
                                title: "",
                                text: JsonData.ErrMsg,
                                html: JsonData.ErrMsg,
                                type: "error",
                                confirmButtonColor: '#ff0000',
                                confirmButtonText: '確定',
                                closeOnConfirm: true
                            });
                        }
                    }
                }
            });
        } else {
            $.unblockUI();
            swal({
                title: "發生錯誤",
                text: "發生錯誤，請重新整理網頁",
                type: "warning",
                showCancelButton: true,
                confirmButtonColor: "#DD6B55",
                confirmButtonText: '確定'
            });
        }
        console.log(incarclean, incarPIC, outcarPIC);

    });


    $("#btnFinish").on("click", function () {
        console.log("btnFinish click");
        blockUI();
        var flag = true;
        var msg = "";
        var incarclean = ($("#incarclean").prop("checked")) ? 1 : 0;        //車內清潔
        var outcarclean = ($("#outcarclean").prop("checked")) ? 1 : 0;      //車外清潔
        var rescue = ($("#rescue").prop("checked")) ? 1 : 0;                //車輛救授
        var dispatch = ($("#dispatch").prop("checked")) ? 1 : 0;        //車輛調度
        var Anydispatch = ($("#Anydispatch").prop("checked")) ? 1 : 0;        //路邊租還車輛調度
        var remark = $("#remark").val();
        var incarPIC = $("#hidincarPic").val();
        var outcarPIC = $("#hidoutcarPic").val();
        if ((rescue == 1 || dispatch == 1 || Anydispatch == 1) && remark == "") {
            flag = false;
            msg = "備註未填寫";
        }
        if (incarPIC == "") {
            if (flag) {
                msg = "車內照未選擇";
            } else {
                msg += "<br>車內照未選擇";
            }
            flag = false;
        }

        if (outcarPIC == "") {
            if (flag) {
                msg = "車外照未選擇";
            } else {
                msg += "<br>車外照未選擇";
            }
            flag = false;
        }
        if (flag) {
            var URL = host + "iMotoWebAPI/api/CleanCarReturn";
            console.log(URL);
            var SendObj = new Object();
            SendObj.UserID = $("#manager").val();
            SendObj.IsCar = isCar;
            SendObj.MachineNo = MachineNo;
            SendObj.CarNo = CarNo;
            SendObj.OrderNum = orderNum;
            SendObj.outsideClean = outcarclean;
            SendObj.insideClean = incarclean;
            SendObj.rescue = rescue;
            SendObj.dispatch = dispatch;
            SendObj.Anydispatch = Anydispatch;
            SendObj.remark = remark;
            SendObj.incarPic = $("#hidincarPic").val();
            SendObj.incarPicType = $("#hidincarPicType").val();
            SendObj.outcarPic = $("#hidoutcarPic").val();
            SendObj.outcarPicType = $("#hidoutcarPicType").val();
            var jdata = JSON.stringify({ "para": SendObj });
            console.log(jdata);
            $.ajax({
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                data: jdata,
                url: URL,
                error: function (xhr, error) { console.log(xhr.responseText + "," + xhr.status + "," + error); },
                success: function (JsonData) {
                    console.log(JsonData.Result + "," + JsonData.ErrMsg);
                    console.log(JsonData);
                    if (JsonData.Result === "0") {
                        $.unblockUI();

                        if (flag) {
                            swal({
                                title: "",
                                text: JsonData.ErrMsg,
                                html: JsonData.ErrMsg,
                                type: "success",
                                confirmButtonColor: '#ff0000',
                                confirmButtonText: '確定',
                                closeOnConfirm: true
                            }, function (isConfirm) { window.location.href = "BookingList"; });
                        }
                    } else {
                        $.unblockUI();

                        if (flag) {
                            swal({
                                title: "",
                                text: JsonData.ErrMsg,
                                html: JsonData.ErrMsg,
                                type: "error",
                                confirmButtonColor: '#ff0000',
                                confirmButtonText: '確定',
                                closeOnConfirm: true
                            }, function (isConfirm) { window.location.href = "BookingList"; });
                        }
                    }
                }
            });
        } else {
            $.unblockUI();
            swal({
                title: "發生錯誤",
                text: "發生錯誤，請重新整理網頁",
                type: "warning",
                showCancelButton: true,
                confirmButtonColor: "#DD6B55",
                confirmButtonText: '確定'
            });
        }
        console.log(incarclean, incarPIC, outcarPIC);

    });*/

}
function blockUI() {
    console.log("i call");
    $.blockUI({
        message: $("#imgBusy"), centerX: true, centerY: true,
        css: { border: 'none', background: 'none', cursor: 'wait' },
        overlayCSS: { backgroundColor: '#FFFFFF', opacity: 0.5, cursor: 'wait' }
    });
}
document.getElementById("show-incarPic").addEventListener('load', function () {
     console.log("call show-incarPic onload");
	var image = new Image();
         var cvs = document.createElement('canvas'),
            ctx = cvs.getContext('2d');
var img = new Image(),
        maxW = 640; //設定最大寬度
        if (this.width > maxW) {
            this.height *= maxW / img.width;
            this.width = maxW;
        }
       // console.log(this.file)
        cvs.width =640; // this.width;
        cvs.height =480; // this.height;
        ctx.clearRect(0, 0, cvs.width, cvs.height);
//        ctx.drawImage(this, 0, 0, this.width, this.height); 
        ctx.drawImage(this, 0, 0, 640, 480);
        var compressRate = getCompressRate(1, inPicSize);
        var dataUrl = cvs.toDataURL('image/jpeg', compressRate);
 var base64 = dataUrl.split(",");
console.log(dataUrl);
                    $("#hidincarPicType").val(base64[0].replace(/^data:/, ''));
                    $("#hidincarPic").val("⊙"+base64[1]);
console.log("after length:"+base64[1].length);
	

});
document.getElementById("show-outcarPic").addEventListener('load', function () {
     console.log("call show-incarPic onload");
	var image = new Image();
         var cvs = document.createElement('canvas'),
            ctx = cvs.getContext('2d');
var img = new Image(),
        maxW = 640; //設定最大寬度
        if (this.width > maxW) {
            this.height *= maxW / img.width;
            this.width = maxW;
        }
        //console.log(this.file)
            cvs.width =640; // this.width;
        cvs.height =480; // this.height;
        ctx.clearRect(0, 0, cvs.width, cvs.height);
//        ctx.drawImage(this, 0, 0, this.width, this.height); 
        ctx.drawImage(this, 0, 0, 640, 480);
        var compressRate = getCompressRate(1, outPicSize);
        var dataUrl = cvs.toDataURL('image/jpeg', compressRate);
 var base64 = dataUrl.split(",");
console.log(dataUrl);
                   $("#hidoutcarPicType").val(base64[0].replace(/^data:/, ''));
                    $("#hidoutcarPic").val("⊙" +base64[1]);
console.log("after length:"+base64[1].length);
	

});
function handleFiles(file, id) {
    console.log("call handleFiles");
    if (file != null) {
        console.log(file)
        if (file.length > 0) {
            var tmpfile = file[0];
            console.log("file size="+file[0].size);
            if (id == "incarPic") {
             
                var reader = new FileReader();
                var fileSize = Math.round(file[0].size / 1024 / 1024);
                inPicSize=fileSize;
                reader.onload = function (e) {
                console.log("call reader.onload ");
                   /* $('#show-incarPic').attr('src', e.target.result);
                    var base64 = e.target.result.split(",");
                    $("#hidincarPicType").val(base64[0].replace(/^data:/, ''));
                    $("#hidincarPic").val("⊙"+base64[1]);*/

                  var url=reader.result;
 var base64 = e.target.result.split(",")[1];
                 console.log("before length:"+base64.length);
		   $('#show-incarPic').attr('src', url);
                    
                };
                reader.readAsDataURL(tmpfile);
               

                $('#show-incarPic').show();
            } else {
                var reader = new FileReader();
                var fileSize = Math.round(file[0].size / 1024 / 1024);
		outPicSize=fileSize;
                reader.onload = function (e) {
                   
                 /*   $('#show-outcarPic').attr('src', e.target.result);
                    var base64 = e.target.result.split(",");
                    $("#hidoutcarPicType").val(base64[0].replace(/^data:/, ''));
                    $("#hidoutcarPic").val("⊙" +base64[1]);*/
                  var url=reader.result;
                var base64 = e.target.result.split(",")[1];
                 console.log("before length:"+base64.length);
		   $('#show-outcarPic').attr('src', url);
                };
                reader.readAsDataURL(tmpfile);
               
                $('#show-outcarPic').show();
            }
        } else {
            if (id == "incarPic") {
                document.getElementById('show-incarPic').src = "";
                $("#hidoutcarPic").val("");
                $('#show-incarPic').hide();
            } else {
                document.getElementById('show-outcarPic').src = "";
                $("#hidoutcarPic").val("");
                $('#show-outcarPic').hide();
            }
        }

    }

}
document.getElementById('show-incarPic').addEventListener('change', function () {
 console.log("call show-incarPic Change");
    var reader = new FileReader();
    var fileSize = Math.round(this.files[0].size / 1024 / 1024);
    reader.onload = function (e) {
        compress(this.files[0], fileSize);
        //呼叫圖片壓縮方法：compress();
    };
    reader.readAsDataURL(this.files[0]);
    //console.log(this.files[0]);
    //以M為單位
    //this.files[0] 該資訊包含：圖片的大小，以byte計算 獲取size的方法如下：this.files[0].size;
}, false);
document.getElementById('show-outcarPic').addEventListener('change', function () {
console.log("call show-outcarPic Change");
    var reader = new FileReader();
    var fileSize = Math.round(this.files[0].size / 1024 / 1024);
    reader.onload = function (e) {
        compress(this.files[0], fileSize)
        //呼叫圖片壓縮方法：compress();
    };
    reader.readAsDataURL(this.files[0]);
   //console.log(this.files[0]);
    //以M為單位
    //this.files[0] 該資訊包含：圖片的大小，以byte計算 獲取size的方法如下：this.files[0].size;
}, false);
//最終實現思路：
//1、設定壓縮後的最大寬度 or 高度；
//2、設定壓縮比例，根據圖片的不同size大小，設定不同的壓縮比。
function compress(res, fileSize) { //res代表上傳的圖片，fileSize大小圖片的大小
    var img = new Image(),
        maxW = 640; //設定最大寬度
    img.onload = function () {
        console.log("img load");
        var cvs = document.createElement('canvas'),
            ctx = cvs.getContext('2d');
        if (img.width > maxW) {
            img.height *= maxW / img.width;
            img.width = maxW;
        }
        cvs.width = img.width;
        cvs.height = img.height;
        ctx.clearRect(0, 0, cvs.width, cvs.height);
        ctx.drawImage(img, 0, 0, img.width, img.height);
        var compressRate = getCompressRate(1, fileSize);
        var dataUrl = cvs.toDataURL('image/jpeg', compressRate);
        document.body.appendChild(cvs);
       // console.log(dataUrl);
        console.log("call compress");
    }
    img.src = res;
}
/*function getCompressRate(allowMaxSize, fileSize) { //計算壓縮比率，size單位為MB
 console.log("call getCompressRate");
    var compressRate = 1;
    if (fileSize / allowMaxSize > 4) {
        compressRate = 0.5;
    } else if (fileSize / allowMaxSize > 3) {
        compressRate = 0.6;
    } else if (fileSize / allowMaxSize > 2) {
        compressRate = 0.7;
    } else if (fileSize > allowMaxSize) {
        compressRate = 0.8;
    } else {
        compressRate = 0.9;
    }
    return compressRate;
}*/
function getCompressRate(allowMaxSize, fileSize) { //計算壓縮比率，size單位為MB
    var compressRate = 1;
    if (fileSize / allowMaxSize > 0.5) {
        compressRate = 0.3;
    } else if (fileSize / allowMaxSize > 0.4) {
        compressRate = 0.4;
    } else if (fileSize / allowMaxSize > 0.3) {
        compressRate = 0.5;
    } else if (fileSize > allowMaxSize) {
        compressRate = 0.6;
    } else {
        compressRate = 0.7;
    }
    return compressRate;
}
function SendMotoCmdFun(machineNo, deviceToken, CMD, hasRent) {
    var userName = $('#manager').val();
    var hasRentFlag = 0;
    if (hasRent == "0") {
        hasRentFlag = 1;
    }
    var obj = new Object();
    obj.CID = machineNo;
    obj.deviceToken = deviceToken;
    obj.UserId = $("#Account").val();

   
    var title = "";
    switch (parseInt(CMD, 10)) {
        case 1:
        case 7:
            title = "是否確定要發動";
            obj.CmdType = 2;
            break;
        case 2:
            title = "是否確定要熄火";
            obj.CmdType = 3;
            break;
        case 3:
            title = "是否確定要開/關電池蓋";
            obj.CmdType = 7;
            break;
        case 4:
            title = "是否確定要開啟坐墊";
            obj.CmdType = 6;
            break;
        case 5:
            title = "是否確定要使用尋車功能(喇叭)";
            obj.CmdType = 4;
            break;
        case 6:
            title = "是否確定要使用尋車功能(方向燈)";
            obj.CmdType = 5;
            break;
    }
    var jsonData = JSON.stringify(obj);
    swal({
        title: title,
        text: title,
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: '確定'
    }, function (isConfirm) {
        if (isConfirm) {
            console.log(jsonData);
            blockUI();
            $.ajax({
                url: jsHost +"SendMotorCMD",
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
                            text: data.ErrorMessage,
                            showCancelButton: false,
                            type: "error",
                            position: "center-left",
                            customClass: "aaaa"
                        }).then(() => {
                            window.location.reload();
                        });
                    }

                },
                error: function (xhr, option, error) {

                }

            });
        }
        else {

        }

    });
}
function SetLock(CID, deviceToken, Action) {
    var userName = $('#Account').val();
    console.log(userName);
    var obj = new Object();
    obj.CID = CID;
    obj.deviceToken = deviceToken;
    obj.UserId = $("#Account").val();
    if (CID.length > 4) {
        obj.IsCens = 1;
    
    } else {
        obj.IsCens = 0;
    
    }
    if (obj.IsCens == 0) {
        obj.CmdType = Action + 5;
    } else {
        obj.CmdType = Action + 18;
    }

    var jdata = JSON.stringify(obj);
    SetLockComplete(jdata);
    blockUI();
}

function SetLockComplete(jdata) {

    var URL = jsHost + "SendCarCMD";
    console.log(URL);
    console.log(jdata);
    $.ajax({
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: jdata,
        url: URL,
        error: function (xhr, error) { console.log(xhr.responseText + "," + xhr.status + "," + error); },
        success: function (JsonData) {
            console.log(JsonData.Result + "," + JsonData.ErrorMessage);
            if (JsonData.Result === "1") {
                //  $.unblockUI();
                warningAlertSubMit(JsonData.ErrorMessage, 2, 1, "formCarDashBoard");
            } else {
                // UPDFail();
                //   $.unblockUI();
                warningAlert(JsonData.ErrorMessage, false, 0, "");
            }
        }
    });
}
function searchCarFun(machineNo, deviceToken) {
    var userName = $('#manager').val();
    var obj = new Object();
    obj.CID = machineNo;
    obj.deviceToken = deviceToken;
    obj.UserId = $("#Account").val();
    
    if (machineNo.length > 4) {
        obj.IsCens = 1;
        obj.CmdType = 15;
    } else {
        obj.IsCens = 0;
        obj.CmdType = 0;
    }
    var jsonData = JSON.stringify(obj);

    swal({
        title: "",
        text: "是否確定尋車",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: '確定'
    }, function (isConfirm) {
        if (isConfirm) {
            blockUI();
            $.ajax({
                url: jsHost +"SendCarCMD",
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
                            text: data.ErrorMessage,
                            showCancelButton: false,
                            type: "error",
                            position: "center-left",
                            customClass: "aaaa"
                        }).then(() => {
                            window.location.reload();
                        });
                    }

                },
                error: function (xhr, option, error) {

                }

            });
        }
        else {

        }

    });
}
function warningAlert(msg, flag, ImgType, site) {
    var errImg = "warning";
    $.unblockUI();
    if (ImgType == 1) {
        errImg = "success";
    }
    if (flag) {
        swal({
            title: "",
            text: msg,
            html: msg,
            type: errImg,
            confirmButtonColor: '#ff0000',
            confirmButtonText: '確定',
            closeOnConfirm: true
        }, function (isConfirm) { });
    } else {
        swal({
            title: "",
            text: msg,
            html: msg,
            type: errImg,
            confirmButtonColor: '#ff0000',
            confirmButtonText: '確定',
            closeOnConfirm: true
        }, function (isConfirm) { });
    }
}