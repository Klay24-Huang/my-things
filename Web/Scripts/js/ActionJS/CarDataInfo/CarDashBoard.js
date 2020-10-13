$(function () {
    //var StationList = localStorage.getItem("StationList");
    //var CarList = localStorage.getItem("CarList");
    //if (typeof StationList !== 'undefined' && StationList !== null) {
    //    StationList = JSON.parse(StationList)
    //}
    //if (typeof CarList !== 'undefined' && CarList !== null) {
    //    CarList = JSON.parse(CarList)
    //}
    ////var StationList = $("#hidStation").val();

    //if (StationList.length > 0) {
       
    //    var Station = new Array();
    //    var StationLen = StationList.length;
    //    for (var i = 0; i < StationLen; i++) {
    //        Station.push(StationList[i].StationName + "(" + StationList[i].StationID + ")");
    //    }
    //    $("#StationID").autocomplete({
    //        source: Station,
    //        minLength: 1,
    //        matchCase: true,
    //          select: function (event, ui) {
    //            var data = ui.item.value.split("(");
    //            var contactData = data[1].split(")");
    //              $("#StationID").val(contactData[0]);
            
    //              $("#StationName").html(data[0]);
    //            return false;
    //        }
    //    });

    //}


    //if (CarList.length > 0) {
    //    var Car = new Array();
    //    var CarLen = CarList.length;
    //    for (var i = 0; i < CarLen; i++) {
    //        Car.push(CarList[i].CarNo);
    //    }

    //    $("#CarNo").autocomplete({
    //        source: Car,
    //        minLength: 1,
    //        matchCase: true
    //    });

    //}
    SetStation($("#StationID"), $("#StationName"));
    SetCar($("#CarNo"))

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