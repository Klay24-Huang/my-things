$(document).ready(function () {
    $("#exampleDownload").on("click", function () {
        window.location.href = "../Content/example/ParkingExample.xlsx";
    });
    init();
    $("#ddlObj").on("change", function () {
        var Mode = $(this).val();
        console.log("Mode=" + Mode);
        switch (Mode) {
            case "":
                init();
                break;
            case "Add":
                $("#fileImport").prop("disabled", "");
                $("#btnSend").show();
                $("#btnReset").show();
                $("#btnSend").text("儲存");
                
                break;
            case "Edit":
                $("#fileImport").prop("disabled", "disabled");
                $("#ParkingName").val("");
                $("#ParkingName").prop("disabled", "");
                $("#btnSend").show();
                $("#btnReset").show();
                $("#btnSend").text("查詢");

                break;
        }
    })
    $("#btnReset").on("click", function () {
        init();
    });
    $("#btnSend").on("click", function () {

    });
    $("#btnReset").on("click", function () {
        init();
    })
    $('table').footable();

});
function init() {
    $("#btnSend").hide();
    $("#btnReset").hide();
    $("#fileImport").val("");
    $("#fileImport").prop("disabled", "disabled");
    $("#ParkingName").val("");
    $("#ParkingName").prop("disabled", "disabled");
    $("#ddlObj").val("");
    $("#panelResult").hide();
    $("#fileImport").on("change", function () {
        var file = this.files[0];
        var fileName = file.name;
        var fileSize = file.size;
        var ext = GetFileExtends(fileName);
        var extName = "";
        if (CheckStorageIsNull(ext)) {
            extName = ext[0];
            console.log(extName.toUpperCase())
        }
        if (extName.toUpperCase() != "XLSX" && extName.toUpperCase() != "XLS") {

            swal({
                title: 'Fail',
                text: "僅允許匯入xlsx或xls格式",
                icon: 'error'
            }).then(function (value) {
            
                $("#fileImport").val("");
            });
        }
    })
}