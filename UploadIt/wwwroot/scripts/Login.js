/// <reference path="../lib/jquery.min.js" />
import * as constants from "./Constants.js";
import * as cookies from "./Cookies.js";

$("#login_form").on("submit", (ev) => {
    ev.preventDefault();
    let form = new FormData();
    let data = Object.values(ev.currentTarget).slice(0, 2);
    form.append("userName", data[0].value);
    form.append("password", data[1].value);

    $.ajax({
        method: "post",
        url: constants.apiUrl + "/Account/Authenticate",
        contentType: false,
        processData: false,
        data: form,
        success: (data) => {
            cookies.addAuthCookie(data);
            document.location = "Index";
        }
    }).always((data) => {
        let statusSpan = $("#login_form_status");
        if (data.responseText) {
            statusSpan.text(data.responseText);
        } else {
            statusSpan.text("");
        }
    });
});
