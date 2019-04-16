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

function downloadFile(name) {
    $.ajax({
        url: constants.apiUrl + "/File/DownloadFile/" + name,
        method: "get",
        headers: {
            "Authorization": "Bearer " + cookies.getAuthCookieTokenOrEmpty()
        },
        success: (ret) => {
            console.log('success');
        }
    });
}

//function scc(data, name) {
//    var BlobBuilder = window.BlobBuilder || window.WebKitBlobBuilder || window.MozBlobBuilder || window.MSBlobBuilder;
//    var builder = new BlobBuilder();
//    builder.append(data);
//    var blob = builder.getBlob("application/pdf");
//    if (!name) name = "Download.bin";
//    if (window.saveAs) {
//        window.saveAs(blob, name);
//    }
//}