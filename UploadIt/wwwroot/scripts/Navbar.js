/// <reference path="../lib/jquery.min.js" />

import * as constants from "./Constants.js";
import * as cookies from "./Cookies.js";

$(document).ready(function () {
    let cookie = cookies.getAuthCookie();
    if (cookie == null) {
        $("#account_nav_ul").append(`<li class="nav-item active"><a class="nav-link" href="Login">Login</a></li>`);
        $("#account_nav_ul").append(`<li class="nav-item active"><a class="nav-link" href="Register">Register</a></li>`);
    }
    else {
        $("#account_nav_ul").append(`<li class="nav-item active"><text class="nav-link">Logged in as ${cookie.userName}</text></li>`);
        $("#account_nav_ul").append(`<li class="nav-item active"><a class="nav-link" href="Logout">Logout</a></li>`);
    }
});