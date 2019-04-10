/// <reference path="../lib/jquery.min.js" />
import * as constants from "./ApiUrl.js";

$(document).ready(() => {
    document.getElementById("file_drop_zone").addEventListener("drop", (ev) => {
        //prevents the default open file behavior
        ev.preventDefault();
        if (ev.dataTransfer.items) {
            for (var i = 0; i < ev.dataTransfer.items.length; i++) {
                // If dropped items aren't files, reject them
                if (ev.dataTransfer.items[i].kind === 'file') {
                    let file = ev.dataTransfer.items[i].getAsFile();
                    console.log(`file[${i}].name = ${file.name}`);

                    let formData = new FormData();
                    formData.append("file", file);

                    let token = null;

                    if (sessionStorage.jwtToken !== undefined) {
                        token = sessionStorage.jwtToken;
                    }

                    $.ajax({
                        method: "post",
                        url: constants.apiUrl + "/File/UploadFile",
                        contentType: false,
                        processData: false,
                        crossDomain: true,
                        mimeType: "multipart/form-data",
                        data: formData,
                        headers: {
                            "Authorization" : "Bearer " + token
                        }
                    });
                }
            }
        }
    });

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