/// <reference path="../lib/jquery.min.js" />

import * as constants from "./Constants.js";
import * as cookies from "./Cookies.js";

$(document).ready(function () {
    let cookie = cookies.getAuthCookie();
    if (cookie == null) {
        window.location = "Login";
    }
});