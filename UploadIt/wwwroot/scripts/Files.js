/// <reference path="../lib/jquery.min.js" />
import * as constants from "./Constants.js";
import * as cookies from "./Cookies.js";

$(document).ready(function () {
    requestFileList();

    $(document).on("click",
        "#file_list_table tbody tr td a",
        (e) => {
            downloadBlob(e.currentTarget.text);
        });

    $(document).on("click",
        "#file_list_table tbody tr td button",
        (e) => {
            let text = e.currentTarget.parentElement.parentElement.getElementsByTagName("a")[0].text;
            console.log($("#confirmationModal .modal-body"));
            $("#confirmationModal .modal-body").html("Do you want to delete " + text + "?");
            $("#confirmationModal").modal("show").on("click",
                "#confirmationModalConfirmationButton",
                () => {
                    deleteFile(text);
                    $("#confirmationModal").modal("hide");
                });
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
    let form = new FormData();
    form.append("fileName", fileName);

    $.ajax({
        url: constants.apiUrl + "/File/Delete",
        method: "post",
        processData: false,
        contentType: false,
        headers: {
            Authorization: "Bearer " + cookies.getAuthCookieTokenOrEmpty()
        },
        data: form
    }).done(() => {
        location.reload();
    });

}

function downloadBlob(fileName) {
    $.ajax({
        method: "get",
        //special characters need to be encoded in query strings
        url: constants.apiUrl + "/File/DownloadBlob?fileName=" + encodeURIComponent(fileName),
        headers: {
            Authorization: "Bearer " + cookies.getAuthCookieTokenOrEmpty()
        }
    }).done((response, status, jqXHR) => {
        let contentType = jqXHR.getResponseHeader("content-type");
        let blob = new Blob([response], { type: contentType });
        let url = URL.createObjectURL(blob);
        let contentDisposition = jqXHR.getResponseHeader("Content-Disposition");
        let pair = contentDisposition.substr(contentDisposition.indexOf("filename="));
        let receivedFilename = pair.substr(pair.indexOf("=") + 1);
        console.log(receivedFilename);

        var a = window.document.createElement("a");
        a.href = url;
        a.download = receivedFilename;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        });
}