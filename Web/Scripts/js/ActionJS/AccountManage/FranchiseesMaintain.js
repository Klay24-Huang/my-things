$(function () {
})
function setPer(obj, per) {
    var objName = "#" + obj;
    var objPerName = "#" + obj + "Bar";
    $(objName).html(per);
    $(objPerName).css("width", per + "%");

}
function handleFiles(file, id) {
    console.log("call handleFiles");
    if (file != null) {
        console.log(file)
        if (file.length > 0) {
            var tmpfile = file[0];
            console.log("file size=" + file[0].size);

            $("#PicInfo").html("檔案建立日：" + new Date(file[0].lastModified).toLocaleDateString() + " " + new Date(file[0].lastModified).toLocaleTimeString());
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
                console.log("before length:" + base64.length);
                $('#tmpENVPIC').attr('src', url);

            };
            reader.readAsDataURL(tmpfile);


            $('#tmpENVPIC').show();
            console.log("show")
   
        } else {

            document.getElementById('tmpENVPICc').src = "";
            $("#hidPic").val("");
 

        }

    }

}
document.getElementById("tmpENVPIC").addEventListener('load', function () {

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
    var dataUrl = cvs.toDataURL('image/jpeg', compressRate);
    var base64 = dataUrl.split(",");
    console.log(dataUrl);
    $("#hidPicType").val(base64[0].replace(/^data:/, ''));
    $("#hidPic").val("⊙" + base64[1]);
    console.log("after length:" + base64[1].length);


});
document.getElementById('tmpENVPIC').addEventListener('change', function () {
    console.log("call tmpENVPIC Change");
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
        var dataUrl = cvs.toDataURL('image/jpeg', compressRate);
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