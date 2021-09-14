$(function () {
    $("#Clear").on('click', function () {

        var file = $("#fileImport")
        file.after(file.clone().val(""));
        file.remove();
    })

    $("#export").on('click', function () {

        var file = $("#fileImport")
        file.after(file.clone().val(""));
        file.remove();
    })
})