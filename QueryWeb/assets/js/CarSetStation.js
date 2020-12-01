$(function () {
    var CarList = $("#hidCar").val();
    CarautoComplete(CarList, "#car_id", 3)
    console.log("CarList=" + CarList);
    //if (CarList !== "") {
    //    var Car = CarList.split(";");
    //    $("#car_id").autocomplete({
    //        source: Car,
    //        minLength: 3,
    //        matchCase: false
    //    });
    //}

});