$(function () {
    var CarList = $("#hidCar").val();
    console.log("CarList=" + CarList);
    if (CarList !== "") {
        var Car = CarList.split(";");
        $("#ObjCar").autocomplete({
            source: Car,
            minLength: 3,
            matchCase: false
        });
    }
    $("#SendDate").datepicker({
        minDate: new Date(2010, 1, 1,0,0,0),
        maxDate:new Date(2030,12,31,23,59,59)
    });
});