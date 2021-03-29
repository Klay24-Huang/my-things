﻿$(function () {
    //$("#fileImport").prop("disabled", "disabled");
    $("#StationType").val(StationType);
    $("#fileImport").prop("disabled", "");
    clearFileInput("fileImport");

    var del = false

    $("#btnDelete").on("click", function () {
        //console.log($.trim($("#fileName1").val));
        del = true
        if ($.trim($("#fileName1").val()) == "" || $.trim($("#fileData1").val()) == "") {
            swal({
                title: 'Fail',
                text: "請選擇圖片",
                icon: 'error'
            });
            return;
        }
        $("#PIC1").attr('src', null);
        $("#fileName1").val('');
        $("#fileData1").val('');
    });

    $("#btnUpload").on("click", function () {
        if ($("#fileImport")[0].files.length != 0) {
            $("#fileName1").val($("#fileImport")[0].files[0].name);
            handleFiles($("#fileImport")[0].files, 1);
        } else {
            swal({
                title: 'Fail',
                text: "請選擇圖片",
                icon: 'error'
            })
        }
    });
    $("#PIC1").on("click", function () {
        if ($(this).attr("src") != "") {
            //console.log("路徑:" + $(this).attr("src"));
            $("#tmpENVPIC").attr("src", $(this).attr("src"));
            $("#surrounding_modal").modal();
        }
    });

    //在 HTML 有提供 File、FileReader、Image 這三組 API，透過他們可以達到檔案上傳的格式、尺寸、大小檢查以及預覽功能
    $('body').on("change", "#fileImport", function () {
        var file = this.files[0]; //原生input file控制元件有個files屬性，該屬性是一個陣列
        if (file != null) {
            var fileName = file.name;

            $('.jfilestyle input[type=text]').val(fileName);
            var ext = GetFileExtends(fileName);
            var extName = "";
            if (CheckStorageIsNull(ext)) {
                extName = ext[0];
                //console.log(extName.toUpperCase())
            }

            //判斷圖片尺寸，看網路寫的，爽
            var reader = new FileReader();
            reader.readAsDataURL(file);
            reader.onload = function (e) {
                var data = e.target.result;
                //加载图片获取图片真实宽度和高度
                var image = new Image();
                image.onload = function () {
                    var width = image.width;
                    var height = image.height;

                    //console.log("Q:" + width)
                    //console.log("Q:" + height)
                    if (width != 343 || height != 80) {
                        swal({
                            title: 'Fail',
                            text: "尺寸不對",
                            icon: 'error'
                        }).then(function (value) {
                            clearFileInput("fileImport");
                        });
                    }
                };
                image.src = data;
            }
            //if (fileSize > 1024000) {
            //    swal({
            //        title: 'Fail',
            //        text: "僅允許1000K",
            //        icon: 'error'
            //    }).then(function (value) {
            //        clearFileInput("fileImport");
            //    });
            //}
            if (extName.toUpperCase() != "PNG" && extName.toUpperCase() != "JPG") {
                swal({
                    title: 'Fail',
                    text: "僅允許png和jpg",
                    icon: 'error'
                }).then(function (value) {
                    // $("#fileImport").val("");
                    clearFileInput("fileImport");
                });
            }

            var fileName2 = fileName.slice(0, -4) //擷取.png之前的文字

            //可以正確判斷，但用正則表達式比較好         
            for (var i = 0; i < fileName2.length; i++) {
                if (fileName2.charCodeAt(i) > 255) {
                    swal({
                        title: 'Fail',
                        text: "我家APP看到中文就會暴走崩潰，所以不准上傳中文檔名",
                        icon: 'error'
                    }).then(function (value) {
                        // $("#fileImport").val("");
                        clearFileInput("fileImport");
                    });
                    break;
                }
            }
        }
    })

    

    $("#btnSend").on("click", function () {
        ShowLoading("資料處理中…");
        var StationType = $("#StationType").val();
        var URL = $("#URL").val();
        var RunHorse = $("#RunHorse").val();
        var SDate = $("#SDate").val();
        var EDate = $("#EDate").val();
        var fn = del == true ? $("#fileData1").val() : 'pp'; //若沒有做刪除圖片，就隨便給個名字騙過這邊的檢核
        var flag = true;
        var checkList = [SDate, EDate, URL, RunHorse, fn];
        var checkErrList = ["有效起日未填", "有效迄日未填", "URL沒填，妳有看到嗎?", "跑馬燈啦，填一下好嗎", "圖片沒傳，睏了嗎?"];

        if (SDate !== "" && EDate !== "") {
            if (SDate > EDate) {
                flag = false;
                errorMsg = "起始日期大於結束日期";
            }
        }

        var checkLen = checkList.length;
        if (flag) {
            for (var i = 0; i < checkLen; i++) {
                if (checkList[i] == "") {
                    flag = false;
                    errorMsg = checkErrList[i];
                    break;
                }
            }
        }
        if (flag) {
            var obj = new Object();
            obj.StationType = StationType;
            obj.URL = URL;
            obj.RunHorse = RunHorse;
            obj.SDate = SDate;
            obj.EDate = EDate;
            obj.fileName1 = $("#fileName1").val();
            obj.fileData1 = $("#fileData1").val();
            obj.UserID = Account;
            obj.SEQNO = $("#SEQ").val();
            DoAjaxAfterGoBack(obj, "BE_Banner", "修改banner發生錯誤")
        } else {
            disabledLoadingAndShowAlert(errorMsg)
        }

    });
});


var inPicSize = 0;
//圖片處理
function handleFiles(file, id) {
    console.log("call handleFiles");
    if (file != null) {
        console.log(file)
        if (file.length > 0) {
            var tmpfile = file[0];
            console.log("file size=" + file[0].size);

            //   $("#PicInfo").html("檔案建立日：" + new Date(file[0].lastModified).toLocaleDateString() + " " + new Date(file[0].lastModified).toLocaleTimeString());
            console.log(new Date(file[0].lastModified).toLocaleDateString() + " " + new Date(file[0].lastModified).toLocaleTimeString());
            var reader = new FileReader();
            var fileSize = Math.round(file[0].size / 1024 / 1024);
            inPicSize = fileSize;
            reader.onload = function (e) {
                console.log("call reader.onload ");
                /* $('#show-incarPic').attr('src', e.target.result);
                 var base64 = e.target.result.split(",");
                 $("#hidincarPicType").val(base64[0].replace(/^data:/, ''));
                 $("#hidincarPic").val("⊙"+base64[1]);*/

                var url = reader.result;
                var base64 = e.target.result.split(",")[1];
                $("#fileData" + id).val(base64);

                // console.log("before length:" + base64.length);
                $('#PIC' + id).attr('src', url);

            };
            reader.readAsDataURL(tmpfile);


            // $('#tmpENVPIC').show();
            console.log("show")
            //  $("#btnReview").show();

        } else {
            //$("#btnReview").hide();
            // document.getElementById('tmpENVPICc').src = "";
            // $("#hidPic").val("");


        }

    }

}





document.getElementById("PIC1").addEventListener('load', function () {

    var cvs = document.createElement('canvas'),
        ctx = cvs.getContext('2d');
    var img = new Image(),
        maxW = 640; //設定最大寬度
    if (this.width > maxW) {
        this.height *= maxW / img.width;
        this.width = maxW;
    }
    // console.log(this.file)
    cvs.width = 640; // this.width;
    cvs.height = 480; // this.height;
    ctx.clearRect(0, 0, cvs.width, cvs.height);
    //        ctx.drawImage(this, 0, 0, this.width, this.height);
    ctx.drawImage(this, 0, 0, 640, 480);
    var compressRate = getCompressRate(1, inPicSize);
    var dataUrl = cvs.toDataURL('image/png', compressRate);
    var base64 = dataUrl.split(",");
    //console.log(dataUrl);    //console.log(dataUrl);
});
document.getElementById('PIC1').addEventListener('change', function () {
    console.log("call PIC1 Change");
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
        var dataUrl = cvs.toDataURL('image/png', compressRate);
        document.body.appendChild(cvs);
        // console.log(dataUrl);
        console.log("call compress");
    }
    img.src = res;
}
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