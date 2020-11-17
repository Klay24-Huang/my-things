$(function () {
    SetManagerStationHasValue($("#ManageStationID", ManagerStationID));
    SetStationByText($("#StationID"), $("#StationName"));
    $("#StationType").val(StationType);
    $("#StationPick").val(StationPick);
    $("#fileImport").prop("disabled", "disabled");
    $("#pic_descript").val("");
    $("#pic_descript").prop("disabled", "disabled");

    if (CityID != "0" && AreaID != "0") {
        SetCityHasSelectedhaveZipCode($("#City"), CityID, $("#Area"), AreaID, $("#ZipCode"));
    } else {
        if (CityID == 0) {
            SetCity($("#City"));
        } else {
            SetCityHasSelectedhaveZipCode($("#City"), CityID, $("#Area"), AreaID, $("#ZipCode"))
        }
    }
    $("#City").on("change", function () {
        SetAreaHasZip($("#Area"), $(this).val(), $("#ZipCode"))
    })
    $("#Area").on("change", function () {
        var ZipCode = $("#Area option:selected").text().split("(")[1].replace(")", "")
        SetZipCode($("#ZipCode"), ZipCode);
    })
    $("#sort").on("change", function () {
        console.log($(this).val());
        if ($(this).val() != "-1") {
            $("#fileImport").prop("disabled","");
          //  $("#fileImport").val($("#fileName" + $(this).val()).val());
            $("#pic_descript").val($("#fileDescript" + $(this).val()).val());
            $("#pic_descript").prop("disabled", "");
            clearFileInput("fileImport");
        } else {
            $("#fileImport").prop("disabled", "disabled");
            $("#pic_descript").val("");
            $("#pic_descript").prop("disabled", "disabled");
            clearFileInput("fileImport");
        }
    });
    $("#btnMap").on("click", function () {
     
        $("#frmPolygon").submit();
    })
    $("#btnUpload").on("click", function () {
        var sort = $("#sort").val();
        console.log(sort);
        var descript = $("#pic_descript").val();
        if (descript == "") {
            swal({
                title: 'Fail',
                text: "請輸入描述",
                icon: 'error'
            })
        } else {
            if (typeof ($("#fileImport")[0].files) != "undefined") { 
                $("#fileDescript" + sort).val(descript);
                $("#fileName" + sort).val($("#fileImport")[0].files[0].name);
                handleFiles($("#fileImport")[0].files, sort);
              
            } else {
                swal({
                    title: 'Fail',
                    text: "請選擇圖片",
                    icon: 'error'
                })
            }
        }
    })
    $("#PIC1").on("click", function () {
        //console.log("aaa");
        if ($(this).attr("src") != "") {
            //console.log($(this).attr("src"));
            $("#tmpENVPIC").attr("src",$(this).attr("src"));
            $("#surrounding_modal").modal();
        }
    
    });
    $("#PIC2").on("click", function () {
        //console.log("aaa");
        if ($(this).attr("src") != "") {
            //console.log($(this).attr("src"));
            $("#tmpENVPIC").attr("src", $(this).attr("src"));
            $("#surrounding_modal").modal();
        }

    });
    $("#PIC3").on("click", function () {
        //console.log("aaa");
        if ($(this).attr("src") != "") {
            //console.log($(this).attr("src"));
            $("#tmpENVPIC").attr("src", $(this).attr("src"));
            $("#surrounding_modal").modal();
        }

    });
    $("#PIC4").on("click", function () {
        //console.log("aaa");
        if ($(this).attr("src") != "") {
            //console.log($(this).attr("src"));
            $("#tmpENVPIC").attr("src", $(this).attr("src"));
            $("#surrounding_modal").modal();
        }

    });
    $("#PIC5").on("click", function () {
        //console.log("aaa");
        if ($(this).attr("src") != "") {
            //console.log($(this).attr("src"));
            $("#tmpENVPIC").attr("src", $(this).attr("src"));
            $("#surrounding_modal").modal();
        }

    });
    $('body').on("change", "#fileImport",function () {
        var file = this.files[0];
        var flag = true;
        var errMsg = "";
        if (file != null) {
            var fileName = file.name;
            var fileSize = file.size;
            console.log(fileSize);
            $('.jfilestyle input[type=text]').val(fileName);
            var ext = GetFileExtends(fileName);
            var extName = "";
            if (CheckStorageIsNull(ext)) {
                extName = ext[0];
                console.log(extName.toUpperCase())
            }
            if (flag) {
                if (fileSize > 1024000) {
                    swal({
                        title: 'Fail',
                        text: "僅允許1000K",
                        icon: 'error'
                    }).then(function (value) {

                        clearFileInput("fileImport");
                    });
                }
            }
            if (extName.toUpperCase() != "PNG" && extName.toUpperCase() != "JPG" && extName.toUpperCase() != "JEPG") {

                swal({
                    title: 'Fail',
                    text: "僅允許png,jpg格式",
                    icon: 'error'
                }).then(function (value) {

                   // $("#fileImport").val("");
                    clearFileInput("fileImport");
                });
            }
        }
     
    })
    $("#btnSend").on("click", function () {
        ShowLoading("資料處理中…");
        var StationType = $("#StationType").val();
        var StationID = $("#StationID").val();
        var StationName = $("#StationName").val();
        var UniCode = $("#UniCode").val();
        var ManageStationID = $("#ManageStationID").val();
        var ZipCode = $("#ZipCode").val();
        var City = $("#City").val();
        var Area = $("#Area").val();
        var Addr = $("#Addr").val();
        var TEL = $("#TEL").val();
        var Longitude = $("#Longitude").val();
        var Latitude = $("#Latitude").val();
        var in_description = $("#in_description").val();
        var show_description = $("#show_description").val();
        var IsRequired = ($("#IsRequired").prop("checked") ? 1 : 0);
        console.log(IsRequired);
        var StationPick = $("#StationPick").val();
        var FCode = $("#FCode").val();
        var SDate = $("#SDate").val();
        var EDate = $("#EDate").val();
        var ParkingNum = $("#ParkingNum").val();
        var OnlineNum  = $("#OnlineNum").val();
        var flag = true;
        var errMsg = "";
        var checkList = [StationID, StationName, ManageStationID, ZipCode, Addr,  Longitude, Latitude, in_description, show_description, FCode, SDate, EDate, ParkingNum, OnlineNum];
        var checkErrList = ["據點代碼未填", "據點名稱未填", "管轄據點代碼未填", "郵遞區號未填", "地址未填", "經度未填", "緯度未填", "據點描述(內部註記)未填", "據點描述(app顯示)未填", "財務部門代碼未填", "有效起日未填", "有效迄日未填", "車位數未填", "實際上線數未填"];
        var checkLen = checkList.length;
        var showAreaName = $("#showAreaName").val();
        if (StationType == "-1") {
            flag = false;
            errorMsg = "請選擇據點類別";
        } else {
            if ((StationType == "3" || StationType == "4") && showAreaName == "") {
                flag = false;
                errorMsg = "據點類別為路邊或機車時，需輸入顯示名稱";
            }
        }
        if (flag) {
            if (City == "0") {
                flag = false;
                errorMsg = "請選擇縣市";
            }
        }
        if (flag) {
            if (Area == "0") {
                flag = false;
                errorMsg = "請選擇縣市";
            }
        }
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
            obj.StationID = StationID;
            obj.StationName = StationName;
            obj.ManagerStationID = ManageStationID;
            obj.UniCode = UniCode;
            obj.CityID = City;
            obj.AreaID = Area;
            obj.Addr = Addr;
            obj.TEL = TEL;
            obj.Longitude = Longitude;
            obj.Latitude = Latitude;
            obj.in_description = in_description;
            obj.show_description = show_description;
            obj.IsRequired = IsRequired;
            obj.StationPick = StationPick;
            obj.FCode = FCode;
            obj.SDate = SDate;
            obj.EDate = EDate;
            obj.ParkingNum=ParkingNum;
            obj.OnlineNum = OnlineNum;
            obj.Area = showAreaName;
            obj.fileName1=$("#fileName1").val();
            obj.fileName2=$("#fileName2").val();
            obj.fileName3=$("#fileName3").val();
            obj.fileName4 = $("#fileName4").val();
            obj.fileName5 = $("#fileName5").val();
            obj.fileData1=$("#fileData1").val();
            obj.fileData2=$("#fileData2").val();
            obj.fileData3=$("#fileData3").val();
            obj.fileData4 = $("#fileData4").val();
            obj.fileData5 = $("#fileData5").val();
            obj.fileDescript1=$("#fileDescript1").val();
            obj.fileDescript2=$("#fileDescript2").val();
            obj.fileDescript3=$("#fileDescript3").val();
            obj.fileDescript4 = $("#fileDescript4").val();
            obj.fileDescript5 = $("#fileDescript5").val();
            obj.UserID = Account;
            obj.Mode = 1;
            DoAjaxAfterGoBack(obj,"BE_HandleStation","修改據點發生錯誤")
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
               $('#PIC'+id).attr('src', url);

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
    console.log(dataUrl);
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
document.getElementById("PIC2").addEventListener('load', function () {

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
    console.log(dataUrl);


});
document.getElementById('PIC2').addEventListener('change', function () {
    console.log("call PIC2 Change");
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
document.getElementById("PIC3").addEventListener('load', function () {

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
    console.log(dataUrl);



});
document.getElementById('PIC3').addEventListener('change', function () {
    console.log("call PIC3 Change");
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
document.getElementById("PIC4").addEventListener('load', function () {

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



});
document.getElementById('PIC4').addEventListener('change', function () {
    console.log("call PIC4 Change");
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