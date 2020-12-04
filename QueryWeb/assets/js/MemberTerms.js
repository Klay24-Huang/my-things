$(document).ready(function () {
    $("#btnSave").on("click", function () {
        var MemberTerms = document.getElementById("MemberTerms");
        var errMsg = "";
        var flag = true;
        var file = MemberTerms.files[0];
        if (file !== null) {
            if (file.size> 0) {
                flag = checkFile();
                if (flag === false) {
                    errMsg = "僅允許上傳pdf檔"
                }
            } else {
                flag = false;
                errMsg = "請先上傳檔案";
                console.log("我沒有上傳檔案");
            }
            
        } else {
            flag = false;
            errMsg = "請先上傳檔案";
            console.log("我沒有上傳檔案");
        }
        if (flag) {
            frmMember.submit();
        } else {
            warningAlert(errMsg, false, 0, "");
        }
    });

});
function ViewTerms(termsID) {
    var site = "ViewTerms/?termsID=" + termsID.toString();
    window.open (site);
}
function checkFile() {
    var f = document.frmMember;
    var re = /\.(pdf|PDF)$/i;  //允許的圖片副檔名 
    var flag = true;
    if (!re.test(f.MemberTerms.value)) {
        flag = false;
    }
    return flag;
}