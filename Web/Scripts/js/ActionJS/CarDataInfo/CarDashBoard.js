$(function () {
    var StationList = $("#hidStation").val();

    if (StationList !== "") {
        var Station = StationList.split(";");

        $("#StationID").autocomplete({
            source: Station,
            minLength: 1,
            matchCase: true,
              select: function (event, ui) {
                var data = ui.item.value.split("(");
                var contactData = data[1].split(")");
                  $("#StationID").val(contactData[0]);
            
                  $("#StationName").html(data[0]);
                return false;
            }
        });

    }
    var CarList = $("#hidCar").val();

    if (CarList !== "") {
        var Car = CarList.split(";");

        $("#CarNo").autocomplete({
            source: Car,
            minLength: 1,
            matchCase: true
        });

    }

    $("#btnSearch").on("click", function () {
        ShowLoading("資料處理中...");
        var flag = true;
        var message = "";
        var CarNo = $("#CarNo").val();
        var StationID = $("#StationID").val();
        var ShowType = $("input[name=ShowType]:checked").val();
      

        if (false==CheckIsUndefined(ShowType)) {
            flag = false;
            message="請選擇顯示方式"
        }
     
        if (flag) {
            var obj = new Object();
            var terms = new Array();
            $("input[name=terms]:checked").each(function () {
                terms.push($(this).val());
            });
            obj.CarNo = CarNo;
            obj.StationID = StationID;
            obj.ShowType = ShowType;
            obj.Terms = terms;
            var json = JSON.stringify(obj);
            $("#queryData").val(json);
            $("#frmCarDashBoard").submit();
        } else {
            disabledLoadingAndShowAlert(message);
        }
        return false;
    })
});