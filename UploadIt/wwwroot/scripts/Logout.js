import * as cookies from "./Cookies.js";

$(document).ready(function () {
    cookies.deleteAuthCookie();
    window.location = "Index";
});