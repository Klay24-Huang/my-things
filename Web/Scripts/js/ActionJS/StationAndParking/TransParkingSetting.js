var NowEditID = 0;
var ParkingName = "";
var ParkingAddress = "";
var Latitude = 0.0;
var Longitude = 0.0;
var OpenTime = "";
var CloseTime = "";
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
               // $("#ParkingName").val("");
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
        $("#frmTransParkingSetting").submit();
    });
    $("#btnReset").on("click", function () {
        init();
    })
    if (Mode != '') {
        $("#ddlObj").val(Mode);
        $("#ddlObj").trigger("click");
      
    }
    if (ResultDataLen > -1) {
        $("#panelResult").show();
        $('table').footable();
    }
   

});
function DoReset(Id) {
    if (NowEditID > 0) {
        NowEditID = 0;
        $("#ParkingName_" + Id).val(ParkingName).hide();
        $("#ParkingAddress_" + Id).val(ParkingAddress).hide();
        $("#Latitude_" + Id).val(Latitude).hide();
        $("#Longitude_" + Id).val(Longitude).hide();
        $("#OpenTime_" + Id).val(OpenTime).hide();
        $("#CloseTime_" + Id).val(CloseTime).hide();
    } else {
        $("#ParkingName_" + Id).hide();
        $("#ParkingAddress_" + Id).hide();
        $("#Latitude_" + Id).hide();
        $("#Longitude_" + Id).hide();
        $("#OpenTime_" + Id).hide();
        $("#CloseTime_" + Id).hide();
    }

    $("#btnReset_" + Id).hide();
    $("#btnSave_" + Id).hide();
    $("#btnEdit_" + Id).show();

}
function DoEdit(Id) {
    if (NowEditID > 0) {
        //先還原前一個
        $("#ParkingName_" + NowEditID).val(ParkingName).hide();
        $("#ParkingAddress_" + NowEditID).val(ParkingAddress).hide();
        $("#Latitude_" + NowEditID).val(Latitude).hide();
        $("#Longitude_" + NowEditID).val(Longitude).hide();
        $("#OpenTime_" + NowEditID).val(OpenTime).hide();
        $("#CloseTime_" + NowEditID).val(CloseTime).hide();
    }
        //再開啟下一個
    /*    NowEditID = Id;
        ParkingName = $("#ParkingName_" + Id).val();
        ParkingAddress = $("#ParkingAddress_" + Id).val();
        Latitude = $("#Latitude_" + Id).val();
        Longitude = $("#Longitude_" + Id).val();
        OpenTime = $("#OpenTime_" + Id).val();
        CloseTime = $("#CloseTime_" + Id).val();
    } else {*/
        NowEditID = Id;
        ParkingName = $("#ParkingName_" + Id).val();
        ParkingAddress = $("#ParkingAddress_" + Id).val();
        Latitude = $("#Latitude_" + Id).val();
        Longitude = $("#Longitude_" + Id).val();
        OpenTime = $("#OpenTime_" + Id).val();
        CloseTime = $("#CloseTime_" + Id).val();
  //  }
    $("#ParkingName_" + Id).show();
    $("#ParkingAddress_" + Id).show();
    $("#Latitude_" + Id).show();
    $("#Longitude_" + Id).show();
    $("#OpenTime_" + Id).show();
    $("#CloseTime_" + Id).show();

    $("#btnReset_" + Id).show();
    $("#btnSave_" + Id).show();
    $("#btnEdit_" + Id).hide();

}
function DoSave(Id) {

}
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