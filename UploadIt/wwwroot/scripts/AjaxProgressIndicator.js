/// <reference path="../lib/jquery.min.js" />



$(document).on("ajaxStart", () => {
    $("#loadingModal").modal("show");
});

$(document).on("ajaxSuccess", () => {
    $("#loadingModal").modal("hide");
})