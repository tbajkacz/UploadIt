import * as cookies from './Cookies.js';
import * as constants from "./Constants.js";

$(document).ready(() => {
    getRegistrationRequests();

    $(document).on("click", "#registrationListTable [data-btn-accept]",
        (e) => {
            let requestId = $(e.currentTarget).parent().parent().children().first().text();
            let form = new FormData();
            form.append("id", requestId.toString());
            $.ajax({
                method: "post",
                url: constants.apiUrl + "/Admin/AcceptRegisterRequest",
                data: form,
                contentType: false,
                processData: false,
                headers: {
                    Authorization: "Bearer " + cookies.getAuthCookieTokenOrEmpty()
                }
            }).done(() => {
                deleteRequestAjax(requestId).then(() => {
                    location.reload();
                }); 
            });
        });

    $(document).on("click", "#registrationListTable [data-btn-delete]",
        (e) => {
            let requestId = $(e.currentTarget).parent().parent().children().first().text();
            deleteRequestAjax(requestId).then(() => {
                location.reload();
            }); 
        });
});

function getRegistrationRequests() {
    $.ajax({
        method: "get",
        url: constants.apiUrl + "/Admin/GetRegistrationRequests",
        headers: {
            Authorization: "Bearer " + cookies.getAuthCookieTokenOrEmpty()
        }
    }).done((ret) => {
        console.log(ret);
        for (var r in ret) {
            $("#registrationListTable tbody")
                .append(`<tr><td>${ret[r].id}</td><td>${ret[r].userName}</td><td>${ret[r].email}</td><td><button class="btn btn-primary" data-btn-accept>Accept</button></td><td><button class="btn btn-danger" data-btn-delete="delete">Delete</button></td></tr>`);
        }
    });
}

function deleteRequestAjax(requestId) {
    let form = new FormData();
    form.append("id", requestId.toString());
    console.log(form);
    return $.ajax({
        method: "post",
        url: constants.apiUrl + "/Admin/DeleteRegisterRequest",
        data: form,
        contentType: false,
        processData: false,
        headers: {
            Authorization: "Bearer " + cookies.getAuthCookieTokenOrEmpty()
        }
    });
}