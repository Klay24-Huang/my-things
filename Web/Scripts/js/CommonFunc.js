//判斷是否為undefined
function CheckIsUndefined(obj) {
    if (typeof obj !== 'undefined') {
        return true;
    } else {
        return false;
    }
}
function ShowLoading(message) {
    $.busyLoadFull("show", {
        text: message,
        spinner: "cube-grid"
    });
}
function disabledLoading() {
    $.busyLoadFull("hide");
}
function disabledLoadingAndShowAlert(message) {
    $.busyLoadFull("hide");
    swal({
        title: 'Fail',
        text: message,
        icon: 'error'
    });
}