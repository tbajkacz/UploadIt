/// <reference path="../lib/jquery.min.js" />
import * as constants from "./Constants.js";

$("#register_form").on("submit", (ev) => {
    ev.preventDefault();
    let form = new FormData();
    let data = Object.values(ev.currentTarget).slice(0, 3);
    form.append("email", data[0].value);
    form.append("username", data[1].value);
    form.append("password", data[2].value);

    $.ajax({
        url: constants.apiUrl + "/Account/Register",
        method: "post",
        data: form,
        success: (resp) => {
            console.log(resp);
        }

    });
})