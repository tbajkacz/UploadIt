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

    $(document).on("click",
        "#file_list_table tbody tr td button",
        (e) => {
            deleteFile(e.currentTarget.parentElement.parentElement.getElementsByTagName("a")[0].text);
        });
});

function requestFileList() {
    $.ajax({
        url: constants.apiUrl + "/File/GetFiles",
        method: "get",
        headers: {
            "Authorization": "Bearer " + cookies.getAuthCookieTokenOrEmpty()
        }
    }).done((data) => {
        for (var f in data) {
            //TODO 
            var size = getFileSizeDisplayString(data[f].size);
            $("#file_list_table tbody")
                .append(`<tr><td><a href="javascript:void(0)">${data[f].name}</a></td><td>${size}</td><td><button class="btn btn-danger">Delete</button></td></tr>`);
        }
    });
}

function downloadFile(fileName) {
    $.ajax({
        url: constants.apiUrl + "/File/GetDownloadToken",
        method: "get",
        headers: {
            "Authorization": "Bearer " + cookies.getAuthCookieTokenOrEmpty()
        }
    }).done((response) => {
        let url = constants.apiUrl + "/File/Download" + "?token=" + response + "&fileName=" + fileName;

        console.log(url);

        //set the hidden <a> to the returned url and click it to trigger save file dialog
        $("#download_action_link").attr("href", url);
        $("#download_action_link")[0].click();
    });
}

function getFileSizeDisplayString(bytesSize) {
    let size = bytesSize / (1024 * 1024);
    let unitString = " MB";
    if (size < 1) {
        size = size * 1024;
        unitString = " KB";
    }
    return size.toFixed(2) + unitString;
}

function deleteFile(fileName) {

}