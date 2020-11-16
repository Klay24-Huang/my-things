var Operator = "";
$(document).ready(function () {

    var hasData = parseInt($("#len").val());
    console.log(hasData);
    if (hasData > 0) {
        $('.table').footable();
    }
  
})

