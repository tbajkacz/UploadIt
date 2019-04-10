/// <reference path="../lib/jquery.min.js" />
import * as constants from "./ApiUrl.js";

$(document).ready(function () {
    console.log("ready");
    requestFileList();
})

function requestFileList() {
    $.ajax({
        url: constants.apiUrl + "/File/GetFiles",
        method: "get",
        headers: {
            "Authorization": "Bearer " + sessionStorage.jwtToken
        },
        success: (data) => {
            for (var f in data.files) {
                //TODO 
                $("#file_list_table tbody").append(`<tr><td>${data.files[f]}</td></tr>`);
            }
        }
    });
}
