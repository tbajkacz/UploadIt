/// <reference path="../lib/jquery.min.js" />
import * as constants from "./Constants.js";
import * as cookies from "./Cookies.js";

$(document).ready(function () {
    requestFileList();

    $(document).on("click",
        "#file_list_table tbody tr td a",
        (e) => {
            downloadFile(e.currentTarget.text);
        });
});

function requestFileList() {
    $.ajax({
        url: constants.apiUrl + "/File/GetFiles",
        method: "get",
        headers: {
            "Authorization": "Bearer " + cookies.getAuthCookieTokenOrEmpty()
        },
        success: (data) => {
            for (var f in data.files) {
                //TODO 
                $("#file_list_table tbody").append(`<tr><td><a href="javascript:void(0)">${data.files[f]}</a></td></tr>`);
            }
        }
    });
}

function downloadFile(fileName) {
    $.ajax({
        url: constants.apiUrl + "/File/GetDownloadToken",
        method: "get",
        headers: {
            "Authorization": "Bearer " + cookies.getAuthCookieTokenOrEmpty()
        },
        success: (response) => {
            let url = constants.apiUrl + "/File/Download" + "?token=" + response + "&fileName=" + fileName;

            console.log(url);

            //set the hidden <a> to the returned url and click it to trigger save file dialog
            $("#download_action_link").attr("href", url);
            $("#download_action_link")[0].click();
        }
    });
}