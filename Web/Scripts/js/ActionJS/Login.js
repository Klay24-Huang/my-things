﻿$(document).ready(function () {
    $(".datePicker").flatpickr();
    $('.dateTimePicker').flatpickr(
        {
            enableTime: true,
            dateFormat: "Y-m-d H:i",
        }
    );
    var IsSuccess = $("#IsSuccess").val();
    var LoginMessage = $("#LoginMessage").val();
    var UserID = $("#UserID").val();
    console.log("UserID:" + UserID);
    if (IsSuccess != "-1") {
        showLogin(parseInt(IsSuccess), LoginMessage, UserID);
    }
    $("#btnLogin").on("click", function () {
        $.busyLoadFull("show", {
            text: "登入中",
            fontawesome: "fa fa-cog fa-spin fa-3x fa-fw"
        });
    })


})

function showLogin(IsSuccess, message,UserID) {
    if (IsSuccess == 1) {
        swal({
            title: 'SUCCESS',
            text: message,
            icon: 'success'
        }).then(function (value) {
            ShowLoading("開始載入據點資料");

            //console.log("UserID:" + UserID);
            if (UserID != "") {
                console.log("UserID:" + UserID);
                var obj = new Object();
                obj.UserID = UserID;
                var json = JSON.stringify(obj);
                console.log(json);
                var site = jsHost + "BE_GetStationList";
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
                                text: "載入據點："+data.ErrorMessage,
                                icon: 'success'
                            }).then(function (value) {
                                var StationList = localStorage.getItem("StationList");
                              if (typeof StationList !== 'undefined' && StationList !== null) {
                                    localStorage.removeItem("StationList");
                                }
                                localStorage.setItem("StationList", JSON.stringify(data.Data.StationList))
                                ShowLoading("開始載入車輛資料");
                               // window.location.href = "../CarDataInfo/CarDashBoard";
                                //載入車輛
                                 site = jsHost + "BE_GetCarList";
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
                                                text: "載入車輛：" + data.ErrorMessage,
                                                icon: 'success'
                                            }).then(function (value) {
                                                var CarList = localStorage.getItem("CarList");
                                                if (typeof CarList !== 'undefined' && CarList !== null) {
                                                    localStorage.removeItem("CarList");
                                                }
                                                localStorage.setItem("CarList", JSON.stringify(data.Data.CarList))
                                                ShowLoading("開始載入帳號資料");

                                                site = jsHost + "BE_GetManagerList";
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
                                                                text: "載入帳號：" + data.ErrorMessage,
                                                                icon: 'success'
                                                            }).then(function (value) {
                                                                var MangerList = localStorage.getItem("MangerList");
                                                                if (typeof MangerList !== 'undefined' && MangerList !== null) {
                                                                    localStorage.removeItem("MangerList");
                                                                }
                                                                localStorage.setItem("MangerList", JSON.stringify(data.Data.ManagerList))
                                                                 window.location.href = "../CarDataInfo/CarDashBoard";
                                                            });
                                                        } else {

                                                            swal({
                                                                title: 'Fail',
                                                                text: data.ErrorMessage,
                                                                icon: 'error'
                                                            }).then(function (value) {
                                                                window.location.href = "../CarDataInfo/CarDashBoard";
                                                            });
                                                        }
                                                    },
                                                    error: function (e) {
                                                        $.busyLoadFull("hide");
                                                        swal({
                                                            title: 'Fail',
                                                            text: "載入車輛資料發生錯誤",
                                                            icon: 'error'
                                                        });
                                                    }

                                                });
                                            });
                                        } else {

                                            swal({
                                                title: 'Fail',
                                                text: data.ErrorMessage,
                                                icon: 'error'
                                            }).then(function (value) {
                                                window.location.href = "../CarDataInfo/CarDashBoard";
                                            });
                                        }
                                    },
                                    error: function (e) {
                                        $.busyLoadFull("hide");
                                        swal({
                                            title: 'Fail',
                                            text: "載入車輛資料發生錯誤",
                                            icon: 'error'
                                        });
                                    }

                                });
                            });
                        } else {

                            swal({
                                title: 'Fail',
                                text: data.ErrorMessage,
                                icon: 'error'
                            }).then(function (value) {
                                window.location.href = "../CarDataInfo/CarDashBoard";
                            });
                        }
                    },
                    error: function (e) {
                        $.busyLoadFull("hide");
                        swal({
                            title: 'Fail',
                            text: "載入據點資料發生錯誤",
                            icon: 'error'
                        });
                    }

                });
            } else {
                swal({
                    title: 'Fail',
                    text: "登入發生錯誤，請重新登入",
                    icon: 'Fail'
                }).then(function (value) {
                    window.location.href = "../Home/Logout";
                });
            }
         //   window.location.href = "../CarDataInfo/CarDashBoard";
        });
    } else {
        swal({
            title: 'Fail',
            text: message,
            icon: 'error'
        });
    }
}
/**
* 禁止輸入非數字
*
*/
function validate(evt) {
    if (evt.keyCode != 8) {

        var theEvent = evt || window.event;
        var key = theEvent.keyCode || theEvent.which;
        key = String.fromCharCode(key);
        console.log("key:" + key);
        var regex = /[0-9]/;
        if (!regex.test(key)) {
            theEvent.returnValue = false;

            if (theEvent.preventDefault)
                theEvent.preventDefault();
        }
    }
}