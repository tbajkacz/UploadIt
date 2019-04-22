﻿/// <reference path="../lib/jquery.min.js" />

export function addAuthCookie(data) {
    let date = new Date(data.validTo);
    document.cookie = `auth=${JSON.stringify(data)}; expires=${date};secure`;
}

export function getAuthCookie() {
    let cookie = document.cookie.split(";").filter((s) => s.indexOf("auth") !== -1)[0];
    if (cookie) {
        let result = cookie.replace("auth=", "");
        return JSON.parse(result);
    }
    return null;
}


export function getAuthCookieTokenOrEmpty() {
    var res = getAuthCookie();
    if (res !== null && res.token !== null) {
        return res.token;
    }
    return "";
}

export function deleteAuthCookie() {

}