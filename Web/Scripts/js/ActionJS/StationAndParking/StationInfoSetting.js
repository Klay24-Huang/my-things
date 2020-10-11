$(document).ready(function () {
    console.log("我有載入");
    var StationList = $("#hidStation").val();

    if (StationList !== "") {
        var Station = StationList.split(";");
        console.log(Station);
        $("#StationID").autocomplete({
            source: Station,
            minLength: 1,
            matchCase: true
        });
        console.log("我有載入");
    }
    
});
function detailMap(lat, lng) {

}