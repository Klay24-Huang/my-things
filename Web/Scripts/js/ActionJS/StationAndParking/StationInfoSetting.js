$(document).ready(function () {

    var StationList = $("#hidStation").val();
 
    if (StationList !== "") {
        var Station = StationList.split(";");
        $("#StationID").autocomplete({
            source: Station,
            minLength: 1,
            matchCase: false
        });
    }
    
});
function detailMap(lat, lng) {

}