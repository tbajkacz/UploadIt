/// <reference path="../lib/jquery.min.js" />
import * as constants from "./Constants.js";
import * as cookies from "./Cookies.js";

$(document).ready(() => {
    document.getElementById("file_drop_zone").addEventListener("drop", (ev) => {
        //prevents the default open file behavior
        ev.preventDefault();
        if (ev.dataTransfer.items) {
            uploadFiles(ev.dataTransfer.items);
        }
    });
    //TODO modal window on drop with a submit button

    document.getElementById("file_drop_zone").addEventListener("dragover", (ev) => {
        ev.preventDefault();
    });

    $("#api_test").on("click",
        () => {
            $.ajax({
                method: "get",
                url: constants.apiUrl + "/File/Test",
                data: {
                    message: "hello"
                }
            }).then((response) => {
                console.log(response);
            });
        });
});

function uploadFiles(dataTransferItems) {
    if (dataTransferItems) {
        for (var i = 0; i < dataTransferItems.length; i++) {
            // If dropped items aren't files, reject them
            if (dataTransferItems[i].kind === 'file') {
                let file = dataTransferItems[i].getAsFile();
                console.log(`file[${i}].name = ${file.name}`);

                let formData = new FormData();
                formData.append("file", file);

                $.ajax({
                    method: "post",
                    url: constants.apiUrl + "/File/UploadFile",
                    contentType: false,
                    processData: false,
                    crossDomain: true,
                    mimeType: "multipart/form-data",
                    data: formData,
                    headers: {
                        "Authorization": "Bearer " + cookies.getAuthCookieTokenOrEmpty()
                    }
                });
            }
        }
    }
}