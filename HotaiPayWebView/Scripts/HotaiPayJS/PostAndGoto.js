function GOTO(formid, goto, data) {
    console.log('3');
    var newform = document.createElement("form");
    newform.id = formid;
    newform.setAttribute("method", "post");
    newform.name = formid;
    //// 新增到 body 中
    document.body.appendChild(newform);
    newform.action = goto;
    $.each(data, function (name, value) {

        var input = document.createElement("input");
        input.setAttribute("type", "text");
        input.setAttribute("name", name);
        input.setAttribute("value", value);
        newform.appendChild(input);
        
        //$("<input type='text' />")
        //    .attr("id", name)
        //    .attr("name", name)
        //    .attr("value", value)
        //    .appendTo(newform);
            
    });
    console.log('4');
    console.log($("#" + formid).serialize());

    newform.submit();
    document.body.removeChild(newform);
    return false;
}